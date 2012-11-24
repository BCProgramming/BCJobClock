using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace BCJobClockLib
{
    public static class ListViewExtensions
    {
        public class DropDownListViewCopyData
        {
            public readonly ListView lvw;
            public readonly ToolStripDropDownButton buttonobject;
            public readonly String ContentsName;
            public readonly String GroupName;
            public readonly Action<String,DropDownItemListViewCopyData> ActFunction;
            public DropDownListViewCopyData(ListView plvw, ToolStripDropDownButton pbuttonobject, String pContentsName,String pGroupName,
                Action<String, DropDownItemListViewCopyData> pActFunction)
            {

                lvw=plvw;
                buttonobject = pbuttonobject;
                ContentsName = pContentsName;
                ActFunction = pActFunction;
                GroupName = pGroupName;
            }



        }
        public class DropDownItemListViewCopyData 
        {
            public readonly ListView lvw;
            public readonly ToolStripMenuItem item;
            public readonly String GroupColumnName;
            public DataLayer.ListViewStringConstants CopyType;
            public Action<String, DropDownItemListViewCopyData> actFunction;

            public DropDownItemListViewCopyData(ListView plvw, ToolStripMenuItem pitem, DataLayer.ListViewStringConstants pCopyID,String pGroupName,
                Action<String, DropDownItemListViewCopyData> pactFunction)
            {

                lvw=plvw;
                item=pitem;
                CopyType = pCopyID;
                GroupColumnName = pGroupName;
                actFunction = pactFunction;
               
            }





        }
        
        public static void InitCopyDropDown(this ListView lvwuse,ToolStripDropDownButton tsbutton,String ContentsName,String GroupColumnName,
            Action<String, DropDownItemListViewCopyData> ActionDelegate)
        {

            //set image and text of the dropdownbutton.
            tsbutton.Image = JobClockConfig.Imageman.GetLoadedImage("copy_s");
            //set text...
            tsbutton.Text = "Copy";
            

            //add a ghost item to the drop down, so it will fire the DropDownOpeningEvent. Actually come to think of it this might not be necessary. Best not to ask
            //too many questions, though.

            tsbutton.DropDownItems.Add("GHOST"); 

            //hook the "DropDownOpening" event.
            tsbutton.DropDownOpening += new EventHandler(tsbutton_DropDownOpening);

            //set the button's tag to a new data object, which will be used in the DropDown and succeeding button Click Events.
            tsbutton.Tag = new DropDownListViewCopyData(lvwuse, tsbutton, ContentsName,GroupColumnName,ActionDelegate);
            //That's IT!



        }

        static void tsbutton_DropDownOpening(object sender, EventArgs e)
        {
            //Step one: acquire toolstripbutton from sender argument.
            ToolStripDropDownButton tsdd = sender as ToolStripDropDownButton;
            Debug.Assert(tsdd != null);

            //step two: grab tag value and cast to a DropDownListViewCopyData.

            DropDownListViewCopyData ddlvcd = tsdd.Tag as DropDownListViewCopyData;


            Debug.Assert(ddlvcd != null);


            //create two (atm) items: one for copying a CSV, and the other for HTML.
            ToolStripMenuItem CSVCopy = new ToolStripMenuItem("CSV (Comma Separated Values)");
            ToolStripMenuItem HTMLCopy = new ToolStripMenuItem("HTML Fragment");


            //set their tags to a new instance of DropDownListItemMenuData
            CSVCopy.Tag = new DropDownItemListViewCopyData(ddlvcd.lvw, CSVCopy, DataLayer.ListViewStringConstants.Copy_CSV,ddlvcd.GroupName,  ddlvcd.ActFunction);
            HTMLCopy.Tag = new DropDownItemListViewCopyData(ddlvcd.lvw, HTMLCopy, DataLayer.ListViewStringConstants.Copy_HTMLFRAGMENT, ddlvcd.GroupName, ddlvcd.ActFunction);


            //hook their event handlers, send them to the same function...
            CSVCopy.Click += new EventHandler(CopyButtons_Click);
            HTMLCopy.Click+=new EventHandler(CopyButtons_Click);
            
            //and of course add them to  the DropDown. would be pretty dumb to not.
            tsdd.DropDown.Items.Clear();
            tsdd.DropDown.Items.AddRange(new ToolStripItem[]
            {
                CSVCopy,
                HTMLCopy
            });

        }

        static void CopyButtons_Click(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
            ToolStripMenuItem casted = (ToolStripMenuItem)sender;
            DropDownItemListViewCopyData ddcd = (DropDownItemListViewCopyData)(casted.Tag);
            String getresult;
            getresult = DataLayer.ListViewToString(ddcd.lvw, ddcd.CopyType,",",Environment.NewLine,ddcd.GroupColumnName);
            //perform the action...
            ddcd.actFunction(getresult,ddcd);






        }


        public static void SizeColumnsEqual(this ListView lvtest)
        {
            //Size all columns to be an equal width.

            int colcount = lvtest.Columns.Count;
            
            //break out early to prevent divide by zero...
            if(colcount==0) return;

            int setwidth = (int)((float)lvtest.ClientSize.Width / (float)colcount);
            //iterate through each...
            foreach(ColumnHeader iteratecolumn in lvtest.Columns)
            {
                iteratecolumn.Width = setwidth;


            }




        }



    }

    public static class ParseX
    {
        //extended parse class.
        public static int ParseIntX(String Value)
        {

            return ParseIntX(Value,0);
        }

        public static int ParseIntX(String Value, int Default)
        {
            int parseresult= Default;

            if(int.TryParse(Value,out parseresult))
                return parseresult;
            else
                return Default;
            


        }
        


    }

    public static class boolEx
    {

    
        public static bool xParse(String Value, bool Default)
        {
            bool parseresult=false;

            if(bool.TryParse(Value,out parseresult))
                return parseresult;
            else
                return Default;
            



        }
        public static bool xParse(String Value)
        {
            return xParse(Value, false);
        }



    }



    public static class ExtendDbReader
    {

        public static TimeSpan GetTimeSpan(this DbDataReader dreader, int ordinal)
        {


            return new TimeSpan((long)(dreader.GetDecimal(ordinal)));


        }

     
            private static String formatstr = "{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}";

            

            public static String getSQLfmt(this DateTime dt)
            {
                //format is YYYY-MM-DD HH:MM:SS


                return String.Format(formatstr, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);







            }
            
            public static DateTime DateTimeFromSQLfmt(String DateTimecvt)
            {
                //MySQL Format for DateTime is:
                //YYYY-MM-DD HH:MM:SS

                String[] splitparts = DateTimecvt.Split(new char[] { '-', ' ', ':' });

                int Year = int.Parse(splitparts[0]);
                int Month = int.Parse(splitparts[1]);
                int Day = int.Parse(splitparts[2]);
                int Hour = int.Parse(splitparts[3]);
                int Minute = int.Parse(splitparts[4]);
                int Second = int.Parse(splitparts[5]);

                return new DateTime(Year, Month, Day, Hour, Minute, Second);




            }

        }
    }

