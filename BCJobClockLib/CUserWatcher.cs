using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace BCJobClockLib
{
    /// <summary>
    /// class for monitoring the database for changes to the list of users and their orders.
    /// </summary>
    public class CUserDataWatcher
    {


        public class UserDataClockStateDifference
        {
            public List<String> BeforeState;
            public List<String> AfterState;

            public UserDataClockStateDifference(List<String> pBefore, List<String> pAfter)
            {
                BeforeState=pBefore;
                AfterState=pAfter;


            }
        }

        /// <summary>
        /// Last User list retrieved from the db. This is updated with each refresh.
        /// </summary>
        private List<DataLayer.UserRecord> _LastUserList = null;
        private Dictionary<int,List<String>> _LastOrderState=null; //index is RecordID of user; Value is list of orders.
        private Dictionary<int,bool> _LastActiveState=null; //index is RecordID of user, value is whether they were Active.
        private int _RefreshTimems;
        private DataLayer database;
        private Timer CheckChanges = null;
        
        public delegate void WatchEventFunc(CUserDataWatcher sender, ChangeInfoConstants changetype, DataLayer.UserRecord oldRecord, DataLayer.UserRecord newRecord,Object extradata);
        public event WatchEventFunc WatchEvent;


        protected void InvokeWatchEvent(ChangeInfoConstants changetype, DataLayer.UserRecord oldRecord, DataLayer.UserRecord newRecord,Object extradata)
        {
            
            var copied = WatchEvent;
            if (copied != null)
            {
                copied(this, changetype, oldRecord, newRecord,extradata);




            }



        }

        public enum ChangeInfoConstants
        {
            /// <summary>
            /// pointless flag, that doesn't really mean, anything.
            /// usually means to update all values.
            /// </summary>
            CIC_NULL ,
            /// <summary>
            /// User Clocked out of an order. Extradata will be a UserDataClockDifference.
            /// </summary>
            CIC_Clockout,
            /// <summary>
            /// User clocked into an order.Extradata will be a UserDataClockDifference.
            /// </summary>
            CIC_Clockin,

            /// <summary>
            /// User was removed from the DB. Odd, but not unheard of, I guess.
            /// </summary>
            CIC_UserRemoved,

            /// <summary>
            /// User was added to the DB.
            /// </summary>
            CIC_UserAdded,

            /// <summary>
            /// User PIN Code was changed.
            /// </summary>
            CIC_PINChanged,

            /// <summary>
            /// User Name was changed.
            /// </summary>
            CIC_NameChanged,

            /// <summary>
            /// user was made active/inactive. Extra Data will be a bool that is the current state.
            /// </summary>
            CIC_ActiveChanged ,

            /// <summary>
            /// fired when all items change.
            /// </summary>
            CIC_AllChanged

        }
       

        
        public CUserDataWatcher(DataLayer dbObject)
        {
            _RefreshTimems = dbObject.Configuration.MonitorRefreshDelay;
            //first, set our "initial" state.
            //optionally, we could not, then we would fire off a "useradded" event for every single user, though.

            database = dbObject;
            _LastUserList = new List<DataLayer.UserRecord>();
            _LastOrderState = new Dictionary<int, List<string>>();
            _LastActiveState = new Dictionary<int, bool>();



            Debug.Print("CUserdataWatcher: ms delay:" + _RefreshTimems);
            CheckChanges = new Timer(CheckChanged, null, 0, _RefreshTimems);

        }
        private bool intimer=false;
        private int timercount = 0;
        private void CheckChanged(Object param)
        {
            if(intimer) return; //cascading calls are not allowed >:|
            intimer=true;

            timercount++;
            if(timercount ==500) timercount=0;

            List<DataLayer.UserRecord> CurrentUsers = database.GetUserRecords();
            Dictionary<int, List<string>> CurrentOrderState = new Dictionary<int, List<string>>();
            Dictionary<int, bool> CurrentActiveState = new Dictionary<int, bool>();

            //get the current clock state.

            foreach (var iterateval in CurrentUsers)
            {

                List<String> userorder = database.GetUserOrders(iterateval.Username, true);
                CurrentOrderState.Add(iterateval.RecordID, userorder);
                CurrentActiveState.Add(iterateval.RecordID, iterateval.Active);

                if (timercount % 250 == 0)
                {
                    //fire an all-emcompassing event...
                    InvokeWatchEvent(ChangeInfoConstants.CIC_AllChanged,
                                     _LastUserList.First((w) => w.RecordID == iterateval.RecordID),
                                     iterateval, CurrentActiveState[iterateval.RecordID]);
                }

                //check for Active diff.
                if (_LastActiveState.Count != 0)
                {
                    if (_LastActiveState.ContainsKey(iterateval.RecordID))
                    {
                        if (CurrentActiveState[iterateval.RecordID] != _LastActiveState[iterateval.RecordID])
                        {
                            //current active state differs from the last, so fire the change event.
                            InvokeWatchEvent(ChangeInfoConstants.CIC_ActiveChanged, _LastUserList.First((w) => w.RecordID == iterateval.RecordID),
                                iterateval, CurrentActiveState[iterateval.RecordID]);

                        }

                    }

                }
                //we can check for a diff here, actually.
                if (_LastOrderState.Count != 0) //first iteration.
                {
                    //make sure key exists. It might not, if a user was added.
                    if (_LastOrderState.ContainsKey(iterateval.RecordID))
                    {
                        if (userorder.Count > _LastOrderState[iterateval.RecordID].Count)
                        {
                            // values have been added...
                            InvokeWatchEvent(ChangeInfoConstants.CIC_Clockin, iterateval, iterateval,
                                             new UserDataClockStateDifference(_LastOrderState[iterateval.RecordID],
                                                                              userorder));

                        }
                        else if ((userorder.Count < _LastOrderState[iterateval.RecordID].Count))
                        {
                            //values have been removed.
                            InvokeWatchEvent(ChangeInfoConstants.CIC_Clockout, iterateval, iterateval,
                                             new UserDataClockStateDifference(_LastOrderState[iterateval.RecordID],
                                                                              userorder));

                        }
                    }


                }

            }

            foreach(var iterate in (from m in CurrentUsers 
                                    where !_LastUserList.Any((y)=>y.RecordID==m.RecordID) select m))
            {
                //iterate through items whose RecordID doesn't exist in the LastUserList but exists in this one.
                //These will, evidently, be User that were Added.
                InvokeWatchEvent(ChangeInfoConstants.CIC_UserAdded, null, iterate, null);

            }

            //do the opposite to catch removed users.

            foreach (var iterate in (from m in _LastUserList
                                     where !CurrentUsers.Any((y) => y.RecordID == m.RecordID)
                                     select m))
            {
                InvokeWatchEvent(ChangeInfoConstants.CIC_UserRemoved, iterate, null, null);

            }



            //Check for PIN changes.
            //find elements that have the same RecordID but different PINs.


            foreach (DataLayer.UserRecord currrecord in CurrentUsers)
            {
                foreach (DataLayer.UserRecord oldrecord in _LastUserList)
                {

                    if (currrecord.RecordID == oldrecord.RecordID)
                    {

                        if (currrecord.PINCode != oldrecord.PINCode)
                        {
                            //PIN Changed.
                            InvokeWatchEvent(ChangeInfoConstants.CIC_PINChanged, oldrecord, currrecord, null);

                        }
                        if (currrecord.Username != oldrecord.Username)
                        {
                            InvokeWatchEvent(ChangeInfoConstants.CIC_NameChanged, oldrecord, currrecord, null);
                            //name changed.
                        }


                    }


                }



            }





            //update the "cached" listing.
            _LastOrderState=CurrentOrderState;
            _LastUserList=CurrentUsers;
            intimer = false;
        }

    }
}
