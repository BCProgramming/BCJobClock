using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BCJobClockLib;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
namespace JCExport
{
#if false 

    public class ExcelExporter : BCJobClockLib.BaseDataExporter
    {
        


        public ExcelExporter()
        {


        }


        public override string getName()
        {
            return "Excel Exporter";
        }

        public override string getDescription()
        {
            return "Excel Export plugin for BCJobClock";
        }

        public override bool hasConfigPage()
        {
            return false;
        }

        public override void Configure(IWin32Window owner)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// helper routine for formatting Order Times.
        /// </summary>
        /// <param name="OrderIDs"></param>
        /// <returns></returns>
        private String FormatOrderTimes(String UserID,List<String> OrderIDs,DataLayer usinglayer)
        {
            //RO#666666 (HH:MM), RO#666667 (HH:MM) etc.

            StringBuilder buildresult = new StringBuilder();


            foreach (String looporder in OrderIDs)
            {


                buildresult.Append("RO#" + looporder + " ");

                //get the time this user spent on this order.

                //usinglayer.GetUserTotalTimeOnOrder
                TimeSpan ttime = usinglayer.GetUserTotalTimeOnOrder(UserID,looporder);
                String FormattedLength = String.Format("({0:00}:{1:00})", ttime.TotalHours, ttime.Minutes);
                buildresult.Append(FormattedLength);


            }

            return buildresult.ToString();


        }

        private void BuildUsersWorkSheet(Excel._Worksheet usesheet,DataLayer dlayer)
        {

            //step one: add headers.
            try {
            usesheet.Cells[1, 1] = "User Name";
            usesheet.Cells[1, 2] = "User PIN";
            usesheet.Cells[1, 3] = "Orders Worked(Finished)";
            usesheet.Cells[1, 4] = "Orders Worked(Active)";
            usesheet.Cells[1, 5] = "Total Work Time";

            Dictionary<String, String> usersinfo = dlayer.GetUsers();
            //key is PIN, value is username itself.
            int currentrow = 2;

            foreach (var loopuser in usersinfo)
            {
                String currusername = loopuser.Value;
                String curruserpin = loopuser.Key;

                //get order information for this user...


                List<String> FinishedOrders = dlayer.GetUserFinishedOrders(currusername);
                List<String> ActiveOrders = dlayer.GetUserOrders(currusername, true);


                String sFinishedOrders = FormatOrderTimes(curruserpin, FinishedOrders, dlayer);
                string sActiveOrders = FormatOrderTimes(curruserpin, ActiveOrders, dlayer);
                //create the "orders Worked" column- format:
                usesheet.Cells[currentrow, 1] = loopuser;
                usesheet.Cells[currentrow, 2] = curruserpin;
                usesheet.Cells[currentrow, 3] = sFinishedOrders;
                TimeSpan totaltime = dlayer.GetTotalClockedTimeForUser(curruserpin, false);
                usesheet.Cells[currentrow, 4] = String.Format("{0:00}:{1:00}", totaltime.TotalHours, totaltime.Minutes);

                currentrow++;




            }
              





            }
            catch (Exception x)
            {
                dlayer.WriteDebugMessage("JCExport, COMException:" + x.Message + " Source:" + x.Source + "Trace:\n" + x.StackTrace);

            }









        }

        public override void PerformExport(DataLayer dbObject)
        {

            Excel.Application xA;
            Excel._Workbook wb;

            try
            {
                xA = new Excel.Application();
                xA.Visible=true; 

                //add a new workbook...
                wb = xA.Workbooks.Add(Missing.Value);


                Excel._Worksheet ws = (Excel._Worksheet)wb.Worksheets.Add(null, null, null, null);

                BuildUsersWorkSheet(ws,dbObject);


                ws.Activate();


                xA.Save(Missing.Value);



            }
            catch(Exception ee)
            {


            }





        }
    }
#endif 
}
