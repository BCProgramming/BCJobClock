using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{
    /*
       `ScheduleJobID` VARCHAR(256) NOT NULL,
   `UserPIN` text NOT NULL COMMENT 'PIN Code of user for this schedule span',
   `OrderID` text NOT NULL COMMENT 'OrderID of order this user is scheduled for',
   `StartTime` datetime DEFAULT NULL COMMENT 'Starting time of this schedule',
   `Duration` bigint(20) NOT NULL COMMENT 'Total time (in ms) for which this scheduled entry lasts.'

     * */

    /// <summary>
    /// represents a single record within ScheduleJobID.
    /// </summary>
    public class ScheduledJob
    {
        public enum ScheduleJobStatus
        {
            /// <summary>
            /// Indicates this schedule is in the future.
            /// </summary>
            Schedule_Future,
            /// <summary>
            /// indicates this schedule is in progress.
            /// </summary>
            Schedule_InProgress,
            /// <summary>
            /// indicates this schedule is in the past.
            /// </summary>
            Schedule_Past 

        }
        private DataLayer dbObj;
        private String _ScheduledJobID;

        private TimeSpan _Duration;
        private DateTime _StartTime;
        private String _UserPIN;
        private String _OrderID;
        private bool _Dirty = false;
        public ScheduleJobStatus Status {
        get {
            if(DateTime.Now > EndTime)
                return ScheduleJobStatus.Schedule_Past; 
            if(DateTime.Now < StartTime)
                return ScheduleJobStatus.Schedule_Future;
            return ScheduleJobStatus.Schedule_InProgress;
            

    }
}
        public void Refresh()
        {
            DbDataReader getreader = thisJobRow();
            _StartTime = getreader.GetDateTime(getreader.GetOrdinal("StartTime"));
            _Duration = new TimeSpan(getreader.GetInt32(getreader.GetOrdinal("Duration")));
            _UserPIN = getreader.GetString(getreader.GetOrdinal("UserPIN"));
            _OrderID = getreader.GetString(getreader.GetOrdinal("OrderID"));
            _Dirty = false;
        }
        private void Update()
        {
            if (!_Dirty) return;
            //Qlookup.Add("UPDATESCHEDULE","UPDATE ScheduleData SET UserPIN=\"{1}\" SET OrderID=\"{2}\" SET StartTime=\"{3}\" SET Duration=\"{4}\" WHERE ScheduleJobID=\"{0}\"";
            String usequery = String.Format(dbObj.qAdapter["UPDATESCHEDULE"], _ScheduledJobID, _UserPIN, _OrderID, _StartTime.getSQLfmt(), _Duration.Ticks);
            ExecDirect(usequery);
            _Dirty = false;
        }
        ~ScheduledJob()
        {
            if(!_Dirty) Update();

        }
        public TimeSpan Duration { get { return _Duration; } set { _Duration = value; _Dirty = true; } }
        public String UserPIN { get { return _UserPIN; } set { _UserPIN = value; _Dirty = true; } }
        public String OrderID { get { return _OrderID; } set { _OrderID = value; _Dirty = true; } }
        public DateTime EndTime
        {
            get { return _StartTime + _Duration; }
            set
            {
                _Duration = (value - _StartTime);
                _Dirty = true;

            }
        }
    
        public DateTime StartTime
        {
            get
            {

                return _StartTime;
            }
            set
            {
                _StartTime = value;
                _Dirty = true;
            }
        }
        private void ExecDirect(String query)
        {
            DbConnection gotcon;
            DbCommand gotcommand = dbObj.GetCommand(out gotcon);
            gotcommand.CommandText = query;
            gotcommand.ExecuteNonQuery();

        }
        private DbDataReader thisJobRow()
        {
            DbConnection gotcon;
            DbCommand gotcommand = dbObj.GetCommand(out gotcon);
            gotcommand.CommandText = String.Format(dbObj.qAdapter["SPECIFICSCHEDULE"], _ScheduledJobID);
            DbDataReader retrieve = gotcommand.ExecuteReader();
            if (!retrieve.HasRows)
            {

                throw new DatabaseException("ScheduledJob could not retrieve schedule data...");
            }
            retrieve.Read();
            return retrieve;

        }
        public ScheduledJob(DataLayer pDB, String JobID, String pUserPIN, String pOrderID, DateTime pStartTime, TimeSpan pDuration)
        {
            dbObj = pDB;
            _ScheduledJobID = JobID;
            _UserPIN = pUserPIN;
            _OrderID = pOrderID;
            _StartTime = pStartTime;
            _Duration = pDuration;

            _Dirty = false;

            String usequery = String.Format(dbObj.qAdapter["SPECIFICSCHEDULE"], JobID);
            DbConnection gotcon;
            DbCommand gotcommand = dbObj.GetCommand(out gotcon);
            gotcommand.CommandText = usequery;
            DbDataReader dreader = gotcommand.ExecuteReader();
            if (!dreader.HasRows)
            {
                //record not present, so create it now.
                dreader.Close();
                //Qlookup.Add("INSERTSCHEDULE", "INSERT INTO ScheduleData (`ScheduleJobID`,`UserPIN`,`OrderID`,`StartTime`,`Duration`) VALUES (\"{0}\",\"{1}\",\"{2}\",\"{3}\")");
                gotcommand.CommandText = String.Format(dbObj.qAdapter["INSERTSCHEDULE"], JobID, _UserPIN, OrderID, _StartTime.getSQLfmt(), _Duration.Ticks);
                gotcommand.ExecuteNonQuery();

            }
            dreader.Close();

        }
        public ScheduledJob(DataLayer pDB,String JobID)
        {
            _ScheduledJobID = JobID;
            dbObj = pDB;
            String usequery = String.Format(dbObj.qAdapter["SPECIFICSCHEDULE"],JobID);
            DbConnection gotcon;
            DbCommand gotcommand = dbObj.GetCommand(out gotcon);
            gotcommand.CommandText = usequery;
            DbDataReader dreader = gotcommand.ExecuteReader();
            if (!dreader.HasRows)
            {
                //record not present, so create it now.
                dreader.Close();
                //Qlookup.Add("INSERTSCHEDULE", "INSERT INTO ScheduleData (`ScheduleJobID`,`UserPIN`,`OrderID`,`StartTime`,`Duration`) VALUES (\"{0}\",\"{1}\",\"{2}\",\"{3}\")");
                gotcommand.CommandText = String.Format(dbObj.qAdapter["INSERTSCHEDULE"],JobID, "NULL", "NULL", "NULL", "0");
                gotcommand.ExecuteNonQuery();

            }
            dreader.Close();
            Update();

        

        }





    }
    /// <summary>
    /// represents a scheduled entry for a single user.
    /// </summary>
    public class Schedule
    {

        private String _UserPIN;
        private String _ScheduleID;

        /// <summary>
        /// PIN of the user for whom this schedule data applies.
        /// </summary>
        public String UserPIN { get { return _UserPIN; } set { _UserPIN = value; } }
        /// <summary>
        /// PIN/ID of this Schedule, used as lookup into DB.
        /// </summary>
        public String ScheduleID { get { return _ScheduleID; } set { _ScheduleID = value; } }


    }
}
