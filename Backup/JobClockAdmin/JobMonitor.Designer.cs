namespace JobClockAdmin
{
    partial class JobMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobMonitor));
            this.mStrip = new System.Windows.Forms.MenuStrip();
            this.monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvwUserListing = new System.Windows.Forms.ListView();
            this.mStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mStrip
            // 
            this.mStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monitorToolStripMenuItem});
            this.mStrip.Location = new System.Drawing.Point(0, 0);
            this.mStrip.Name = "mStrip";
            this.mStrip.Size = new System.Drawing.Size(296, 24);
            this.mStrip.TabIndex = 0;
            this.mStrip.Text = "menuStrip1";
            // 
            // monitorToolStripMenuItem
            // 
            this.monitorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            this.monitorToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.monitorToolStripMenuItem.Text = "&Monitor";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lvwUserListing
            // 
            this.lvwUserListing.FullRowSelect = true;
            this.lvwUserListing.GridLines = true;
            this.lvwUserListing.Location = new System.Drawing.Point(0, 27);
            this.lvwUserListing.Name = "lvwUserListing";
            this.lvwUserListing.ShowItemToolTips = true;
            this.lvwUserListing.Size = new System.Drawing.Size(295, 306);
            this.lvwUserListing.TabIndex = 1;
            this.lvwUserListing.UseCompatibleStateImageBehavior = false;
            this.lvwUserListing.View = System.Windows.Forms.View.Details;
            // 
            // JobMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 345);
            this.Controls.Add(this.lvwUserListing);
            this.Controls.Add(this.mStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mStrip;
            this.MaximizeBox = false;
            this.Name = "JobMonitor";
            this.Text = "JobMonitor";
            this.Load += new System.EventHandler(this.JobMonitor_Load);
            this.VisibleChanged += new System.EventHandler(this.JobMonitor_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobMonitor_FormClosing);
            this.Resize += new System.EventHandler(this.JobMonitor_Resize);
            this.mStrip.ResumeLayout(false);
            this.mStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mStrip;
        private System.Windows.Forms.ToolStripMenuItem monitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListView lvwUserListing;

    }
}