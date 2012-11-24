using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCJobClockLib;

namespace JobClockAdmin
{
    public partial class JobMonitor : Form
    {
        private NotifyIcon NotificationIcon;
        private BeforeSelItemChange MonitorSelChange;
        private Timer MonitorUpdateTimer;
        public JobMonitor()
        {
            InitializeComponent();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            //TODO: don't actually close, but instead minimize to the  Notification Area.
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //REALLY close if they press Control when clicking the menu item.
            if (KeyboardInfo.IsPressed(Keys.ControlKey))
            {
                AllowExit=true;
                Close();
            }
            else
            {
                //otherwise, minimize to the tray.
                Visible=false;
            }
        }


        ContextMenuStrip NotificationMenu=null;
        ToolStripMenuItem RestoreItem=null;
        ToolStripMenuItem HideItem = null;
        ToolStripMenuItem QuitItem=null;

        private bool ShowHideNotification=true;
        private bool AllowExit=false;
        GenericListViewSorter MonitorSorter;
        DataLayer Database = new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Monitor);
        private Color[] LEDColorsMiddle=new Color[] {Color.Red,Color.Green};
        private Color[] LEDColorsOuter = new Color[] { Color.DarkRed,Color.DarkGreen};
        private Bitmap[] LEDs = null;
        private Graphics[] LEDGraphics = null;
        private ImageList imlListView=null;
        private CUserDataWatcher userwatch =null;
        private ListViewGroup Clockedingroup=null, Clockedoutgroup=null;
        private void JobMonitor_Load(object sender, EventArgs e)
        {

            NotificationIcon = new NotifyIcon();
            
            //same icon as this form.
            NotificationIcon.Icon=this.Icon;

            //set the ContextMenuStrip....
            NotificationMenu = new ContextMenuStrip();

            HideItem = new ToolStripMenuItem("&Hide");
            
            RestoreItem = new ToolStripMenuItem("&Restore");
            //make it bold to indicate it is the "default" action.
            RestoreItem.Font = new Font(RestoreItem.Font, FontStyle.Bold);
            QuitItem = new ToolStripMenuItem("&Quit");
            
            //set handlers...
            NotificationMenu.Opening += new CancelEventHandler(NotificationMenu_Opening);
            MonitorSorter = new GenericListViewSorter(lvwUserListing, null);

            HideItem.Click += new EventHandler(HideItem_Click);
            QuitItem.Click += new EventHandler(QuitItem_Click);
            RestoreItem.Click += new EventHandler(RestoreItem_Click);
            NotificationMenu.Items.AddRange(new ToolStripItem[] { RestoreItem, HideItem, QuitItem });

            NotificationIcon.DoubleClick += new EventHandler(RestoreItem_Click);
            NotificationIcon.ContextMenuStrip=NotificationMenu;

            
            //That's the Notification Icon setup. 

            //ready the db...

                        tryagain:
            try
            {
                //Database.GetConnection();
                Database.DbConnection_Progress();



            }
            catch (Exception exx)
            {
                //oh no!
                //log the error.
                DataLayer.LogAdmin("Exception:" + exx.Message + " Stack Trace:" + exx.StackTrace);
                //show a message.




                if (MessageBox.Show(this, "Error Connecting to the Database:" + exx.Message, "Database Error", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    goto tryagain;
                Close();

            }
            DbConnection gotcon = Database.GetConnection();
            

            //draw our "LED" bitmaps.

            LEDs = new Bitmap[LEDColorsMiddle.Length];
            LEDGraphics = new Graphics[LEDColorsMiddle.Length];


            for (int i = 0; i < LEDColorsMiddle.Length; i++)
            {
                LEDs[i] = new Bitmap(16, 16);
                Graphics g = LEDGraphics[i] = Graphics.FromImage(LEDs[i]);
                g.Clear(Color.Transparent);
                //draw an ellipse...
                GraphicsPath gpp = new GraphicsPath();
                gpp.AddEllipse(2,2,14,14);
                

                PathGradientBrush pathbrush = new PathGradientBrush(gpp);
                pathbrush.CenterColor = LEDColorsMiddle[i];
                Color[] surround = pathbrush.SurroundColors;

                for (int j = 0; j < surround.Length; j++)
                    surround[j] = LEDColorsOuter[i];

                pathbrush.SurroundColors = surround;

                pathbrush.CenterPoint= new PointF(8,8);


                g.FillPath(pathbrush, gpp);

                gpp.Dispose();
                pathbrush.Dispose();



                


            }

            //create the ImageList.
            imlListView = new ImageList();
            imlListView.ImageSize=new Size(16,16);
            imlListView.ColorDepth = ColorDepth.Depth32Bit;
            imlListView.Images.AddRange(LEDs);


            //add the columns to the listview.
            //right now- a "unlabelled" one, 17 pixels wide, to show the LED;
            //and their Name. simple enough, really.

            lvwUserListing.SmallImageList=imlListView;
            
            lvwUserListing.Columns.Add("NAME", "Name",100);
            lvwUserListing.Columns.Add("BUSY", "Time", 100);
            lvwUserListing.SizeColumnsEqual();

            //add  two groups: one for clocked-in and one for clocked-out.

            Clockedingroup = lvwUserListing.Groups.Add("CLOCKEDIN", "Clocked In");
            Clockedoutgroup = lvwUserListing.Groups.Add("CLOCKEDOUT", "Clocked Out");

            MonitorSelChange = new BeforeSelItemChange(lvwUserListing);
            MonitorSelChange.fireChange += new BeforeSelItemChange.BeforeItemChangeFunction(MonitorSelChange_fireChange);

            userwatch = new CUserDataWatcher(Database);
            userwatch.WatchEvent += new CUserDataWatcher.WatchEventFunc(userwatch_WatchEvent);

            //set up the timer as well, which will periodically fire...
            MonitorUpdateTimer = new Timer();
            MonitorUpdateTimer.Interval = 500;
            MonitorUpdateTimer.Tick += new EventHandler(MonitorUpdateTimer_Tick);
            //MonitorUpdateTimer.Start();
  

        }
        bool Updateactive=false;
        void MonitorUpdateTimer_Tick(object sender, EventArgs e)
        {

            
            if(Updateactive) return;
            Updateactive=true;
            //task: we want to update all the items in the list.
            //so we cheat, send some change event when there wasn't actually a change.


            //tag is in the format Record##, where ## is the RecordID of the value.
            //we don't want this to happen on a separate thread, either, if an invoke is required,
            //invoke ourself...
         
            try
            {
                lock (Database)
                {

                    if (InvokeRequired)
                        Invoke((MethodInvoker) (() => MonitorUpdateTimer_Tick(sender, e)));


                    foreach (ListViewItem iterateitem in lvwUserListing.Items)
                    {

                        DataLayer.UserRecord oldrecord = ((DataLayer.UserRecord) iterateitem.Tag);
                        int userecid = oldrecord.RecordID;
                        DataLayer.UserRecord t;
                        //create the updated record for this item...
                        t = DataLayer.UserRecord.CreateRecord(userecid, Database);
                        //fire!
                        userwatch_WatchEvent(userwatch, CUserDataWatcher.ChangeInfoConstants.CIC_AllChanged, oldrecord,
                                             t,
                                             null);
                        //and cross our fingers and hope that worked, of course.



                    }
                }
            }
            catch
            {
                //hmm...
            }
            finally
            {
                Updateactive = false;
            }
        }

        void MonitorSelChange_fireChange(ListViewItem previousItem, ListViewItem CurrentItem)
        {



            //throw new NotImplementedException();
        }

        private DateTime? GetEarliestStartTime(String UserPIN,List<String> orderIDs, out String RO)
        {
            //Pre: orderIDs are all Active.

            //return earliest time of these orders.

            DateTime currentminimum = DateTime.MaxValue;

            RO = "";
            foreach (String looporderid in orderIDs)
            {
                DateTime? currcheck = Database.GetLastStartTimeForOrder(UserPIN, looporderid);
                if (currcheck != null)
                {
                    if (currcheck.Value < currentminimum)
                    {
                        currentminimum = currcheck.Value;
                        RO = looporderid;
                    }
                }
        }

            if(currentminimum == DateTime.MaxValue) return null; //null to indicate that the search is invalid as there aren't any orders with a starttime (no active orders
            return currentminimum;

            




        }
        String FormatUserOrderInfo(String UserPIN, String OrderID)
        {
            TimeSpan outparam= new TimeSpan();
            return FormatUserOrderInfo(UserPIN, OrderID, ref outparam);



        }
        TimeSpan GetUserOrderElapsedTime(String UserPIN, String OrderID)
        {
            String screwitusesql = "SELECT StartTime FROM OrderData WHERE `UserPinCode`=\"{0}\" AND `Order`=\"{1}\"";
            DbConnection getcon = Database.GetConnection();
            DbCommand makecommand = getcon.CreateCommand();
            makecommand.CommandText = String.Format(screwitusesql, UserPIN, OrderID);
            return new TimeSpan();
        }

        String FormatUserOrderInfo(String UserPIN, String OrderID,ref TimeSpan ElapsedTime)
        {
            String buildresult="";
            buildresult += "RO#" + OrderID + " ";
            //now print "since" #time# for that order.
            String screwitusesql = "SELECT StartTime FROM OrderData WHERE `UserPinCode`=\"{0}\" AND `Order`=\"{1}\"";
            DbConnection getcon = Database.GetConnection();
            DbCommand makecommand = getcon.CreateCommand();
            makecommand.CommandText = String.Format(screwitusesql, UserPIN, OrderID);
            DateTime? gettimex = makecommand.ExecuteScalar() as DateTime?;
           
            if (gettimex == null) return "";

            DateTime gettime = gettimex.Value;
            //if within last 24 hours...
            String thedatestring = "";
          /*
            if ((DateTime.Now - gettime).Days == 0)
            {
                //show only hour/minute.
                thedatestring = gettime.ToShortTimeString();

            }
            else
            {
                //otherwise, show a weekday name as well.
                thedatestring = gettime.DayOfWeek.ToString() + " at " + gettime.ToShortTimeString();


            }*/
            TimeSpan elapsed = (Database.DBTime() - gettime).Duration();
            thedatestring = String.Format("{0:00}:{1:00}", Math.Floor(elapsed.TotalHours), elapsed.Minutes);
            ElapsedTime += elapsed;
            return buildresult + "Elapsed:" + thedatestring;

        }
        private Dictionary<ListViewItem, bool> PreviousWarnStates = null; 
        private void CheckWarningLength(ListViewItem relevantitem, TimeSpan thetimespan)
        {
            return; //DISABLED...
            if (Database.Configuration.Monitor_WarningLength.Ticks == 0) return; //0 is the "flag" to ignore warning length.
            DataLayer.UserRecord gotrecord = (DataLayer.UserRecord)relevantitem.Tag;

            if (PreviousWarnStates == null)
            {
                PreviousWarnStates = new Dictionary<ListViewItem, bool>();


            }

            if(!PreviousWarnStates.ContainsKey(relevantitem))
            {
                //if not in the dict, add it as false...
                PreviousWarnStates.Add(relevantitem, false);

            }
            bool currwarnstate = thetimespan > Database.Configuration.Monitor_WarningLength;
            if (currwarnstate) relevantitem.BackColor = Color.Plum;
            if (currwarnstate && !(PreviousWarnStates[relevantitem]))
            {
                if (NotificationIcon.Visible)
                {
                    NotificationIcon.ShowBalloonTip(750, "Length Notification","Tech " + gotrecord.Username + " has exceeded Warning limit of " + DataLayer.FormatTimeSpan(Database.Configuration.Monitor_WarningLength) + ".", ToolTipIcon.Info);


                }

            }
            PreviousWarnStates[relevantitem] = currwarnstate;

        }

        void userwatch_WatchEvent(CUserDataWatcher sender, CUserDataWatcher.ChangeInfoConstants changetype, DataLayer.UserRecord oldRecord, DataLayer.UserRecord newRecord, object extradata)
        {
            
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) (() => userwatch_WatchEvent(sender, changetype, oldRecord, newRecord, extradata)));
                return;
            }
            Debug.Print("WatchEvent..." + changetype.ToString());
            switch (changetype)
            {
                case CUserDataWatcher.ChangeInfoConstants.CIC_UserAdded:

                    
                    if(!newRecord.Active) return; //don't show inactive users.
                    List<String> gotorders = Database.GetUserOrders(newRecord.Username);

                    ListViewItem lvi = new ListViewItem();
                    String RO;
                    
                    lvi.Text = newRecord.Username;
                    lvi.Tag = newRecord;
                    lvi.Name = "Record" + newRecord.RecordID.ToString();
                    String busytext = "";
                    bool isclocked=false;
                    DateTime? earlytime = GetEarliestStartTime(newRecord.PINCode, gotorders, out RO);
                    if (earlytime==null)
                    {
                        busytext = " Not Clocked in";
                        lvi.ImageIndex = 1;
                        lvi.Group = Clockedoutgroup;
                        isclocked=false;
                    }
                    else
                    {
                        isclocked=true;
                        lvi.Group = Clockedingroup;
                        //otherwise, it needs to show the RO#'s they are clocked into with their time.
                        //I get the feeling users won't be able to clock in more  than one order at a time but may as well be flexible.

                        List<String> useractiveorders = Database.GetUserOrders(newRecord.Username);

                        Debug.Assert(useractiveorders.Count > 0); //should always have clocked in order if we get here.


                        TimeSpan elapsedvalue = new TimeSpan();

                        if (useractiveorders.Count == 1)
                        {
                            
                            busytext = FormatUserOrderInfo(newRecord.PINCode, useractiveorders[0],ref elapsedvalue);







                        }
                        else
                        {
                            List<String> buildlisting = (from o in useractiveorders let k = FormatUserOrderInfo(newRecord.PINCode, o,ref elapsedvalue) where k!="" select k  ).ToList();

                            busytext = String.Join(",", buildlisting.ToArray());




                        }

                        CheckWarningLength(lvi, elapsedvalue);
                        //busytext = "Busy since " + earlytime.Value.ToShortTimeString();
                        lvi.ImageIndex = 0;
                    }
                    lvi.SubItems.Add(busytext);
                    lvi.Tag = newRecord;
                    lvwUserListing.Items.Add(lvi);

                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_UserRemoved:
                    //remove the item.
                    ListViewItem finditem = lvwUserListing.Items["Record" + oldRecord.RecordID.ToString()];
                    lvwUserListing.Items.Remove(finditem);
                    
                    
                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_ActiveChanged:

                    //we fake it by just refiring the event with Add or remove, respectively.

                    bool newstate = (bool)extradata;
                    if (newstate)
                    {
                        //visibilify.
                        userwatch_WatchEvent(sender, CUserDataWatcher.ChangeInfoConstants.CIC_UserAdded, oldRecord, newRecord, null);
                    }
                    else
                    {
                        //invisibilify
                        userwatch_WatchEvent(sender, CUserDataWatcher.ChangeInfoConstants.CIC_UserRemoved, oldRecord, newRecord, null);
                    }



                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_AllChanged:
                    ListViewItem ItemChanged = lvwUserListing.Items["Record" + newRecord.RecordID.ToString()];
                    if (ItemChanged != null) //null check, since it could fire immediately before a Add...
                    {
                        ItemChanged.Tag = newRecord;
                        ItemChanged.Text = newRecord.Username;
                        var activeorders = Database.GetUserOrders(newRecord.Username, true);
                        TimeSpan refspan = new TimeSpan();
                        String[] formatted =
                            (from m in activeorders select FormatUserOrderInfo(newRecord.PINCode, m,ref refspan)).ToArray();
                        String usesubitemtext = String.Join(",", formatted).Trim();
                        if (usesubitemtext != "")
                            ItemChanged.SubItems[1].Text = usesubitemtext;
                        CheckWarningLength(ItemChanged, refspan);
                    }
                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_PINChanged:
                    ListViewItem PINItem = lvwUserListing.Items["Record" + newRecord.RecordID.ToString()];
                    //well... actually we don't care if the PIN changes, but we need to keep things up to date nonetheless.
                    PINItem.Tag = newRecord;
                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_NameChanged:
                    ListViewItem changednameitem = lvwUserListing.Items["Record" + newRecord.RecordID.ToString()];
                    changednameitem.Text = newRecord.Username;
                    changednameitem.Tag = newRecord;
                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_Clockin:
                    ListViewItem clockedin = lvwUserListing.Items["Record" + newRecord.RecordID.ToString()];
                    List<String> inuserorders = Database.GetUserOrders(newRecord.Username, true);
                    bool isclockedout = inuserorders.Count == 0;
                    clockedin.ImageIndex = isclockedout ? 1 : 0;
                    if (!isclockedout) clockedin.Group = Clockedingroup;

                    clockedin.Group = Clockedingroup;

                    if (inuserorders.Count == 0)
                    {
                        if (Database.Configuration.Monitor_NotifyAvailableTech)
                        {

                            NotificationIcon.ShowBalloonTip(750, newRecord.Username + " Available.", "A Tech no longer has active work orders.", ToolTipIcon.Info);

                        }

                    }
                    clockedin.Tag = newRecord;

                    break;
                case CUserDataWatcher.ChangeInfoConstants.CIC_Clockout:

                    ListViewItem clockedout= lvwUserListing.Items["Record" + newRecord.RecordID.ToString()];
                    List<String> userorders = Database.GetUserOrders(newRecord.Username, true);
                    bool isout = userorders.Count == 0;
                    clockedout.ImageIndex = isout ? 1 : 0;
                    clockedout.BackColor = SystemColors.Window;


                    //recalc the UserOrder Format...



                    if (isout) clockedout.Group = Clockedoutgroup;

                    if (userorders.Count == 0)
                    {
                        //set subitem...
                        clockedout.SubItems[1].Text = "Not Clocked in";
                        if (Database.Configuration.Monitor_NotifyAvailableTech)
                        {

                            NotificationIcon.ShowBalloonTip(750, newRecord.Username + " Available.", "A Tech no longer has active work orders.", ToolTipIcon.Info);

                        }

                    }

                    clockedout.Tag = newRecord;
                    break;

            }



        }
      

        void RestoreItem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Show();
        }

        void QuitItem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            AllowExit=true;
            Close();
        }

        void HideItem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            ShowHideNotification=false;
            Visible=false;
        }

        void NotificationMenu_Opening(object sender, CancelEventArgs e)
        {
            //enable/disable Restore and Hide Items based on our visible state.
            RestoreItem.Enabled = !Visible;
            HideItem.Enabled=Visible;
            RestoreItem.Font = Visible?new Font (RestoreItem.Font,FontStyle.Regular):RestoreItem.Font;
            RestoreItem.Font = Visible ? new Font(RestoreItem.Font, FontStyle.Regular) : RestoreItem.Font;
        }

        private void JobMonitor_VisibleChanged(object sender, EventArgs e)
        {
            //when we are hidden, tell the notificationIcon to show a balloon.
            NotificationIcon.Visible = !Visible;
            if (!Visible)
            {
                if (ShowHideNotification)
                {
                    Debug.Print("Showing Notification");
                    NotificationIcon.BalloonTipIcon = ToolTipIcon.Info;
                    
                    
                    NotificationIcon.Text = "BCJobClock Monitor Module";
                    NotificationIcon.Visible=true;
                    NotificationIcon.ShowBalloonTip(2500,
                        "Job Monitor is still running",
                        "Job Monitor will continue to run in the background. To reopen it, click this icon.",ToolTipIcon.Info);



                }


            }
            else
            {
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
            }

            

            if(Visible) ShowHideNotification=true;

        }

        private void JobMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason==CloseReason.TaskManagerClosing || e.CloseReason==CloseReason.WindowsShutDown) return; //don't obstruct Windows, or Task manager.
           
            if (!AllowExit)
            {
                e.Cancel=true;
                Hide();
            }

        }

        private void JobMonitor_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
               // Visible=false;
                


            }

            lvwUserListing.Location = new Point(ClientRectangle.Left, mStrip.Bottom);
            lvwUserListing.Size = ClientSize;


        }
    }
}
