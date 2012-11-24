using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data.Common;
using BASeCamp.Configuration;
using BCJobClockLib;
using Timer=System.Threading.Timer;
using BASeCamp.Updating;
namespace JobClockAdmin
{
    public partial class frmJobClockAdmin : Form
    {
        public BCUpdate UpdateObject = null;
        public const int UpdateID = 24; //updateID to give the update mechanism.
        //public DataLayer Database = new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Admin);
        public DataLayer Database;
        private FormPositionSaver formposobject;
        private GenericListViewSorter UserListSorter;
        private GenericListViewSorter OrderListSorter;
        private GenericListViewSorter ReportListSorter;
        private BeforeSelItemChange UserSelChange;
        private BeforeSelItemChange OrderSelChange;
        //private BeforeSelItemChange ReportSelChange;
        public frmJobClockAdmin()
        {
            InitializeComponent();
            //hook up the PositionSaver...
            
        }

        private void tStripRefreshUsers_Click(object sender, EventArgs e)
        {
            RefreshUserList();
        }

//        GetCompareValue(GenericListViewSorter Sorter, String ColumnName, ListViewItem Item);
        private Object GetCompareValue_User(GenericListViewSorter Sorter, String ColumnName, ListViewItem Item)
        {

            return null;


            //UserName,PINCode,CLOCKED,ACTIVE,LASTCLOCK
            //ORDERID,INITIALSTART,TOTALTIME,ACTIVEWORKERS,LASTCLOCK,DESCRIPTION


        }


        private String SpecialTimeFormat(TimeSpan formatit)
        {




            return String.Format("{0:D1}:{1:D2}", formatit.Hours, formatit.Minutes);


        }
        private void RefreshLogList()
        {
            //lvwLogData.Columns.Add("DATE", "TimeStamp");
            //lvwLogData.Columns.Add("MESSAGE", "Message");
            //lvwLogData.Columns.Add("LOGID", "Type");

            DbConnection usecon = Database.GetConnection();
            DbCommand madecommand = usecon.CreateCommand();

            String getlogquery = Database.qAdapter["GETMESSAGELOG"];
            madecommand.CommandText = getlogquery;
            lock (Database)
            {
                using (DbDataReader readlog = madecommand.ExecuteReader())
                {
                    while (readlog.Read())
                    {

                        //get the ID...
                        int LogentryID = readlog.GetInt32(readlog.GetOrdinal("ID"));

                        //the logID is used as the tag on the respective ListItem. See if one is present in the list.

                        ListViewItem modifyItem = SearchListView_First(lvwLogData,
                                                                       (w) =>
                                                                       (long.Parse(w.Tag.ToString()) == LogentryID));

                        //if it is null, create a new one.
                        if (modifyItem == null)
                        {
                            modifyItem = new ListViewItem(new string[] {"", "", ""});
                            modifyItem.Tag = LogentryID;
                            lvwLogData.Items.Add(modifyItem);



                        }

                        modifyItem.Text = readlog.GetDateTime(readlog.GetOrdinal("Tstamp")).ToString();
                        modifyItem.SubItems[1].Text = readlog.GetString(readlog.GetOrdinal("Message"));
                        modifyItem.SubItems[2].Text = readlog.GetString(readlog.GetOrdinal("Type"));

                    }


                }
            }

        }

        private void RefreshUserList()
        {
           // Thread.Sleep(500);
            //lvwUsers << Users List.
            /*List<String> cachedselection = new List<string>();
            foreach (String iteratethestring in lvwUsers.SelectedItems)
            {
                cachedselection.Add(iteratethestring);


            }*/
            //clear...
           // lvwUsers.Items.Clear();
           // lvwUsers.Columns.Clear();

            //acquire the Connection object ourselves to perform a query.
            DbConnection usecon = Database.GetConnection();

            //create a command object....
            DbCommand madecommand = usecon.CreateCommand();

            String getusersquery = Database.qAdapter["GETUSERS"];
            madecommand.CommandText = getusersquery;
            List<String> AllUserNames = new List<string>(), AllPinCodes = new List<string>();
            List<int> AllRecordIDs = new List<int>();
            List<bool> AllActivestates = new List<bool>();
            lock (Database)
            {
                using (DbDataReader readusers = madecommand.ExecuteReader())
                {

                    if (readusers.HasRows)
                    {

                        while (readusers.Read())
                        {




                            bool activestate = readusers.GetInt16(readusers.GetOrdinal("ACTIVE")) > 0;
                            String currUserName = readusers.GetString(readusers.GetOrdinal("UserName"));
                            String currPINCode = readusers.GetString(readusers.GetOrdinal("PINCode"));
                            int currID = readusers.GetInt32(readusers.GetOrdinal("RecordID"));

                            AllUserNames.Add(currUserName);
                            AllPinCodes.Add(currPINCode);
                            AllRecordIDs.Add(currID);
                            AllActivestates.Add(activestate);
                        }
                    }
                    readusers.Close();
                }
            }
            for (int i = 0; i < AllUserNames.Count; i++)
            {
                String currUserName = AllUserNames[i];
                String currPINCode = AllPinCodes[i];
                bool astate = AllActivestates[i];
                String showclockedTime = DataLayer.FormatTimeSpan( Database.GetTotalClockedTimeForUser(currPINCode,DataLayer.TotalClockedTimeTypeConstants.ClockedTime_Active));
                String totalclockedTime = DataLayer.FormatTimeSpan(Database.GetTotalClockedTimeForUser(currPINCode, 
                    DataLayer.TotalClockedTimeTypeConstants.ClockedTime_Active | DataLayer.TotalClockedTimeTypeConstants.ClockedTime_Completed));
                long totalactiveCount = Database.GetActiveJobCountForUser(currPINCode);
                DateTime? lastclockout = Database.GetLastClockOutForUser(currPINCode);
                String sclockout = lastclockout == null ? "null" : lastclockout.ToString();
                List<String> thisusersorders = Database.GetUserOrders(currUserName,true);

                //if there is an item with the given record ID, edit it.
                ListViewItem modifyItem = SearchListView_First(lvwUsers, (w) => (long.Parse(w.Tag.ToString()) == AllRecordIDs[i]));

                if (modifyItem == null)
                {
                    modifyItem =
                        new ListViewItem(new String[]
                                             {currUserName, currPINCode, showclockedTime,totalclockedTime, totalactiveCount.ToString(),"",sclockout});
                    lvwUsers.Items.Add(modifyItem);
                }
                modifyItem.Tag = AllRecordIDs[i];
                //lvwUsers.Columns.Add("UserName", "UserName");
                //lvwUsers.Columns.Add("PINCode", "PINCode");
                //lvwUsers.Columns.Add("CLOCKED", "Clocked Time");
                //lvwUsers.Columns.Add("ACTIVE", "Active Orders");
                //lvwUsers.Columns.Add("ALLORDERS", "Orders");
                //lvwUsers.Columns.Add("LASTCLOCK", "Last Clock Out");




                modifyItem.Text = currUserName;
                modifyItem.SubItems[1].Text = currPINCode;
                modifyItem.SubItems[2].Text = showclockedTime;
                modifyItem.SubItems[3].Text = totalclockedTime;
                modifyItem.SubItems[4].Text = totalactiveCount.ToString();

                String builduserorderstring;
                List<String> allthisusersorders = Database.GetUserOrders(currUserName,false);
                List<String> usersactiveorders = Database.GetUserOrders(currUserName, true);
                builduserorderstring = Database.FormatUserOrderList(currPINCode, allthisusersorders);

                if (usersactiveorders.Count == 0)
                {
                    //no orders. Show as green (available).
                    modifyItem.BackColor = Color.Lime;


                }
                else
                {
                    modifyItem.BackColor = Color.Red;
                }


                modifyItem.SubItems[5].Text = builduserorderstring;
                modifyItem.SubItems[6].Text = sclockout;
                if (!astate)
                {
                    modifyItem.ForeColor = Database.IsUserActive(currPINCode) ? SystemColors.WindowText : SystemColors.InactiveCaptionText;

                }

                



            }
            /*
            //reselect the items that were deselected.
            foreach (String loopitem in cachedselection)
            {

                //find the item with the same UserName in the new list and select it.
                foreach (ListViewItem loopvalit in lvwUsers.Items)
                {
                    if (SelUserItem != null)
                    {
                        if (SelUserItem.Text.Equals(loopvalit.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            SelUserItem = loopvalit;

                        }


                    }
                    if (String.Equals(loopvalit.Text, loopitem))
                    {
                        loopvalit.Selected = true;
                        break;
                    }

                }




            }
            */


        }

        //functions for searching a listview.
        private IEnumerable<ListViewItem> SearchListView(ListView search, Func<ListViewItem, bool> Searchpredicate)
        {

            foreach (ListViewItem searchitem in search.Items)
            {

                if(Searchpredicate(searchitem))
                    yield return searchitem;

            }

          



        }
        private ListViewItem SearchListView_First(ListView search, Func<ListViewItem, bool> Searchpredicate)
        {
            var lookquery = SearchListView(search, Searchpredicate);

            if (lookquery.Any())
            {
                return SearchListView(search, Searchpredicate).First();
            }
            else
            {
                return null;
            }

        }

        private void UpdateOrderListView()
        {
            if(txtOrderID.Text=="") return;
            if(txtDescription.Text=="") return;
            if(selOrderItem==null) return;
            String sqlfmt;
            String grabtag = (selOrderItem.Tag.ToString());
            if (grabtag == "")
            {
                //empty string is sentinel.

                sqlfmt = Database.qAdapter["INSERTORDERFMT"];

            }
            else
            {
                sqlfmt = Database.qAdapter["UPDATEORDERFMT"];
            }
            DbConnection currcon = Database.GetConnection();
            DbCommand usecmd = currcon.CreateCommand();
            usecmd.CommandText = String.Format(sqlfmt, grabtag, txtOrderID.Text, txtDescription.Text);
            usecmd.ExecuteNonQuery();

            selOrderItem.Tag = txtOrderID.Text;

            if (grabtag != "")
            {
                //if it isn't empty, we need to update All the records that refer to OrderID grabtag and change it 
                // to the new Order ID (txtOrderID.text).
                String sqlupdateOrderIDfmt = Database.qAdapter["UPDATEORDERIDFMT"];
                usecmd.CommandText = String.Format(sqlupdateOrderIDfmt, grabtag, txtOrderID.Text);
                Debug.Print("Updated existing records in OrderData to change existing Order code " + grabtag + " to " + txtOrderID.Text);



            }


            RefreshOrderList();

        }


        private void UpdateUserListView()
        {
            //updates the user list, this is called when messing about with the textboxes underneath the list itself.
            if (txtPINCode.Text == "") return;
            if (txtUserName.Text == "") return;
            if (SelUserItem == null) return;
            long grabtag = long.Parse((SelUserItem.Tag.ToString()));
            String currentUsername = SelUserItem.Text;
            String CurrentPIN = SelUserItem.SubItems[1].Text;
            SelUserItem.Text = txtUserName.Text;
            SelUserItem.SubItems[1].Text = txtPINCode.Text;

            //also I lied,  because it changes the DB as well. meh.
            String[] sqlupdatefmt = {"", ""};
            DbConnection currcon = Database.GetConnection();
            if (grabtag == -1)
            {
                //-1 is a sentinel for when we need to INSERT as opposed to update. we set the tag to -1 when we create a new item, and then
                //when it validates it get's added. (by this routine being called)



                sqlupdatefmt[0] = "INSERT INTO USERS (UserName,PINCode) VALUES (\"{1}\",\"{2}\")";
            }
            else
            {

                sqlupdatefmt[0] = "UPDATE USERS SET UserName=\"{1}\", PINCode=\"{2}\", ACTIVE=\"{5}\" WHERE RecordID=\"{0}\"";
                sqlupdatefmt[1] =
                    "UPDATE CLOCKED SET UserPIN=\"{2}\" WHERE UserPIN=\"{3}\"";
            }
            //Format codes: 
            //0 : RecordID
            //1 : New User Name
            //2 : New PIN Code
            //3 : Current PIN Code
            //4 : Current User Name
            DbCommand usecmd = currcon.CreateCommand();
            for (int i = 0; i < sqlupdatefmt.Length; i++)
            {

                if (sqlupdatefmt[i] != "")
                {
                    usecmd.CommandText = String.Format(sqlupdatefmt[i], long.Parse(SelUserItem.Tag.ToString()),
                        txtUserName.Text, txtPINCode.Text, CurrentPIN, currentUsername,chkActive.Checked?"1":"0");
                    usecmd.ExecuteNonQuery();
                }
            }

            if (grabtag == -1)
            {
                //set the right tag...
                usecmd.CommandText = String.Format(Database.qAdapter["GETRECORDID"], txtUserName.Text, txtPINCode.Text);
                SelUserItem.Tag = long.Parse(usecmd.ExecuteScalar().ToString());
            }

            //Finally, update the OrderData so all PINCode's that are the old PIN are updated to the new PIN.


            String updateOrderDataPinFmt = Database.qAdapter["UPDATEPINCODEFMT"];
            String usequery = String.Format(updateOrderDataPinFmt, CurrentPIN, txtPINCode.Text);

            usecmd.CommandText = usequery;
            Debug.Print("Updating Existing records in OrderData to change existing PIN Code " + CurrentPIN + " to " + txtPINCode.Text);

            SelUserItem.ForeColor = chkActive.Checked ? SystemColors.WindowText : SystemColors.InactiveCaptionText;


        }

        private void lvwUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            UpdateUserFields();
        }
        //private ListViewItem SelUserItem = null;
        private ListViewItem SelUserItem
        {
            get {
            if(lvwUsers.SelectedItems.Count==0) return null;
            return lvwUsers.SelectedItems[0];
            
            
            }

        }
        private void UpdateOrderFields()
        {
            DropDownUsers.Enabled = (selOrderItem != null);

            if (selOrderItem == null)
            {
                txtOrderID.Enabled = false;
                txtDescription.Enabled = false;
                txtOrderID.Text = "";
                txtDescription.Text = "";

                DropDownOrders.Enabled = false;
            }
            else
            {
                txtOrderID.Enabled = true;
                txtDescription.Enabled = true;



                txtOrderID.Text = selOrderItem.Text;
                txtDescription.Text = selOrderItem.SubItems[5].Text;
                
                DropDownOrders.Enabled = true;
            }


        }

        private void UpdateUserFields()
        {
            toolstripdeleteuser.Enabled = (lvwUsers.SelectedItems.Count > 0);
            if (lvwUsers.SelectedItems.Count == 0)
            {
                txtUserName.Enabled=false;
                txtPINCode.Enabled=false;
                txtUserName.Text = "";
                txtPINCode.Text = "";
                chkActive.Checked=false;
                chkActive.Enabled=false;
                DropDownOrders.Enabled=false;
            }
            else
            {
                txtUserName.Enabled=true;
                txtPINCode.Enabled=true;
                chkActive.Enabled=true;
                
                
                txtUserName.Text = SelUserItem.Text;
                txtPINCode.Text = SelUserItem.SubItems[1].Text;
                chkActive.Checked=  Database.IsUserActive(SelUserItem.SubItems[1].Text);


                DropDownOrders.Enabled = true;
            }


        }
        ToolTip validationTip = new ToolTip();
        private static Random rnd = new Random();
        private void AddNewUser()
        {
            //currUserName, currPINCode, showclockedTime, totalactiveCount.ToString(),sclockout
            ListViewItem lvi = new ListViewItem(new string[] { "User" + rnd.Next(0,32768) , String.Format("{0:0000}",rnd.Next(0,10000)),
            "","","","",""});
            lvi.Tag = "-1";
            lvwUsers.Items.Add(lvi);
            lvwUsers.SelectedItems.Clear();
            
            lvi.Selected=true;



        }
        private void AddNewOrder()
        {

            ListViewItem addorderitem = new ListViewItem(new string[] { String.Format("{0:0000000}", rnd.Next(10000000)), "", "", "", "", "","" });


            addorderitem.Tag = "";
            lvwOrders.Items.Add(addorderitem);
            lvwOrders.SelectedItems.Clear();
            addorderitem.Selected=true;


         /*
          *             lvwOrders.Columns.Add("ORDERID","OrderID");
            lvwOrders.Columns.Add("INITIALSTART","Initial Start"); //initial time, the very first entry for this order. (earliest FirstTime)
            lvwOrders.Columns.Add("TOTALTIME", "Total Worker Time"); //Total time, accumulated by all workers. will include time not clocked out yet.
            lvwOrders.Columns.Add("ACTIVEWORKERS", "Active Workers"); // Number of active workers, a number followed by a comma-separated listing of names.
            lvwOrders.Columns.Add("LASTCLOCK", "Last Clock Out");
            lvwOrders.Columns.Add("DESCRIPTION", "Description");*/


        }

        private void txtUserName_Validated(object sender, EventArgs e)
        {
            UpdateUserListView();
        }
        private void setDefaultTip(Control forcontrol)
        {

            validationTip.SetToolTip(forcontrol, getDefaultTip(forcontrol));

        }

        private String getDefaultTip(Control forcontrol)
        {
            String usestring="";
            if (forcontrol==txtPINCode)
            {
                usestring = "User PIN Code. This will be entered in the client Application when this user Clocks in and out.";
                

            }
            else if (forcontrol==txtUserName)
            {

                usestring = "Name of the User. Ideally, it should be unique, but that isn't necessary.";

            }

            return usestring;

        }

        private void txtPINCode_Validated(object sender, EventArgs e)
        {
            txtPINCode.BackColor = SystemColors.Window;
            setDefaultTip(txtPINCode);
            UpdateUserListView();
        }

        private Dictionary<Control, Label> ControlValidationErrorObjects = new Dictionary<Control, Label>();


        private void SetError(Control forcontrol, String ErrorMessage)
        {
            if (String.IsNullOrEmpty(ErrorMessage))
            {
                validationTip.SetToolTip(forcontrol, getDefaultTip(forcontrol));
            }
            else
            {
                validationTip.SetToolTip(forcontrol, ErrorMessage);
            }
            //   lblErrorShow.Text = ErrorMessage;
          //  lblErrorShow.Visible=true;
            /*
            Label theerrorlabel=null;
            if (ControlValidationErrorObjects.ContainsKey(forcontrol))
            {
                //show it and set the text of the label.
                 theerrorlabel = ControlValidationErrorObjects[forcontrol];
            }
            else
            {
                theerrorlabel = new Label();
                
                Controls.Add(theerrorlabel);
                theerrorlabel.AutoSize=false;
                
                theerrorlabel.Location = new Point(forcontrol.Left, forcontrol.Bottom);
                theerrorlabel.Width = forcontrol.Width;
                theerrorlabel.BackColor = Color.Pink;
                theerrorlabel.BringToFront();
                //move to below 


            }
            theerrorlabel.Text=ErrorMessage;

            if(String.IsNullOrEmpty(ErrorMessage))
                theerrorlabel.Visible=false;
            else
            {

                theerrorlabel.Visible = true;
            }
            */
        }

        private void txtPINCode_Validating(object sender, CancelEventArgs e)
        {
            
            String testvalue = txtPINCode.Text;
            if(SelUserItem!=null)
                if(testvalue==SelUserItem.SubItems[1].Text) return;

            //if it's the same no need to complain.
            //must not be shorter than 4 characters...
            if (testvalue.Length < 4)
            {
                e.Cancel=true;
                
                txtPINCode.BackColor=Color.Pink;
                SetError(txtPINCode, "PIN must be longer than 4 characters.");
            }
            else
            {
                //more "sophisticated" check; PIN must also be unique among all users.

                //get the connection object.
                DbConnection grabconnection = Database.GetConnection();
                DbCommand execcmd = grabconnection.CreateCommand();
                execcmd.CommandText= String.Format(Database.qAdapter["USERNAMEFROMPINFMT"],testvalue);
                using (DbDataReader readerobj = execcmd.ExecuteReader())
                {
                    
                    if (readerobj.Read())
                    {
                        
                        //it exists.
                        String grabusername = readerobj.GetString(readerobj.GetOrdinal("UserName"));
                        e.Cancel = true;
                        
                        SetError(txtPINCode, "PINCode cannot conflict with the existing PIN of " + grabusername);
                        txtPINCode.BackColor = Color.Pink;

                    }
                    
                }

            }


        }

        private void lvwUsers_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void l(object sender, PaintEventArgs e)
        {

        }

        private void tstripNewUser_Click(object sender, EventArgs e)
        {
            AddNewUser();
        }
        private void HookTextBoxes()
        {
            
            foreach(Control loopcontrol in Controls)
            {
                TextBox castored = (TextBox)loopcontrol;
                if (loopcontrol is TextBox)
                {

                    if (!castored.Multiline)
                    {
                        castored.KeyDown+=new KeyEventHandler(castored_KeyDown);


                    }


                }


            }



        }

        void castored_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                ValidateChildren(ValidationConstraints.Visible);
        }
        private System.Threading.Timer refreshTimer=null;
        private static readonly String UserReportString = "Tech";
        private static readonly String ROReportString = "RO";
        private static readonly String DateReportString = "Date";
        private static String GetAssemblyFile()
        {

       

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return path;



        

        }
        public void UpdateDownloadComplete(BCUpdate.UpdateInfo uiinfo, AsyncCompletedEventArgs asynccomplete)
        {




            String launchit = uiinfo.DownloadedFilename;
            Process launchprocess = new Process();
            launchprocess.StartInfo.FileName = launchit;
            
            
            launchprocess.Start();
            //after launching, we exit.
            Application.Exit();
        }
        private String GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }

        private bool doExit = false;
        private void frmJobClockAdmin_Load(object sender, EventArgs e)
        {
            try
            {
                
                Database = new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Admin);
                Database.WriteDebugMessage("DataLayer Initialized.");

            }
            catch (frmPasswordPrompt.CredentialsNotGivenException)
            {
                //Close();
                //Environment.Exit(0);
                doExit = true;
                return;
            }

            //FIRST: check for updates, if the setting is...err.. set.
            if (Database.Configuration.CheckUpdates)
            {
                //this is in a try, because there could be all sorts of problems checking.
                //since updating isn't a super-critical thing, we'll silently catch any errors, and if any occur, we'll log them but just pretend
                //everything is fine.
                try
                {
                    UpdateObject = new BCUpdate();

                    //"Register" with our update library...
                    BCUpdate.RegisterApplication("BCJobClock", UpdateID, GetCurrentVersion(), GetAssemblyFile());

                    String strid = UpdateObject.CheckUpdate(UpdateID);

                    String useupdatetext = "A new Version of BCJobClock is available. Would you like to Update?";
                    if (!string.IsNullOrEmpty(strid))
                    {
                        if (
                            MessageBox.Show(
                                String.Format(useupdatetext, Application.ProductName,
                                              Assembly.GetExecutingAssembly().GetName().Version.ToString(), strid),
                                "Update", MessageBoxButtons.YesNo) == DialogResult.Yes)

                        //        Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\n" +
                        //       "New Version:" + strid +
                        //       "\n Would you like to update?", "Update Available", MessageBoxButtons.YesNo) ==
                        //  DialogResult.Yes)
                        {

                            BCUpdate.UpdateInfo objupdate =
                                (from n in UpdateObject.LoadedUpdates where n.dlID == UpdateID select n).
                                    First
                                    ();
                            //I assume we are supposed to now install said update. 
                            //MessageBox.Show("On Demand Update Not yet implemented. Please use Game->Check for Updates to check for and download updates.");
                            objupdate.DownloadUpdate(null,UpdateDownloadComplete);
                            //idle loop, then return.
                            while (Application.OpenForms.Count > 0)
                            {
                                Application.DoEvents();


                            }
                            return;


                        }


                    }





                }
                catch (Exception exx)
                {
                    //Assembly not found, webexception, etc.
                   // DataLayer.LogAdmin("Exception attempting to check for update-" + exx.ToString());

                }




            }




            tryagain:
            try
            {
                //Database.GetConnection();
                Database.WriteDebugMessage("DbConnection_Progress...()");
                Database.DbConnection_Progress();



            }
             catch(Exception exx)
            {
                //oh no!
                //log the error.
                DataLayer.LogAdmin("Exception:" + exx.Message + " Stack Trace:" + exx.StackTrace);
                //show a message.




                if(MessageBox.Show(this, "Error Connecting to the Database:" + exx.Message,"Database Error",MessageBoxButtons.RetryCancel)==DialogResult.Retry)
                    goto tryagain;
                Close();

            }
            DbConnection gotcon = Database.GetConnection();

            try
            {
                formposobject = new FormPositionSaver(this, Database.Configuration.INIObject, true);
            }
            catch (Exception ff)
            {
                Debug.Print("exception " + ff.ToString());


            }
            //load the tab images...
            ImageList tabimagelist = new ImageList();
            String[] createimages = new string[] { "config", "users", "Orders", "reports","log","update" };
          
            
            tabimagelist.ImageSize = new Size(16,16);
            foreach (String addthis in createimages)
            {
                tabimagelist.Images.Add(addthis.ToUpper(), JobClockConfig.Imageman.GetLoadedImage(addthis));


            }




            tabAdminpanel.ImageList = tabimagelist;
            TabPageGeneral.ImageKey="CONFIG";
            TabPageOrders.ImageKey="ORDERS";
            TabPageReporting.ImageKey="REPORTS";
            TabPageUsers.ImageKey="USERS";
            TabPageLogs.ImageKey = "LOG";
            tabUpdate.ImageKey = "UPDATE";
            tabUpdate.Visible = Database.Configuration.Admin_UpdateTabVisible;
            cmdReinitialize.Image = JobClockConfig.Imageman.GetLoadedImage("reinitdb");

            tstripNewUser.Image = JobClockConfig.Imageman.GetLoadedImage("user-add");
            tStripRefreshUsers.Image = JobClockConfig.Imageman.GetLoadedImage("refresh");
            toolstripdeleteuser.Image = JobClockConfig.Imageman.GetLoadedImage("user-delete");
            tStripImportUsers.Image = JobClockConfig.Imageman.GetLoadedImage("import");

            tStripNewOrder.Image = JobClockConfig.Imageman.GetLoadedImage("note-add");
            tStripRefreshOrders.Image = JobClockConfig.Imageman.GetLoadedImage("refresh");
            tStripRemoveOrder.Image = JobClockConfig.Imageman.GetLoadedImage("note-delete");
            tStripCopyUsers.Image = JobClockConfig.Imageman.GetLoadedImage("copy");
            toolstripcopyorders.Image = JobClockConfig.Imageman.GetLoadedImage("copy");
            tstripCopyResult.Image = JobClockConfig.Imageman.GetLoadedImage("copy_s");
            
            //add columns; UserName, and PINCode
            lvwUsers.Columns.Add("UserName", "UserName");
            lvwUsers.Columns.Add("PINCode", "PINCode" );
            lvwUsers.Columns.Add("CLOCKED", "Clocked Time");
            lvwUsers.Columns.Add("TOTALTIME", "Total Time"); //Total time for this user.
            lvwUsers.Columns.Add("ACTIVE", "Active Orders");
            lvwUsers.Columns.Add("ALLORDERS", "Orders");
            lvwUsers.Columns.Add("LASTCLOCK", "Last Clock Out");
            //columns for Order List...
            lvwOrders.Columns.Add("ORDERID","OrderID");
            lvwOrders.Columns.Add("INITIALSTART","Initial Start"); //initial time, the very first entry for this order. (earliest FirstTime)
            lvwOrders.Columns.Add("TOTALTIME", "Total Time");
            lvwOrders.Columns.Add("ACTIVEWORKERS", "Active Workers"); // Number of active workers, a number followed by a comma-separated listing of names.
            lvwOrders.Columns.Add("LASTCLOCK", "Last Clock Out");
            lvwOrders.Columns.Add("DESCRIPTION", "Description");

            lvwLogData.Columns.Add("DATE", "TimeStamp");
            lvwLogData.Columns.Add("MESSAGE", "Message");
            lvwLogData.Columns.Add("TYPE", "Type");


            //changelistitem = new ListViewItem(new string[] {"ORDERID","INITSTART","TOTALTIME","ACTIVEWORKERS","LASTCLOCKOUT","DESCRIPTION"});

            //init the drop downs...
            lvwReportResult.InitCopyDropDown(
                tstripCopyResult, "Copy","RO", (r,q) =>
                         {
                             Debug.Print("Lambda Copy...");
                             TextDataFormat tdf = q.CopyType == DataLayer.ListViewStringConstants.Copy_HTMLFRAGMENT ? TextDataFormat.Html : TextDataFormat.CommaSeparatedValue;

                             Clipboard.SetText(r,TextDataFormat.Text);
                             Clipboard.SetText(r, tdf);
                             
                             //Clipboard.SetData(r, tdf);
                             //Clipboard.SetData(DataFormats.Text, r);

                         });

            //same for export.
            lvwReportResult.InitCopyDropDown(
                tStripExportResult, "Export", "RO",ExporterFunction);


            foreach (ListView loopview in new ListView[] { lvwUsers, lvwOrders ,lvwLogData,lvwReportResult})
            {
                loopview.SizeColumnsEqual();



            }
            tStripExportResult.Image = JobClockConfig.Imageman.GetLoadedImage("export_small");
            tStripExportResult.ToolTipText = "Export Report to a file";

            if (KeyboardInfo.IsPressed(Keys.ShiftKey) && KeyboardInfo.IsPressed(Keys.ControlKey))
            {
                cmdReinitialize_Click(cmdReinitialize, new EventArgs());


            }

            //changed export tab to "Reporting".

            cboReportBy.Items.AddRange(new String[] { UserReportString, ROReportString,DateReportString});
            //Possible change: make it load/save the selected index to/from the INI file (?).
            cboReportBy.SelectedIndex = 1;

            //initialize the listbox for exporting, too.

           // lstExporters.Items.Clear();
       
            /*
            var gotexportermanager = Database.mtypemanager[typeof(BaseDataExporter)];
            foreach (Type iterateit in gotexportermanager.ManagedTypes)
            {
                //create an instance, and add a new listitem.
                try
                {
                    BaseDataExporter grabbedexporter = (BaseDataExporter)Activator.CreateInstance(iterateit);


                    ListViewItem lvi = new ListViewItem(new string[] { grabbedexporter.getName() });
                    lvi.Tag = grabbedexporter;

                   // lvwExporters.Items.Add(lvi);

                }
                catch (Exception ee)
                {
                    Debug.Print("Unexpected exception instantiating BASeDataExporter");

                }



            }
            */


            chkHidePIN.Checked = Database.Configuration.Client_PasswordPIN;

            TabPageLogs.Visible=false; //"secret"...
            Database.WriteDebugMessage("Calling RefreshGeneral()...");
            RefreshGeneral();
            Database.WriteDebugMessage("Calling RefreshUserList()...");
            RefreshUserList();
            Database.WriteDebugMessage("Calling RefreshOrderList()...");
            RefreshOrderList();
            DateRange_Report = new DateRange(DayStart().AddDays(-1), DayEnd().AddDays(1));
            Image asc = JobClockConfig.Imageman.GetLoadedImage("UPARROW");
            Image desc = JobClockConfig.Imageman.GetLoadedImage("DOWNARROW");
            UserListSorter = new GenericListViewSorter(lvwUsers, null);
            OrderListSorter = new GenericListViewSorter(lvwOrders, null);
            ReportListSorter = new GenericListViewSorter(lvwReportResult,null);


            OrderSelChange = new BeforeSelItemChange(lvwOrders);
            UserSelChange = new BeforeSelItemChange(lvwUsers);
            OrderSelChange.fireChange += new BeforeSelItemChange.BeforeItemChangeFunction(OrderSelChange_fireChange);
            UserSelChange.fireChange += new BeforeSelItemChange.BeforeItemChangeFunction(UserSelChange_fireChange);

            lvwOrders.GotFocus += new EventHandler(lvwOrders_GotFocus);
            lvwUsers.GotFocus += new EventHandler(lvwUsers_GotFocus);
            //refreshTimer = new Timer(PeriodicRefresh, null, 0, 1000); //refresh every second
            //use RefreshInterval from INI.
            Database.WriteDebugMessage("Database.Configuration.Admin_RefreshIntervalms set to " + Database.Configuration.Admin_RefreshIntervalms.ToString());
            refreshTimer = new Timer(PeriodicRefresh, null, 0, Database.Configuration.Admin_RefreshIntervalms);
            //set minimum size to our current size.
            MinimumSize = Size;
            TabPageLogs.Visible = false;
        }

        void lvwUsers_GotFocus(object sender, EventArgs e)
        {
            UpdateUserFields();
        }

        void lvwOrders_GotFocus(object sender, EventArgs e)
        {

            UpdateOrderFields();
        }

        void UserSelChange_fireChange(ListViewItem previousItem, ListViewItem CurrentItem)
        {
            //throw new NotImplementedException();
        }

        void OrderSelChange_fireChange(ListViewItem previousItem, ListViewItem CurrentItem)
        {
            

            //throw new NotImplementedException();
        }
        private void ExporterFunction(String ExportThis,BCJobClockLib.ListViewExtensions.DropDownItemListViewCopyData ddlc)
        {
            String usefilter;
            Debug.Print("ExporterFunction called");
            switch (ddlc.CopyType)
            {
                case DataLayer.ListViewStringConstants.Copy_HTMLFRAGMENT:
                    usefilter = "HTML Files (*.html)|*.html|All Files(*.*)|*.*";
    break;
                case DataLayer.ListViewStringConstants.Copy_CSV:
                    usefilter = "CSV Files (*.csv)|*.csv|All Files(*.*)|*.*";
                    break;
                default:
                    return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter=usefilter;
            sfd.OverwritePrompt=true;
            if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writeit = new StreamWriter(new FileStream(sfd.FileName, FileMode.Create));
                writeit.Write(ExportThis);

                writeit.Close();


            }

        }

        private void RefreshGeneral()
        {
            txtAdminConnectionString.Text = Database.Configuration.AdminConnectionString;
            txtClientConnectionString.Text = Database.Configuration.ClientConnectionString;
            txtDatabaseName.Text = Database.Configuration.DatabaseName;
            chkAutoAddOrders.Checked = Database.Configuration.AutoAddOrders;
            chkSingleWorkorderMode.Checked = Database.Configuration.SingleWorkOrderMode != "";
            txtSingleWorkOrderMode.Text = Database.Configuration.SingleWorkOrderMode;
            chkuserlisting.Checked = Database.Configuration.Client_ShowUserList;


        }
        bool inRefreshRoutine = false;
        private void PeriodicRefresh(Object objy)
        {
            if (inRefreshRoutine) return;
            inRefreshRoutine = true;

            try
            {
                this.BeginInvoke((MethodInvoker)(() =>
                                                 {
                                                     try
                                                     {
                                                         Database.WriteDebugMessage("PeriodicRefresh-UserList...");
                                                         RefreshUserList();
                                                         Database.WriteDebugMessage("PeriodicRefresh-OrderList...");
                                                         RefreshOrderList();
                                                         Database.WriteDebugMessage("PeriodicRefresh-LogList...");
                                                         //RefreshLogList();
                                                     }
                                                     catch (Exception refreshexception)
                                                     {
                                                         Debug.Print("Exception during refresh:" +
                                                                     refreshexception.Message);


                                                     }
                                                 }));

            }
            catch (ObjectDisposedException)
            {
                refreshTimer.Dispose();



            }
            finally
            {
                inRefreshRoutine = false;

            }
        }

        private void RefreshOrderList()
        {
           // Thread.Sleep(500);
            List<String> uniqueOrders;
            lock (Database)
            {
                uniqueOrders = Database.GetAllOrderIDs();
            }
            var orderdates=Database.GetWorkOrderDates();
            foreach (String looporder in uniqueOrders)
            {
                //String gotDescription
                //use the IEnumerable And LINQ to see if there is one already there...
                var changelistitem = SearchListView_First(lvwOrders,(lstitem)=>((String)lstitem.Tag).Equals(looporder));

                if (changelistitem == null)
                {
                    //add a new one to the list.
                    changelistitem = new ListViewItem(new string[] {"ORDERID","INITSTART","TOTALTIME","ACTIVEWORKERS","LASTCLOCKOUT","DESCRIPTION"});
                    lvwOrders.Items.Add(changelistitem);


                }

                changelistitem.Text=looporder;

                //get the initial start time of this order...
                String iStartString = "";
                if (!orderdates.ContainsKey(looporder))
                {
                    iStartString = "Never";
                }
                else
                {
                    iStartString = orderdates[looporder].ToString();
                }
                
                //Get the total worker time...
                TimeSpan TotalClockedTime = Database.GetTotalClockedTimeForOrder(looporder,false);
                //get the number and name of Active Workers...
                List<String> ActiveWorkers = Database.GetWorkOrderActiveWorkers(looporder);

              

                changelistitem.SubItems[1].Text = iStartString;
                changelistitem.SubItems[2].Text = DataLayer.FormatTimeSpan(TotalClockedTime);
                changelistitem.SubItems[3].Text = ActiveWorkers.Count.ToString() + ((ActiveWorkers.Count == 0) ? "" :
                    "(" + String.Join(",", ActiveWorkers.ToArray()) + ")");

                DateTime? getlastclockout = Database.GetLastClockOutForOrder(looporder);
                String lastclockout = getlastclockout == null ? "" : getlastclockout.Value.ToString();
                changelistitem.SubItems[4].Text = lastclockout;
                changelistitem.SubItems[5].Text = Database.GetOrderDescription(looporder);

                if (ActiveWorkers.Count > 0)
                {
                    //green for active, red for inactive.
                    changelistitem.BackColor = Color.Lime;



                }
                else
                {
                    changelistitem.BackColor = Color.Red;
                }

                changelistitem.Tag = looporder;
            }










        }

        private void toolstripdeleteuser_Click(object sender, EventArgs e)
        {
            if (SelUserItem == null) return;

            String gotusername = SelUserItem.Text;
            string gotuserpin = SelUserItem.SubItems[1].Text;
            String deletequery = Database.qAdapter["DELONUSERPINFMT"];
            DbConnection usecon = Database.GetConnection();
            DbCommand exec = usecon.CreateCommand();
            exec.CommandText=String.Format(deletequery,gotusername,gotuserpin);
            exec.ExecuteNonQuery();
            lvwUsers.Items.Remove(SelUserItem);

            RefreshUserList();


           
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lvwUsers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateUserFields();
            
        }

        private void DropDownOrders_DropDownOpening(object sender, EventArgs e)
        {
            
            //Clear out the drop down, first.
            ToolStripDropDownButton showforbutton = (ToolStripDropDownButton)sender;
            showforbutton.DropDownItems.Clear();


            if (SelUserItem == null)
            {
                //if no item is selected, show a "disabled" item showing why.
                ToolStripMenuItem createditem = new ToolStripMenuItem("<No User Selected>");
                createditem.Enabled=false;
                showforbutton.DropDownItems.Add(createditem);
                return; 
            }
            int GrabrecordID = int.Parse(SelUserItem.Tag.ToString());


            //grab the data for the item...
           
                DbConnection dbcon = Database.GetConnection();
                DbCommand executor = dbcon.CreateCommand();
                String sName, sPIN;
                executor.CommandText = String.Format(Database.qAdapter["FROMRECORDIDFMT"], GrabrecordID.ToString());
                using (DbDataReader usereader = executor.ExecuteReader())
                {
                    usereader.Read();
                    if (!usereader.HasRows) return;
                    sName = usereader.GetString(usereader.GetOrdinal("UserName"));
                    sPIN = usereader.GetString(usereader.GetOrdinal("PINCode"));

                }


             if (Database.Configuration.PopulateUserOrderDropdown)
                        {

                //armed with the username and pin code, now we can get a list of all the Active WorkOrders for this user.
                List<String> AllOrders = Database.GetAllOrderIDs();

                //if there aren't any orders, display a disabled toolstripitem stating as much.

                if (AllOrders.Count == 0)
                {
                    ToolStripMenuItem noordersitem = new ToolStripMenuItem("<No Orders in Database>");
                    noordersitem.Enabled = false;
                    showforbutton.DropDownItems.Add(noordersitem);

                    return;


                }

                List<String> AllUserOrders = Database.GetUserOrders(sName, true);




                //create an item that will be used to essentially "unclock" the selected user from that order.

                foreach (String createorder in AllOrders)
                {
                    ToolStripMenuItem ClockOutMade = new ToolStripMenuItem();
                    if (AllUserOrders.Contains(createorder))
                    {
                        ClockOutMade.Text = "Clock " + sName + " Out of Work Order " + createorder;
                        ClockOutMade.ForeColor = Color.Red;
                    }
                    else
                    {
                        //set images here, as well.
                        ClockOutMade.Text = "Clock " + sName + " in to Work Order " + createorder;
                        ClockOutMade.ForeColor = Color.Green;
                        //ClockOutMade.Image = 
                    }

                    ClockOutMade.Tag = sPIN + "," + createorder;
                    Debug.Print(sPIN, createorder);

                    ClockOutMade.Click += new EventHandler(ClockOutMade_Click);


                    //add it to the menu.
                    showforbutton.DropDownItems.Add(ClockOutMade);

                }


                //add a separator

                showforbutton.DropDownItems.Add(new ToolStripSeparator());
            

            //and add one to remove them from all clocked in tasks.

           


                } //Database.Configuration.ShowUserOrderDropDown 



            ToolStripMenuItem createremoveitem = new ToolStripMenuItem("Clock out of all WorkOrders");
            createremoveitem.Click += new EventHandler(createremoveitem_Click);
            createremoveitem.Tag = sName + "," + sPIN;
            showforbutton.DropDownItems.Add(createremoveitem);






            //add a single item for each Order in the selected item.
            


        }

        void createremoveitem_Click(object sender, EventArgs e)
        {
            
            //task: remove all Orders for the given user.
            String grabname,grabpin;
            String[] splitvar = (((String)((ToolStripMenuItem)(sender)).Tag)).Split(new char[] { ','});
            grabname = splitvar[0];
            grabpin = splitvar[1];
            var gotallorders = Database.GetUserOrders(grabname);


            foreach (String removeit in gotallorders)
            {

               // Database.ToggleUserClockin(grabpin, removeit);
                Database.ClockOutOrder(grabpin, removeit);
            }

            RefreshUserList();
        }

        void ClockOutMade_Click(object sender, EventArgs e)
        {
            String gottag = (String)(((ToolStripMenuItem)sender).Tag);
            
            //split at comma.
            String[] result = gottag.Split(new char[] { ',' });
            Debug.Print("UserPIN:" + result[0], "OrderID:" + result[1]);
            //first is UserPIN, second is OrderID. we just want to toggle it...
            Database.ToggleUserClockin(result[0], result[1]);

            RefreshUserList();
        }
       

        private ListViewItem selOrderItem
        {
            get {
                if (lvwOrders.SelectedItems.Count ==0) return null;
            return lvwOrders.SelectedItems[0];
            
            }


        }
        
            private void lvwOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
                
            tStripRemoveOrder.Enabled=true;
            UpdateOrderFields();
        }

        private void lvwOrders_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
        }

        private void tStripRefreshOrders_Click(object sender, EventArgs e)
        {
            RefreshOrderList();
        }

        private void tStripRemoveOrder_Click(object sender, EventArgs e)
        {
            //removes all orders.

            if (MessageBox.Show(this, "This will Remove ALL Database entries related to RO#" + selOrderItem.Text + ". This Operation is not undoable! Are you sure?", "Confirmation Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {

                //remove ALL orders with a given order ID.
                String removeorderqueryfmt = "DELETE FROM OrderData WHERE `Order`=\"{0}\";DELETE FROM TheOrders WHERE `OrderID`=\"{0}\"";
                String removeorderquery = String.Format(removeorderqueryfmt, selOrderItem.Text);

                Database.ExecuteQueryDirect(removeorderquery);



            }



        }

        private void frmJobClockAdmin_Resize(object sender, EventArgs e)
        {
            cmdClose.Top = ClientSize.Height- cmdClose.Height - 4;
            cmdClose.Left = ClientSize.Width - cmdClose.Width - 4;
            tabAdminpanel.Location = new Point(2, 2);
            tabAdminpanel.Size = new Size(ClientSize.Width-4,cmdClose.Top-6);


            Invalidate();
            Update();

        }

        private void TabPageUsers_Resize(object sender, EventArgs e)
        {
            lvwUsers.Top = ToolStripUsers.Height + 3;
            GroupEditingUser.Location = new Point(2, TabPageUsers.ClientSize.Height - GroupEditingUser.Height - 2);
            GroupEditingUser.Width = TabPageUsers.ClientSize.Width - 4;
            lvwUsers.Height = GroupEditingUser.Top - 2 - lvwUsers.Top;
            lvwUsers.Width = TabPageUsers.ClientSize.Width - 4;
        }

        private void tabAdminpanel_Resize(object sender, EventArgs e)
        {

        }

        private void TabPageOrders_Resize(object sender, EventArgs e)
        {
            lvwOrders.Top = tStripOrders.Height + 3;
            GroupEditingOrder.Location = new Point(2, TabPageOrders.ClientSize.Height - GroupEditingOrder.Height - 2);
            GroupEditingOrder.Width = TabPageOrders.ClientSize.Width - 4;
            lvwOrders.Height = GroupEditingOrder.Top - 2 - lvwOrders.Top;
            lvwOrders.Width = TabPageOrders.ClientSize.Width - 4;
        }

        private void cmdApplyDB_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to change the database configuration?", "Change Config", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {

                //change it and restart.

                Database.Configuration.AdminConnectionString = txtAdminConnectionString.Text;
                Database.Configuration.ClientConnectionString = txtClientConnectionString.Text;
                Database.Configuration.DatabaseName=txtDatabaseName.Text;
                Application.Restart();


            }
        }

        private void cmdReinitialize_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reinitialize the Database? this will delete all data that currently resides in the database named \"" + Database.DatabaseName + "\".", "Reinitialize", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                Database.ReInitializeDB();
                //Application.Restart();


            }
        }

        private void tStripNewOrder_Click(object sender, EventArgs e)
        {
            AddNewOrder();
            //create a new order.
    //        lvwOrders.Columns.Add("ORDERID", "OrderID");
    //        lvwOrders.Columns.Add("INITIALSTART", "Initial Start"); //initial time, the very first entry for this order. (earliest FirstTime)
    //        lvwOrders.Columns.Add("TOTALTIME", "Total Worker Time"); //Total time, accumulated by all workers. will include time not clocked out yet.
    //        lvwOrders.Columns.Add("ACTIVEWORKERS", "Active Workers"); // Number of active workers, a number followed by a comma-separated listing of names.
    //        lvwOrders.Columns.Add("LASTCLOCK", "Last Clock Out");
    //        lvwOrders.Columns.Add("DESCRIPTION", "Description");

            


        }

        private void chkSingleWorkorderMode_CheckedChanged(object sender, EventArgs e)
        {
            txtSingleWorkOrderMode.Enabled = chkSingleWorkorderMode.Checked;
            if (chkSingleWorkorderMode.Checked = false)
                txtSingleWorkOrderMode.Text = "";

        }

        private void cmdPurgeLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Purge the Log?", "Purge Log", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                String sqldroplog = Database.qAdapter["DROPMESSAGES"];
                DbConnection cons = Database.GetConnection();
                DbCommand usecmd = cons.CreateCommand();
                usecmd.CommandText = sqldroplog;
                usecmd.ExecuteNonQuery();
                RefreshLogList();


            }
        }

        private void tStripImportUsers_Click(object sender, EventArgs e)
        {
            //import from an external file.

            List<String> CurrentPINs = (from m in Database.GetUsers().Keys select m).ToList();


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                String usefilename = ofd.FileName;

                FileStream fs = new FileStream(usefilename,FileMode.Open);
                StreamReader sr = new StreamReader(fs);


                String currentline;
                while (!sr.EndOfStream)
                {
                    currentline = sr.ReadLine();
                    String[] splitresult = currentline.Split(',');
                    String addusername = "", adduserpin = "";

                    if(splitresult.Length==0) continue;
                    //if we weren't given a second parameter, or if the second parameter is in the list of current PINs...
                    if (splitresult.Length == 1 || CurrentPINs.Contains(splitresult[1]))
                    {
                        //regenerate the PIN.
                        do
                        {
                            adduserpin = String.Format("{0:0000}", rnd.Next(0, 10000));
                        } while (CurrentPINs.Contains(adduserpin));
                        CurrentPINs.Add(adduserpin); //add it to our list so we don't use it again, either.
                    }
                    else
                    {
                        adduserpin = splitresult[1];
                    }
                    addusername = splitresult[0];
                    //add this user to the DB.
                    Database.AddUser(addusername, adduserpin);


                }


                RefreshUserList();





            }




        }

        private void cmdReportExcel_Click(object sender, EventArgs e)
        {

        }

        private void txtOrderID_Validated(object sender, EventArgs e)
        {
            UpdateOrderListView();
        }

        private void txtDescription_Validated(object sender, EventArgs e)
        {
            UpdateOrderListView();
        }

        private void DropDownUsers_DropDownOpening(object sender, EventArgs e)
        {

            //populate with users
            bool hasclockedinusers=false;
            ToolStripDropDownButton tsd = (ToolStripDropDownButton)sender;

            tsd.DropDownItems.Clear();
            if(selOrderItem==null) return; //do nothing...
            //get a list of all the users. This is sort of obvious.
            String selectedRO = (String)selOrderItem.Tag;
            Dictionary<String, String> allusers = Database.GetUsers();
            List<String> AllPINs = (from m in allusers 
                                    where Database.IsUserActive(m.Value) 
                                    select Database.GetUserPIN(m.Value)).ToList();

            if (allusers.Count == 0)
            {
                ToolStripMenuItem createitem = new ToolStripMenuItem("No Active Users in DB...");
                createitem.Enabled=false;
                tsd.DropDownItems.Add(createitem);
                return;
            }
            //now add a new item for each one, set the Tag to the User's PIN.
            String joinedusernames = String.Join("\0",allusers.Values.ToArray());
            String JoinedPINs = String.Join("\0", AllPINs.ToArray());
            foreach (var iteratevalue in allusers)
            {
                ToolStripMenuItem OrderDropItem = new ToolStripMenuItem();


                List<String> results = Database.GetUserOrders(iteratevalue.Value,true);
                //if this order (selectedRO) is present, the user is clocked in, so show the "clock user out" text.
                //other wise, they aren't show show the clock in text.
                if (results.Contains(selectedRO))
                {
                    OrderDropItem.Text="Clock User \"" + iteratevalue.Value + " \" out of RO#" + selectedRO;
                    
                    OrderDropItem.ForeColor=Color.Red;
                    OrderDropItem.Tag = "OUT:" + iteratevalue.Key;
                    hasclockedinusers=true;
                }
                else
                {
                    OrderDropItem.Text = "Clock user \"" + iteratevalue.Value + " \" into RO#" + selectedRO;
                    OrderDropItem.ForeColor = Color.Green;
                    OrderDropItem.Tag = "IN:" + iteratevalue.Key;
                }


                

                tsd.DropDownItems.Add(OrderDropItem);
                
                //add the event handler.
                OrderDropItem.Click += new EventHandler(OrderDropItem_Click);                



            }
            if (hasclockedinusers)
            {
                tsd.DropDownItems.Add(new ToolStripSeparator());

                //and a new "Clock All Users out of order RO#" item.
                ToolStripMenuItem Clockoutallorders =
                    new ToolStripMenuItem("Clock All Users out of Order RO#" + selectedRO, null, OrderDropItem_Click);
                // ToolStripMenuItem Clockinallorders = new ToolStripMenuItem("Clock All Users in to Order RO#" + selectedRO, null, OrderDropItem_Click);
                Clockoutallorders.Tag = "OUT\0" + JoinedPINs;
                // Clockinallorders.Tag = "IN\0" + joinedusernames;
                Clockoutallorders.ForeColor = Color.Red;
                Clockoutallorders.Font = new Font(Clockoutallorders.Font, FontStyle.Bold);
                //     Clockinallorders.ForeColor=Color.Green;
                // Clockinallorders.Font = new Font(Clockinallorders.Font, FontStyle.Bold);
                tsd.DropDownItems.Add(Clockoutallorders);
                // tsd.DropDownItems.Add(Clockinallorders);
            }

        }

        void OrderDropItem_Click(object sender, EventArgs e)
        {
            //tag is the UserID. OrderID is SelOrderItem.
            ToolStripMenuItem clicked = (ToolStripMenuItem) sender;
            String RO = (String)selOrderItem.Tag;
            String UID = (String)clicked.Tag; 


            //tag for the "clock out all" and "clock in all" will be
            //a null delimited list, first item either "OUT" or "IN".

            if(UID.Contains("\0"))
            {
                String[] splitvalues = UID.Split('\0');

                bool doclockin = false;
                if (splitvalues[0] == "IN")
                    doclockin = true;



                for (int u = 1; u < splitvalues.Length; u++)
                {

                    if (!doclockin)
                        Database.ClockOutOrder(splitvalues[u], RO);


                }


            }
            else
            {
                String cinout = UID.Split(':')[0];
                String uidinout = UID.Split(':')[1];

                if (cinout == "IN")
                {
                    Debug.Print("Attempting to clock user \"" + uidinout + "\" into RO#" + RO);
                    Database.ClockUserIn(uidinout, RO);


                }
                else
                {
                    Debug.Print("Attempting to clock user \"" + uidinout + "\" out of RO#" + RO);
                    Database.ClockOutOrder(uidinout, RO);
                }




            }
            RefreshOrderList();
            //throw new NotImplementedException();
        }
        /*
        private void lstExporters_SelectedValueChanged(object sender, EventArgs e)
        {
            cmdRunExporter.Enabled=true;
        }

        private void lvwExporters_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdRunExporter.Enabled=true;
        }

        private void cmdRunExporter_Click(object sender, EventArgs e)
        {
            if(lvwExporters.SelectedItems.Count==0) return;
            ListViewItem selectitem = lvwExporters.SelectedItems[0];
            BaseDataExporter grabexporter = (BaseDataExporter)(selectitem.Tag);
            //if it has a configuration page, show it before we run the exporter.
            if (grabexporter.hasConfigPage())
            {
                grabexporter.Configure(this);

            }
            //run it!
            try
            {
                grabexporter.PerformExport(Database);
            }
            catch (Exception exportexception)
            {
                //hmmm.



            }
        }
        */
        private void button1_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show("Client con:" + Database.Configuration.ClientConnectionString + "\nAdmin con:" + Database.Configuration.AdminConnectionString);
            MessageBox.Show("Con:\"" + Database.buildConnectionString(DataLayer.ConnectionTypeConstants.Connection_Admin) + "\"\nClient:\"" + 
                Database.buildConnectionString(DataLayer.ConnectionTypeConstants.Connection_Client) + "\"");
        }

        private void tStripCopyUsers_Click(object sender, EventArgs e)
        {
            //copy the Users Listview.
            DoCopyListView(lvwUsers);

        }
        
        private static void DoCopyListView(ListView copyit)
        {
            DataLayer.ListViewToClipboard(copyit);

     


        }

        private void toolstripcopyorders_Click(object sender, EventArgs e)
        {
            DoCopyListView(lvwOrders);
        }

        private void DropDownUsers_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Here:" + DateTime.Now + Environment.NewLine + "There:" + Database.DBTime());
        }


        private class ROReportListEntry
        {
            public readonly String _Order;
            public readonly DataLayer _layerobject;
            public ROReportListEntry(DataLayer dblayer, String OrderID)
            {
                _Order=OrderID;
                _layerobject=dblayer;

            }

            public override string ToString()
            {
                String buildstr = "";
                if (_layerobject.Configuration.ReportList_ShowOrderDescription)
                {
                    String desc = _layerobject.GetOrderDescription(_Order);

                    return "RO#" + _Order + "(" + desc + ")";
                }
                else
                {
                    //no need to hit the database if no description is needed/wanted.
                    return "RO#" + _Order;
                }

            }


        }
        /// <summary>
        /// class used for addition in the Second Report By Field, which lists the items to report.
        /// </summary>
        private class UserReportListEntry
        {
            public readonly String UserPIN;
            public readonly String UserName;
            public readonly DataLayer layerobject;



            public UserReportListEntry(DataLayer dblayer, String UserPIN)
            {
                this.UserPIN = UserPIN;
                layerobject = dblayer;
                UserName = dblayer.UserNameFromPIN(UserPIN);
            }
            public UserReportListEntry(DataLayer dblayer, String UserPIN, String UserName)
            {
                this.UserPIN = UserPIN;
                this.UserName = UserName;
                layerobject=dblayer;



            }

            public override string ToString()
            {
                String uname = UserName;
                String response = uname;
                if (layerobject.Configuration.ReportList_ShowUserPINCode)
                {
                    response += "(" + UserPIN + ")";
                }
                return response;


            }


        }
        //returns the start of today (midnight the previous night.)
        private static DateTime DayStart()
        {
            DateTime n =  DateTime.Now;
            return n.Subtract(n.TimeOfDay);

            
        }
        //returns the end of today. (midnight of tonight)
        private static DateTime DayEnd()
        {
            DateTime n =  DateTime.Now;

            return n.AddDays(1).Subtract(n.TimeOfDay);


        }
        //TODO: possibly make SelectedDateRange something that is saved/restored in some fashion?
        private DateRange _SelectedDateRange = new DateRange(DayStart(),DayEnd()); //zero day...

        public DateRange DateRange_Report
        {
            get { return _SelectedDateRange; }
            set
            {
                _SelectedDateRange = value;
                lblReportPeriod.Text = String.Format("Reporting From {0} To {1}", _SelectedDateRange.StartTime.ToShortDateString(),
                                                     _SelectedDateRange.EndTime.ToShortDateString());
            }

        }


        private void cboReportBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            String Selectedval = (String)cboReportBy.SelectedItem;


            //ROReportString
            //    UserReportString 
            //clear the reportby List
            cboReportByListing.Items.Clear();
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            if (Selectedval.Equals(ROReportString))
            {
                //populate cboReportByListing with all RO #'s.
                cboReportByListing.Enabled = true;
                var AllOrders = Database.GetAllOrderIDs();
                
                foreach(String iterateOrder in AllOrders)
                {
                    //create a new ROReportListEntry, and add it to the list.
                    ROReportListEntry ROEntry = new ROReportListEntry(Database, iterateOrder);
                    cboReportByListing.Items.Add(ROEntry);




                }

                //Also, add another item "All Work Orders". But only if .ReportList_ShowAllOptionOrders is true.
                if (Database.Configuration.ReportList_ShowAllOptionOrders)
                    cboReportByListing.SelectedIndex =cboReportByListing.Items.Add(AllOrdersReportString);
                

                //Task completed.... for now, anyway.


            }
            else if (Selectedval.Equals(UserReportString))
            {
                //populate cboReportByListing with all Users.
                var allUsers = Database.GetUsers();
                cboReportByListing.Enabled = true;

                foreach (var iterateval in allUsers)
                {
                    String PIN = iterateval.Key;
                    String Name = iterateval.Value;
                    UserReportListEntry urle = new UserReportListEntry(Database, PIN, Name);
                    //add it to the list.
                    cboReportByListing.Items.Add(urle);





                }

                //if specified in INI show the "All" value.
                if (Database.Configuration.ReportList_ShowAllOptionUsers)
                    cboReportByListing.SelectedIndex = cboReportByListing.Items.Add(AllUsersReportString);



            }
            else if (Selectedval.Equals(DateReportString))
            {
                cboReportByListing.Items.Clear();
                cboReportByListing.Enabled = false;



            }



            System.Windows.Forms.Cursor.Current = Cursors.Arrow;

            //our  task is completed for the time being.



        }
        private static readonly String AllUsersReportString = "<All Users>";
        private static readonly String AllOrdersReportString = "<All Orders>";
        private void cmdChangeRange_Click(object sender, EventArgs e)
        {
            frmDateRangePicker.BasicDateRange currentrange = new frmDateRangePicker.BasicDateRange(DateRange_Report.StartTime, DateRange_Report.EndTime);
            frmDateRangePicker.BasicDateRange bdr = frmDateRangePicker.ChooseRange(this, currentrange, "Change Report Range", 
                Database.Configuration.DateRangePicker_ColumnCount, Database.Configuration.DateRangePicker_RowCount);

            DateTime usestartdate = bdr.StartDate.Subtract(bdr.StartDate.TimeOfDay);
            DateTime useEndDate = bdr.EndDate.Subtract(bdr.EndDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

            DateRange_Report = new DateRange(bdr.StartDate, bdr.EndDate);

        }

        private void TabPageReporting_Resize(object sender, EventArgs e)
        {
            fraDateRange.Width = TabPageReporting.ClientSize.Width - 5 - fraDateRange.Left;
            fraReportResult.Size = new Size(fraDateRange.Width, TabPageReporting.ClientRectangle.Bottom - fraReportResult.Top);
            
        }

        private void fraReportResult_Resize(object sender, EventArgs e)
        {
            lvwReportResult.Location = new Point(2, toolStripReportResults.Height+2 + toolStripReportResults.Top);
            lvwReportResult.Size = new Size(fraReportResult.ClientSize.Width-4-lvwReportResult.Left,fraReportResult.ClientSize.Height - 4 - lvwReportResult.Top);
        }

        private void cmdReport_Click(object sender, EventArgs e)
        {

            bool hascleared = false;
            if(cboReportBy.SelectedItem == DateReportString)
            {
                ReportByDate(DateRange_Report);


                
            }
            else if (cboReportBy.SelectedItem == ROReportString)
            {
                //By Order
                if (cboReportByListing.SelectedItem == AllOrdersReportString)
                {
                    hascleared=false;
                    lvwReportResult.ShowGroups = true;
                    //all orders reporting...
                    foreach (var iteratevalue in cboReportByListing.Items)
                    {
                        ROReportListEntry castentrydata = iteratevalue as ROReportListEntry;
                        if (castentrydata != null)
                        {
                            if (!hascleared)
                            {
                                hascleared = true;
                                
                                ReportByOrder(castentrydata._Order, DateRange_Report, true, true);



                            }
                            else
                            {
                                
                                ReportByOrder(castentrydata._Order, DateRange_Report, false, true);
                            }
                        }

                    }



                }
                else
                {
                    lvwReportResult.ShowGroups = false;
                    ROReportListEntry castentrydata = cboReportByListing.SelectedItem as ROReportListEntry;
                    if (castentrydata != null) ReportByOrder(castentrydata._Order, DateRange_Report, true, false);
                    
                    
                }

            }
            else if (ReferenceEquals(cboReportBy.SelectedItem, UserReportString))
            {

                if (cboReportByListing.SelectedItem == AllUsersReportString)
                {
                    //All Users reporting...
                    hascleared = false;
                    lvwReportResult.ShowGroups=true;
                    foreach (var iteratevalue in cboReportByListing.Items)
                    {
                        //skip items that aren't a UserReportListEntry...
                        UserReportListEntry castentrydata = iteratevalue as UserReportListEntry;
                        if (castentrydata != null)
                        {
                            if (!hascleared)
                            {
                                //if we haven't cleared it yet, do so, by passing in the appropriate param.
                                //we're looking at multiple users so pass in that as true as well.
                             
                                ReportByUser(castentrydata.UserPIN, DateRange_Report, true, true);

                                hascleared = true; //set flag to true so we don't clear it again.
                            }
                            else
                            {
                          
                                ReportByUser(castentrydata.UserPIN, DateRange_Report, false, true);
                            }

                        }

                    }



                }
                else
                {
                    lvwReportResult.ShowGroups=false;
                    UserReportListEntry userentrydata = cboReportByListing.SelectedItem as UserReportListEntry;
                    //By User.
                    if (userentrydata != null) ReportByUser(userentrydata.UserPIN, DateRange_Report, true, false);
                }
            }

            //MessageBox.Show("Result Count:" + lvwReportResult.Items.Count);
 
            






        }

        private void ReportByOrder(String OrderID, DateRange reportrange)
        {
            ReportByOrder(OrderID, reportrange, true, false);


        }
        private void ReportByDate(DateRange dates)
        {
            //add columns: Date, Techs, Orders (no date column, that will be handled by the  group...)
            //Date shows the date (duh). Techs lists the Techs that were active on that day, (count, with names in brackets).
            //orders is the same idea sa the Tech's listing but for Orders instead.


            lvwReportResult.Items.Clear();
            lvwReportResult.Columns.Clear();
            lvwReportResult.Groups.Clear();
            lvwReportResult.ShowGroups = true;
            lvwReportResult.Columns.Add("TECHS", "Techs");
            lvwReportResult.Columns.Add("ORDERS", "Orders");
            
            List<DateRange> dayssplit = dates.SplitToDays();
            //Totals for each user (by PIN).
            Dictionary<String, TimeSpan> UserTotals = new Dictionary<string, TimeSpan>();
            Dictionary<String, TimeSpan> OrderTotals = new Dictionary<string, TimeSpan>();
            Dictionary<String, Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>> UserStacks =
    new Dictionary<string, Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>>();
            Dictionary<String, Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>> OrderStacks =
                new Dictionary<string, Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>>();
            foreach (DateRange iteraterange in dayssplit)
            {

                //add the group...
                ListViewGroup lvg= lvwReportResult.Groups.Add(iteraterange.StartTime.ToShortDateString(), iteraterange.StartTime.ToShortDateString());

                //get the clocked entries in that "range" (for the day)...
                var clockedentries = Database.GetClockedInRange(iteraterange);
                if (clockedentries.Count > 0)
                {
                    Debug.Print("Clockedentriescount>0");

                }
                List<String> thePINs = new List<string>();
                List<String> theOrders = new List<string>();


                Dictionary<String, TimeSpan> userTimes = new Dictionary<string, TimeSpan>();
                Dictionary<String, TimeSpan> orderTimes = new Dictionary<string, TimeSpan>();

                //first, get all the different UserPIN's and Orders in the returned list.
                foreach (var iterate in clockedentries)
                {
                    if (!thePINs.Contains(iterate.PIN)) thePINs.Add(iterate.PIN);
                    if (!theOrders.Contains(iterate.OrderID)) theOrders.Add(iterate.OrderID);
                    //if (!UserTotals.ContainsKey(iterate.PIN)) UserTotals.Add(iterate.PIN,new TimeSpan());
                    //if (!OrderTotals.ContainsKey(iterate.OrderID)) OrderTotals.Add(iterate.OrderID, new TimeSpan());

                }
                UserStacks.Clear();
                OrderStacks.Clear();
                foreach (var U in thePINs)
                {
                    UserStacks.Add(U, new Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>());
                    userTimes.Add(U, new TimeSpan());
                    if(!UserTotals.ContainsKey(U)) UserTotals.Add(U,new TimeSpan());
                }
                foreach (var O in theOrders)
                {
                    OrderStacks.Add(O, new Stack<KeyValuePair<DateTime, DataLayer.ClockDataEntry>>());
                    orderTimes.Add(O, new TimeSpan());
                    if(!OrderTotals.ContainsKey(O)) OrderTotals.Add(O, new TimeSpan());
                }
                
                
                //now, armed with that data, we can iterate through it in ascending order...
                foreach (var p in (from m in clockedentries orderby m.STAMP ascending select m))
                {
                    
                    if (p.EventType == "IN")
                    {
                        //push the Order onto the appropriate UserStack
                        //and Push the User onto the Appropriate OrderStack
                        //both with the STAMP as their key.
                        UserStacks[p.PIN].Push(new KeyValuePair<DateTime, DataLayer.ClockDataEntry>(p.STAMP, p));
                        OrderStacks[p.OrderID].Push(new KeyValuePair<DateTime, DataLayer.ClockDataEntry>(p.STAMP, p));


                    }
                    else if (p.EventType == "OUT")
                    {
                        if (UserStacks[p.PIN].Count > 0)
                        {
                            var result = UserStacks[p.PIN].Pop();
                            if (UserStacks[p.PIN].Count == 0)
                            {
                                //last item popped; get the timespan...
                                TimeSpan currspan = p.STAMP - result.Key;
                                //add to UserTimes....
                                userTimes[p.PIN] += currspan.Duration();
                                UserTotals[p.PIN] += currspan.Duration();
                            }
                        }
                        else
                        {
                            //if the stack has no items And the current accumulated time is 0,
                            //add in the difference between the start of the daterange and this entry.

                            //this will account for time when a user clocks in before the date range, and clocks out during it, so we'll
                            //have that block of time.

                            if (((int)userTimes[p.PIN].TotalMilliseconds) == 0)
                            {
                                userTimes[p.PIN] += (p.STAMP - iteraterange.StartTime).Duration();
                                UserTotals[p.PIN] += (p.STAMP - iteraterange.StartTime).Duration();

                            }
                        }

                        if (OrderStacks[p.OrderID].Count > 0)
                        {
                            var result = OrderStacks[p.OrderID].Pop();
                            if (OrderStacks[p.OrderID].Count == 0)
                            {
                                //last item popped, get timespan and add to OrderTimes.
                                TimeSpan currspan = (p.STAMP - result.Key).Duration();
                                orderTimes[p.OrderID] += currspan;
                                OrderTotals[p.OrderID] += currspan;

                            }


                        }
                        else
                        {
                            //if the stack has no items and the current accumulated time is 0,
                            //add in the diff between the start of this daterange and this entry.
                            //this wil laccount for time when an order is clocked in before a date range,
                            //and clocked out during.
                            if (((int)orderTimes[p.OrderID].TotalMilliseconds) == 0)
                            {
                                orderTimes[p.OrderID] += (p.STAMP - iteraterange.StartTime).Duration();
                                OrderTotals[p.OrderID] += (p.STAMP - iteraterange.StartTime).Duration();
                            }
                        }

                    }


                }

                //now we need to "drain" the stacks. (to account for orders and users that were timed in at the end of the 
                //interval)
                foreach (var iteratedict in UserStacks)
                {

                    while (iteratedict.Value.Count > 0)
                    {
                        var popsy = iteratedict.Value.Pop();
                        if (popsy.Value.EventType == "IN")
                        {
                            //if there are "IN" Events still in the stack, we want to add their times to the total.
                            userTimes[popsy.Value.PIN] += (popsy.Value.STAMP - iteraterange.StartTime).Duration();
                            UserTotals[popsy.Value.PIN] += (popsy.Value.STAMP - iteraterange.StartTime).Duration();


                        }



                    }



                }
                //'drain' Order stack...
                foreach (var iteratedict in OrderStacks)
                {

                    while (iteratedict.Value.Count > 0)
                    {
                        var popsy = iteratedict.Value.Pop();
                        if (popsy.Value.EventType == "IN")
                        {
                            //if there are 'IN' Events still on the stack, add the times to the totals.
                            orderTimes[popsy.Value.OrderID] += popsy.Value.STAMP - iteraterange.StartTime;
                            OrderTotals[popsy.Value.OrderID] += popsy.Value.STAMP - iteraterange.EndTime;


                        }


                    }


                }



                //Tech SUmmary:
                //Total Time will be sum of all times in userTimes.
                //each one will of course be the "detail"...

                //if no userTimes, show "<No Data>"...
                String stechsum = "";
                if (userTimes.Count == 0)
                {
                    stechsum = "<No Data>";
                }
                else
                {


                    List<String> TechSummary = new List<string>();
                    TimeSpan techsum = new TimeSpan();
                    foreach (var LoopTech in userTimes)
                    {
                        //key is userPIN; value is total timespan.
                        //get the name...
                        String username = Database.UserNameFromPIN(LoopTech.Key);
                        TechSummary.Add(username + " - " +
                                        String.Format("{0:00}:{1:00}", Math.Floor(LoopTech.Value.TotalHours), LoopTech.Value.Minutes));
                        techsum += LoopTech.Value;



                    }
                     stechsum = String.Format("{0:00}:{1:00}", Math.Floor(techsum.TotalHours), techsum.Minutes) + "(" +
                                      String.Join(",", TechSummary.ToArray()) + ")";
                }
                //Order Summary:
                //Total time will be sum of times in orderTimes
                //each entry will be the detail.
                List<string> OrderSummary = new List<string>();
                TimeSpan ordersum = new TimeSpan();
                foreach (var loopOrder in orderTimes)
                {
                    //key is OrderID, value is total timespan.
                    OrderSummary.Add("RO#" + loopOrder.Key + " - " + String.Format("{0:00}:{1:00}",
                        Math.Floor(loopOrder.Value.TotalHours), loopOrder.Value.Minutes));
                    ordersum += loopOrder.Value;

                }
                String sordersum = String.Format("{0:00}:{1:00}", Math.Floor(ordersum.TotalHours),ordersum.Minutes) + 
                    "(" + String.Join(",", OrderSummary.ToArray()) + ")";


                ListViewItem lvi = new ListViewItem(new String[] { stechsum, sordersum });
                lvi.Group = lvg;
                lvwReportResult.Items.Add(lvi);



            }






            ListViewGroup fullrangegroup = lvwReportResult.Groups.Add("FULLRANGE","Range Totals " + dates.ToString());
            //list ALL techs worked, and their total times. This information is stored in UserTotals...
            String techtotalsummary="", ordertotalsummary = "";
            List<String> utotals = new List<string>();
            List<String> ototals = new List<string>();
            TimeSpan usum = new TimeSpan();
            TimeSpan osum = new TimeSpan();
            foreach (var loopUser in UserTotals)
            {
                utotals.Add(Database.UserNameFromPIN(loopUser.Key) + " - " + String.Format("{0:00}:{1:00}", Math.Floor(loopUser.Value.TotalHours), loopUser.Value.Minutes));
                usum += loopUser.Value;


            }
            techtotalsummary = String.Format("{0:00}:{1:00}", Math.Floor(usum.TotalHours), usum.Minutes) + "(" + String.Join(",", utotals.ToArray()) + ")";

            foreach (var loopOrder in OrderTotals)
            {

                ototals.Add(loopOrder.Key + " - " + String.Format("{0:00}:{1:00}", Math.Floor(loopOrder.Value.TotalHours), loopOrder.Value.Minutes));
                osum += loopOrder.Value;

            }

            ordertotalsummary = String.Format("{0:00}:{1:00}", Math.Floor(osum.TotalHours), osum.Minutes) + "(" + String.Join(",", ototals.ToArray()) + ")";



            ListViewItem totalitem = new ListViewItem(new string[] { techtotalsummary, ordertotalsummary });
            lvwReportResult.Items.Add(totalitem);
            totalitem.Group = fullrangegroup;



        }


        /// <param name="ClearCurrent">whether to clear the current listing</param>
        /// <param name="MultiOrder">whether to add a "OrderID" Column to distinguish multiple users. Used only when the report contains multiple Orders.</param>
        private void ReportByOrder(String OrderID, DateRange reportrange, bool ClearCurrent, bool MultiOrder)
        {

                //nothing...
                /*
                * 
                * 2. Report for Date Range by order such as RO#000000 was worked on by 
                 * user1 with time 0:32 and user2 with time 00:14 for total time 00:46 and show  for all orders
                * in a specific date range.
                * 
                * */
            //step one: get all Order data for this Order.

            var orderuserdata = Database.GetUserDataFromOrder(OrderID, false, reportrange.StartTime,reportrange.EndTime);

            //we need to "merge" all the order data's for each user into a single order.

            #region Merge User Orders into single item.
            Dictionary<String, List<DataLayer.BasicUserOrderData>> convertuserdata = new Dictionary<String, List<DataLayer.BasicUserOrderData>>();


            foreach (var iterateorder in orderuserdata)
            {
                List<DataLayer.BasicUserOrderData> addtolist;
                if (convertuserdata.ContainsKey(iterateorder.Username))
                    addtolist = convertuserdata[iterateorder.Username];
                else
                {
                    addtolist = new List<DataLayer.BasicUserOrderData>();
                    convertuserdata.Add(iterateorder.Username, addtolist);
                }
                addtolist.Add(iterateorder);

            }
            //clear orderuserdata...
            orderuserdata = new List<DataLayer.BasicUserOrderData>();

            //iterate through dictionary...
            foreach (var iteratedict in convertuserdata)
            {

                //create a new BasicUserOrderData that merges all the elements in the list (by adding their times together.
                TimeSpan timesum = new TimeSpan();
                DataLayer.BasicUserOrderData buod = iteratedict.Value.First();
                foreach (var iteratelist in iteratedict.Value)
                {


                    timesum += iteratelist.TotalTime;


                }
                DataLayer.BasicUserOrderData createnew = new DataLayer.BasicUserOrderData(buod.OrderID, buod.Username, timesum, buod.isClocked);
                //add to the list.
                orderuserdata.Add(createnew);





            }
            #endregion



            //now, if we are supposed to clear it...
            if (ClearCurrent)
            {
                //clear it...
                lvwReportResult.Items.Clear();
                lvwReportResult.Columns.Clear();
                lvwReportResult.Groups.Clear();
                if (MultiOrder) fraReportResult.Text = "Users Report - All Orders";
                //first column is RO, but only if MultiOrder is true.
              //  if (MultiOrder)
              //      lvwReportResult.Columns.Add("RO", "RO#");
                

                //otherwise, we have Total time, and a breakdown of how the sum was reached.. Well actually we have those anyway, too.
                lvwReportResult.Columns.Add("TOTAL", "Time");
                lvwReportResult.Columns.Add("BREAKDOWN", "Techs");


                //resize equally.
                lvwReportResult.SizeColumnsEqual();
                //change first col to be shorter...
                //lvwReportResult.Columns["RO"].Width = 64;

            }
            String headertext = "Report for RO#" + OrderID + "; Range:" +
                                   reportrange.StartTime.ToShortDateString() + "-" +
                                   reportrange.EndTime.ToShortDateString() + Environment.NewLine;


            //add a group for this order. (if multiuser).
            ListViewGroup usegroup=null;
            if (MultiOrder)
            {
                try
                {
                    usegroup = lvwReportResult.Groups["RO" + OrderID];
                    if (usegroup == null) throw new NullReferenceException();


                }
                catch
                {
                    usegroup = new ListViewGroup("RO" + OrderID, "RO#" + OrderID);
                    usegroup.Header= "RO#" + OrderID;
                    usegroup.HeaderAlignment=HorizontalAlignment.Left;
                    lvwReportResult.Groups.Add(usegroup);
                }


            }
            String Tfmt = "{0:00}:{1:00}";
            TimeSpan AccumulatedTime=new TimeSpan();
            String[] buildbreakdown = new String[orderuserdata.Count];
            int i = 0;
            TimeSpan TotalTimeSum=new TimeSpan();
            foreach (DataLayer.BasicUserOrderData bud in orderuserdata)
            {
                //used for displaying the time.
                //buildbreakdown[i] = bud.Username + " For " + String.Format(Tfmt, bud.TotalTime.TotalHours, bud.TotalTime.Minutes);
                DateRange[] userworkrange = Database.GetUserWorkOrderRanges(Database.GetUserPIN(bud.Username), OrderID);

                TimeSpan[] tspans = (from m in userworkrange select m.Span).ToArray();

                TimeSpan sum= new TimeSpan();
                String usebreakdown = "";
                
               // String[] breakup = new String[tspans.Length];
                for(int o=0;o<tspans.Length;o++)
                {
                    TimeSpan iteratespan = tspans[o];
                    sum+=iteratespan;
                   // breakup[o]= bud.Username + " with time " +  String.Format(Tfmt, iteratespan.TotalHours, iteratespan.Minutes);
                    
                }

                //usebreakdown = String.Join(",", breakup);
                
                String useTotalTime = String.Format(Tfmt, Math.Floor(sum.TotalHours), sum.Minutes);
                TotalTimeSum+=sum;
                usebreakdown=bud.Username + " - " + useTotalTime; 
                ListViewItem additem;
                if (MultiOrder)
                {
                    Debug.Print("Item added");
                    //RO|TotalTime|Breakdown
                    //additem = new ListViewItem(new String[] { OrderID, useTotalTime, usebreakdown });
                    additem = new ListViewItem(new String[] { useTotalTime, usebreakdown });
                    //usegroup.Items.Add(additem);
                    additem.Group = usegroup;
                    //usegroup.Items.Add(additem);

                }
                else
                {
                    //TotalTime|Breakdown
                    Debug.Print("Item added");
                    additem = new ListViewItem(new String[] { useTotalTime, usebreakdown });
                    

                }
                lvwReportResult.ShowGroups=true;
                lvwReportResult.Items.Add(additem);

                AccumulatedTime+=bud.TotalTime;
                i++;
                
               
            }
        

               











        }

        private void ReportByUser(String UserPIN, DateRange reportrange)
        {

           ReportByUser(UserPIN, reportrange, true,false);


        }

        private String NaturalLanguageCopy = ""; //created in the report entries using "natural language" to try to describe the values that are shown in a listview.
        /// <summary>
        /// Subroutine populates the Report listview with a report for the given user in the given range.
        /// </summary>
        /// <param name="UserPIN"></param>
        /// <param name="reportrange"></param>
        /// <param name="ClearCurrent">whether to clear the current listing</param>
        /// <param name="MultiUser">whether to add a "User" Column to distinguish multiple users. Used only when the report contains multiple users.</param>
        private void ReportByUser(String UserPIN, DateRange reportrange,bool ClearCurrent,bool MultiUser)
        {
             //I need to be able to look at it by date,tech (user), and RO. The reports that I would want to see would be:
             //1.Report for date range by user
             //ex select User1 for 12/01/11 - 12/10/11 and it would display the orders he worked on and the elapsed time for each.
            //columns: (For a single user, we don't show the Username column)

            //simply display the orders, and the time worked on each by that user.
            String username = Database.UserNameFromPIN(UserPIN);
            List<String> usersorders = Database.GetUserOrders(Database.UserNameFromPIN(UserPIN));
            List<String> transformed = new List<string>();

            //go through the list, and strip out any orders that don't start in the given date range.
            foreach (String loopuserorder in usersorders)
            {

                //get the Date information for this order.
                DateTime? dateinfo = Database.GetWorkOrderDate(loopuserorder);
                if (dateinfo != null)
                {
                    //does it fall within the range?
                    if (reportrange.Contains(dateinfo.Value))
                    {
                        //yep; add it to the transformed List.
                        transformed.Add(loopuserorder);

                    }



                }

            }

            //"transformed" now contains the list of all Orders worked on by this user that <STARTED> in the given Range of Dates.

            //iterate through each one and add an item to the  ListView... but first, clear existing items and columns and add them afresh.
            //if "ClearCurrent" is specified, clear the current items and columns and reset the "NaturalLanguageCopy" variable.
            if (ClearCurrent)
            {
                lvwReportResult.Items.Clear();
                lvwReportResult.Columns.Clear();
                lvwReportResult.Groups.Clear();
                NaturalLanguageCopy = "";
                if (MultiUser) fraReportResult.Text = "Users Report - All Users";

                //add Columns. OrderID, Elapsed Time
                //if (MultiUser) lvwReportResult.Columns.Add("USER", "User");
                lvwReportResult.Columns.Add("ORDERID", "RO");
                lvwReportResult.Columns.Add("ELAPSED", "Elapsed Times");
                

                //size the columns equally (extension method, ExtendDbReader.cs)
                lvwReportResult.SizeColumnsEqual();
            }


            
            String headertext = "Report for Tech \"" + username + "\"; Range:" +
                                    reportrange.StartTime.ToShortDateString() + "-" +
                                    reportrange.EndTime.ToShortDateString() + Environment.NewLine;
            NaturalLanguageCopy += Environment.NewLine + "^&^" + headertext;
            if(!MultiUser) fraReportResult.Text = headertext;
            //now, iterate through transformed...

            foreach(var IterateOrder in transformed)
            {




                String useOrderID = "RO#" + IterateOrder;

                //grab the data for the user on this specific order.
                DateRange[] orderranges;
                TimeSpan Totaltime = Database.GetUserTotalTimeOnOrder(UserPIN, IterateOrder, out orderranges);


                //we have the list of date ranges. Create a hour:minute for each, and build an array out of it.
                String[] DaterangeStrings =
                    (from m in orderranges select String.Format("{0:00}:{1:00}", Math.Floor(m.Span.TotalHours), m.Span.Minutes)).
                        ToArray();

                String joinedrange = String.Join(",", DaterangeStrings);
                String columnstring = String.Format("{0:00}:{1:00}({2})", Math.Floor(Totaltime.TotalHours), Totaltime.Minutes,
                                                    joinedrange);
                //build the "pretty" string for the "natural" format.
                NaturalLanguageCopy += "RO#" + IterateOrder + " Total Time " +
                                       String.Format("{0:00}:{1:00}", Math.Floor(Totaltime.TotalHours), Totaltime.Minutes) +
                                       " Worked on in spans of " + joinedrange + Environment.NewLine;


                //if we are in "MultiUser" mode, also see if there is a group for this user, and if not, add one.
                ListView.ListViewItemCollection lvwic = null;
                ListViewGroup usegroup = null;

                try
                {
                    usegroup = lvwReportResult.Groups[username];
                    lvwic = usegroup.Items;
                }
                catch
                {
                    usegroup = lvwReportResult.Groups.Add(username, username);
                    lvwic = usegroup.Items;
                    usegroup.Header = DataLayer.ProperCase(username);
                    usegroup.HeaderAlignment = HorizontalAlignment.Left;


                }





                //create this ListViewItem, and add it to the report List.
                ListViewItem createitem = null;
                //if Multiple users, pass in the username as well.
                if (MultiUser)
                {
                createitem = new ListViewItem(new string[] {useOrderID, columnstring});
                createitem.Group = usegroup;
            }
        else
                    createitem = new ListViewItem(new String[] { useOrderID, columnstring });

                lvwReportResult.Items.Add(createitem);





            }
















        }

        private void frmJobClockAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Database!=null)
                Database.Configuration.Save();
        }

        private void chkHidePIN_CheckedChanged(object sender, EventArgs e)
        {
            Database.Configuration.Client_PasswordPIN = chkHidePIN.Checked;
        }

        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Validate();
        }

        private void txtPINCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPINCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Validate();
        }

        private void txtOrderID_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Validate();
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Validate();
        }

        private void frmJobClockAdmin_Activated(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            //UpdateUserListView();
        }

        private void chkActive_Validated(object sender, EventArgs e)
        {




            UpdateUserListView();
        }

        private void chkuserlisting_CheckedChanged(object sender, EventArgs e)
        {
            Database.Configuration.Client_ShowUserList=chkuserlisting.Checked;
        }

        private void txtSingleWorkOrderMode_Validated(object sender, EventArgs e)
        {
            Database.Configuration.SingleWorkOrderMode=txtSingleWorkOrderMode.Text;
        }
        private void RefreshUpdateView()
        {
            String InstalledVer = GetCurrentVersion();
            String LatestVer = "";
            try
            {
                if (UpdateObject == null) UpdateObject = new BCUpdate();
                InstalledVer = UpdateObject.getinstalledVersion(UpdateID);
                LatestVer = UpdateObject.getUpdateVersion(UpdateID);
                cmdDownloadUpdate.Enabled = true;
            }
            catch (Exception eex)
            {
                LatestVer = "Latest Version:unable to determine latest version-\n" + eex.Message + ">";
                
                cmdDownloadUpdate.Enabled = false;

            }

            if (LatestVer == "")
                LatestVer = "<Unable to determine latest version>";


            lblInstalledVersion.Text = "Installed Version:" + InstalledVer;
            lblLatestVersion.Text = "Latest Version:" + LatestVer;





        }

        private void tabAdminpanel_TabIndexChanged(object sender, EventArgs e)
        {

            



        }

        private void tabAdminpanel_Selected(object sender, TabControlEventArgs e)
        {
            if (Database.Configuration.RefreshTabOnClick)
            {
                RefreshGeneral();
                if (e.TabPage == tabUpdate) RefreshUpdateView();
                if (e.TabPage == TabPageUsers) RefreshUserList();
                if (e.TabPage == TabPageOrders) RefreshOrderList();
            }
        }

        private void frmJobClockAdmin_Shown(object sender, EventArgs e)
        {
            if (doExit)
                Close();
        }

        private void cmdDownloadUpdate_Click(object sender, EventArgs e)
        {
            //download it...


            String strid = UpdateObject.CheckUpdate(UpdateID);


            BCUpdate.UpdateInfo objupdate =
                              (from n in UpdateObject.LoadedUpdates where n.dlID == UpdateID select n).
                                  First
                                  ();
            //I assume we are supposed to now install said update. 
            
            objupdate.DownloadUpdate(null, UpdateDownloadComplete);
            

        }

        
    }
}
