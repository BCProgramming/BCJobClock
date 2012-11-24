using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BASeCamp.Configuration;

namespace BCJobClockLib
{
    public class JobClockConfig
    {
        public static readonly string AppIdentifier = "BCJobClock";
        public static ImageManager Imageman = new ImageManager();
        public static String GetAppDataFolder()
        {

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppIdentifier);


        }
        public static String GetExportLibFolder()
        {
            return Path.Combine(GetAppDataFolder(), "ExportPlugins");


        }
        

        public static String GetINIFilePath()
        {
            return Path.Combine(GetAppDataFolder(), AppIdentifier + ".ini");


        }

        public static INIFile OurINI = new INIFile(GetINIFilePath());


        public JobClockConfig()
        {


        }
        public String DatabaseName
        {

            get {
                return OurINI["General"]["DatabaseName"].Value;
            }


            set {
                OurINI["General"]["DatabaseName"].Value=value;
            }


        }
        //OurINI["Database"]["ConnectionString"].Value; <<ConnectionString
        public bool Client_ShowUserList
        {
            get { return boolEx.xParse(OurINI["Client.Settings"]["ShowUsersList","true"].Value); }
            set { OurINI["Client.Settings"]["ShowUsersList"].Value = value.ToString(); }



        }
        public bool PurgeLogOnStartup
        {
            get { return OurINI["General"]["PurgeLogOnStartup"].GetBoolean(); }
            set { OurINI["General"]["PurgeLogOnStartup"].Value = value.ToString(); }



        }
        public string DatabaseType
        {
            get { return OurINI["Database"]["DatabaseType"].Value; }
            set { OurINI["Database"]["DatabaseType"].Value = value; }

        }

        public TimeSpan Monitor_WarningLength {

            get { return OurINI["Monitor.Settings"]["WarningLength"].GetValue<TimeSpan>(new TimeSpan()); }
            set { OurINI["Monitor.Settings"]["WarningLength"].SetValue(value); }

        }

        public bool CheckUpdates {
            get { return OurINI["General"]["CheckForUpdates"].GetValue<bool>(false); }
            set { OurINI["General"]["CheckForUpdates"].SetValue(value); }

        }

        public bool Admin_UpdateTabVisible {
            get { return OurINI["Admin.Settings"]["UpdateTabVisible"].GetValue(false); }
            set { OurINI["Admin.Settings"]["UpdateTabVisible"].SetValue(false); }

        }

        public bool RefreshTabOnClick {

            get
            {
                return OurINI["Admin.Settings"]["RefreshTabOnClick"].GetValue(true);
        }
            set { OurINI["Admin.Settings"]["RefreshTabOnClick"].SetValue(value); }
        }

        public int Admin_RefreshIntervalms
        {

            get
            {
                
                int tparse;
                if(int.TryParse(OurINI["Admin.Settings"]["UpdateInterval","1000"].Value, out tparse))
                    return tparse;
                else
                    return 1000;
                

            }

            set {
            OurINI["Admin.Settings"]["UpdateInterval"].Value = value.ToString();
            
            
            }


        }
        public bool ReportList_ShowOrderDescription
        {
            get { return boolEx.xParse(OurINI["Admin.Settings"]["ReportList_ShowOrderDescription", "true"].Value); }
            set { OurINI["Admin.Settings"]["ReportList_ShowOrderDescription"].Value = value.ToString(); }

        }
        public bool ReportList_ShowAllOptionOrders
        {
            get { return boolEx.xParse(OurINI["Admin.Settings"]["ReportList_ShowAllOptionOrders", "true"].Value); }
            set { OurINI["Admin.Settings"]["ReportList_ShowAllOptionOrders"].Value = value.ToString() ;} 


        }
        public bool ReportList_ShowAllOptionUsers
        {

            get { return boolEx.xParse(OurINI["Admin.Settings"]["ReportList_ShowAllOptionUsers", "true"].Value); }
            set { OurINI["Admin.Settings"]["ReportList_ShowAllOptionUsers"].Value = value.ToString(); }


        }
        
        public bool ReportList_ShowUserPINCode
        {
            get { return boolEx.xParse(OurINI["Admin.Settings"]["ReportList_ShowUserPINCode", "true"].Value); }
            set { OurINI["Admin.Settings"]["ReportList_ShowUserPINCode"].Value = value.ToString(); }

        }
        public int DateRangePicker_ColumnCount
        {
            get { return ParseX.ParseIntX(OurINI["Admin.Settings"]["DateRangePicker_ColumnCount", "3"].Value); }
            set { OurINI["Admin.Settings"]["DateRangePicker_ColumnCount"].Value = value.ToString(); }




        }
        public int DateRangePicker_RowCount
        {
            get { return ParseX.ParseIntX(OurINI["Admin.Settings"]["DateRangePicker_RowCount", "1"].Value); }
            set { OurINI["Admin.Settings"]["DateRangePicker_RowCount"].Value = value.ToString(); }


        }

        public bool PopulateUserOrderDropdown
        {
            get { return boolEx.xParse(OurINI["Admin.Settings"]["PopulateUserOrderDropDown", "false"].Value); }
            set { OurINI["Admin.Settings"]["PopulateUserOrderDropDown"].Value = value.ToString(); }


        }
        public bool Client_ShowActivityLog
        {
            get { return boolEx.xParse(OurINI["Client.Settings"]["ShowActivity","true"].Value); }
            set { OurINI["Client.Settings"]["ShowActivity"].Value = value.ToString(); }

        }
        public int MonitorRefreshDelay
        {
            get { return ParseX.ParseIntX(OurINI["Monitor.Settings"]["RefreshDelayTime", "5"].Value, 5); }
            set { OurINI["Monitor.Settings"]["RefreshDelayTime"].Value = value.ToString(); }


        }
        public String AdminConnectionString
        {
            get { return OurINI["Database"]["AdminConnectionString"].Value; }
            set { OurINI["Database"]["AdminConnectionString"].Value = value; }
        }
        public string AdminCredentials
        {
            get { return OurINI["Admin.Settings"]["Credentials", "!prompt!"].Value; }
            set { OurINI["Admin.Settings"]["Credentials"].Value = value; }




        }
        public string ClientCredentials
        {
            get { return OurINI["Client.Settings"]["Credentials", "!prompt!"].Value; }
            set { OurINI["Client.Settings"]["Credentials"].Value = value; }




        }
        public String ClientConnectionString
        {
            get { return OurINI["Database"]["ClientConnectionString"].Value; }
            set { OurINI["Database"]["ClientConnectionString"].Value = value; }
        }

        public bool AutoAddOrders
        {
            get { return (Boolean.Parse(OurINI["General"]["AutoAddOrders"].Value)); }
            set { OurINI["General"]["AutoAddOrders"].Value = value.ToString(); }
        }

    /// <summary>
    /// Retrieves the "SingleWorkOrderMode" Option.
    /// default is an empty string.
    /// 
    /// When this is not an empty string or 0, the client will not display the "prompt" for a WorkOrder code, and will instead pretend that the given value was given.
    /// </summary>
        public String SingleWorkOrderMode
        {
            get { 
                
                String parseme = OurINI["Client.Settings"]["SingleWorkOrderMode"].Value;
                if(String.IsNullOrEmpty(parseme) || parseme=="0")
                {
                    return "";

                }
                else
                    return parseme;
                

            
            }
            set { OurINI["Client.Settings"]["SingleWorkOrderMode"].Value = value.ToString(); }



        }
        public int WorkOrderMinLength
        {
            get
            {
                String parsedval = OurINI["Client.Settings"]["WorkOrderMinLength", "4"].Value;
                int tparse;
                if (int.TryParse(parsedval, out tparse))
                    return tparse;
                else
                    return 0;


            }
            set {OurINI["Client.Settings"]["WorkOrderMinLength"].Value=value.ToString(); }

        }

        public int PINCodeMaxLength
        {
            get
            {
                String parsedval = OurINI["Client.Settings"]["PINCodeMaxLength", "4"].Value;
                int tparse;
                if (int.TryParse(parsedval, out tparse))
                {
                    return tparse;

                }
                else
                {
                    return 0;
                }

            }
            set {OurINI["Client.Settings"]["PINCodeMaxLength"].Value=value.ToString(); }
        }

        public int PINCodeMinLength
        {
            get {
            String parsedval = OurINI["Client.Settings"]["PINCodeMinLength","4"].Value;
                int tparse;
            if(int.TryParse(parsedval,out tparse))
                return tparse;
                else
                return 4;
            
            }
            set {
                OurINI["Client.Settings"]["PINCodeMinLength"].Value = value.ToString();
            
            }

        }
        public int WorkOrderMaxLength
        {
            get {
            String parsedval = OurINI["Client.Settings"]["WorkOrderMaxLength","7"].Value;
            int tparse;
            if (int.TryParse(parsedval, out tparse))
            {
                return tparse;

            }
            else
                return 0;
            }

            set {
            OurINI["Client.Settings"]["WorkOrderMaxLength"].Value=value.ToString();
            
            }

        }

        public string MonitorConnectionString
        {
            get { return OurINI["Database"]["MonitorConnectionString"].Value; }
            set { OurINI["Database"]["MonitorConnectionString"].Value=value; }
        }

        public string MonitorCredentials
        {
            get { return OurINI["Monitor.Settings"]["Credentials"].Value; }
            set { OurINI["Monitor.Settings"]["Credentials"].Value=value; }
        }
        public bool Monitor_NotifyAvailableTech
        {
            get { return OurINI["Monitor.Settings"]["NotifyAvailableTech", "false"].GetValue(false); }
            set { OurINI["Monitor.Settings"]["NotifyAvailableTech"].Value = value.ToString(); }

        }

        public bool Client_PasswordPIN
        {
            get { return OurINI["Client.Settings"]["PasswordPIN", "false"].GetValue(false); }
            set { OurINI["Client.Settings"]["PasswordPIN", "false"].Value = value.ToString(); }




        }

        public INIFile INIObject
        {
            get { return OurINI; }
        }

        public void Save()
        {

            OurINI.Save();

        }
    }
}
