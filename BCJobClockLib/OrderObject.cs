using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{
    //represents an Order.
    public class OrderObject
    {
        DataLayer dlayer=null;

        private String _OrderID;


        public static OrderObject getOrder(DataLayer layerobject, String OrderID)
        {
            return new OrderObject(layerobject, OrderID);

        }

        internal OrderObject(DataLayer dataobject, String OrderID)
        {
            dlayer=dataobject;
            _OrderID=OrderID;

        }

    }
}
