using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer=System.Threading.Timer;

namespace BCJobClockLib
{
    public partial class frmWaitMessage : Form
    {
        String usedisplaymessage;
        System.Windows.Forms.Timer updateTimer;
        private bool _cancelthread=false;
        private Exception connectionexception;
        public frmWaitMessage(String showmessage):this()
        {

            usedisplaymessage=showmessage;

        }

        public frmWaitMessage()
        {
            InitializeComponent();
        }

        private void frmWaitMessage_Load(object sender, EventArgs e)
        {
            lblDisplayMessage.Text = usedisplaymessage;
            pBarPleaseWait.Style = ProgressBarStyle.Continuous;
            updateTimer = new System.Windows.Forms.Timer();
            
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = 1;
            updateTimer.Start();
            
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            
            int currvalue = pBarPleaseWait.Value;
            currvalue += 5;
            if (currvalue > pBarPleaseWait.Maximum)
                currvalue = pBarPleaseWait.Minimum;
            Debug.Print("Ticking...(" + currvalue.ToString() + ")");
            pBarPleaseWait.Value = currvalue;
            pBarPleaseWait.Invalidate();
            pBarPleaseWait.Update();

         
        }
        public void SetException(Exception exx)
        {
            connectionexception=exx;

            
            

        }

        private void frmWaitMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer.Stop();
            if (connectionexception != null)
            {

                MessageBox.Show("Connection Exception:" + connectionexception.Message);
                Application.Exit();

            }

        }

        private void frmWaitMessage_Shown(object sender, EventArgs e)
        {
           
        }
    }
}
