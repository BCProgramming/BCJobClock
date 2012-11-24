using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCJobClockLib
{
    public partial class frmPasswordPrompt : Form
    {
        bool cancelpressed = false;
        public class CredentialsNotGivenException : ApplicationException
        {
            public CredentialsNotGivenException():this("Credentials not given.")
            {


            }
            public CredentialsNotGivenException(String message):this(message,null)
            {


            }
            public CredentialsNotGivenException(String message, Exception innerexception):base(message,innerexception)
            {


            }



        }

        public frmPasswordPrompt()
        {
            InitializeComponent();
        }

        private String mPassword, mUserName;


        public static void DoShowPrompt(out String username,out String password)
        {

            frmPasswordPrompt promptdialog = new frmPasswordPrompt();
            promptdialog.ShowPrompt(out username, out password);

        }


        public void ShowPrompt(out String userID, out String password)
        {

            if (ShowDialog() == DialogResult.OK)
            {

                userID = txtUserID.Text;
                password = txtPassword.Text;
            }
            else
            {
                throw new CredentialsNotGivenException("Login cancelled.");

            }

        }


        private void cmdOK_Click(object sender, EventArgs e)
        {
            mUserName=txtUserID.Text;
            mPassword=txtPassword.Text;
            Close();
        }

        private void frmPasswordPrompt_Load(object sender, EventArgs e)
        {
            
        }




    }
}
