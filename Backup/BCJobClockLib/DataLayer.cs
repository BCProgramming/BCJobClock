using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using BASeCamp.Configuration;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace BCJobClockLib
{

    [Serializable]
    public class DatabaseStructureCorruptedException : ApplicationException
    {

        public DatabaseStructureCorruptedException(String pMessage):base(pMessage)
        {


        }


    }

    public class MessageActivityLogItem
    {
        public int MessageID { get; set; }
        public String Message { get; set; }
        public String MessageType { get; set; }
        public DateTime LogTime { get; set; }
        public String LogID { get; set; }
        public MessageActivityLogItem(int pMessageID,String pMessage, String pType,DateTime gotdatetime,String pLogID)
        {
            //LogTime = DBTime();
            Message = pMessage;
            MessageType = pType;
            MessageID=pMessageID;
            LogTime=gotdatetime;
            LogID=pLogID;




        }



    }


    public class DataLayer
    {
        public MultiTypeManager mtypemanager;
       public String DatabaseName=null;


        //problem: admin program is not scalable, because interval-based refreshes of large data-sets are intensive.

        //idea:
        //instead of refreshing at intervals, have another table- "DirtyData". When data is added or modified in the database, a record is added.
        //another piece of code can poll that table at intervals, and if it finds there were changes commence a "real" update. This is less of a problem because
        //the full time for the refresh will only occur when there is new data to display.
        //optionally of course this could change an indicator or something on the window.

        //A trigger can be set to add a value to the DirtyData Table when Users,Orders or OrderData are changed.


        




       public void UpdateUserTableAddActiveField()
       {
           //if needed...

           //("USERSACTIVEFIELDQUERY"

           //("ADDACTIVEFIELDTOUSERS",

           DbConnection gotconnection;
           DbCommand execcmd = GetCommand(out gotconnection);

           execcmd.CommandText = qAdapter["USERSACTIVEFIELDQUERY"];
           try
           {
               using (DbDataReader dr = execcmd.ExecuteReader())
               {
                   if (dr.HasRows)
                   {

                       //we've determined the field exists and an update is not necessary.
                       return;





                   }





               }


           }
           catch (Exception ee)
           {
               
                   //it doesn't have the field, so we need to add it.
                  
                   execcmd.CommandText = qAdapter["ADDACTIVEFIELDTOUSERS"];
                   execcmd.ExecuteNonQuery();
               


           }



       }
       private void setConfigSettingDB(String SectionName, String ValueName, String Value)
       {
           //"INSERTCONFIGSETTING"
           //and "UPDATECONFIGSETTING"


           //first, attempt to update, if that fails, insert. If that fails, pretend
           // it all went according ot plan and whistle non-chalantly.
           DbConnection usecon;
           DbCommand usecmd = GetCommand(out usecon);
           try
           {
               String updatesql = String.Format(qAdapter["UPDATECONFIGSETTING"], SectionName, ValueName, Value);
               usecmd.CommandText=updatesql;
               usecmd.ExecuteNonQuery();



           }
           catch
           {
               //error, blithly try to Insert the value instead. if THAT fails... well, ignore it.
               Debug.Print("Exception trying to setConfigSetting in the DB, Section={0}, ValueName={1}, Value={2}", SectionName, ValueName, Value);
               try
               {
                   String updatesql = String.Format(qAdapter["INSERTCONFIGSETTING"], SectionName, ValueName, Value);
                   usecmd.CommandText = updatesql;
                   usecmd.ExecuteNonQuery();
               }
               catch
               {
                   Debug.Print("Exception trying to Create ConfigSetting in the DB, Section={0}, ValueName={1}, Value={2}", SectionName, ValueName, Value);
               }

           }


       }

        private String getConfigSettingDB(String SectionName, String ValueName)
       {

           //"GETCONFIGSETTING"
           //first is section, then value.
           String usequery = String.Format(qAdapter["GETCONFIGSETTING"], SectionName, ValueName);

           DbConnection usecon;
           DbCommand usecmd = GetCommand(out usecon);

           try
           {
               usecmd.CommandText = usequery;
               using (DbDataReader dreader = usecmd.ExecuteReader())
               {

                   if (dreader.HasRows)
                       if (dreader.Read())
                           return dreader.GetString(dreader.GetOrdinal("Value"));



               }



           }
           catch
           {
               
           }

           return null; //null to indicate either value doesn't exist, or a DB error occured.



       }

        public  void ReInitializeDB()
       {

           String DropDB = String.Format(qAdapter["DROPDBIFEXISTSFMT"],DatabaseName);
           String CreateDB = String.Format(qAdapter["CREATEDBFMT"],DatabaseName);

           String CreateOrderDataTable = qAdapter["CREATEORDERDATATABLE"];



           String CreateOrdersTable =qAdapter["CREATEORDERSTABLE"];

           String CreateUsersTable = qAdapter["CREATEUSERSTABLE"];

           String CreateMessageLogTable = qAdapter["CREATEMESSAGELOGTABLE"];

           String CreateClockedTable = qAdapter["CREATECLOCKEDTABLE"];

           //assume we've gone through any prompts.
           //Drop the database...
           try
           {
               DbConnection gotconnection;
               DbCommand execcmd = GetCommand(out gotconnection);

               String[] commandsequence = new string[] { DropDB,CreateDB,CreateUsersTable,CreateOrdersTable,CreateOrderDataTable,CreateMessageLogTable,CreateClockedTable};

               foreach (String loopexec in commandsequence)
               {
                   LogAdmin("Reinit::Executing Command-" + loopexec);
                   execcmd.CommandText = loopexec;
                   execcmd.ExecuteNonQuery(); 
               }

               //re-create the tables..


           }
           catch (Exception initerror)
           {
               LogAdmin("Exception occured while trying to reinitialize DB-" + initerror.Message);


           }

       }

        //DataLayer class. tries to abstract away database oriented code (storage of users and their PINs, work orders, times, etc.



        //What needs to be stored?
        public JobClockConfig Configuration = new JobClockConfig();


        public static String Clientlogfile = Path.Combine(JobClockConfig.GetAppDataFolder(), "client.log");
        public static String AdminAppletlogfile = Path.Combine(JobClockConfig.GetAppDataFolder(), "admin.log");
        private QueryAdapter _qAdapter;
        private static StreamWriter _ClientLog=null;
        private static StreamWriter _AdminLog=null;
        public QueryAdapter qAdapter { get { return _qAdapter; } set { _qAdapter = value; } }
            public static void LogAdmin(String Message)
        {
            String buildMessage = "PID({0}):{1}:{2}";
            Process g = Process.GetCurrentProcess();
            String usemessage = String.Format(buildMessage, g.Id, DateTime.Now, Message);
            AdminLog().WriteLine(usemessage);

             
        }
        public static void LogClient(String Message)
        {
            String buildMessage = "PID({0}):{1}:{2}";
            Process g = Process.GetCurrentProcess();
            String usemessage = String.Format(buildMessage, g.Id, DateTime.Now, Message);
            ClientLog().WriteLine(usemessage);


        }


        private static StreamWriter ClientLog()
        {
            if (_ClientLog == null)
            {
                _ClientLog = new StreamWriter(new FileStream(Clientlogfile,
                    FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1,
                    FileOptions.WriteThrough));

            }
            return _ClientLog;
        }
        private static StreamWriter AdminLog()
        {

            if (_AdminLog == null)
            {
                _AdminLog = new StreamWriter(new FileStream(AdminAppletlogfile, 
                    FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1,
                    FileOptions.WriteThrough));


            }
            return _AdminLog;

        }



        private String CurrentConString;
        private TimeSpan ServerOffset = new TimeSpan();


        //a sort of hacky function.
        //after much testing. I determined  that some bugs were being caused by a discrepancy between the server time (the SQL database PC) and the local client.
        //particularly, since the "CLOCKED" entry, and most other entries, are updated on the server-side via auto-stamp, we need to retrieve the time on the server for some things.
        //now the datalayer uses this function instead of DateTime.Now.

        public DateTime DBTime()
        {
            String sqluse = qAdapter["DBTIME"];
            DbConnection conuse;
            DbCommand cmduse = GetCommand(out conuse);
            cmduse.CommandText=sqluse;
            try
            {
                //a small gotcha: we cannot execute a Command if a open DataReader exists already, up the stack, usually.
                //There isn't, to my understanding, a good way of determining if a DataReader is already open on a connection, though.
                
                //since this is common, we simply catch it with a try...catch.
                Object result = cmduse.ExecuteScalar();
                DateTime casted = (DateTime)result;
                //if successful, cache the offset between the server time and the local time.
                ServerOffset=casted - DateTime.Now;
                return casted;
            }
            catch
            {
                return DateTime.Now + ServerOffset; //make do with Server offset, we hopefully calculated before...

            }
            
        }

        public String buildConnectionString(ConnectionTypeConstants contype)
        {
            String sbuild="";
            String usecredentials="";
            
            switch (contype)
            {
                case ConnectionTypeConstants.Connection_Admin:
                    sbuild= Configuration.AdminConnectionString;
                    usecredentials = Configuration.AdminCredentials;
                    break;
                case ConnectionTypeConstants.Connection_Client:
                    sbuild=Configuration.ClientConnectionString;
                    usecredentials = Configuration.ClientCredentials;
                    break;
                case ConnectionTypeConstants.Connection_Monitor:
                    sbuild = Configuration.MonitorConnectionString;
                    usecredentials = Configuration.MonitorCredentials;
                    break;
            }

            if (usecredentials.ToUpper() == "!PROMPT!")
            {

                String prompteduser, promptedpassword;
             
                    frmPasswordPrompt.DoShowPrompt(out prompteduser, out promptedpassword);
                    usecredentials = prompteduser + "," + promptedpassword;
      

            }
            if (!(usecredentials.ToUpper() == "NONE"))
            {
                //don't do this if the credentials are "NONE".
                String[] ss = usecredentials.Split(',');
                if (ss.Length >= 2)
                    sbuild += String.Format("; User ID={0};Password={1}", ss[0].Trim(), ss[1].Trim());
                else
                    sbuild += String.Format("; User ID={0}", ss[0].Trim());
                    
                
                
            }
            WriteDebugMessage("BuildConnectionString returning \"" + sbuild + "\"");
            return sbuild;
        }


       

        /// <summary>
        /// Looks up the given PIN Code, returning either an empty string of not found, or the name of the user associated with the PIN.
        /// </summary>
        /// <param name="PINLookup"></param>
        /// <returns></returns>
        public String LookupPIN(String PINLookup)
        {
            String QueryPIN = String.Format(qAdapter["PINLOOKUP"],PINLookup);
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            
            makecmd.CommandText = QueryPIN;
            using (DbDataReader makereader = makecmd.ExecuteReader())
            {

                if (makereader.HasRows)
                {
                    makereader.Read();
                    return (String) makereader.GetString(makereader.GetOrdinal("UserName"));


                }
                else
                {
                    return ""; //return empty string, as per spec.
                }


            }

        }
        /// <summary>
        /// retrieve the PIN for a given user, or an empty string of the User doesn't exist.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public String GetUserPIN(String UserName)
        {
            String QueryUser = String.Format(qAdapter["USERPIN"], UserName) ;
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);

            makecmd.CommandText = QueryUser;
            lock (this)
            {
                using (DbDataReader makereader = makecmd.ExecuteReader())
                {

                    if (makereader.HasRows)
                    {
                        makereader.Read();
                        return (String) makereader["PINCode"];


                    }
                    else
                    {
                        return ""; //return empty string, as per spec.
                    }

                }
            }
        }

        /// <summary>
        /// Adds a user (Username and PIN) to the DB.
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="PIN"></param>
        /// <returns>True for success. False otherwise</returns>
        /// //***
        public bool AddUser(String Username, String PIN)
        {

            String QueryAddUserfmt = qAdapter["ADDUSERFMT"];
            try
            {
                DbConnection getcon;
                DbCommand makecmd = GetCommand(out getcon);
                makecmd.CommandText = String.Format(QueryAddUserfmt, Username, PIN);

                makecmd.ExecuteNonQuery();
                makecmd.Dispose();
                
                return true;
            }
            catch (Exception oo)
            {
                Debug.Print(oo.Message);
                return false;


            }
        }
        /// <summary>
        /// Removes a user from the Database. Can optionally remove all data associated with this user as well (OrderData Records)
        /// </summary>
        /// <param name="Username">Name of user to remove</param>
        /// <param name="flRemoveAssociations">whether to remove all associated data from orderData as well.</param>
        /// <returns>True if the item is no longer present in the database- true is returned if it wasn't found at all, as well. False otherwise.</returns>
        public bool RemoveUser(String Username, bool flRemoveAssociations)
        {
            if (Username == "") return true;
            String thisusersPIN = GetUserPIN(Username);
            if (thisusersPIN == "") return true; //nothing to delete since the user doesn't exist...
            String DeleteUserqueryfmt = qAdapter["DELETEUSERFMT"];
            String ClearOrdersfmt = qAdapter["CLEARUSERORDERS"];

            try
            {
                DbConnection getcon;
                DbCommand makecmd = GetCommand(out getcon);
                makecmd.CommandText = String.Format(DeleteUserqueryfmt, Username);
                makecmd.ExecuteNonQuery();
                if (flRemoveAssociations)
                {
                    makecmd.CommandText = String.Format(ClearOrdersfmt, thisusersPIN);
                    makecmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }



        }
        public static String FormatTimeSpan(TimeSpan formatthis)
        {
//            return String.Format("{0:00}:{1:00}:{2:00}", Math.Floor(formatthis.TotalHours), formatthis.Minutes,formatthis.Seconds);
            return String.Format("{0:00}:{1:00}", Math.Floor(formatthis.TotalHours), formatthis.Minutes);

        }
        /// <summary>
        /// adds a record to the CLOCKED table.
        /// </summary>
        /// <param name="UserPIN">User PIN</param>
        /// <param name="OrderID">Order</param>
        /// <param name="INOUT">either 'IN' or 'OUT' to represent clocking in or out respectively.</param>
        private void InsertClockedRecord(String UserPIN, String OrderID, String INOUT)
        {

            String usesqlformat = qAdapter["INSERTCLOCKED"];

            String runsql = String.Format(usesqlformat, UserPIN, OrderID, INOUT);

            DbConnection dconnection;
            DbCommand dmc = GetCommand(out dconnection);

            dmc.CommandText=runsql;

            dmc.ExecuteNonQuery();







        }
        public void ClockUserIn(String UserPIN, String Fromorder)
        {

            if (!GetUserClockState(UserPIN, Fromorder))
            {

                ToggleUserClockin(UserPIN, Fromorder); 

            }
            //otherwise do nothing...


        }

        public bool GetUserClockState(String UserPIN, String OrderID)
        {
            String checkuserordersqlfmt = qAdapter["CHECKUSERFMT"];

            DbConnection con;
            DbCommand gotcommand = GetCommand(out con);
            gotcommand.CommandText = String.Format(checkuserordersqlfmt, UserPIN, OrderID);
            using (DbDataReader dr = gotcommand.ExecuteReader())
            {
                if (dr.Read())
                {
                    if (!dr.IsDBNull(dr.GetOrdinal("StartTime")))
                    {
                        //they are clocked in.
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }


            }
            return false;
        }

        public void ClockOutOrder(String UserPIN, String OrderID)
        {
            String checkuserordersqlfmt =
               qAdapter["CHECKUSERFMT"];
            //String sqlupdateorderfmt = " UPDATE OrderData SET StartTime=\u0022{0}\u0022 WHERE UserPinCode=\u0022{1}\u0022 AND `Order`=\u0022{2}\u0022";
            String sqlupdateTotalTimefmt = qAdapter["UPDATETOTALTIMEFMT"];
            DbConnection con;
            DbCommand gotcommand = GetCommand(out con);
            gotcommand.CommandText = String.Format(checkuserordersqlfmt, UserPIN, OrderID);
            
            
                using (DbDataReader dr = gotcommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        //if StartTime is not null, they've clocked in. Otherwise, break out.
                        if (!dr.IsDBNull(dr.GetOrdinal("StartTime")))
                        {
                            //clock them out.
                            //how? First Grab starttime and totaltime...
                            DateTime stime = dr.GetDateTime(dr.GetOrdinal("StartTime"));
                            TimeSpan currtotal = new TimeSpan((long)dr.GetDecimal(dr.GetOrdinal("TotalTime")));

                            //acquire the timespan from then to now...
                            TimeSpan duration = DBTime() - stime;
                            //add it...
                            currtotal = currtotal + duration;


                            //and update the database value.
                            //close the DataReader...
                            dr.Close();
                            String execquery = String.Format(sqlupdateTotalTimefmt, currtotal.Ticks,UserPIN,OrderID);
                            gotcommand.CommandText = execquery;
                            gotcommand.ExecuteNonQuery();

                            //also update the Timestamp for lastclockout..

                            gotcommand.CommandText = String.Format(qAdapter["UPDATECLOCKOUTFMT"],UserPIN);
                            gotcommand.ExecuteNonQuery();

                            //and update the "CLOCKED" table..
                            InsertClockedRecord(UserPIN, OrderID, "OUT");


                        }
                        else
                        {
                          
                        }


                    }

                
            }



        }
        public DateTime? GetLastClockOutForOrder(String OrderID)
        {
            //returns the last time a given Order was clocked out.
            String lastorderclockfmt = qAdapter["LASTORDERCLOCKFMT"];
            String usequery = String.Format(lastorderclockfmt, OrderID);

            DbConnection dcon;
            
            DbCommand dcmd = GetCommand(out dcon);
            dcmd.CommandText = String.Format(lastorderclockfmt, OrderID);
            using (DbDataReader usereader = dcmd.ExecuteReader())
            {
                DateTime? LatestValue = null;
                while (usereader.Read())
                {
                    DateTime? returnthis = usereader.GetDateTime(usereader.GetOrdinal("LastClockOut"));
                    if(LatestValue==null || returnthis > LatestValue)
                        LatestValue=returnthis;
                    


                }
                return LatestValue ;



            }



        }
        public DateTime? GetLastClockOutForUserOrder(String UserPIN, String OrderID)
        {
            string LastClockOutForUserOrderfmt = qAdapter["LASTCLOCKOUTFORUSERORDERFMT"];
            string usequery = String.Format(LastClockOutForUserOrderfmt, UserPIN);
            DbConnection dcon;
            DbCommand dcmd = GetCommand(out dcon);
            dcmd.CommandText = String.Format(usequery, UserPIN);
            using (DbDataReader usereader = dcmd.ExecuteReader())
            {
                DateTime? LatestTime = null;

                while (usereader.Read())
                {
                    DateTime? returnthis = usereader.GetDateTime(usereader.GetOrdinal("LastClockOut"));
                        if(LatestTime==null || returnthis > LatestTime) LatestTime=returnthis;



                }

                return LatestTime;
            }




        }

        public DateTime? GetLastClockOutForUser(String UserPIN)
        {

            //String usequeryfmt = "SELECT * FROM OrderData WHERE UserPinCode=\"{0}\" AND LastClockOut IS NOT NULL";
            String usequeryfmt = qAdapter["LASTCLOCKOUTFORUSERORDERFMT"];
            String usequery = String.Format(usequeryfmt, UserPIN);

            DbConnection dcon;
            DbCommand dcmd = GetCommand(out dcon);
            dcmd.CommandText = String.Format(usequery, UserPIN);
            using (DbDataReader usereader = dcmd.ExecuteReader())
            {
                DateTime? LatestTime=null;

                while (usereader.Read())
                {
                    DateTime? returnthis = usereader.GetDateTime(usereader.GetOrdinal("LastClockOut"));
                    if (LatestTime == null || returnthis > LatestTime) LatestTime = returnthis;


                }
                Debug.Print("returning LatestTime for UserPIN:" + UserPIN + " value:" + LatestTime.ToString());
                return LatestTime;



            }



        }

        public bool ToggleUserClockin(String UserPin, String OrderID)
        {
            //If the user is clocked in on that Order, clocks them out. if they are clocked out, clocks them in.

            //First, see if there is a record in OrderData that has this UserPin as UserPinCode, and the Order field of OrderID.

            String checkuserordersqlfmt =
                qAdapter["CHECKUSERORDERFMT"];
            String sqlupdateorderfmt = " UPDATE OrderData SET StartTime=\u0022{0}\u0022 WHERE UserPinCode=\u0022{1}\u0022 AND `Order`=\u0022{2}\u0022";
            //String sqlupdateTotalTimefmt = " UPDATE OrderData SET TotalTime=\u0022{0}\u0022, StartTime=NULL WHERE UserPinCode=\u0022" +
            //                               UserPin + "\u0022 AND `Order`=\u0022" + OrderID + "\u0022";
            DbConnection getcon;
            DbCommand gotcommand = GetCommand(out getcon);
            gotcommand.CommandText = String.Format(checkuserordersqlfmt,UserPin,OrderID);
            using (DbDataReader dr = gotcommand.ExecuteReader())
            {

            if (dr.HasRows)
            {
                dr.Read();
                //if StartTime is not null, they've already clocked in.
                if (!dr.IsDBNull(dr.GetOrdinal("StartTime")))
                {
                    Debug.Print("User with pin " + UserPin + "Already clocked into job #" + OrderID);
                    dr.Close();
                    ClockOutOrder(UserPin, OrderID);
                    /*
                    //clock them out.
                    //how? First Grab starttime and totaltime...
                    DateTime stime = dr.GetDateTime(dr.GetOrdinal("StartTime"));
                    TimeSpan currtotal = new TimeSpan(dr.GetDecimal(dr.GetOrdinal("TotalTime")));

                    //acquire the timespan from then to now...
                    TimeSpan duration = DBTime() - stime;
                    //add it...
                    currtotal = currtotal + duration;

                    
                    //and update the database value.
                    //close the DataReader...
                    dr.Close();
                    String execquery = String.Format(sqlupdateTotalTimefmt, currtotal.Ticks);
                    gotcommand.CommandText = execquery;
                    gotcommand.ExecuteNonQuery();

                    //we also need to update LastClockOut to reflect the last time they clocked out...

                    gotcommand.CommandText = "UPDATE OrderData SET LastClockOut=NOW() WHERE UserPinCode=\"" + UserPin + "\"";
                    gotcommand.ExecuteNonQuery();
                    */


                    return false; //clocked out...


                }
                else
                {
                    //otherwise, it's null and therefore they aren't clocked in.
                    //BUT the record exists, so we use UPDATE.
                    
                    dr.Close();

                    String usesqlstring = String.Format(sqlupdateorderfmt, DBTime().getSQLfmt(),UserPin,OrderID);
                    gotcommand.CommandText = usesqlstring;
                    gotcommand.ExecuteNonQuery();

                    //plonk a record into the CLOCKED table as well.
                    InsertClockedRecord(UserPin, OrderID, "IN");

                    //they ought to be clocked in now, return true.
                    return true;

                }


            }
            else
            {
                //no rows were returned, this user has not clocked into this job before, so insert a new record for them.
                //UserPinCode, StartTime,TotalTime, Order 
                dr.Close();
                String clockinsqlfmt =
                   qAdapter["CLOCKINSQLFMT"];
                gotcommand.CommandText = String.Format(clockinsqlfmt, UserPin, ExtendDbReader.getSQLfmt(DBTime()), 0, OrderID);
                gotcommand.ExecuteNonQuery();
                //and insert a record into CLOCKED as well...
                InsertClockedRecord(UserPin, OrderID, "IN");
                return true;

            }
        }


    }

        


        public class BasicUserOrderData
        {
            public String Username;
            public TimeSpan TotalTime;
            public bool isClocked;
            public String OrderID;
            public BasicUserOrderData(String pOrderID, String pUsername, TimeSpan pTotalTime, bool pisClocked)
            {
                OrderID = pOrderID;
                Username = pUsername;
                TotalTime = pTotalTime;
                isClocked = pisClocked;


            }
            public override string ToString()
            {
                return Username + "(" + FormatTimeSpan(TotalTime) + (isClocked ? "+" : "") + ")";
            }

        }
        public List<String> GetValidOrders()
        {
            //returns the list of all Valid Order IDs.
            //found in Table TheOrders.
            List<String> returnlist = new List<string>();
            String QueryTheOrders = qAdapter["QUERYORDERS"];
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = QueryTheOrders;
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {
                if (datreader.HasRows)
                {
                    while (datreader.Read())
                    {

                        String getfield = datreader.GetString(datreader.GetOrdinal("OrderID"));
                        returnlist.Add(getfield);


                    }

                    return returnlist;
                }
                return new List<string>(); //return an empty list.
            }
        }
        public bool AddOrder(String OrderID)
        {
            return AddOrder(OrderID, false);

        }

        public bool AddOrder(String OrderID,bool force)
        {
            //if not being forced and AutoAddOrders is false, don't do it.

            if (!Configuration.AutoAddOrders && !force)
                return false;
            else
            {
                String addorderSQL = qAdapter["ADDORDER"];
                DbConnection getcon;
                DbCommand Makecmd = GetCommand(out getcon);
                Makecmd.CommandText = addorderSQL;
                Makecmd.ExecuteNonQuery(); //the insertion...
                return true;
            }




        }
        public long GetActiveJobCountForUser(String UserPIN)
        {
            String queryit = qAdapter["ACTIVEJOBCOUNTUSERFMT"];
            String Queryuse = String.Format(queryit, UserPIN);
            DbConnection getcon;
            DbCommand usecmd = GetCommand(out getcon);
            usecmd.CommandText = Queryuse;
            return (long)usecmd.ExecuteScalar();



        }
        public TimeSpan GetTotalClockedTimeForOrder(string OrderID)
        {
            return GetTotalClockedTimeForOrder(OrderID,true);


        }

        public TimeSpan GetTotalClockedTimeForOrder(String OrderID, bool flOnlyCompleted)
        {
            //return the total clocked time on an order. 
            String getOrderSql = qAdapter["SELECTORDERFMT"];

            DbConnection getcon;
            DbCommand getcmd = GetCommand(out getcon);
            getcmd.CommandText = String.Format(getOrderSql, OrderID);

            TimeSpan TotalTimeClocked=new TimeSpan();
            using (DbDataReader dbreader = getcmd.ExecuteReader())
            {
                while (dbreader.Read())
                {

                    //if StartTime is null
                    if (dbreader.IsDBNull(dbreader.GetOrdinal("StartTime")))
                    {
                        //means this is a "clocked out" order.
                        
                        TimeSpan getts = new TimeSpan((long)dbreader.GetDecimal(dbreader.GetOrdinal("TotalTime")));
                        Debug.Print("Found clocked out order for OrderID:" + OrderID + " Duration:" + getts.ToString());
                        TotalTimeClocked+=getts;


                    }
                    else if (!flOnlyCompleted)
                    {
                        DateTime grabStartTime = dbreader.GetDateTime(dbreader.GetOrdinal("StartTime"));
                        TimeSpan getts = DBTime() - grabStartTime;
                        Debug.Print("Found clocked in order for OrderID:" + OrderID + " Duration:" + getts.ToString());
                        TotalTimeClocked += getts; 


                    }


                }





            }

            return TotalTimeClocked;




        }




        public TimeSpan GetTotalClockedTimeForUser(String UserPIN)
        {
            return GetTotalClockedTimeForUser(UserPIN,TotalClockedTimeTypeConstants.ClockedTime_Active| TotalClockedTimeTypeConstants.ClockedTime_Completed);


        }

        public class ClockDataEntry
        {
            public int ID { get; set; }
            public String PIN { get; set; }
            public String OrderID { get; set; }
            public DateTime STAMP { get; set; }
            public String EventType { get; set; }
            public ClockDataEntry(int pID, String pUserPIN, String pOrderID, DateTime pStamp, String peventType)
            {
                ID = pID;
                PIN = pUserPIN;
                OrderID=pOrderID;
                STAMP = pStamp;
                EventType = peventType;




            }


        }

        [Flags]
        public enum TotalClockedTimeTypeConstants
        {
            ClockedTime_Active,
            ClockedTime_Completed


        }
        public List<ClockDataEntry> GetClockedInRange(DateRange userange)
        {
            String sqlquery = "SELECT * FROM CLOCKED WHERE `STAMP`>\"{0}\" AND `STAMP`<\"{1}\"";
            string sqluse = String.Format(sqlquery, userange.StartTime.getSQLfmt(), userange.EndTime.getSQLfmt());

            DbConnection dcon;
            DbCommand dcmd = GetCommand(out dcon);
            List<ClockDataEntry> result = new List<ClockDataEntry>();
            dcmd.CommandText = sqluse;
            lock (this)
            {
                using (DbDataReader dreader = dcmd.ExecuteReader())
                {

                    while (dreader.Read())
                    {
                        int _ID = dreader.GetInt32(dreader.GetOrdinal("ID"));
                        String _PIN = dreader.GetString(dreader.GetOrdinal("UserPIN"));
                        String _OrderID = dreader.GetString(dreader.GetOrdinal("OrderID"));
                        DateTime _Stamp = dreader.GetDateTime(dreader.GetOrdinal("STAMP"));
                        String _EType = dreader.GetString(dreader.GetOrdinal("EventType"));

                        result.Add(new ClockDataEntry(_ID, _PIN, _OrderID, _Stamp, _EType));

                    }



                }



            }
            return result;

        }



        /// <summary>
        /// Returns the total time clocked for a given user. This only counts complete times- it doesn't count running time if they are currently clocked in.
        /// </summary>
        /// <param name="UserPIN"></param>
        /// <param name="flOnlyCompleted">if true, only count 'completed' Time, that is, if they are clocked in at the moment, don't count 
        /// any of that time (only time spans they started and completed are counted)</param>
        /// <returns></returns>
        public TimeSpan GetTotalClockedTimeForUser(String UserPIN,TotalClockedTimeTypeConstants DataFlags)
        {

            String queryTimes = qAdapter["ORDERBYUSER"];
            String sqlqueryuse = String.Format(queryTimes, UserPIN);
            DbConnection getcon;
            DbCommand usecmd = GetCommand(out getcon);
            
            usecmd.CommandText = sqlqueryuse;
            TimeSpan runningSum = new TimeSpan(0);
            using (DbDataReader gotreader = usecmd.ExecuteReader())
            {

                if (!gotreader.HasRows)
                    return new TimeSpan(0);
                
                while (gotreader.Read())
                {
                    //get the TotalTime, as long as StartTime is null, to prevent abberations.
                    if (gotreader.IsDBNull(gotreader.GetOrdinal("StartTime")))
                    {
                        if ((DataFlags & TotalClockedTimeTypeConstants.ClockedTime_Completed) == TotalClockedTimeTypeConstants.ClockedTime_Completed)
                        {
                            TimeSpan getts = new TimeSpan((long) gotreader.GetDecimal(gotreader.GetOrdinal("TotalTime")));
                            runningSum += getts;
                        }
                    }
                    else if ((DataFlags&TotalClockedTimeTypeConstants.ClockedTime_Active)==TotalClockedTimeTypeConstants.ClockedTime_Active) 
                    {
                        //if StartTime is not null, they are clocked in, so we want to get the running time of that Order, which will be DBTime()- the StartTime
                        //only allow this to be added if the "Active" flag was passed.
                        DateTime grabStartTime = gotreader.GetDateTime(gotreader.GetOrdinal("StartTime"));
                        TimeSpan getts = DBTime()-grabStartTime;
                        runningSum+=getts; 




                    }



                }

                gotreader.Close();
            }
            Trace.TraceInformation("GetTotalClockedTimeForUser returning " + runningSum + " for User PIN:" + UserPIN);
            return runningSum;

            



        }
        public class UserRecord
        {
            public readonly int RecordID;
            public readonly String PINCode;
            public readonly String Username;
            public readonly bool Active;
            public static UserRecord CreateRecord(int pRecordID,DataLayer usedatabase)
            {
                

                //String recid = pRecordID;
                DbConnection usecon = usedatabase.GetConnection();
                DbCommand usecmd = usecon.CreateCommand();
                //select * from the record ID...
                usecmd.CommandText = String.Format("SELECT * FROM USERS WHERE `RecordID`=\"{0}\"", pRecordID);
                using (DbDataReader execreader = usecmd.ExecuteReader())
                {
                    if(execreader.HasRows)
                        if (execreader.Read())
                        {

                            //RecordID, Active, UserName, PINCode...
                            String uname = execreader.GetString(execreader.GetOrdinal("UserName"));
                            String PIN = execreader.GetString(execreader.GetOrdinal("PINCode"));
                            bool Activestate = (execreader.GetInt16(execreader.GetOrdinal("ACTIVE")) > 0);
                            return new UserRecord(pRecordID, PIN, uname, Activestate);




                        }




                }


                return null;


            }

            public UserRecord(int pRecordID, String pPINCode, String pUsername,bool pActive)
            {
                RecordID = pRecordID;
                PINCode = pPINCode;
                Username = pUsername; 
                Active=pActive;

            }


        }
        
        public DateTime? GetLastStartTimeForOrder(String UserPIN, String OrderID)
        {

            String laststart = qAdapter["LATESTSTARTUSERORDER"];
            DbConnection dbcon;
            DbCommand dcommand = GetCommand(out dbcon);
            dcommand.CommandText=String.Format(laststart,UserPIN,OrderID);
            DateTime currentMax = DateTime.MinValue;
            using (DbDataReader dreader = dcommand.ExecuteReader())
            {
                while (dreader.Read())
                {
                    //StartTime
                    if (!dreader.IsDBNull(dreader.GetOrdinal("StartTime")))
                    {

                        DateTime stime = dreader.GetDateTime(dreader.GetOrdinal("StartTime"));
                        if(stime > currentMax) currentMax=stime;


                    }




                }




            }
            if (currentMax == DateTime.MinValue) return null; //no clocked in orders.
            return currentMax;
            





        }

        public List<UserRecord> GetUserRecords()
        {
            List<UserRecord> createlisting = new List<UserRecord>(); 

            String queryuse = qAdapter["ALLUSERS"];
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = queryuse;
            lock (this)
            {
                using (DbDataReader datreader = makecmd.ExecuteReader())
                {
                    while (datreader.Read())
                    {
                        String grabname = (String) datreader.GetString(datreader.GetOrdinal("UserName"));
                        String grabPIN = (String) datreader.GetString(datreader.GetOrdinal("PINCode"));
                        int grabID = datreader.GetInt32(datreader.GetOrdinal("RecordID"));
                        bool grabactive = datreader.GetInt16(datreader.GetOrdinal("ACTIVE")) != 0;
                        createlisting.Add(new UserRecord(grabID, grabPIN, grabname, grabactive));

                    }
                }
            }

            return createlisting;


        }

        public Dictionary<String, String> GetUsers()
        {
            Dictionary<String, String> createdict = new Dictionary<string, string>();


            String queryuse = qAdapter["ALLUSERS"];
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = queryuse;
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {
                while (datreader.Read())
                {
                    String grabname = (String) datreader.GetString(datreader.GetOrdinal("UserName"));
                    String grabPIN = (String) datreader.GetString(datreader.GetOrdinal("PINCode"));
                    createdict.Add(grabPIN, grabname);

                }
            }


            return createdict;


        }

        //Messaging... for the... well, message data stuff.
        /*`ID` int(11) NOT NULL AUTO_INCREMENT,
`Type` varchar(64) NOT NULL,
`Tstamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Set when record is created. identifies the Timestamp of this message.',
`Message` text NOT NULL COMMENT `Message Text.',*/


        public void LogMessage(String LogID, String MessageType, String Message)
        {
            String addsqlfmt = qAdapter["ADDMESSAGE"];
            String usesql = String.Format(addsqlfmt, LogID, MessageType, Message);
            DbConnection getcon;
            DbCommand execcmd = GetCommand(out getcon);
            execcmd.CommandText=usesql;
            execcmd.ExecuteNonQuery();

            foreach (var loopentry in MessageLogCollections)
            {
                var copied = loopentry.Value;
                UpdateMessages(ref copied, loopentry.Key);


            }



        }
        public void UpdateMessages(ref ObservableCollection<MessageActivityLogItem> updateCollection, String LogID)
        {
            DbConnection usecon;
            DbCommand usecmd = GetCommand(out usecon);
            if(!MessageLogCollections.ContainsValue(updateCollection)) MessageLogCollections.Add(LogID,updateCollection);
            String sqlgetquery = String.Format(qAdapter["GETLOGIDFMT"],LogID);
            usecmd.CommandText = sqlgetquery;
            using (DbDataReader readerobj = usecmd.ExecuteReader())
            {
                while (readerobj.Read())
                {
                    int grabID = readerobj.GetInt32(readerobj.GetOrdinal("ID"));
                    String GrabLogID = readerobj.GetString(readerobj.GetOrdinal("LogID"));
                    String Messagetype = readerobj.GetString(readerobj.GetOrdinal("Type"));
                    DateTime TStamp = readerobj.GetDateTime(readerobj.GetOrdinal("Tstamp"));
                    String Message = readerobj.GetString(readerobj.GetOrdinal("Message"));



                    //determine if updateCOllection has this value...
                    MessageActivityLogItem changeitem;

                    if(null!=(changeitem=updateCollection.FirstOrDefault((w)=>w.MessageID==grabID)))
                    {
                        //there already was an item. remove it...
                        updateCollection.Remove(changeitem);
                        
                        

                    }
                    else
                    {
                        //none, create anew.
                        changeitem = new MessageActivityLogItem(grabID, Message, Messagetype, TStamp,GrabLogID);
                        

                    }
                    changeitem.LogID=GrabLogID;
                    changeitem.LogTime=TStamp;
                    changeitem.Message=Message;
                    changeitem.MessageID=grabID;
                    changeitem.MessageType=Messagetype;

                    updateCollection.Add(changeitem);






                }





            }



        }
        private Dictionary<String,ObservableCollection<MessageActivityLogItem>> MessageLogCollections = new Dictionary<String,ObservableCollection<MessageActivityLogItem>>();
        public ObservableCollection<MessageActivityLogItem> GetLogMessages(String LogID)
        {
           ObservableCollection<MessageActivityLogItem> createcollection;
            if(MessageLogCollections.ContainsKey(LogID))
            {
                createcollection = MessageLogCollections[LogID];
            }

        else
            {


                createcollection = new ObservableCollection<MessageActivityLogItem>();
                MessageLogCollections.Add(LogID, createcollection);
            }
            
            UpdateMessages(ref createcollection, LogID);
            return createcollection;





        }
        /*
        public void AddLogMessage(String LogID, String messagetype,String Message)
        {

            String sqlfmt = "INSERT INTO MESSAGELOG (`LogID`,`Type`,`Message`) VALUES ({0},{1},{2})";
            String execsql = String.Format(sqlfmt, LogID, messagetype, Message);



        }
        */
        public void ExecuteQueryDirect(String queryrun)
        {
            DbConnection dbcon;
            DbCommand usecommand = GetCommand(out dbcon);
            usecommand.CommandText = queryrun;
            usecommand.ExecuteNonQuery();



        }
        public String GetOrderDescription(String OrderID)
        {

            String usequeryfmt = qAdapter["ORDERDESCRIPTIONFMT"];
            String runquery = String.Format(usequeryfmt, OrderID);


            DbConnection getconnect;
            DbCommand grCom = GetCommand(out getconnect);
            grCom.CommandText=runquery;
            return (String)grCom.ExecuteScalar();


        }

        public List<String> GetAllOrderIDs()
        {

            String getallorderIDsQuery = qAdapter["ALLORDERIDS"];
            List<String> resultvalue = new List<string>();
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = getallorderIDsQuery;
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {

                while (datreader.Read())
                {
                    var acquired = datreader.GetString(datreader.GetOrdinal("Order"));
                    if(!resultvalue.Contains(acquired))
                        resultvalue.Add(acquired);



                }


            }
            makecmd.CommandText = qAdapter["DISTINCTORDERDATA"];
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {

                while (datreader.Read())
                {
                    String acquired = datreader.GetString(datreader.GetOrdinal("OrderID"));
                    if(!resultvalue.Contains(acquired))
                        resultvalue.Add(acquired);


                }



            }




            return resultvalue;



        }
        public enum ListViewStringConstants
        {
            Copy_CSV,
            Copy_HTMLFRAGMENT 


        }
        public static void ListViewToClipboard(ListView copythis)
        {
            ListViewToClipboard(copythis, ListViewStringConstants.Copy_CSV);

        }
        public static String ListViewToString(ListView copythis, ListViewStringConstants lvsc)
        {
            return ListViewToString(copythis, lvsc, ",", Environment.NewLine,"Group");

        }

        public static String ListViewToString(ListView copythis, ListViewStringConstants lvsc,String columnseparator,String rowseparator,String GroupColumnName)
        {
            StringBuilder sbuild = new StringBuilder();


            if (copythis.ShowGroups)
            {


                sbuild.Append(GroupColumnName + ",");

            }
            //step one: copy columnheaders
            foreach (ColumnHeader itheader in copythis.Columns)
            {

                sbuild.Append(itheader.Text.Trim());
                sbuild.Append(columnseparator);


            }
            foreach (ListViewItem iterateitem in copythis.Items)
            {
                sbuild.Append(rowseparator);
                if (iterateitem.Group != null)
                    sbuild.Append(iterateitem.Group.Header + ",");
                foreach (ListViewItem.ListViewSubItem loopsubitem in iterateitem.SubItems)
                {

                    sbuild.Append(loopsubitem.Text);
                    sbuild.Append(columnseparator);

                }



            }
            String madestring = sbuild.ToString();
            //remove trailing commas.
            madestring = madestring.Replace(columnseparator + rowseparator, rowseparator);
            //if HTML, we need to make some replacements.
            if (lvsc == ListViewStringConstants.Copy_HTMLFRAGMENT)
            {

                String nativeHTMLString =
@"Version:0.9
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
StartSelection:<<<<<<<3
EndSelection:<<<<<<<4
<!DOCTYPE>
<html>
<head>
<title>HTML Copy paste from BCJobClock</title>
</head>
<body>
<!-- StartFragment -->
";

                madestring = "<table><tr>" + madestring + "</tr></table>";
                madestring = madestring.Replace(Environment.NewLine, "</tr>" + rowseparator + "<tr>");
                madestring = madestring.Replace(columnseparator, "</td><td>");

                nativeHTMLString = nativeHTMLString + madestring +
                    @"<!-- EndFragment -->
</body></html>";

                string utf8EncodedHTMLString = Encoding.GetEncoding(0).GetString(Encoding.UTF8.GetBytes(nativeHTMLString));


                StringBuilder sb = new StringBuilder();
                sb.Append(utf8EncodedHTMLString);
                sb.Replace("<<<<<<<1",
                (utf8EncodedHTMLString.IndexOf("<HTML>") + "<HTML>".Length).ToString("D8"));
                sb.Replace("<<<<<<<2",
                (utf8EncodedHTMLString.IndexOf("</HTML>")).ToString("D8"));
                sb.Replace("<<<<<<<3",
                (utf8EncodedHTMLString.IndexOf("<!--StartFragment -->") + "<!--StartFragment -->".Length).ToString("D8"));
                sb.Replace("<<<<<<<4",
                (utf8EncodedHTMLString.IndexOf("<!--EndFragment -->")).ToString("D8"));
                string clipboardString = sb.ToString();


                //Clipboard.SetText(clipboardString, TextDataFormat.Html);
                return clipboardString;

            }

            return madestring;
            
        }
        public static String ProperCase(String propcaseit)
        {
            string rtext = "";
            try
            {

                System.Globalization.TextInfo tinfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
                rtext = tinfo.ToTitleCase(propcaseit);
                return rtext;
            }
            catch
            {
                return propcaseit;
            }



        }

        public static void ListViewToClipboard(ListView copythis,ListViewStringConstants lvsc)
        {




            Clipboard.SetText(ListViewToString(copythis,lvsc));


        }

        public Dictionary<int, bool> GetUserActiveStates()
        {
            String usequery = "SELECT * FROM USERS";
            DbConnection getcon;
            DbCommand usecmd = GetCommand(out getcon);
            usecmd.CommandText = usequery;

            Dictionary<int, bool> makedict = new Dictionary<int, bool>();


            using (DbDataReader dread = usecmd.ExecuteReader())
            {
                while (dread.Read())
                {

                    int addint = dread.GetInt32(dread.GetOrdinal("RecordID"));
                    bool usebool = dread.GetInt16(dread.GetOrdinal("ACTIVE")) != 0;

                    makedict.Add(addint, usebool);



                }




            }
            return makedict;





        }

        public void SetUserActive(String UserPIN, bool Activestate)
        {
            String Queryfmt = qAdapter["SETUSERACTIVE"];
            //PINCode,Active...
            string useactive = Activestate ? "1" : "0";
            DbConnection getcon;
            DbCommand usecmd = GetCommand(out getcon);


            usecmd.CommandText = String.Format(Queryfmt, UserPIN, Activestate);
            usecmd.ExecuteNonQuery(); 





        }

        public bool IsUserActive(String UserPIN)
        {

            //GetUserActive, first param is USERPIN.

            String queryfmt = qAdapter["GETUSERACTIVE"];
            String useq = String.Format(queryfmt, UserPIN);

            DbConnection usecon;
            DbCommand usecmd = GetCommand(out usecon);
            usecmd.CommandText = useq; 
            using (DbDataReader ddr = usecmd.ExecuteReader())
            {
                
                if(!ddr.HasRows)
                    return false; //hmm, User wasn't there... so I guess they can't be active.

                else
                {
                    ddr.Read();


                    return ddr.GetInt16(ddr.GetOrdinal("ACTIVE")) != 0;




                }




            }


        }
    /// <summary>
    /// retrieves the elapsed Times for Users and Orders
    /// </summary>
    /// <param name="dr"></param>
    /// <param name="orderTimes">resulting dictionary of order times, indexed by RO#. Values</param>
    /// <param name="userTimes"></param>
    /// <remarks></remarks>
        public void GetElapsedData(DateRange dr, out Dictionary<String, TimeSpan> orderTimes,out Dictionary<String,TimeSpan> userTimes )
        {

            Dictionary<String, Stack<DateTime>> StartTimeOrderStack = new Dictionary<string, Stack<DateTime>>();
            Dictionary<String, Stack<DateTime>> StartTimeUserStack = new Dictionary<string, Stack<DateTime>>();
            Dictionary<String, TimeSpan> orderresult = new Dictionary<string, TimeSpan>();
            Dictionary<String, TimeSpan> userresult = new Dictionary<string, TimeSpan>();
            TimeSpan PrevOrderChunk = new TimeSpan();
            TimeSpan PrevUserChunk = new TimeSpan();
            List<ClockDataEntry> clockdata = new List<ClockDataEntry>();
            String clockedorder = String.Format("SELECT * FROM `CLOCKED` ORDER BY `STAMP` WHERE `STAMP`<\"{0}\" AND `STAMP`>\"{1}\"",dr.StartTime.getSQLfmt(),dr.EndTime.getSQLfmt());
            DbConnection dbcon;
            DbCommand dcmd = GetCommand(out dbcon);

            lock (this)
            {
                using(DbDataReader dreader = dcmd.ExecuteReader())
                {
                    while (dreader.Read())
                    {
                        int get_ID = dreader.GetInt32(dreader.GetOrdinal("ID"));
                        String get_PIN = dreader.GetString(dreader.GetOrdinal("UserPIN"));
                        String get_Order = dreader.GetString(dreader.GetOrdinal("OrderID"));
                        DateTime get_STAMP = dreader.GetDateTime(dreader.GetOrdinal("STAMP"));
                        String get_Event = dreader.GetString(dreader.GetOrdinal("EventType"));
                        if (!orderresult.ContainsKey(get_Order)) orderresult.Add(get_Order, new TimeSpan());
                        if (!userresult.ContainsKey(get_PIN)) userresult.Add(get_PIN, new TimeSpan());
                        if (!StartTimeOrderStack.ContainsKey(get_Order)) StartTimeOrderStack.Add(get_Order, new Stack<DateTime>());
                        if (!StartTimeUserStack.ContainsKey(get_PIN)) StartTimeUserStack.Add(get_PIN, new Stack<DateTime>());
                        clockdata.Add(new ClockDataEntry(get_ID, get_PIN, get_Order, get_STAMP, get_Event));
                        
                    }

                }


            }

            foreach(var iterate in (from m in clockdata orderby m.STAMP ascending select m))
            {
                if (iterate.EventType == "IN")
                {
                    StartTimeOrderStack[iterate.OrderID].Push(iterate.STAMP);
                    StartTimeUserStack[iterate.PIN].Push(iterate.STAMP);

                }
                else if (iterate.EventType == "OUT")
                {
                    //if the stack is empty, this has no corresponding IN.
                    //this can happen if there was a clock-in before the daterange and we find the out entry after.
                    if (StartTimeOrderStack[iterate.OrderID].Count == 0)
                    {
                        //add the interval from the start of the range to this value.
                        //subtract prevorder chunk so we only end up with the "largest" value, if there are multiple time outs.
                        orderresult[iterate.OrderID] += (iterate.STAMP - dr.StartTime)-PrevOrderChunk;

                        PrevOrderChunk = (iterate.STAMP - dr.StartTime);
                    }
                    else
                    {
                        DateTime dopop = StartTimeOrderStack[iterate.OrderID].Pop();
                        if (StartTimeOrderStack[iterate.OrderID].Count == 0)
                        {
                            //if it is empty add the interval to the timespan.
                            orderresult[iterate.OrderID] += (iterate.STAMP - dopop);


                        }
                    }
                    //if the user stack is empty, then there is no corresponding user time in event for this item.
                    if (StartTimeUserStack[iterate.PIN].Count == 0)
                    {
                        //add the interval from the start of the range to this value.
                        userresult[iterate.PIN] += (iterate.STAMP - dr.StartTime)-PrevUserChunk;
                        PrevUserChunk = (iterate.STAMP - dr.StartTime);

                    }
                    else
                    {


                        DateTime dopopu = StartTimeUserStack[iterate.PIN].Pop();
                        if (StartTimeUserStack[iterate.PIN].Count == 0)
                        {
                            userresult[iterate.PIN] += (iterate.STAMP - dopopu);


                        }
                    }


                }


            }

            //lastly, we want to get the earliest Time in the StartTimeUserStack and StartTimeOrderStack, for each Order and User,
            //and add it to the respective orderresult and userresult entries. (to account for cases where we have a Time IN but no time out because of the daterange)

            //iterate through the KeyValuePairs...
            if (StartTimeOrderStack.Count > 0)
            {
                foreach (var kvp in StartTimeOrderStack)
                {
                    //find the earliest Time in this stack.
                    DateTime currearliest = DateTime.MaxValue;
                    foreach (var iterateearly in kvp.Value)
                    {
                        if (iterateearly < currearliest)
                            currearliest = iterateearly;
                    }
                    orderresult[kvp.Key] += (dr.EndTime-currearliest);


                }
            }

            //repeat for the UserStack...

            if (StartTimeUserStack.Count > 0)
            {
                foreach (var kvp in StartTimeUserStack)
                {
                    //find earliest time...
                    DateTime earlyusertime = DateTime.MaxValue;
                    foreach (var iterateearly in kvp.Value)
                    {
                        if (iterateearly < earlyusertime)
                            earlyusertime = iterateearly;

                    }
                    userresult[kvp.Key] += (dr.EndTime - earlyusertime);


                }



            }

            //and return the results.

        orderTimes = orderresult;
            userTimes = userresult;
        }

        



        public List<BasicUserOrderData> GetUserDataFromOrder(String OrderID)
        {
            
            return GetUserDataFromOrder(OrderID, false);
        }
        public List<BasicUserOrderData> GetUserDataFromOrder(String OrderID, bool flActiveOnly)
        {
            return GetUserDataFromOrder(OrderID, flActiveOnly, DateTime.MinValue, DateTime.MaxValue);

        }

        public List<BasicUserOrderData> GetUserDataFromOrder(String OrderID, bool flActiveOnly,DateTime StartRange,DateTime EndRange)
        {
            List<BasicUserOrderData> BuildList = new List<BasicUserOrderData>();

            String querySQLfmt = qAdapter["USERDATAFROMORDER"];
            String QuerySQL = String.Format(querySQLfmt, OrderID);
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = QuerySQL;
            List<Object[]> createlist = new List<Object[]>();
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {

                if (datreader.HasRows)
                {

                    while (datreader.Read())
                    {


                        //acquire field names. UserName, PINCode, TotalTime, StartTime, Order
                        String sUserName = (String) datreader.GetString(datreader.GetOrdinal("UserName"));
                        String sUserPIN = (String) datreader.GetString(datreader.GetOrdinal("PINCode"));
                        TimeSpan tTotalTime = datreader.GetTimeSpan(datreader.GetOrdinal("TotalTime"));
                        bool stimenull = datreader.IsDBNull(datreader.GetOrdinal("StartTime"));
                        DateTime tStartTime=new DateTime();
                        if (!stimenull)
                            tStartTime = datreader.GetDateTime(datreader.GetOrdinal("StartTime"));

                        String sOrderID = datreader.GetString(datreader.GetOrdinal("Order"));


                        //stimenull= true for inactive orders
                        //flActiveOnly means to return everything that doesn't give us stime null.
                        bool doadd=false;

                        doadd = flActiveOnly;


                        if (!stimenull)
                        {

                            doadd = doadd && (tStartTime > StartRange && tStartTime < EndRange);


                        }

                        doadd = !(flActiveOnly && stimenull);


                        if (doadd)
                        {
                            createlist.Add(new Object[] {sUserPIN, tTotalTime, tStartTime, sOrderID});

                            BasicUserOrderData createorderdata = new BasicUserOrderData(sOrderID, sUserName, tTotalTime,
                                                                                        stimenull);
                            BuildList.Add(createorderdata);
                        }

                    }

                    
                    
                    Debug.Print("Returning List of users for OrderID " + OrderID + ":" + BuildList.ToString());

                    return BuildList;
                }
                else
                {
                    return new List<BasicUserOrderData>();
                }
            }


        }
        public DateRange[] GetUserWorkOrderRanges(String UserID, String OrderID)
        {

            String RelevantRecordsSQL = qAdapter["USERTOTALTIMEQUERY"];
            //select all records in the CLOCKED table (which is used to store all clock-in/out events) that have the given UserPIN and OrderID.

            DbConnection dcon;
            DbCommand usecmd = GetCommand(out dcon);


            usecmd.CommandText = String.Format(RelevantRecordsSQL, UserID, OrderID);

            DateTime usecurrent = DBTime();
            //NOTE: ASSUMPTION: we assume, naturally, that a user cannot clock into a order twice. This could be the case as far as the CLOCKED table, though in case of DB corruption
            //or other circumstances, so if that happens we will throw a exception.
            //DatabaseStructureCorruptedException
            List<DateRange> buildlisting = new List<DateRange>();
            DateTime? getstarttime = null, getendtime = null;
            TimeSpan accumulator = new TimeSpan();
            using (DbDataReader readdata = usecmd.ExecuteReader())
            {
                DateTime laststamp = new DateTime();

                while (readdata.Read())
                {
                    DateTime getstamp = readdata.GetDateTime(readdata.GetOrdinal("STAMP"));
                    laststamp = getstamp;
                    String etype = readdata.GetString(readdata.GetOrdinal("EventType"));


                    if (etype == "IN")
                    {
                        getstarttime = getstamp;

                    }
                    else
                    {

                        if (getstarttime != null)
                        {
                            getendtime = getstamp;
                            DateRange buildrange = new DateRange(getstarttime.Value, getendtime.Value);
                            buildlisting.Add(buildrange);
                            getstarttime = null;
                            //accumulator += buildrange.Span;
                        }
                    }



                }

                if (getstarttime != null)
                {
                    if (laststamp > usecurrent)
                    {
                        Debug.Print("laststamp > DBTime()");
                    }
                    DateRange buildrange2 = new DateRange(laststamp, usecurrent);
                    Debug.Print(buildrange2.ToString());
                    buildlisting.Add(buildrange2);
                    //accumulator+=buildrange2.Span;
                    //TimeSpan addvalue = (DBTime() - getstarttime.Value).Duration();
                    // Debug.Print("adding accumulated value:" + addvalue.ToString());

                }
            }


            return buildlisting.ToArray();


        }



        public TimeSpan GetUserTotalTimeOnOrder(String UserID,String OrderID)
        {
            DateRange[] discardout;


            return GetUserTotalTimeOnOrder(UserID, OrderID,out discardout);


        }

        public TimeSpan GetUserTotalTimeOnOrder(String UserID, String OrderID, out DateRange[] ranges)
        {

            String RelevantRecordsSQL = qAdapter["USERTOTALTIMEQUERY"];
            //select all records in the CLOCKED table (which is used to store all clock-in/out events) that have the given UserPIN and OrderID.

            DbConnection dcon;
            DbCommand usecmd = GetCommand(out dcon);


            usecmd.CommandText = String.Format(RelevantRecordsSQL,UserID,OrderID);

            DateTime usecurrent = DBTime();
            if (UserID.Equals("0812"))
            {
                Debug.Print("bug...");

            }
            List<DateRange> buildlisting = GetUserWorkOrderRanges(UserID, OrderID).ToList();
            long sumticks = 0;
            foreach (var blisting in buildlisting)
            {

                sumticks+=blisting.Span.Ticks;

            }
            ranges = buildlisting.ToArray();
            TimeSpan totalresult =  new TimeSpan(sumticks);
            return totalresult;
            //return accumulator;


        }

        public List<String> GetUserOrders(String forUser)
        {
            return GetUserOrders(forUser, false);

        }
        public DateTime? GetWorkOrderDate(String OrderID)
        {
            String SqlFindEarliestSQL = qAdapter["WORKORDERDATE"];
            DbConnection usecon;
            DbCommand usecmd = GetCommand(out usecon);
            usecmd.CommandText = String.Format(SqlFindEarliestSQL, OrderID);
            DateTime? gotearliest = (DateTime?)usecmd.ExecuteScalar();

            return gotearliest;
        }
        public List<String> GetWorkOrderActiveWorkers(String OrderID)
        {
            //get the PINs of all workers who are clocked in on this order.
            //a Order is "clocked in" when the StartTime value is NOT null

            String sqlactiveworkers = qAdapter["ACTIVEWORKERS"];
            DbConnection gotconnection;
            DbCommand useexec = GetCommand(out gotconnection);

            useexec.CommandText = String.Format(sqlactiveworkers, OrderID);

            List<String> buildreturn = new List<string>();


            using (DbDataReader dreader = useexec.ExecuteReader())
            {

                while (dreader.Read())
                {

                    buildreturn.Add(dreader.GetString(dreader.GetOrdinal("UserPinCode")));

                }

                return buildreturn;
            }

            


        }
        public String UserNameFromPIN(String PINCode)
        {

            String sqlget =qAdapter["USERNAMEFROMPIN"];
            DbConnection getcon;
            DbCommand usecommand = GetCommand(out getcon);
            usecommand.CommandText = String.Format(sqlget,PINCode);
            return (String)usecommand.ExecuteScalar();



        }

        public List<String> GeWorkOrderActiveWorkerNames(String OrderID)
        {
            //retrieves a list of Names corresponding to the active (clocked-in) workers on this order.
            return (from m in GetWorkOrderActiveWorkers(OrderID) select UserNameFromPIN(m)).ToList();
        }
        public TimeSpan GetOrderTime(String OrderID)
        {
            //This retrieves the total time the Order was active.
            //this does NOT count overlapping users working on the order.


            //Step one, get all records relating to this order.
            //String relatedsql = "SELECT * FROM CLOCKED WHERE `OrderID`=\"{0}\" ORDER BY `STAMP` ASC ";
            //String execsql = String.Format(relatedsql, OrderID);

            var result  = GetOrderActiveIntervals(OrderID);

            TimeSpan buildsum = new TimeSpan();
            foreach (DateRange looptime in result)
            {

                buildsum+=looptime.Span;

            }
            return buildsum;

        }

        private DateRange[] GetOrderActiveIntervals(string OrderID)
        {
            DbConnection dcon;
            DbCommand dcmd = GetCommand(out dcon);

            //dcmd.CommandText=execsql;


            Dictionary<String,String> Allusers  =GetUsers();

            
            //with each User/Pin Pair, we need to do the following:
            Dictionary<String, List<DateRange>> BuildRanges = new Dictionary<string, List<DateRange>>();
            foreach (var loopuserpin in Allusers)
            {

                //get ALL the data  from CLOCKED with that UserPIN and OrderID. Ordered by STAMP, so that
                //successive records are newer as we traverse them.

                //create the item for this dictionary element.
                List<DateRange> builddates = new List<DateRange>();

                DateTime starting, ending;
                String buildsql =qAdapter["ORDERCLOCKED"];
                String execsql = String.Format(buildsql, loopuserpin.Key, OrderID);
                dcmd.CommandText = execsql;



                using (DbDataReader readerloop = dcmd.ExecuteReader())
                {
                    //this will be a successive list of every CLOCK event by that user.
                    //for "IN" events, we set the "Start" time. for  "OUT" events, we set the END time, and then create a new entry we than
                    //add to builddates.
                    DateTime usestart=new DateTime(), useend;
                    while (readerloop.Read())
                    {
                        //ID,UserPIN,OrderID,STAMP,EventType
                        String getetype = readerloop.GetString(readerloop.GetOrdinal("EventType"));
                        if (getetype == "IN")
                        {
                            usestart = readerloop.GetDateTime(readerloop.GetOrdinal("STAMP"));


                        }
                        else if (getetype == "OUT")
                        {
                            useend = readerloop.GetDateTime(readerloop.GetOrdinal("STAMP"));

                            builddates.Add(new DateRange(usestart, useend));


                        }







                    }
                    BuildRanges.Add(loopuserpin.Key, builddates);






                }
            






            }

            List<DateRange> buildlist = new List<DateRange>();
            foreach (var loopit in BuildRanges)
            {

                buildlist.AddRange(loopit.Value);



            }

            //coalesce.
            DateRange[] coalesced = DateRange.CoalesceRanges(buildlist.ToArray());
            return coalesced;

        }

        public Dictionary<String,DateTime> GetWorkOrderDates()
        {
            //task: retrieve all the WorkOrderDates; the resulting dictionary object will have one item for every
            //unique WorkOrder, with the value of the Earliest "FirstDate" found in the db with that order.

            //first, get all the Orders.
            var gotorders = GetAllOrderIDs();
            Dictionary<String, DateTime> BuildResult = new Dictionary<string, DateTime>();
            //iterate through the list, and for each one, find the earliest FirstDate in the DB.

           
          



            foreach (String looporderid in gotorders)
            {
                //find the earliest date...
                DateTime? gotearliest = GetWorkOrderDate(looporderid);
                if(gotearliest !=null)
                    BuildResult.Add(looporderid, gotearliest.Value);



            }




            return BuildResult;



        }
        public List<String> GetUserFinishedOrders(String foruser)
        {
            //the finished orders will be all orders sans the active orders.
            var allorders = GetUserOrders(foruser);
            var activeorders = GetUserOrders(foruser, true);


            foreach (var removeactive in activeorders)
            {
                allorders.Remove(removeactive);
            }

            return allorders;



        }

        public List<String> GetUserOrders(String forUser,bool onlyactive)
        {
            String userpin = GetUserPIN(forUser);
            String UserOrdersQuery = "";
            if (onlyactive)
            {
                UserOrdersQuery = qAdapter["USERORDERQUERYACTIVE"];
            }
            else
            {
                UserOrdersQuery = qAdapter["USERORDERQUERYALL"];
            }
            DbConnection getcon;
            DbCommand makecmd = GetCommand(out getcon);
            makecmd.CommandText = String.Format(UserOrdersQuery, userpin);
            using (DbDataReader datreader = makecmd.ExecuteReader())
            {
                //we want to return the Order IDs that this user is involved in. Create a list...

                if (datreader.HasRows)
                {

                    List<String> buildlist = new List<string>();
                    while (datreader.Read())
                    {
                        String thisorder = (String) datreader["Order"];
                        //This could probably be better implemented with something in the SQL query (SELECT DISTINCT) but admittedly my 
                        //confidence with SQL is rather low...
                        if (!buildlist.Contains(thisorder))
                            buildlist.Add(thisorder);


                    }

                    return buildlist;


                }
                else
                {
                    return new List<string>();
                }
            }



        }
        public void SeedDB()
        {
            //seeds the DB with some test data
            String[] addnames = new string[] { "Mike", "Joe", "Paul", " Bill", "Rolfe" };
            String[] addPINs = new string[] { "4554", "1223", "9665", "1224", "9887" };


            for (int i = 0; i < addnames.Length; i++)
            {
                AddUser(addnames[i], addPINs[i]);


            }


        }

    

        private DbCommand GetCommand(out DbConnection getcon)
        {
            getcon = ConnectDB();

            return getcon.CreateCommand();
        }


        public DataLayer(ConnectionTypeConstants contype)
        {
            Debug.Print("DataLayer Initializing...");


            WebPermission wp = new WebPermission();
            wp.Demand();

            switch (Configuration.DatabaseType.ToUpper().Trim())
            {
                case  "":
                case "MYSQL":
                    qAdapter = new MySQLQueryAdapter();
                    break;
                case "SQL":
                case "SQL SERVER":
                    qAdapter = new SQLServerQueryAdapter();
                    break;
            }
            Debug.Assert(qAdapter != null);
           // if (Configuration.PurgeLogOnStartup)
           // {
                String dbgfile = getDebugOutputFilepath();
                if (File.Exists(dbgfile))
                    File.Delete(dbgfile);


            //}
                WriteDebugMessage("--DataLayer Initializing-" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "--");
            DatabaseName = Configuration.DatabaseName;
            try
            {
                CurrentConString = buildConnectionString(contype);
            }
            catch (frmPasswordPrompt.CredentialsNotGivenException exx)
            {
                throw;


            }
            Debug.Print("Using ConnectionString:" + CurrentConString);

            //initialize our Type Manager.
            String[] lookfolders = new String[] { JobClockConfig.GetExportLibFolder()};

            mtypemanager = new MultiTypeManager(lookfolders,
                new Type[] { typeof(BaseDataExporter) }, null, 
                new Nullcallback(), null, 
                new Assembly[] { Assembly.GetCallingAssembly() });
             



        }
        private void SwapDates(ref DateTime dt1, ref DateTime dt2)
        {

            DateTime temp = dt1;
            dt1=dt2;
            dt2=temp;



        }

        
        public DbConnection GetConnection() //friendlier name...
        {
            return ConnectDB();

        }
        DbConnection CacheCon = null;
        /// <summary>
        /// returns the number of active Orders for the given user PIN.
        /// </summary>
        /// <param name="byUserPIN"></param>
        /// <returns></returns>
        public int GetNumActiveOrders(String byUserPIN)
        {
            String sqlfmt =qAdapter["NUMACTIVEORDERS"];
            String usequery = String.Format(sqlfmt, byUserPIN);

            DbConnection grabcon;
            DbCommand gotcmd;
            gotcmd = GetCommand(out grabcon);
            gotcmd.CommandText=usequery;

            return int.Parse((String)gotcmd.ExecuteScalar());
            




        }
        public enum ConnectionTypeConstants
        {
            Connection_Client,
            Connection_Admin ,
            Connection_Monitor

        }
        public delegate void DbConnectionComplete(DbConnection connectionresult);
        System.Threading.Thread DbconnectThread;
        frmWaitMessage WaitingForm;
        private void DbConnection_Async(DbConnectionComplete resultconnection )
        {
            Debug.Print("DbConnectionAsync");
            //connects to the DB asynchronously, calling the specified callback when the DB connection attempt succeeds... or fails.
            WaitingForm = new frmWaitMessage("Connecting to Database...");
            
            WaitingForm.Show();
            Application.DoEvents();
           
            DbconnectThread = new Thread(DbConnectionThreadRoutine);
            DbconnectThread.Start(resultconnection);

        }
        private bool waitingconnection=false;
        private DbConnection Acquiredconnection;
        public DbConnection DbConnection_Progress()
        {
            Debug.Print("DbConnection_Progress");
            waitingconnection=true;

            DbConnection_Async(AwaitingConnection);
           
            while(waitingconnection)
            {

                Thread.Sleep(100);

            }

            return Acquiredconnection;



        }
        public void SetConnectionString(String setcon)
        {

            CurrentConString = setcon;

        }

        private void AwaitingConnection(DbConnection conobject)
        {
            Debug.Print("AwaitingConnection");
            Acquiredconnection=conobject;
            waitingconnection=false;

        }

        private void DbConnectionThreadRoutine(Object connectdelegate)
        {
            Debug.Print("DbConnectionThreadRoutine");
            //call the same routine we would synchronously...
            try
            {
                DbConnection createdconnection = ConnectDB();

                DbConnectionComplete castroutine = (DbConnectionComplete)connectdelegate;
                castroutine(createdconnection);

                WaitingForm.Invoke((MethodInvoker)(() => WaitingForm.Close()));
            }
            catch (frmPasswordPrompt.CredentialsNotGivenException exx)
            {
                throw;


            }
            catch (Exception exx)
            {
                DbConnectionComplete castroutine = (DbConnectionComplete)connectdelegate;
                castroutine(null);
                WaitingForm.Invoke((MethodInvoker)(() =>
                                                       {
                                                           WaitingForm.SetException(exx);
                                                           WaitingForm.Close();

                                                       }));

            }



        }
        public String GetMDBFilePath()
        {
            String buildit = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String AppFolder = buildit;
            buildit = Path.Combine(buildit, "BCJobClock\\blankdb.mdb");


            //copy that file to a new file.
            String createfile = Path.Combine(buildit,"BCJobClock\\JobClock.mdb");
            if(!File.Exists(createfile))
            {

                File.Copy(buildit, createfile);

            }

            return createfile;


        }
        public String CreateMDBConnectionString()
        {
            String gotMDBPath = GetMDBFilePath();
            return CreateMDBConnectionString(gotMDBPath);



        }

        public String CreateMDBConnectionString(String Forfilename)
        {
            //ConnectionString=Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%APPDATA%\BCJobClock\JobClockData.mdb
            String fmt = "ConnectionString=Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}";
            return String.Format(fmt, Forfilename);




        }

        private DbConnection CreateInnerConnection(String ConString)
        {
            String checktype = Configuration.DatabaseType;
            DbConnection madecon=null;
            switch (checktype.ToUpper().Trim())
            {
                case "MYSQL":
                    madecon = new MySqlConnection(ConString);
                    break;
                case "SQLSERVER":
                    madecon = new SqlConnection(ConString);
                    break;
                case "ODBC":
                    madecon = new OdbcConnection(ConString);
                    break;
                case "ACCESS":
                case "JET":
                    madecon = new OleDbConnection(ConString);
                    break;


            }
            return madecon;

        }

        private DbConnection ConnectDB()
        {
            if (CacheCon == null)
            {
                try
                {

                    WriteDebugMessage("Connecting to " + CurrentConString);


                    //CacheCon = new MySqlConnection(CurrentConString);
                    CacheCon = CreateInnerConnection(CurrentConString);

                    CacheCon.Open();




                }
               
                catch (Exception ex)
                {
                    //throw new ApplicationException("Database connection failed- Connection String:\"" + CurrentConString + "\"; inner exception Message:\"" + ex.Message);
                    WriteDebugMessage("Exception:" + GetExceptionData(ex));
                    throw;
                }

                //Also, select the JobClockDB, for MySQL connections...
                //add a handler to the INI file, this is an attempt to allow for configuration information to be persisted back and forth to and from the db.
                //JobClockConfig.OurINI.BeforeRetrieveValue += new INIFile.RetrieveValueFunc(OurINI_BeforeRetrieveValue);
                //JobClockConfig.OurINI.BeforeSetValue += new INIFile.SetValueFunc(OurINI_BeforeSetValue);
                if (Configuration.DatabaseType.ToUpper().Trim() == "MYSQL")
                {
                    String seldb = Configuration.DatabaseName;
                    DbCommand seldbcmd = CacheCon.CreateCommand();
                    seldbcmd.CommandText = "USE `" + seldb + "`";
                    try
                    {
                        seldbcmd.ExecuteNonQuery(); //yip!

                        //clear empty messages.
                        String clearempty = "DELETE FROM MESSAGELOG WHERE TRIM(Message)=\"\"";
                        seldbcmd.CommandText = clearempty;
                        seldbcmd.ExecuteNonQuery();
                        UpdateUserTableAddActiveField();

                     
                      

                    }
                    catch (Exception dbexception)
                    {
                        //exception...
                        WriteDebugMessage("Error USE-ing Database named \"" + seldb + "\"; Error:" + dbexception.Message);

                    }
                }

            }
            else
            {
                //if it's not null, verify that it is still valid, otherwise we could get a "MySQL Server has gone away" exception
                // when an attempt is made to execute.
                //we verify that it is still alive by issuing a simple command (SELECT 1), and catching the error that will occur
                //if the connection is dead.
                try
                {
                    DbCommand issueselect = CacheCon.CreateCommand();
                    issueselect.CommandText = "SELECT 1";
                    issueselect.ExecuteNonQuery();

                }
                catch (MySqlException mse)
                {
                    //bugfix Jan 12 2012 8:15 AM: not responding error
                  if(mse.Message.Contains("gone away"))
                    {
                        //close, and re-open.
                        //we set the value to null and call this routine recursively,haha.
                        CacheCon = null;
                        //CacheCon will now be set in the recursive call, too.
                        return ConnectDB();
                    }
                  else
                  {
                      LogAdmin("Unexpected MySqlException:" + mse.ToString());
                  }

                }

            }
            return CacheCon;
        }

        void OurINI_BeforeSetValue(string SectionName, string ValueName, ref string val)
        {
            //throw new NotImplementedException();
            setConfigSettingDB(SectionName, ValueName, val);
        }

        void OurINI_BeforeRetrieveValue(string SectionName, string ValueName, ref string val)
        {
            
            //attempt to access the DB value instead.
            String testDBvalue = getConfigSettingDB(SectionName, ValueName);
            if (testDBvalue != null)
                val = testDBvalue;


        }
        public static String GetExceptionData(Exception except)
        {
            String returnit = "Type:" + except.GetType().Name;
                
                returnit+= "Message:" + except.Message;
            returnit += "\nSource:" + except.Source;
            returnit += "\nStack Trace:" + except.StackTrace;
            
            if (except.InnerException != null)
                returnit += "\n Inner Exception:\n{" + GetExceptionData(except.InnerException) + "\n}";


            return returnit;

        }

        public String FormatUserOrderList(String UserPIN, List<String> Orders)
        {
            //Formats this users listing of orders, like so:
            //RO#666666 (HH:MM), RO#534435 (HH:MM), etc.
            int i = 0;
            String[] buildarray = new string[Orders.Count];
            foreach (String looporderid in Orders)
            {
                String makeentry;
                makeentry = "RO#" + looporderid + " (";

                TimeSpan gottime = GetUserTotalTimeOnOrder(UserPIN, looporderid);

                String tsstring = FormatTimeSpan(gottime);
                makeentry += tsstring + ")";
                
                buildarray[i] = makeentry;
                i++;

            }

            return String.Join(",", buildarray);




        }

        private String getDebugOutputFilepath()
        {
            String TargetLogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TargetLogPath = Path.Combine(TargetLogPath, "BCJobClock\\BCJobClock_app.log");
            return TargetLogPath;
        }

        public void WriteDebugMessage(String message)
        {

            String TargetLogPath = getDebugOutputFilepath();
            //open this file, append to it...
            FileStream logout = new FileStream(TargetLogPath, FileMode.Append);
            StreamWriter fw = new StreamWriter(logout);

            //fw.WriteLine();

            
            fw.WriteLine(DateTime.Now.ToShortDateString() + ";" + 
                String.Format("{0:00}:{1:00}:{2:00}.{3:00}",DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second,DateTime.Now.Millisecond)
                + message);
            fw.Flush();
            fw.Close();

        }

        public void CreateExcelExport()
        {
            /*The reporting module needs to provide reporting by date first then job number (we call job number: RO#),
 such as RO#0256251 Completed by Tech(s) Mike 00:26, John 00:12 AND by date first then user such as 
 Mike completed: RO#0256251 00:26, RO#0256252 00:14 Total time: 00:40 (laid out in a column format of course,
  or an excel export if that is easier.) It also would be nice to have a text input box for searches by job number also.*/






        }






    }
}
