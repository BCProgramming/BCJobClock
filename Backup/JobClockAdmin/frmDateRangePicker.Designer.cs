namespace JobClockAdmin
{
    partial class frmDateRangePicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDateRangePicker));
            this.RangePicker = new System.Windows.Forms.MonthCalendar();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.lblSelectedRange = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RangePicker
            // 
            this.RangePicker.CalendarDimensions = new System.Drawing.Size(3, 1);
            this.RangePicker.Location = new System.Drawing.Point(1, 1);
            this.RangePicker.MaxSelectionCount = 265;
            this.RangePicker.Name = "RangePicker";
            this.RangePicker.TabIndex = 0;
            this.RangePicker.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.RangePicker_DateSelected);
            this.RangePicker.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.RangePicker_DateChanged);
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(596, 165);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(515, 165);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // lblSelectedRange
            // 
            this.lblSelectedRange.AutoSize = true;
            this.lblSelectedRange.Location = new System.Drawing.Point(13, 169);
            this.lblSelectedRange.Name = "lblSelectedRange";
            this.lblSelectedRange.Size = new System.Drawing.Size(251, 13);
            this.lblSelectedRange.TabIndex = 3;
            this.lblSelectedRange.Text = "Selected Range: From 11/11/1111 To 11/11/1111";
            // 
            // frmDateRangePicker
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(683, 194);
            this.Controls.Add(this.lblSelectedRange);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.RangePicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDateRangePicker";
            this.Text = "Date Range Picker";
            this.Load += new System.EventHandler(this.frmDateRangePicker_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDateRangePicker_FormClosing);
            this.Resize += new System.EventHandler(this.frmDateRangePicker_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar RangePicker;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label lblSelectedRange;
    }
}