using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JobClockAdmin
{
    class BeforeSelItemChange
    {
        //implements- a psuedo "BeforeItemChange" event for a listview.
        private ListView lvwListView =null;
        private ListViewItem currentitem=null;
        public delegate void BeforeItemChangeFunction(ListViewItem previousItem,ListViewItem CurrentItem);
        public event BeforeItemChangeFunction fireChange;
        public void InvokeChange(ListViewItem previtem, ListViewItem nextitem)
        {
            var copied = fireChange;
            if (copied != null)
                copied.Invoke(previtem, nextitem);


        }

        public BeforeSelItemChange(ListView lvwobj)
        {
            lvwListView = lvwobj;
            //hook the events so that we know when it changes.
            //also, note that <really> we aren't firing the event before it changes,
            //but rather after it changes, and passing in the previous value.
            lvwListView.SelectedIndexChanged += new EventHandler(lvwListView_SelectedIndexChanged);


        }

        void lvwListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentitem != null)
            {
                InvokeChange(currentitem, lvwListView.SelectedItems[0]);

            }
        }




    }
}
