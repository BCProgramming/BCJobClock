using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCJobClockLib;

namespace JobClockAdmin
{
    public partial class Schedules : Form
    {
        private DataLayer _layer;
        public Schedules(DataLayer uselayer):this()
        {
            _layer = uselayer;

        }
        public Schedules()
        {
            InitializeComponent();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void RefreshDailyView(DateTime dateview)
        {
            lvwDailyView.Items.Clear();
            var getresult = _layer.GetScheduleJobs(dateview);


            foreach (var iterate in (from u in getresult select u.Value))
            {

                //add new listitem.


            }

        }
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            RefreshDailyView(e.Start);
        }
    }
}
