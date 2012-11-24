using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using BASeCamp.Configuration;

namespace BASeCamp.Configuration
{

    /// <summary>
    /// FormPositionSaver class: works in conjunction with the BASeCamp.Configuration.INIFile class to Save and Load a form's
    /// placement when it is opened and closed.
    /// </summary>

    public class FormPositionSaver
    {
        //Declarations for POINTAPI,RECT, and WINDOWPLACEMENT.
        //we use POINTAPI and RECT rather than the .NET class types so we can control the TryParse() and ToString() implementations, which we use 
        //to leverage the INIDataItem Extension class additions.

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTAPI
        {
            internal int x;
            internal int y;


            public POINTAPI(int px, int py)
            {
                x = px;
                y = py;
            }

            public static void TryParse(String parseit, out POINTAPI result)
            {
                //format: (X,Y)
                //strip out parens.
                String[] parsed = parseit.Replace("(", "").Replace(")", "").Split(new char[] {','});
                int kx = int.Parse(parsed[0]);
                int ky = int.Parse(parsed[1]);


                result = new POINTAPI(kx, ky);
            }

            public override string ToString()
            {
                return "(" + x + "," + y + ")";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;

            public override string ToString()
            {
                return "{" + new POINTAPI(Left, Top).ToString() + "-" + new POINTAPI(Right, Bottom).ToString() + "}";
            }

            public RECT(int pLeft, int pTop, int pRight, int pBottom)
            {
                Left = pLeft;
                Top = pTop;
                Right = pRight;
                Bottom = pBottom;
            }

            public static void TryParse(String parsestr, out RECT result)
            {
                //strip out braces...
                parsestr = parsestr.Replace("{", "").Replace("}", "");
                //split at ")-("...
                String[] Pointstrings = parsestr.Split(new string[] {")-("}, StringSplitOptions.RemoveEmptyEntries);
                POINTAPI firstpoint, secondpoint;
                //parse the resulting values. re-add the parens that were removed by the split.
                POINTAPI.TryParse(Pointstrings[0] + ")", out firstpoint);
                POINTAPI.TryParse("(" + Pointstrings[1], out secondpoint);


                result = new RECT(firstpoint.y, firstpoint.y, secondpoint.x, secondpoint.y);
            }
        }


        [DllImport("user32.dll")]
        private static extern int OffsetRect(ref RECT lpRect, int x, int y);

        [DllImport("user32.dll")]
        private static extern int GetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int SetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT lpwndpl);

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            internal int Length;
            internal int flags;
            internal int showCmd;
            internal POINTAPI ptMinPosition;
            internal POINTAPI ptMaxPosition;
            internal RECT rcNormalPosition;

            public override string ToString()
            {
                return String.Join(",", new string[]
                                            {
                                                flags.ToString(), showCmd.ToString(),
                                                ptMinPosition.x.ToString(), ptMinPosition.y.ToString(),
                                                ptMaxPosition.x.ToString(), ptMaxPosition.y.ToString(),
                                                rcNormalPosition.Left.ToString(), rcNormalPosition.Top.ToString(),
                                                rcNormalPosition.Right.ToString(), rcNormalPosition.Bottom.ToString()
                                            });
            }

            //parsed a string into a WINDOWPLACEMENT structure.
            public static bool TryParse(String parseme, out WINDOWPLACEMENT result)
            {
                try
                {
                    String[] splitvalues = parseme.Split(',');
                    int[] parsedvalues = new int[splitvalues.Length];


                    for (int i = 0; i < parsedvalues.Length; i++)
                    {
                        int.TryParse(splitvalues[i], out parsedvalues[i]);
                    }

                    result = new WINDOWPLACEMENT
                                 {
                                     flags = parsedvalues[0],
                                     showCmd = parsedvalues[1],
                                     ptMinPosition = new POINTAPI(parsedvalues[2], parsedvalues[3]),
                                     ptMaxPosition = new POINTAPI(parsedvalues[4], parsedvalues[5]),
                                     rcNormalPosition =
                                         new RECT(parsedvalues[6], parsedvalues[7], parsedvalues[8], parsedvalues[9])
                                 };

                    return true;
                }
                catch
                {
                    result = new WINDOWPLACEMENT();
                    return false;
                }
            }
        }


        private Form FormObject = null;
        private INIFile Configuration = null;
        private static readonly String usesectionName = "WindowPositions";


        /// <summary>
        /// Create the FormPositionSaver
        /// </summary>
        /// <param name="FormObj">Form to deal with</param>
        /// <param name="configfile">INIFile to load and save</param>
        /// <param name="alreadyloaded">whether the Load event has fired. If true, will try to set the form position immediately. otherwise, it hooks the Load event and waits.</param>
        public FormPositionSaver(Form FormObj, INIFile configfile, bool alreadyloaded)
        {
            Configuration = configfile;
            FormObject = FormObj;

            FormObject.FormClosed += new FormClosedEventHandler(FormObject_FormClosed);


            if (!alreadyloaded)
                FormObject.Load += new EventHandler(FormObject_Load);
            else
            {
                FormObject_Load(FormObject, new EventArgs());
            }
        }

        //save the placement...
        private void FormObject_FormClosed(object sender, FormClosedEventArgs e)
        {
            //save placement.
            //all the "tough work" is handled above, and by the INIDataItem Extension methods. Here we
            //can simply use SetValue<> and set the value. Nice and clean.
            WINDOWPLACEMENT grabplacement = new WINDOWPLACEMENT();
            GetWindowPlacement(FormObject.Handle, ref grabplacement);
            Configuration[usesectionName][FormObject.Name].SetValue(grabplacement);
        }

        //Load event: load the form placement, if present, from the INI file we were given in our constructor.
        private void FormObject_Load(object sender, EventArgs e)
        {
            WINDOWPLACEMENT currplacement = new WINDOWPLACEMENT();
            GetWindowPlacement(FormObject.Handle, ref currplacement);
                //default is wherever it is now if there is a parse problem.

            WINDOWPLACEMENT getplacement =
                Configuration[usesectionName][FormObject.Name].GetValue(currplacement);

            //check for previous instances, and offset if there are.
            String thisproc = Process.GetCurrentProcess().ProcessName;
            Process[] existing = Process.GetProcessesByName(thisproc);
            if (existing.Length > 1)
            {
                //more than one, so offset...
                OffsetRect(ref getplacement.rcNormalPosition, 16*existing.Length, 16*existing.Length);
            }


            SetWindowPlacement(FormObject.Handle, ref getplacement);

            //load placement...
        }
    }
}