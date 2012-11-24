namespace BCJobClockLib
{
    partial class frmWaitMessage
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
            this.pBarPleaseWait = new System.Windows.Forms.ProgressBar();
            this.lblDisplayMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pBarPleaseWait
            // 
            this.pBarPleaseWait.Location = new System.Drawing.Point(9, 40);
            this.pBarPleaseWait.Name = "pBarPleaseWait";
            this.pBarPleaseWait.Size = new System.Drawing.Size(294, 28);
            this.pBarPleaseWait.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pBarPleaseWait.TabIndex = 0;
            this.pBarPleaseWait.Value = 100;
            this.pBarPleaseWait.Visible = false;
            // 
            // lblDisplayMessage
            // 
            this.lblDisplayMessage.AutoSize = true;
            this.lblDisplayMessage.Location = new System.Drawing.Point(11, 12);
            this.lblDisplayMessage.Name = "lblDisplayMessage";
            this.lblDisplayMessage.Size = new System.Drawing.Size(0, 13);
            this.lblDisplayMessage.TabIndex = 1;
            // 
            // frmWaitMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 76);
            this.ControlBox = false;
            this.Controls.Add(this.lblDisplayMessage);
            this.Controls.Add(this.pBarPleaseWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWaitMessage";
            this.Text = "Please Wait...";
            this.Load += new System.EventHandler(this.frmWaitMessage_Load);
            this.Shown += new System.EventHandler(this.frmWaitMessage_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWaitMessage_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pBarPleaseWait;
        private System.Windows.Forms.Label lblDisplayMessage;
    }
}