using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JobClockAdmin
{
    public partial class frmDateRangePicker : Form
    {
        public struct BasicDateRange : ICloneable 
        {
            public readonly DateTime StartDate;
            public readonly DateTime EndDate;

            public BasicDateRange(DateTime pStartDate, DateTime pEndDate)
            {



                //chose small value for start, larger value for end.
                StartDate = pStartDate<pEndDate?pStartDate:pEndDate;
                EndDate = pStartDate<pEndDate?pEndDate:pStartDate;


            }


            public object Clone()
            {
                return new BasicDateRange(StartDate, EndDate);
            }
        }
        /// <summary>
        /// DateRange as it was passed in (if passed in at all...)
        /// </summary>
        public  BasicDateRange CurrentDateRange;
        public BasicDateRange SelectedRange;
        private static readonly String DefaultTitle = "Select Date Range";
        private String _useTitle = DefaultTitle;
        private Button ButtonPressed=null;
        Size useDimensions;

        public static BasicDateRange ChooseRange(IWin32Window owner,BasicDateRange currentvalue,String useTitle,int cols,int rows)
        {

            frmDateRangePicker pickerform = new frmDateRangePicker(currentvalue, useTitle, cols, rows);
            BasicDateRange OriginalValue = (BasicDateRange)currentvalue.Clone();
            
            switch (pickerform.ShowDialog(owner))
            {
                case System.Windows.Forms.DialogResult.OK:

                    //return the selected value.
                    return pickerform.CurrentDateRange;

                    break;

                case System.Windows.Forms.DialogResult.Cancel:
                    //cancel, return the original value, unfettered.
                    return OriginalValue;
                    break;
            }
            return OriginalValue;
        }


        public frmDateRangePicker(BasicDateRange currentvalue,String usetitle,int Cols,int Rows):this()
        {
            

            useDimensions = new Size(Cols, Rows);
            CurrentDateRange=currentvalue; 
            _useTitle=usetitle;

        }

        public frmDateRangePicker()
        {
            InitializeComponent();
        }

        private void frmDateRangePicker_Load(object sender, EventArgs e)
        {
            //set the Month View Calendar size. to the values specified in the constructor.
            RangePicker.CalendarDimensions=useDimensions;
            RangePicker.SetSelectionRange(CurrentDateRange.StartDate, CurrentDateRange.EndDate);
            Text = _useTitle;

            //resize the form to accomodate the RangePicker.
            
            this.ClientSize = new Size(RangePicker.Right, RangePicker.Height + cmdCancel.Height + 10);
            //now, move the controls. First, the OK button.

            cmdOK.Location = new Point(ClientSize.Width - cmdOK.Width - 5, ClientSize.Height - cmdOK.Height - 5);
            //now the cancel button...
            cmdCancel.Location = new Point(cmdOK.Left - 3 - cmdCancel.Width, cmdOK.Top);

            //we want about 5 pixels between the buttons and the rangepicker, as well as the bottom of the form.

            Height += 10;
            

 

            

            




        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            ButtonPressed = cmdOK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            ButtonPressed=cmdCancel;
            Close();
        }

        private void frmDateRangePicker_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        private void RangePicker_DateChanged(object sender, DateRangeEventArgs e)
        {

            UpdateRange(e.Start, e.End);

        }
        private void UpdateRange(DateTime sTime, DateTime eTime)
        {

            String fmtuse = "Selected Range: {0} To {1}";


            String usetext = String.Format(fmtuse, sTime.ToShortDateString(), eTime.ToShortDateString());

            lblSelectedRange.Text = usetext;
            CurrentDateRange = new BasicDateRange(sTime, eTime);

        }

        private void RangePicker_DateSelected(object sender, DateRangeEventArgs e)
        {
            UpdateRange(e.Start, e.End);
        }

        private void frmDateRangePicker_Resize(object sender, EventArgs e)
        {

        }
    }
}
