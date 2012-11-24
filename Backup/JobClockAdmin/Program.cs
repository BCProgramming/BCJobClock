using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BCJobClockLib;

namespace JobClockAdmin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            String[] targs = Environment.GetCommandLineArgs();
            String[] args= new List<String>().ToArray();
            if (targs.Length > 1)
            {
                args = new string[targs.Length - 1];
                Array.Copy(targs, 1, args, 0, args.Length);
                for (int i = 1; i < args.Length; i++)
                {
                    args[i] = args[i].Replace("\\", "\\\\");

                }

            }

            if (args.Any((t) => t.TestRegex(CreateSwitchRegEx('m', "monitor"))))
                Application.Run(new JobMonitor());
            else if (args.Any((t) => t.TestRegex(CreateSwitchRegEx('s', "simpleuser"))))
            {

                Application.Run(new SimpleUserManager());

            }
            else
                Application.Run(new frmJobClockAdmin());
        }

        static String CreateSwitchRegEx(char singlecharform, String longform)
        {
            String fmt = "[/-]-*{1}|[/-]{0}";
            return String.Format(fmt, singlecharform, longform);


        }

        static String GetExceptionData(Exception except)
        {
            String returnit = "Message:" + except.Message;
            returnit += "\nSource:" + except.Source;
            returnit += "\nStack Trace:" + except.StackTrace;
            if(except.InnerException !=null)
            returnit += "\n Inner Exception:\n{" + GetExceptionData(except.InnerException) + "\n}";


            return returnit;

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            if (e.ExceptionObject is frmPasswordPrompt.CredentialsNotGivenException)
            {
                Application.Exit();
                return;
            }
            //open the log file. we have no idea what the program state is now, so we will put it in the log directory. We have to "re-craft" the path, though.
            String TargetLogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TargetLogPath = Path.Combine(TargetLogPath,"BCJobClock\\BCJobClock.log");

            //open this file, append to it...
            FileStream logout = new FileStream(TargetLogPath, FileMode.Append);
            StreamWriter fw = new StreamWriter(logout);

            fw.WriteLine();

            fw.WriteLine("An Unhandled Exception occured.");
            Exception getobj = e.ExceptionObject as Exception;
            fw.WriteLine(GetExceptionData(getobj));

            fw.Close();
            MessageBox.Show("An unexpected exception occured in BCJobClock. Information on this error has been logged to " + TargetLogPath + " .");







        }
    }
}
