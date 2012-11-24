using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;

namespace BCJobClockLib
{
    /// <summary>
    /// class used to Monitor a given Access /OLEDB database for changes in users and RO, and 
    /// place those changes into "our" MySQL database.
    /// </summary>
    class AccessWatcher
    {

        private DataLayer useDBLayer;
        private OleDbConnection oledbcon;
        private String usedConString = "";
        private Timer tmrWatcher;

        public AccessWatcher(String AccessDB, String Username, String Password)
        {
            String accessdbstring = "ConnectionString=Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}";

            usedConString = String.Format(accessdbstring, AccessDB);

            oledbcon = new OleDbConnection(usedConString);
            oledbcon.Open();

            //
            tmrWatcher = new Timer(TmrWatcher_Tick,null,0,500);
            
            

        }

        private struct SimpleUserData
        {
            public String PIN;
            public String Name;
            public SimpleUserData(String pPIN, String pName)
            {
                PIN = pPIN;
                Name = pName;

            }

        }

        private List<SimpleUserData> PrevUsers = new List<SimpleUserData>(); 
        private List<String> PrevRO = new List<string>();

        /// <summary>
        /// using the oledb connection, retrieve the user information and return it.
        /// </summary>
        /// <returns></returns>
        private List<SimpleUserData> GetAccessUserData()
        {
            return null;

        }
        private List<String> GetAccessRecordData()
        {
            //retrieve list of RO#'s from access DB.
            return null;

        }


        private bool intick = false;
        private void TmrWatcher_Tick(Object param)
        {
            if (intick) return;
            intick = true;

            try
            {

                //look in AccessDB.  We want to sync:
                //Users and ROs with our Database in DataLayer.









            }
            catch
            {


            }

            finally
            {
                intick = false; 
            }


        }

        //AccessWatcher class: Watches an Access database for changes, and delegates them back to the DataLayer.
    }
}
