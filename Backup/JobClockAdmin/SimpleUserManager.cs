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
    public partial class SimpleUserManager : Form
    {
        private class UserData
        {
            public String UserName { get; set; }
            public String PINCode { get; set; }
            public bool IsActive { get; set; }

            public UserData(String pUserName, String pPINCode, bool pIsActive)
            {
                UserName = pUserName;
                PINCode = pPINCode;
                IsActive = pIsActive;


            }

        }

        private DataLayer dlayer;


        public SimpleUserManager()
        {
            InitializeComponent();
        }

        private void SimpleUserManager_Load(object sender, EventArgs e)
        {
            try
            {
                dlayer = new DataLayer(DataLayer.ConnectionTypeConstants.Connection_Admin);
            }
            catch (Exception ex)
            {
                String messagemake="Unexpected Exception starting Simple User Manager:" + ex.ToString();
                MessageBox.Show(messagemake);
                DataLayer.LogAdmin(messagemake);

                Close();
                
            }

            lvwUsers.Columns.Add("NAME", "UserName");
            lvwUsers.Columns.Add("PIN", "PIN");

            lvwUsers.SizeColumnsEqual();
            RefreshList();

        }
        private void RefreshList()
        {
            lvwUsers.Items.Clear();

            var allusers = dlayer.GetUsers();
            foreach(var useritem in allusers)
            {
                //key is PIN; value is name.
                String usename = useritem.Value;
                String usepin = useritem.Key;
                bool useactive = dlayer.IsUserActive(usepin);
                UserData ud = new UserData(usename, usepin, useactive);

                ListViewItem additem = new ListViewItem(new string[] { usename, usepin });
                additem.Tag = ud;
                lvwUsers.Items.Add(additem);
                



            }



        }

        private void lvwUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void lvwUsers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            txtPIN.Enabled = 
                txtUserName.Enabled = 
                chkActive.Enabled = 
                (lvwUsers.SelectedItems.Count == 0);

            RefreshFieldData();
            
        }
        private void RefreshFieldData()
        {
            if (lvwUsers.SelectedItems.Count == 0) return;
            ListViewItem selitem = lvwUsers.SelectedItems[0];
            UserData seldata = (UserData)selitem.Tag;
            txtUserName.Text = seldata.UserName;
            txtPIN.Text = seldata.PINCode;
            chkActive.Checked = seldata.IsActive;


        }
        
    }
}
