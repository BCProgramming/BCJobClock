using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Threading;
using BCJobClockLib;
using Image=System.Drawing.Image;

namespace BCJobClock
{


 


    
    /// <summary>
    /// Interaction logic for wndJobClock.xaml
    /// </summary>
    public partial class wndJobClock : Window
    {




        public ObservableCollection<MessageActivityLogItem> MessageLog = new ObservableCollection<MessageActivityLogItem>();
        public enum OperatingModeConstants
        {
            OpMode_PinEntry, //Screen for entering pin
            OpMode_ScanOrder, //Screen for scanning orders.
            OpMode_MessageAlert, //Screen for "Alerts"...
            OpMode_Admin  //the admin panel.


        }
        private OperatingModeConstants _OperatingMode = OperatingModeConstants.OpMode_PinEntry;
        
        private void SetGridEnable(Grid disableit,bool setvalue)
        {

            foreach (UIElement velement in disableit.Children)
            {
                velement.IsEnabled=setvalue;
                


            }



        }


        


        public OperatingModeConstants OperatingMode { get { return _OperatingMode; } 
            
            
            set 
            { 
                if(value==_OperatingMode) return; //do nothing if the mode isn't changing.

                //otherwise, first determine our current mode, and what needs to be changed to leave it.
                switch (_OperatingMode)
                {
                    case OperatingModeConstants.OpMode_PinEntry:
                        PinEntry.IsEnabled=false;
                        break;
                    case OperatingModeConstants.OpMode_ScanOrder:
                        //we need to hide the Scan Order box (WorkOrder), as well as clear it's text.
                        //but we only do this if we aren't switching to the alert window.
                        if (value != OperatingModeConstants.OpMode_MessageAlert)
                            WorkOrder.Visibility = System.Windows.Visibility.Collapsed;
                        else
                        {
                            //if we are switching to the alert mode, move the workorder "dialog"
                            //behind...
                            

                        }



                        break;
                    case OperatingModeConstants.OpMode_MessageAlert:
                        AlertWindow.Visibility = System.Windows.Visibility.Collapsed;
                        ContentPresenter cp = AlertWindow.FindVisualParent<ContentPresenter>();
                        Canvas.SetZIndex(cp, 99);
                        
                        break;
                    case OperatingModeConstants.OpMode_Admin:
                        //nothing yet, as I haven't written the Admin mode panel.. thing, yet.
                        break;

                }

                //now, what do we have to do to switch TO the value being set?
                switch (value)
                {
                    case OperatingModeConstants.OpMode_PinEntry:
                        PinEntry.Visibility = Visibility.Visible;
                        PinEntry.IsEnabled = true;
                        PinEntry.Opacity = 1.0f;
                        break;
                    case OperatingModeConstants.OpMode_ScanOrder:
                        PinEntry.Opacity = 0.4f;
                        lblPIN.Content = "";
                        lblPINpass.Content = PassIt(BuildPin);
                        BuildPin = "";
                        
                        WorkOrder.Visibility = System.Windows.Visibility.Visible;
                        WorkOrderEntry.Focus();
                        break;
                    case OperatingModeConstants.OpMode_MessageAlert:
                        PinEntry.Opacity = 0.4f;
                        AlertWindow.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case OperatingModeConstants.OpMode_Admin:
                        break;
                }
                //and finally, set the value.
                _OperatingMode=value; 


            } 
        }

            public wndJobClock()
        {
            InitializeComponent();
                
        }
        private DataLayer dbase=new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Client);
        private String BuildPin = "";
        private OperatingModeConstants BeforeAlertMode;

        private DispatcherTimer refreshUserListTimer=null;


        private void ShowAlertWindow(String AlertContent)
        {
            //sets message text and switches to "Alert" mode.
            AlertLabel.Text = AlertContent;
            BeforeAlertMode=OperatingMode;
            OperatingMode = OperatingModeConstants.OpMode_MessageAlert;



        }
        public void EmitClickSound(object sender, MouseEventArgs e)
        {
      

        }
        public void WorkOrderButtonEntryClick(object sender, RoutedEventArgs e)
        {
            ClickSound();
            //entry logic, similar to EntryButtonClick but we want to change lblScanPrompt.Text.

            WorkOrderEntry.Focus(); //set focus
            String gotcontent = (String)((Button)sender).Content;

            int intresult =0;
            if(int.TryParse(gotcontent,out intresult))
            {
                //a number was entered.
                if (WorkOrderEntry.Text.Length >= dbase.Configuration.WorkOrderMaxLength && dbase.Configuration.WorkOrderMaxLength>0)
                {
                    playthesound("errorsound.wav");

                }
                else
                {
                    WorkOrderEntry.Text = WorkOrderEntry.Text + gotcontent;
                }


            }
            else if (gotcontent.Equals("c",StringComparison.OrdinalIgnoreCase))
            {
                WorkOrderEntry.Text = "";
                

            }
            else if (gotcontent.Equals("ü", StringComparison.OrdinalIgnoreCase))
            {
                //Enter button logic.
                WorkOrderEntry_Enter();



            }



        }

        public void EntryButtonClick(object sender, RoutedEventArgs e)
        {
            ClickSound();
            String gotcontent = (String)((Button)sender).Content;
            Debug.Print("Button Clicked: " + gotcontent);
            int intresult=0;
            if (int.TryParse(gotcontent,out intresult))
            {

                if (BuildPin.Length >= dbase.Configuration.PINCodeMaxLength)
                {
                    playthesound("errorsound.wav");
                }
                else
                {


                    BuildPin += gotcontent;
                }

            }
            else if (gotcontent.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                BuildPin = "";

            }

            else if (gotcontent.Equals("ü", StringComparison.OrdinalIgnoreCase))
            {
                //ENTER key...
                //proceed to next step, according to the spec...
                //This is to accept input from a Bar Code scanner, for a Work Order.
                //First we will need to store the PIN for when the ScanOrder completes. Or, we could just use the same one, I suppose.
                try {

                    if (BuildPin.Length < dbase.Configuration.WorkOrderMinLength)
                    {
                        ShowAlertWindow("PIN must be at least " + dbase.Configuration.WorkOrderMinLength.ToString() + " Characters.");
                        lblPIN.Content = "";
                        lblPINpass.Content = "";
                        BuildPin = "";
                    }
                    else
                    {
                        //make sure the PIN Exists...
                        String acquireduser = "";

                        bool pinvalid = ((acquireduser = dbase.LookupPIN(BuildPin)) != "");
                        String AlertMessage = "Invalid PIN.";
                        if (pinvalid) //no need to check if it's active if we already know it's invalid.
                        {
                            if (!dbase.IsUserActive(BuildPin))
                            {
                                pinvalid = false; //user must be active as well.
                                AlertMessage = "Given PIN Not Active.";
                            }
                        }
                        if (pinvalid)
                        {
                            mScanUserPin = BuildPin;
                            mScanUserName = acquireduser;
                            lblScanPrompt.Text = acquireduser + ", Scan Work Order...";

                            //determine whether to show "clockedin" expander or not.
                            List<String> usersorders = dbase.GetUserOrders(acquireduser, true);

                            if (usersorders.Count > 0)
                            {
                                Clockedin.Visibility = System.Windows.Visibility.Visible;
                                //populate an Observable Collection with the RO data...
                                ObservableCollection<ClockedOrderListItem> setorders =
                                    new ObservableCollection<ClockedOrderListItem>();
                                foreach (String looporderid in usersorders)
                                {
                                    setorders.Add(new ClockedOrderListItem(looporderid,
                                                                           dbase.GetOrderDescription(looporderid)));



                                }

                                //and plonk it to the list...
                                lstExistingOrders.ItemsSource = setorders;



                            }
                            else
                            {
                                Clockedin.Visibility = System.Windows.Visibility.Hidden;
                            }


                        }
                        else
                        {
                            ShowAlertWindow(AlertMessage);
                            lblPIN.Content = "";
                            lblPINpass.Content = "";
                            BuildPin = "";
                            return;
                        }
                        //set the variables to be used when the scan completes...

                        //do we need to do a scan at all?
                        if (!String.IsNullOrEmpty(dbase.Configuration.SingleWorkOrderMode))
                        {
                            ProcessOrderEntry(mScanUserPin, dbase.Configuration.SingleWorkOrderMode);

                        }
                        else
                        {
                            WorkOrderEntry.Text = "";
                            OperatingMode = OperatingModeConstants.OpMode_ScanOrder;
                        }

                    }
                  

                }
                catch (Exception exr)
                {
                    CurrentDomain_UnhandledException(this, new UnhandledExceptionEventArgs(exr, false));


                }
            }
            lblPIN.Content = BuildPin;
            lblPINpass.Content = PassIt(BuildPin);
        }
        private String mScanUserPin;
        private String mScanUserName;
        private String PassIt(String input)
        {

            StringBuilder build = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                build.Append("*");

            }
            return build.ToString();

        }

        private void CancelScanEntry_Click(object sender, RoutedEventArgs e)
        {
            //Cancel the work order scan by setting it back to Pinpad mode.
            OperatingMode = OperatingModeConstants.OpMode_PinEntry;
        }
        /// <summary>
        /// used to populate the label near the bottom showing the version number of the JobClock and the Library.
        /// </summary>
        /// <returns></returns>
        private String GetVersionData()
        {
            Assembly JobClock = Assembly.GetCallingAssembly();
            Assembly JobClockLib = Assembly.GetAssembly(typeof(DataLayer));

           // String buildverstring = JobClock.GetName().Name + "(" + JobClock.GetName().Version.ToString() + "),";
           // buildverstring += JobClockLib.GetName().Name + "(" + JobClock.GetName().Version.ToString() + ")";





            return String.Join(",",new string[] { GetVersionData(JobClock), this.GetVersionData(JobClockLib) });



        }
        public String GetVersionData(Assembly forAssembly)
        {
            String BuildVerData = forAssembly.GetName().Name + " Ver. ";
            BuildVerData += forAssembly.GetName().Version.ToString();
            DateTime AssBuildDate = GetAssemblyDate(forAssembly);
            BuildVerData += String.Format("({0}.{1}.{2})", AssBuildDate.Year, AssBuildDate.Month, AssBuildDate.Day);

            return BuildVerData;
        }

        private DateTime GetAssemblyDate(Assembly forAssembly)
        {

            return File.GetLastWriteTime(forAssembly.Location);
        }
        private class ClockedOrderListItem
        {
            public String RO { get; set; }
            public String Description { get; set; }

            public ClockedOrderListItem(String pRO,String pDescription)
            {
                RO=pRO;
                Description = pDescription;


            }
            public override string ToString()
            {
                return "RO# " + RO + " - " + Description;
            }

        }
        private class UserNameListItem
        {
            public String Name { get; set; }
            public String PINCode{get;set;}
            public long ActiveJobCount { get; set; }
            public String ActiveJobCountString { get { return ActiveJobCount == 0 ? "" : ActiveJobCount.ToString(); } }

            /*
            public System.Drawing.Image properUserImage
            {
                get {
                    Image returnimage = null;
                    if (ActiveJobCount > 0) returnimage= JobClockConfig.Imageman.GetLoadedImage("user-offline");
                    returnimage=JobClockConfig.Imageman.GetLoadedImage("user-online");
                    Debug.Assert(returnimage != null);
                    return returnimage;
                
                }


            }*/
                private Bitmap hbmpuser_offline = null;
                private Bitmap hbmpuser_online = null;
                private ImageSource user_online = null;
                private ImageSource user_offline = null;

            public ImageSource properUserImage {

                get {
                    Image returnimage = null;
                    if (ActiveJobCount > 0)
                    {
                        if (user_offline == null)
                        {
                            returnimage = JobClockConfig.Imageman.GetLoadedImage("user-offline");
                            hbmpuser_offline = new Bitmap(returnimage);
                            user_offline = Imaging.CreateBitmapSourceFromHBitmap(hbmpuser_offline.GetHbitmap(),
                                                                                 IntPtr.Zero,
                                                                                 Int32Rect.Empty,
                                                                                 BitmapSizeOptions.FromEmptyOptions());
                        }
                        return user_offline;

                    }
                    else
                    {

                        if (user_online == null)
                        {
                            returnimage = JobClockConfig.Imageman.GetLoadedImage("user-online");
                            hbmpuser_online = new Bitmap(returnimage);
                            user_online = Imaging.CreateBitmapSourceFromHBitmap(hbmpuser_online.GetHbitmap(), IntPtr.Zero, 
                                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());



                        }

                        return user_online;
                    }
                    
                    
                
                
                }

            }

            public UserNameListItem(String pName, String pPinCode, long pActiveJobCount)
            {

                Name=pName;
                PINCode=pPinCode;
                ActiveJobCount = pActiveJobCount;
            }


        }
        private ObservableCollection<UserNameListItem> usersbinding = new ObservableCollection<UserNameListItem>();

        private void UpdateUserListing()
        {
            //updates the listing on lstuserNames.

            //clear all the current contents. First, disable redraw... (edit: that isn't necessary, thx to WPF)

            
            //lstUserNames.Items.Clear();

            ObservableCollection<UserNameListItem> createcol = new ObservableCollection<UserNameListItem>();

            //now, acquire a list of all the users. Balls to the data layer, we'll access it directly ourselves...
            var gotusers = dbase.GetUsers();
            foreach (var loopuser in gotusers)
            {
                String currpin = loopuser.Key;
                String currname = loopuser.Value;
                long activecount = dbase.GetActiveJobCountForUser(currpin);
                if(dbase.IsUserActive(currpin))
                    createcol.Add(new UserNameListItem(currname, currpin, activecount));



            }
            usersbinding=createcol;
            lstUserNames.ItemsSource=usersbinding;


            




        }
        static String GetExceptionData(Exception except)
        {
            String returnit = "Message:" + except.Message;
            returnit += "\nSource:" + except.Source ;
            returnit += "\nStack Trace:" + except.StackTrace;
            if (except.InnerException != null)
                returnit += "\n Inner Exception:\n{" + GetExceptionData(except.InnerException) + "\n}";

            returnit += "(ToString of Exception Object:\n" + except.ToString();
            return returnit;

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //open the log file. we have no idea what the program state is now, so we will put it in the log directory. We have to "re-craft" the path, though.
            String TargetLogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TargetLogPath = System.IO.Path.Combine(TargetLogPath, "BCJobClock\\BCJobClock_Entry.log");

            //open this file, append to it...
            FileStream logout = new FileStream(TargetLogPath, FileMode.Create);
            StreamWriter fw = new StreamWriter(logout);

            fw.WriteLine();

            fw.WriteLine("An Unhandled Exception occured.");
            Exception getobj = e.ExceptionObject as Exception;
            fw.WriteLine(GetExceptionData(getobj));
            fw.Flush();
            fw.Close();
            MessageBox.Show("An unexpected exception occured in BCJobClock. Information on this error has been logged to " + TargetLogPath + " .");







        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
               try
            {
                //demand WebPermissions.
                WebPermission wp = new WebPermission();
                DnsPermission dperm = new DnsPermission(PermissionState.Unrestricted);
                dperm.Demand();
                wp.Demand();
                //dbase.DbConnection_Progress();
               DbConnection getcon = dbase.GetConnection();
               



            }
            catch(Exception exx)
            {
               
                //log the error.
                DataLayer.LogClient("Exception:" + exx.Message + " Stack Trace:" + exx.StackTrace);
                //show a message.
                dbase.WriteDebugMessage(DataLayer.GetExceptionData(exx));
                MessageBox.Show(this,"Error Connecting to the Database:" + exx.Message);
                Close();
                

            }
            OperatingMode = OperatingModeConstants.OpMode_PinEntry;
            //dbase = new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Client);
            lstLogEntries.ItemsSource = MessageLog;
            //connect to the DB right NAU!
            WorkOrderEntry.MaxLength = dbase.Configuration.WorkOrderMaxLength;

            //make various items collapsed if the config calls for it...
            lstLogEntries.Visibility = !dbase.Configuration.Client_ShowActivityLog ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            
            UserListExpander.Visibility = !dbase.Configuration.Client_ShowUserList ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            lblLogEntry.Visibility = lstLogEntries.Visibility;

            if (dbase.Configuration.Client_PasswordPIN)
            {
                lblPIN.Visibility=System.Windows.Visibility.Collapsed;
                lblPINpass.Visibility = System.Windows.Visibility.Visible;


            }
            //now, bind lstLogEntries to the MESSAGELOG database...

            ObservableCollection<MessageActivityLogItem> GeneralLog = dbase.GetLogMessages("GENERAL");
            lstLogEntries.ItemsSource=GeneralLog;


            //lstLogEntries 
            
       



            VersionLabel.Content = GetVersionData();

            
            //set the timer that will periodically (every 20 seconds or so) refresh the listbox.
            //the IDEAL case would be to use some sort of binding but...meh. 

            refreshUserListTimer = new DispatcherTimer();
            refreshUserListTimer.Interval=new TimeSpan(0,0,0,20);
            refreshUserListTimer.Tick += new EventHandler(refreshUserListTimer_Tick);
            refreshUserListTimer.Start();
            refreshUserListTimer_Tick(refreshUserListTimer, null);
            //now that there is the admin panel, the seeding code is unneeded :D
            try
            {
                //dbase.SeedDB();
            }
            catch (Exception seedexception)
            {
                Debug.Print("Exception while seeding...");

            }
        }

        void refreshUserListTimer_Tick(object sender, EventArgs e)
        {
            UpdateUserListing();
            updateLog();
        }
        void updateLog()
        {
            ObservableCollection<MessageActivityLogItem> GeneralLog = dbase.GetLogMessages("GENERAL");
            lstLogEntries.ItemsSource=GeneralLog; 
        }

        private void WorkOrderEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
        }
        private bool ProcessOrderEntry(String forUserPIN, String wo)
        {

            //Processing a Work Order entry.

            //for the user with foruserPIN, and workorder wo.

            try
            {
                //toggle their clocking... 
                Debug.Print(String.Format("Process Order Entry, UserPin={0}, WorkOrder={1}", forUserPIN, wo));

                bool returnval = dbase.ToggleUserClockin(forUserPIN, wo);
                //returns whether they are now clocked in or out.
                String uname = dbase.UserNameFromPIN(forUserPIN);
                long activejob = dbase.GetActiveJobCountForUser(forUserPIN);

                DateTime? lastclockout = dbase.GetLastClockOutForUserOrder(forUserPIN, wo);
                String displaymessage = "";
                String Messagefmt;
                if (returnval)
                {
                    Messagefmt = "{0} clocked into Work Order {1} (total {2} Active Work orders)";

                }
                else
                {
                    Messagefmt = "{0} clocked out Work Order {1}, Length {3:00}:{4:00} (total {2} Active Work orders)";
                }
                if (lastclockout != null)
                {
                    TimeSpan diff = dbase.DBTime() - lastclockout.Value;
                    displaymessage = String.Format(Messagefmt, uname, wo, activejob, diff.TotalHours, diff.Minutes);
                }
                else
                {
                    displaymessage = String.Format(Messagefmt, uname, wo, activejob);
                }

                dbase.LogMessage("GENERAL", "STANDARD", displaymessage);

                UpdateUserListing();
                updateLog();
                return returnval;

            }
            catch (Exception exr)
            {

                CurrentDomain_UnhandledException(this, new UnhandledExceptionEventArgs(exr, false));
                return false;
            }

        }

        private void WorkOrderEntry_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled=true;
                WorkOrderEntry_Enter();
                return;
            }
        }

        private void WorkOrderEntry_Enter()
        {
            String AcquiredCode = WorkOrderEntry.Text;


            if (AcquiredCode.Length < dbase.Configuration.WorkOrderMinLength)
            {
                ShowAlertWindow("WorkOrder Entry must be at least " + dbase.Configuration.WorkOrderMinLength + " Characters.");

            }
            else if (AcquiredCode.Length > dbase.Configuration.WorkOrderMaxLength)
            {
                //how the heck....
                ShowAlertWindow("WorkOrder Entry can be no longer than " + dbase.Configuration.WorkOrderMinLength + " Characters.");
            }
            else if (!dbase.Configuration.AutoAddOrders)
            {
                //determine if the given order already exists.
                List<String> allorders = dbase.GetAllOrderIDs();
                if(allorders.Count==0)
                {
                    ShowAlertWindow(@"Error: Initialization setting 'AutoAddOrders' disabled, but no orders are present in the database.
Either toggle the INI setting, or add some orders using the administration applet.");
                    return;



                }

                if (dbase.GetAllOrderIDs().Contains(AcquiredCode))
                {
                    //it does exist, so allow it.
                    Debug.Print("Acquired code was:" + AcquiredCode);



                    ProcessOrderEntry(mScanUserPin, AcquiredCode);
                    //change mode back to normal
                    OperatingMode = OperatingModeConstants.OpMode_PinEntry;
                    
                }
                else
                {
                    ShowAlertWindow("Order ID \"" + AcquiredCode + "\" Not found.");
                    return;
                }


            }
            else
            {


                String comparecode = AcquiredCode;
                Debug.Print("Acquired code was:" + AcquiredCode);

                ProcessOrderEntry(mScanUserPin, AcquiredCode);




                //change mode back to normal, for the time being.
                OperatingMode = OperatingModeConstants.OpMode_PinEntry;
            }
        }

        private void AlertOK_Click(object sender, RoutedEventArgs e)
        {
            OperatingMode = BeforeAlertMode; //go  back to previous mode, which depends on what we were in before.

        }
        private void ClickSound()
        {
            playthesound("click.wav");

        }
        private void playthesound(String soundplay)
        {

            String soundfilepath = System.IO.Path.Combine(JobClockConfig.GetAppDataFolder(),"Sound\\" + soundplay);
            
            SoundPlayer player = new SoundPlayer(soundfilepath);
            player.Load();
            player.Play();

        }

        private void EventSetter_Click(object sender, RoutedEventArgs e)
        {
            ClickSound();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

     
        private void WorkOrderEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //double-clicked on the User list.
            var gotsender=            (Label)sender;
            UserNameListItem un = (UserNameListItem)gotsender.Tag;
            /*
            BuildPin = un.PINCode;
            lblPIN.Content=BuildPin;
            lblPINpass.Content = PassIt(BuildPin);
            EmitClickSound(sender, e);
            */



        }

        private void lstExistingOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //set the 
            var grabitem = (ClockedOrderListItem)lstExistingOrders.SelectedItem;

            WorkOrderEntry.Text = grabitem.RO;


        }

        private void lstExistingOrders_Selected(object sender, RoutedEventArgs e)
        {
            var grabitem = (ClockedOrderListItem)lstExistingOrders.SelectedItem;

            WorkOrderEntry.Text = grabitem.RO;
        }

        private void jmtclick_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Quit!
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Char? Keypressed=null;
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.D0:
                    Keypressed = '0';
                    break;
                case Key.NumPad1:
                case Key.D1:
                    Keypressed = '1';
                    break;
                case Key.NumPad2:
                case Key.D2:
                    Keypressed = '2';
                    break;
                case Key.NumPad3:
                case Key.D3:
                    Keypressed = '3';
                    break;
                case Key.NumPad4:
                case Key.D4:
                    Keypressed = '4';
                    break;
                case Key.NumPad5:
                case Key.D5:
                    Keypressed = '5';
                    break;
                case Key.NumPad6:
                case Key.D6:
                    Keypressed = '6';
                    break;
                case Key.NumPad7:
                case Key.D7:
                    Keypressed = '7';
                    break;
                case Key.NumPad8:
                case Key.D8:
                    Keypressed = '8';
                    break;
                case Key.NumPad9:
                case Key.D9:
                    Keypressed = '9';
                    break;
                case Key.Delete:
                case Key.Back:
                    Keypressed='C';
                    break;
                case Key.Return :
                    Keypressed = 'ü';
                    break;

            }
            if (Keypressed != null)
            {
                Button forcepressed = null;
                switch (OperatingMode)
                {
                    case OperatingModeConstants.OpMode_PinEntry:
                        forcepressed = findKeybutton(Keypressed.ToString(), new Button[]{EntryButton0,EntryButton1,EntryButton2,EntryButton3,
                                                                            EntryButton4,EntryButton5,EntryButton6,EntryButton7,
                                                                            EntryButton8,EntryButton9,EntryButtonOK,EntryButtonC}, EntryButtonClick);
                        break;
                    case OperatingModeConstants.OpMode_ScanOrder:
                        forcepressed = findKeybutton(Keypressed.ToString(), new Button[] { WOButton0,WOButton1,WOButton2,WOButton3,
                                                                                        WOButton4,WOButton5,WOButton6,WOButton7,
                                                                                        WOButton8,WOButton9,WOButton0,WOButtonC,WOButtonOK}, WorkOrderButtonEntryClick); 
                        break;
                        
                }

            }

        }
        private Button findKeybutton(String lookcaption, Button[] searchthrough,RoutedEventHandler Invokefunction)
        {


            foreach (Button searchbutton in searchthrough)
            {
                if (searchbutton.Content is String)
                {
                    if ((searchbutton.Content as String).Equals(lookcaption))
                    {
                        Invokefunction(searchbutton, null);
                        return searchbutton;
                    }

                }
            }

            return null;
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void Clockedin_Expanded(object sender, RoutedEventArgs e)
        {
            PinEntry.Focus();

        }

        private void Clockedin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void Clockedin_Collapsed(object sender, RoutedEventArgs e)
        {
            PinEntry.Focus();
        }
    }
    public class AlternateRowStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style AlternateStyle { get; set; }
        //track row index...
        private int i = 0;

        public override Style SelectStyle(object item, DependencyObject container)
        {
            //reset counter if this is the first item...
            ItemsControl ctrl = ItemsControl.ItemsControlFromItemContainer(container);
            if (item == ctrl.Items[0])
            {
                i = 0;

            }
            i++;

            //choose between the two styles...
            return i % 2 == 1 ? DefaultStyle : AlternateStyle;


        }



    }


    static class dependextender
    {
        public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                T typed = parent as T;
                if (typed != null)
                {
                    return typed;

                }
                parent = VisualTreeHelper.GetParent(parent);




            }
            return null;


        }


    }
    public class MyLabel : Label
    {

        static MyLabel()
        {

            ContentProperty.OverrideMetadata(typeof(MyLabel),

                new FrameworkPropertyMetadata(

                    new PropertyChangedCallback(OnContentChanged)));

        }



        private static void OnContentChanged(DependencyObject d,

            DependencyPropertyChangedEventArgs e)
        {

            MyLabel mcc = d as MyLabel;

            if (mcc.ContentChanged != null)
            {

                DependencyPropertyChangedEventArgs args

                    = new DependencyPropertyChangedEventArgs(

                        ContentProperty, e.OldValue, e.NewValue);

                mcc.ContentChanged(mcc, args);

            }

        }



        public event DependencyPropertyChangedEventHandler ContentChanged;

    }

 


}
