using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{
    //represents a User.
    public class User
    {
        private DataLayer dbobj;
        private String _UserPIN;
        public String PIN
        {
            get
            {
                return _UserPIN;
            }
            set
            {
                //changing a userPIN:
                //change all items in USERS table with the current PIN to the new PIN.
                //change all items in OrderData with the current PIN as UserPinCode to use the new PIN.
                //change CLOCKED table to update UserPIN item to new PIN.

                String changeUSERfmt = "UPDATE USERS SET `PINCode`=\"{1}\" WHERE `PINCode`=\"{0}\"";
                string changeOrderDatafmt = "UPDATE OrderData SET `UserPinCode`=\"{1}\" WHERE `UserPinCode`=\"{0}\"";
                string changeClockedfmt = "UPDATE CLOCKED SET `UserPIN`=\"{1}\" WHERE `UserPIN`=\"{0}\"";

                String[] runqueries = new string[] { changeUSERfmt, changeOrderDatafmt, changeClockedfmt };
                DbConnection useconnection = dbobj.GetConnection();
                DbTransaction trans = useconnection.BeginTransaction();
                bool erroroccured = false;

                try
                {
                    foreach (String loopq in runqueries)
                    {
                        String gq = String.Format(loopq, value, _UserPIN);

                        dbobj.ExecuteQueryDirect(gq);



                    }
                }
                catch
                {
                    erroroccured=true;

                }
                if (erroroccured)
                {
                    trans.Rollback();

                }
                else
                {


                    _UserPIN = value;
                    trans.Commit();
                }



            }
        }


        public String Name
        {
            get { return dbobj.UserNameFromPIN(_UserPIN); }
            set
            {
                //update the name directly in the db.
                string setquery = "UPDATE USERS SET UserName=\"{0}\" WHERE PINCode=\"{1}\"";
                String usequery = String.Format(setquery, value, _UserPIN);
                dbobj.ExecuteQueryDirect(usequery);


            }
        }
        public List<OrderObject> GetOrders()
        {
            List<String> orderIDs = dbobj.GetUserOrders(Name);
            List<OrderObject> retrieveval = new List<OrderObject>();
            foreach (String looporderid in orderIDs)
            {
                retrieveval.Add(new OrderObject(dbobj, looporderid));


            }
            return retrieveval;

        }

        public static User GetUser(DataLayer dbObject,String UserPIN)
        {
            return new User(dbObject,UserPIN);

        }


        internal User(DataLayer dbObject, String UserPIN)
        {

            dbobj = dbObject;
            _UserPIN=UserPIN;

        }

    }
}
