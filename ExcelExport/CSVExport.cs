using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCJobClockLib;

namespace JCExport
{
    class CSVExport :BCJobClockLib.BaseDataExporter
    {
        private String CSVTargetFile = "";
        public override string getName()
        {
            return "CSV Export Module";
        }

        public override string getDescription()
        {
            return "CSV Export Module for BCJobClock Application";
        }

        public override bool hasConfigPage()
        {
            return true; 
        }

        public override void Configure(IWin32Window owner)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            String Grabfilename;
            ofd.OverwritePrompt=true;
            ofd.Filter = "Comma Separated Values (*.CSV)|*.CSV|All Files(*.*)|*.*";
            if (ofd.ShowDialog(owner) == DialogResult.OK)
            {
                CSVTargetFile=ofd.FileName; 


            }

        }
        private String FormatTS(TimeSpan Formatme)
        {
            return String.Format("{0:00}:{1:00}", Formatme.TotalHours,Formatme.Minutes);


        }

        public override void PerformExport(DataLayer dbObject)
        {
            if(CSVTargetFile=="") return;
            //perform a export to Comma-Separated-Value file.
            //first, open the file. This should be rather obvious.
            FileStream dowritefile = new FileStream(CSVTargetFile,FileMode.Create);
            using (StreamWriter CSV = new StreamWriter(dowritefile))
            {
                /*
                     * The reporting module needs to provide reporting by date first then job number (we call job number: RO#),
     such as RO#0256251 Completed by Tech(s) Mike 00:26, John 00:12 AND by date first then user such as 
     Mike completed: RO#0256251 00:26, RO#0256252 00:14 Total time: 00:40 (laid out in a column format of course,
      or an excel export if that is easier.) It also would be nice to have a text input box for searches by job number also.
                     * */

                //currently: Date first, then job number.
                //Date we will assume to be the "starting" date.
                //soooo first, retrieve all the orders, sorted by their starting date.

                List<String> allorders = dbObject.GetAllOrderIDs();

                //iterate through all these orders.
                CSV.WriteLine("RO#,Overall Time(HH:MM),Total User Time(HH:MM),Users,Active Users");
                foreach (String looporder in allorders)
                {
                    String buildline = "RO#" + looporder + ",";
                    TimeSpan TotalROTime = dbObject.GetOrderTime(looporder);
                    TimeSpan sumROTime = dbObject.GetTotalClockedTimeForOrder(looporder, false);
                    buildline += FormatTS(TotalROTime) + ",";

                    buildline += FormatTS(sumROTime) +",";

                    //now, summarize the total time spent by all the users on this order.

                    var gotuserdata = dbObject.GetUserDataFromOrder(looporder);


                    foreach (var looporderdata in gotuserdata)
                    {

                        buildline += looporderdata.Username + "(" + FormatTS(looporderdata.TotalTime) + ");";



                    }
                    buildline += ",";
                    gotuserdata = dbObject.GetUserDataFromOrder(looporder,true);
                    foreach (var looporderdata in gotuserdata)
                    {

                        buildline += looporderdata.Username + "(" + FormatTS(looporderdata.TotalTime) + ");";



                    }
                    






                    CSV.WriteLine(buildline);



                }

                CSV.Close();

            }





        }
    }
}
