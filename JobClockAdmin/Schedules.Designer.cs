namespace JobClockAdmin
{
    partial class Schedules
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
            this.tabSchedule = new System.Windows.Forms.TabControl();
            this.TabDaily = new System.Windows.Forms.TabPage();
            this.lvwDailyView = new System.Windows.Forms.ListView();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmdClose = new System.Windows.Forms.Button();
            this.tabSchedule.SuspendLayout();
            this.TabDaily.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSchedule
            // 
            this.tabSchedule.Controls.Add(this.TabDaily);
            this.tabSchedule.Controls.Add(this.tabPage2);
            this.tabSchedule.Location = new System.Drawing.Point(13, 13);
            this.tabSchedule.Name = "tabSchedule";
            this.tabSchedule.SelectedIndex = 0;
            this.tabSchedule.Size = new System.Drawing.Size(517, 295);
            this.tabSchedule.TabIndex = 0;
            // 
            // TabDaily
            // 
            this.TabDaily.Controls.Add(this.lvwDailyView);
            this.TabDaily.Controls.Add(this.monthCalendar1);
            this.TabDaily.Location = new System.Drawing.Point(4, 22);
            this.TabDaily.Name = "TabDaily";
            this.TabDaily.Padding = new System.Windows.Forms.Padding(3);
            this.TabDaily.Size = new System.Drawing.Size(509, 269);
            this.TabDaily.TabIndex = 0;
            this.TabDaily.Text = "Daily";
            this.TabDaily.UseVisualStyleBackColor = true;
            // 
            // lvwDailyView
            // 
            this.lvwDailyView.Location = new System.Drawing.Point(261, 27);
            this.lvwDailyView.Name = "lvwDailyView";
            this.lvwDailyView.Size = new System.Drawing.Size(242, 236);
            this.lvwDailyView.TabIndex = 1;
            this.lvwDailyView.UseCompatibleStateImageBehavior = false;
            this.lvwDailyView.View = System.Windows.Forms.View.Details;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(9, 27);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(425, 204);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(455, 314);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 1;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // Schedules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 349);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.tabSchedule);
            this.Name = "Schedules";
            this.Text = "Schedules";
            this.tabSchedule.ResumeLayout(false);
            this.TabDaily.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSchedule;
        private System.Windows.Forms.TabPage TabDaily;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ListView lvwDailyView;
    }
}