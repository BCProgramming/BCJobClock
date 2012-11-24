namespace JobClockAdmin
{
    partial class frmJobClockAdmin
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJobClockAdmin));
            this.tabAdminpanel = new System.Windows.Forms.TabControl();
            this.TabPageGeneral = new System.Windows.Forms.TabPage();
            this.cmSchedule = new System.Windows.Forms.Button();
            this.chkuserlisting = new System.Windows.Forms.CheckBox();
            this.chkHidePIN = new System.Windows.Forms.CheckBox();
            this.txtSingleWorkOrderMode = new System.Windows.Forms.TextBox();
            this.chkSingleWorkorderMode = new System.Windows.Forms.CheckBox();
            this.chkAutoAddOrders = new System.Windows.Forms.CheckBox();
            this.cmdReinitialize = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtClientConnectionString = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdApplyDB = new System.Windows.Forms.Button();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.lblDatabaseName = new System.Windows.Forms.Label();
            this.txtAdminConnectionString = new System.Windows.Forms.TextBox();
            this.lblConnection = new System.Windows.Forms.Label();
            this.TabPageUsers = new System.Windows.Forms.TabPage();
            this.GroupEditingUser = new System.Windows.Forms.GroupBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.DropDownOrders = new System.Windows.Forms.ToolStripDropDownButton();
            this.ordersGhostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtPINCode = new System.Windows.Forms.TextBox();
            this.lblPIN = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ToolStripUsers = new System.Windows.Forms.ToolStrip();
            this.tStripRefreshUsers = new System.Windows.Forms.ToolStripButton();
            this.tstripNewUser = new System.Windows.Forms.ToolStripButton();
            this.toolstripdeleteuser = new System.Windows.Forms.ToolStripButton();
            this.tStripImportUsers = new System.Windows.Forms.ToolStripButton();
            this.tStripCopyUsers = new System.Windows.Forms.ToolStripButton();
            this.lvwUsers = new System.Windows.Forms.ListView();
            this.TabPageOrders = new System.Windows.Forms.TabPage();
            this.GroupEditingOrder = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtOrderID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tStripEditOrder = new System.Windows.Forms.ToolStrip();
            this.DropDownUsers = new System.Windows.Forms.ToolStripDropDownButton();
            this.editGhostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tStripOrders = new System.Windows.Forms.ToolStrip();
            this.tStripRefreshOrders = new System.Windows.Forms.ToolStripButton();
            this.tStripNewOrder = new System.Windows.Forms.ToolStripButton();
            this.tStripRemoveOrder = new System.Windows.Forms.ToolStripButton();
            this.toolstripcopyorders = new System.Windows.Forms.ToolStripButton();
            this.lvwOrders = new System.Windows.Forms.ListView();
            this.TabPageReporting = new System.Windows.Forms.TabPage();
            this.cboReportByListing = new System.Windows.Forms.ComboBox();
            this.lblReportByItem = new System.Windows.Forms.Label();
            this.cmdReport = new System.Windows.Forms.Button();
            this.fraReportResult = new System.Windows.Forms.GroupBox();
            this.toolStripReportResults = new System.Windows.Forms.ToolStrip();
            this.tstripCopyResult = new System.Windows.Forms.ToolStripDropDownButton();
            this.tStripExportResult = new System.Windows.Forms.ToolStripDropDownButton();
            this.lvwReportResult = new System.Windows.Forms.ListView();
            this.fraDateRange = new System.Windows.Forms.GroupBox();
            this.cmdChangeRange = new System.Windows.Forms.Button();
            this.lblReportPeriod = new System.Windows.Forms.Label();
            this.cboReportBy = new System.Windows.Forms.ComboBox();
            this.lblReportBy = new System.Windows.Forms.Label();
            this.TabPageLogs = new System.Windows.Forms.TabPage();
            this.cmdPurgeLog = new System.Windows.Forms.Button();
            this.lvwLogData = new System.Windows.Forms.ListView();
            this.tabUpdate = new System.Windows.Forms.TabPage();
            this.cmdDownloadUpdate = new System.Windows.Forms.Button();
            this.lblLatestVersion = new System.Windows.Forms.Label();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tTools = new System.Windows.Forms.ToolTip(this.components);
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.tabAdminpanel.SuspendLayout();
            this.TabPageGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TabPageUsers.SuspendLayout();
            this.GroupEditingUser.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.ToolStripUsers.SuspendLayout();
            this.TabPageOrders.SuspendLayout();
            this.GroupEditingOrder.SuspendLayout();
            this.tStripEditOrder.SuspendLayout();
            this.tStripOrders.SuspendLayout();
            this.TabPageReporting.SuspendLayout();
            this.fraReportResult.SuspendLayout();
            this.toolStripReportResults.SuspendLayout();
            this.fraDateRange.SuspendLayout();
            this.TabPageLogs.SuspendLayout();
            this.tabUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAdminpanel
            // 
            this.tabAdminpanel.Controls.Add(this.TabPageGeneral);
            this.tabAdminpanel.Controls.Add(this.TabPageUsers);
            this.tabAdminpanel.Controls.Add(this.TabPageOrders);
            this.tabAdminpanel.Controls.Add(this.TabPageReporting);
            this.tabAdminpanel.Controls.Add(this.TabPageLogs);
            this.tabAdminpanel.Controls.Add(this.tabUpdate);
            this.tabAdminpanel.Location = new System.Drawing.Point(6, 8);
            this.tabAdminpanel.Name = "tabAdminpanel";
            this.tabAdminpanel.SelectedIndex = 0;
            this.tabAdminpanel.Size = new System.Drawing.Size(455, 328);
            this.tabAdminpanel.TabIndex = 0;
            this.tabAdminpanel.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabAdminpanel_Selected);
            this.tabAdminpanel.TabIndexChanged += new System.EventHandler(this.tabAdminpanel_TabIndexChanged);
            this.tabAdminpanel.Resize += new System.EventHandler(this.tabAdminpanel_Resize);
            // 
            // TabPageGeneral
            // 
            this.TabPageGeneral.Controls.Add(this.cmSchedule);
            this.TabPageGeneral.Controls.Add(this.chkuserlisting);
            this.TabPageGeneral.Controls.Add(this.chkHidePIN);
            this.TabPageGeneral.Controls.Add(this.txtSingleWorkOrderMode);
            this.TabPageGeneral.Controls.Add(this.chkSingleWorkorderMode);
            this.TabPageGeneral.Controls.Add(this.chkAutoAddOrders);
            this.TabPageGeneral.Controls.Add(this.cmdReinitialize);
            this.TabPageGeneral.Controls.Add(this.groupBox1);
            this.TabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.TabPageGeneral.Name = "TabPageGeneral";
            this.TabPageGeneral.Size = new System.Drawing.Size(447, 302);
            this.TabPageGeneral.TabIndex = 3;
            this.TabPageGeneral.Text = "General";
            this.TabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // cmSchedule
            // 
            this.cmSchedule.Location = new System.Drawing.Point(358, 168);
            this.cmSchedule.Name = "cmSchedule";
            this.cmSchedule.Size = new System.Drawing.Size(75, 23);
            this.cmSchedule.TabIndex = 7;
            this.cmSchedule.Text = "Schedule...";
            this.tTools.SetToolTip(this.cmSchedule, "View/Edit Schedule information.");
            this.cmSchedule.UseVisualStyleBackColor = true;
            this.cmSchedule.Visible = false;
            // 
            // chkuserlisting
            // 
            this.chkuserlisting.AutoSize = true;
            this.chkuserlisting.Location = new System.Drawing.Point(22, 245);
            this.chkuserlisting.Name = "chkuserlisting";
            this.chkuserlisting.Size = new System.Drawing.Size(151, 17);
            this.chkuserlisting.TabIndex = 6;
            this.chkuserlisting.Text = "Show User Listing in Client";
            this.tTools.SetToolTip(this.chkuserlisting, "Whether to show the Listing of users in the client touchscreen program.");
            this.chkuserlisting.UseVisualStyleBackColor = true;
            this.chkuserlisting.CheckedChanged += new System.EventHandler(this.chkuserlisting_CheckedChanged);
            // 
            // chkHidePIN
            // 
            this.chkHidePIN.AutoSize = true;
            this.chkHidePIN.Location = new System.Drawing.Point(22, 222);
            this.chkHidePIN.Name = "chkHidePIN";
            this.chkHidePIN.Size = new System.Drawing.Size(108, 17);
            this.chkHidePIN.TabIndex = 5;
            this.chkHidePIN.Text = "Hide PIN in client";
            this.tTools.SetToolTip(this.chkHidePIN, "Show or hide the visible PIN in the touchscreen client.");
            this.chkHidePIN.UseVisualStyleBackColor = true;
            this.chkHidePIN.CheckedChanged += new System.EventHandler(this.chkHidePIN_CheckedChanged);
            // 
            // txtSingleWorkOrderMode
            // 
            this.txtSingleWorkOrderMode.Location = new System.Drawing.Point(40, 170);
            this.txtSingleWorkOrderMode.Name = "txtSingleWorkOrderMode";
            this.txtSingleWorkOrderMode.Size = new System.Drawing.Size(206, 20);
            this.txtSingleWorkOrderMode.TabIndex = 4;
            this.tTools.SetToolTip(this.txtSingleWorkOrderMode, "OrderID of workorder to be used.");
            this.txtSingleWorkOrderMode.Validated += new System.EventHandler(this.txtSingleWorkOrderMode_Validated);
            // 
            // chkSingleWorkorderMode
            // 
            this.chkSingleWorkorderMode.AutoSize = true;
            this.chkSingleWorkorderMode.Location = new System.Drawing.Point(21, 147);
            this.chkSingleWorkorderMode.Name = "chkSingleWorkorderMode";
            this.chkSingleWorkorderMode.Size = new System.Drawing.Size(146, 17);
            this.chkSingleWorkorderMode.TabIndex = 3;
            this.chkSingleWorkorderMode.Text = "Single Work Order Mode:";
            this.tTools.SetToolTip(this.chkSingleWorkorderMode, "Single work order mode enforces the use of only a single work order. Useful for s" +
                    "ituations where there is only one \"Job\", such as for use with retail clock/in cl" +
                    "ock/out and scheduling.");
            this.chkSingleWorkorderMode.UseVisualStyleBackColor = true;
            this.chkSingleWorkorderMode.CheckedChanged += new System.EventHandler(this.chkSingleWorkorderMode_CheckedChanged);
            // 
            // chkAutoAddOrders
            // 
            this.chkAutoAddOrders.AutoSize = true;
            this.chkAutoAddOrders.Location = new System.Drawing.Point(22, 199);
            this.chkAutoAddOrders.Name = "chkAutoAddOrders";
            this.chkAutoAddOrders.Size = new System.Drawing.Size(144, 17);
            this.chkAutoAddOrders.TabIndex = 2;
            this.chkAutoAddOrders.Text = "Automatically Add Orders";
            this.tTools.SetToolTip(this.chkAutoAddOrders, "When a user clocks into a non-existent order, it will be created.");
            this.chkAutoAddOrders.UseVisualStyleBackColor = true;
            // 
            // cmdReinitialize
            // 
            this.cmdReinitialize.Location = new System.Drawing.Point(332, 256);
            this.cmdReinitialize.Name = "cmdReinitialize";
            this.cmdReinitialize.Size = new System.Drawing.Size(112, 43);
            this.cmdReinitialize.TabIndex = 1;
            this.cmdReinitialize.Text = "Reinitialize";
            this.cmdReinitialize.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.tTools.SetToolTip(this.cmdReinitialize, "use to reinitialize the database. This will delete all data and information in it" +
                    "!");
            this.cmdReinitialize.UseVisualStyleBackColor = true;
            this.cmdReinitialize.Click += new System.EventHandler(this.cmdReinitialize_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtClientConnectionString);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmdApplyDB);
            this.groupBox1.Controls.Add(this.txtDatabaseName);
            this.groupBox1.Controls.Add(this.lblDatabaseName);
            this.groupBox1.Controls.Add(this.txtAdminConnectionString);
            this.groupBox1.Controls.Add(this.lblConnection);
            this.groupBox1.Location = new System.Drawing.Point(7, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Settings";
            // 
            // txtClientConnectionString
            // 
            this.txtClientConnectionString.Location = new System.Drawing.Point(113, 44);
            this.txtClientConnectionString.Name = "txtClientConnectionString";
            this.txtClientConnectionString.Size = new System.Drawing.Size(254, 20);
            this.txtClientConnectionString.TabIndex = 7;
            this.tTools.SetToolTip(this.txtClientConnectionString, "Connection to use for Clients.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Client Connection:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 26);
            this.label2.TabIndex = 5;
            this.label2.Text = "Note: Running clients using this INI file will need to be restarted for these cha" +
                "nges to take effect.";
            // 
            // cmdApplyDB
            // 
            this.cmdApplyDB.Location = new System.Drawing.Point(341, 94);
            this.cmdApplyDB.Name = "cmdApplyDB";
            this.cmdApplyDB.Size = new System.Drawing.Size(71, 26);
            this.cmdApplyDB.TabIndex = 4;
            this.cmdApplyDB.Text = "&Apply";
            this.cmdApplyDB.UseVisualStyleBackColor = true;
            this.cmdApplyDB.Click += new System.EventHandler(this.cmdApplyDB_Click);
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(112, 69);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(255, 20);
            this.txtDatabaseName.TabIndex = 3;
            this.tTools.SetToolTip(this.txtDatabaseName, "Name of the database.");
            // 
            // lblDatabaseName
            // 
            this.lblDatabaseName.AutoSize = true;
            this.lblDatabaseName.Location = new System.Drawing.Point(11, 68);
            this.lblDatabaseName.Name = "lblDatabaseName";
            this.lblDatabaseName.Size = new System.Drawing.Size(87, 13);
            this.lblDatabaseName.TabIndex = 2;
            this.lblDatabaseName.Text = "Database Name:";
            // 
            // txtAdminConnectionString
            // 
            this.txtAdminConnectionString.Location = new System.Drawing.Point(111, 20);
            this.txtAdminConnectionString.Name = "txtAdminConnectionString";
            this.txtAdminConnectionString.Size = new System.Drawing.Size(256, 20);
            this.txtAdminConnectionString.TabIndex = 1;
            this.tTools.SetToolTip(this.txtAdminConnectionString, "Connection to use for the administrative program.");
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Location = new System.Drawing.Point(11, 23);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(96, 13);
            this.lblConnection.TabIndex = 0;
            this.lblConnection.Text = "Admin Connection:";
            // 
            // TabPageUsers
            // 
            this.TabPageUsers.Controls.Add(this.GroupEditingUser);
            this.TabPageUsers.Controls.Add(this.ToolStripUsers);
            this.TabPageUsers.Controls.Add(this.lvwUsers);
            this.TabPageUsers.Location = new System.Drawing.Point(4, 22);
            this.TabPageUsers.Name = "TabPageUsers";
            this.TabPageUsers.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageUsers.Size = new System.Drawing.Size(447, 302);
            this.TabPageUsers.TabIndex = 0;
            this.TabPageUsers.Text = "Users";
            this.TabPageUsers.UseVisualStyleBackColor = true;
            this.TabPageUsers.Resize += new System.EventHandler(this.TabPageUsers_Resize);
            // 
            // GroupEditingUser
            // 
            this.GroupEditingUser.Controls.Add(this.chkActive);
            this.GroupEditingUser.Controls.Add(this.toolStrip1);
            this.GroupEditingUser.Controls.Add(this.txtPINCode);
            this.GroupEditingUser.Controls.Add(this.lblPIN);
            this.GroupEditingUser.Controls.Add(this.txtUserName);
            this.GroupEditingUser.Controls.Add(this.label1);
            this.GroupEditingUser.Location = new System.Drawing.Point(7, 208);
            this.GroupEditingUser.Name = "GroupEditingUser";
            this.GroupEditingUser.Size = new System.Drawing.Size(434, 88);
            this.GroupEditingUser.TabIndex = 2;
            this.GroupEditingUser.TabStop = false;
            this.GroupEditingUser.Text = "&Editing";
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(6, 43);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 17);
            this.chkActive.TabIndex = 5;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            this.chkActive.CheckedChanged += new System.EventHandler(this.chkActive_CheckedChanged);
            this.chkActive.Validated += new System.EventHandler(this.chkActive_Validated);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DropDownOrders});
            this.toolStrip1.Location = new System.Drawing.Point(361, 14);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(58, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // DropDownOrders
            // 
            this.DropDownOrders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DropDownOrders.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ordersGhostToolStripMenuItem});
            this.DropDownOrders.Image = ((System.Drawing.Image)(resources.GetObject("DropDownOrders.Image")));
            this.DropDownOrders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DropDownOrders.Name = "DropDownOrders";
            this.DropDownOrders.Size = new System.Drawing.Size(55, 22);
            this.DropDownOrders.Text = "Orders";
            this.DropDownOrders.DropDownOpening += new System.EventHandler(this.DropDownOrders_DropDownOpening);
            // 
            // ordersGhostToolStripMenuItem
            // 
            this.ordersGhostToolStripMenuItem.Name = "ordersGhostToolStripMenuItem";
            this.ordersGhostToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.ordersGhostToolStripMenuItem.Text = "OrdersGhost";
            // 
            // txtPINCode
            // 
            this.txtPINCode.Location = new System.Drawing.Point(235, 16);
            this.txtPINCode.Name = "txtPINCode";
            this.txtPINCode.Size = new System.Drawing.Size(118, 20);
            this.txtPINCode.TabIndex = 3;
            this.txtPINCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPINCode_PreviewKeyDown);
            this.txtPINCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtPINCode_Validating);
            this.txtPINCode.Validated += new System.EventHandler(this.txtPINCode_Validated);
            // 
            // lblPIN
            // 
            this.lblPIN.AutoSize = true;
            this.lblPIN.Location = new System.Drawing.Point(203, 19);
            this.lblPIN.Name = "lblPIN";
            this.lblPIN.Size = new System.Drawing.Size(28, 13);
            this.lblPIN.TabIndex = 2;
            this.lblPIN.Text = "PIN:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(72, 17);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(115, 20);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtUserName_PreviewKeyDown);
            this.txtUserName.Validated += new System.EventHandler(this.txtUserName_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&UserName:";
            // 
            // ToolStripUsers
            // 
            this.ToolStripUsers.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolStripUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tStripRefreshUsers,
            this.tstripNewUser,
            this.toolstripdeleteuser,
            this.tStripImportUsers,
            this.tStripCopyUsers});
            this.ToolStripUsers.Location = new System.Drawing.Point(3, 3);
            this.ToolStripUsers.Name = "ToolStripUsers";
            this.ToolStripUsers.Size = new System.Drawing.Size(441, 39);
            this.ToolStripUsers.TabIndex = 1;
            this.ToolStripUsers.Text = "toolStrip1";
            // 
            // tStripRefreshUsers
            // 
            this.tStripRefreshUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripRefreshUsers.Image = ((System.Drawing.Image)(resources.GetObject("tStripRefreshUsers.Image")));
            this.tStripRefreshUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripRefreshUsers.Name = "tStripRefreshUsers";
            this.tStripRefreshUsers.Size = new System.Drawing.Size(36, 36);
            this.tStripRefreshUsers.Text = "Refresh";
            this.tStripRefreshUsers.Click += new System.EventHandler(this.tStripRefreshUsers_Click);
            // 
            // tstripNewUser
            // 
            this.tstripNewUser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tstripNewUser.Image = ((System.Drawing.Image)(resources.GetObject("tstripNewUser.Image")));
            this.tstripNewUser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tstripNewUser.Name = "tstripNewUser";
            this.tstripNewUser.Size = new System.Drawing.Size(36, 36);
            this.tstripNewUser.Text = "New";
            this.tstripNewUser.ToolTipText = "New User";
            this.tstripNewUser.Click += new System.EventHandler(this.tstripNewUser_Click);
            // 
            // toolstripdeleteuser
            // 
            this.toolstripdeleteuser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripdeleteuser.Enabled = false;
            this.toolstripdeleteuser.Image = ((System.Drawing.Image)(resources.GetObject("toolstripdeleteuser.Image")));
            this.toolstripdeleteuser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripdeleteuser.Name = "toolstripdeleteuser";
            this.toolstripdeleteuser.Size = new System.Drawing.Size(36, 36);
            this.toolstripdeleteuser.Text = "Delete";
            this.toolstripdeleteuser.ToolTipText = "Delete User";
            this.toolstripdeleteuser.Click += new System.EventHandler(this.toolstripdeleteuser_Click);
            // 
            // tStripImportUsers
            // 
            this.tStripImportUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripImportUsers.Image = ((System.Drawing.Image)(resources.GetObject("tStripImportUsers.Image")));
            this.tStripImportUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripImportUsers.Name = "tStripImportUsers";
            this.tStripImportUsers.Size = new System.Drawing.Size(36, 36);
            this.tStripImportUsers.Text = "Import";
            this.tStripImportUsers.ToolTipText = "Import a list of Users from an external file";
            this.tStripImportUsers.Click += new System.EventHandler(this.tStripImportUsers_Click);
            // 
            // tStripCopyUsers
            // 
            this.tStripCopyUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripCopyUsers.Image = ((System.Drawing.Image)(resources.GetObject("tStripCopyUsers.Image")));
            this.tStripCopyUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripCopyUsers.Name = "tStripCopyUsers";
            this.tStripCopyUsers.Size = new System.Drawing.Size(36, 36);
            this.tStripCopyUsers.Text = "Copy";
            this.tStripCopyUsers.Click += new System.EventHandler(this.tStripCopyUsers_Click);
            // 
            // lvwUsers
            // 
            this.lvwUsers.FullRowSelect = true;
            this.lvwUsers.GridLines = true;
            this.lvwUsers.HideSelection = false;
            this.lvwUsers.Location = new System.Drawing.Point(2, 56);
            this.lvwUsers.Name = "lvwUsers";
            this.lvwUsers.Size = new System.Drawing.Size(439, 146);
            this.lvwUsers.TabIndex = 0;
            this.lvwUsers.UseCompatibleStateImageBehavior = false;
            this.lvwUsers.View = System.Windows.Forms.View.Details;
            this.lvwUsers.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwUsers_ItemSelectionChanged);
            this.lvwUsers.SelectedIndexChanged += new System.EventHandler(this.lvwUsers_SelectedIndexChanged);
            this.lvwUsers.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwUsers_MouseClick);
            // 
            // TabPageOrders
            // 
            this.TabPageOrders.Controls.Add(this.GroupEditingOrder);
            this.TabPageOrders.Controls.Add(this.tStripOrders);
            this.TabPageOrders.Controls.Add(this.lvwOrders);
            this.TabPageOrders.Location = new System.Drawing.Point(4, 22);
            this.TabPageOrders.Name = "TabPageOrders";
            this.TabPageOrders.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageOrders.Size = new System.Drawing.Size(447, 302);
            this.TabPageOrders.TabIndex = 1;
            this.TabPageOrders.Text = "Orders";
            this.TabPageOrders.UseVisualStyleBackColor = true;
            this.TabPageOrders.Resize += new System.EventHandler(this.TabPageOrders_Resize);
            // 
            // GroupEditingOrder
            // 
            this.GroupEditingOrder.Controls.Add(this.txtDescription);
            this.GroupEditingOrder.Controls.Add(this.txtOrderID);
            this.GroupEditingOrder.Controls.Add(this.label5);
            this.GroupEditingOrder.Controls.Add(this.label4);
            this.GroupEditingOrder.Controls.Add(this.tStripEditOrder);
            this.GroupEditingOrder.Location = new System.Drawing.Point(5, 206);
            this.GroupEditingOrder.Name = "GroupEditingOrder";
            this.GroupEditingOrder.Size = new System.Drawing.Size(438, 95);
            this.GroupEditingOrder.TabIndex = 2;
            this.GroupEditingOrder.TabStop = false;
            this.GroupEditingOrder.Text = "Edit Order";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(14, 53);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(342, 20);
            this.txtDescription.TabIndex = 4;
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            this.txtDescription.Validated += new System.EventHandler(this.txtDescription_Validated);
            // 
            // txtOrderID
            // 
            this.txtOrderID.Location = new System.Drawing.Point(58, 17);
            this.txtOrderID.Name = "txtOrderID";
            this.txtOrderID.Size = new System.Drawing.Size(298, 20);
            this.txtOrderID.TabIndex = 3;
            this.txtOrderID.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtOrderID_PreviewKeyDown);
            this.txtOrderID.Validated += new System.EventHandler(this.txtOrderID_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Description:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "OrderID:";
            // 
            // tStripEditOrder
            // 
            this.tStripEditOrder.Dock = System.Windows.Forms.DockStyle.None;
            this.tStripEditOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DropDownUsers});
            this.tStripEditOrder.Location = new System.Drawing.Point(364, 16);
            this.tStripEditOrder.Name = "tStripEditOrder";
            this.tStripEditOrder.Size = new System.Drawing.Size(60, 25);
            this.tStripEditOrder.TabIndex = 0;
            this.tStripEditOrder.Text = "toolStrip2";
            // 
            // DropDownUsers
            // 
            this.DropDownUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DropDownUsers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editGhostToolStripMenuItem});
            this.DropDownUsers.Image = ((System.Drawing.Image)(resources.GetObject("DropDownUsers.Image")));
            this.DropDownUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DropDownUsers.Name = "DropDownUsers";
            this.DropDownUsers.Size = new System.Drawing.Size(48, 22);
            this.DropDownUsers.Text = "Users";
            this.DropDownUsers.DropDownOpening += new System.EventHandler(this.DropDownUsers_DropDownOpening);
            this.DropDownUsers.Click += new System.EventHandler(this.DropDownUsers_Click);
            // 
            // editGhostToolStripMenuItem
            // 
            this.editGhostToolStripMenuItem.Name = "editGhostToolStripMenuItem";
            this.editGhostToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.editGhostToolStripMenuItem.Text = "EditGhost";
            // 
            // tStripOrders
            // 
            this.tStripOrders.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tStripOrders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tStripRefreshOrders,
            this.tStripNewOrder,
            this.tStripRemoveOrder,
            this.toolstripcopyorders});
            this.tStripOrders.Location = new System.Drawing.Point(3, 3);
            this.tStripOrders.Name = "tStripOrders";
            this.tStripOrders.Size = new System.Drawing.Size(441, 39);
            this.tStripOrders.TabIndex = 1;
            this.tStripOrders.Text = "toolStrip2";
            // 
            // tStripRefreshOrders
            // 
            this.tStripRefreshOrders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripRefreshOrders.Image = ((System.Drawing.Image)(resources.GetObject("tStripRefreshOrders.Image")));
            this.tStripRefreshOrders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripRefreshOrders.Name = "tStripRefreshOrders";
            this.tStripRefreshOrders.Size = new System.Drawing.Size(36, 36);
            this.tStripRefreshOrders.Text = "Refresh";
            this.tStripRefreshOrders.ToolTipText = "Refresh Orders";
            this.tStripRefreshOrders.Click += new System.EventHandler(this.tStripRefreshOrders_Click);
            // 
            // tStripNewOrder
            // 
            this.tStripNewOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripNewOrder.Image = ((System.Drawing.Image)(resources.GetObject("tStripNewOrder.Image")));
            this.tStripNewOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripNewOrder.Name = "tStripNewOrder";
            this.tStripNewOrder.Size = new System.Drawing.Size(36, 36);
            this.tStripNewOrder.Text = "New Order";
            this.tStripNewOrder.Click += new System.EventHandler(this.tStripNewOrder_Click);
            // 
            // tStripRemoveOrder
            // 
            this.tStripRemoveOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripRemoveOrder.Enabled = false;
            this.tStripRemoveOrder.Image = ((System.Drawing.Image)(resources.GetObject("tStripRemoveOrder.Image")));
            this.tStripRemoveOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripRemoveOrder.Name = "tStripRemoveOrder";
            this.tStripRemoveOrder.Size = new System.Drawing.Size(36, 36);
            this.tStripRemoveOrder.Text = "Remove Order";
            this.tStripRemoveOrder.Click += new System.EventHandler(this.tStripRemoveOrder_Click);
            // 
            // toolstripcopyorders
            // 
            this.toolstripcopyorders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripcopyorders.Image = ((System.Drawing.Image)(resources.GetObject("toolstripcopyorders.Image")));
            this.toolstripcopyorders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripcopyorders.Name = "toolstripcopyorders";
            this.toolstripcopyorders.Size = new System.Drawing.Size(36, 36);
            this.toolstripcopyorders.Text = "Copy";
            this.toolstripcopyorders.Click += new System.EventHandler(this.toolstripcopyorders_Click);
            // 
            // lvwOrders
            // 
            this.lvwOrders.FullRowSelect = true;
            this.lvwOrders.GridLines = true;
            this.lvwOrders.HideSelection = false;
            this.lvwOrders.Location = new System.Drawing.Point(2, 56);
            this.lvwOrders.Name = "lvwOrders";
            this.lvwOrders.ShowItemToolTips = true;
            this.lvwOrders.Size = new System.Drawing.Size(439, 145);
            this.lvwOrders.TabIndex = 0;
            this.lvwOrders.UseCompatibleStateImageBehavior = false;
            this.lvwOrders.View = System.Windows.Forms.View.Details;
            this.lvwOrders.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwOrders_ItemSelectionChanged);
            this.lvwOrders.SelectedIndexChanged += new System.EventHandler(this.lvwOrders_SelectedIndexChanged);
            // 
            // TabPageReporting
            // 
            this.TabPageReporting.Controls.Add(this.cboReportByListing);
            this.TabPageReporting.Controls.Add(this.lblReportByItem);
            this.TabPageReporting.Controls.Add(this.cmdReport);
            this.TabPageReporting.Controls.Add(this.fraReportResult);
            this.TabPageReporting.Controls.Add(this.fraDateRange);
            this.TabPageReporting.Controls.Add(this.cboReportBy);
            this.TabPageReporting.Controls.Add(this.lblReportBy);
            this.TabPageReporting.Location = new System.Drawing.Point(4, 22);
            this.TabPageReporting.Name = "TabPageReporting";
            this.TabPageReporting.Size = new System.Drawing.Size(447, 302);
            this.TabPageReporting.TabIndex = 2;
            this.TabPageReporting.Text = "Reporting";
            this.TabPageReporting.UseVisualStyleBackColor = true;
            this.TabPageReporting.Resize += new System.EventHandler(this.TabPageReporting_Resize);
            // 
            // cboReportByListing
            // 
            this.cboReportByListing.FormattingEnabled = true;
            this.cboReportByListing.Location = new System.Drawing.Point(276, 9);
            this.cboReportByListing.Name = "cboReportByListing";
            this.cboReportByListing.Size = new System.Drawing.Size(150, 21);
            this.cboReportByListing.TabIndex = 6;
            // 
            // lblReportByItem
            // 
            this.lblReportByItem.AutoSize = true;
            this.lblReportByItem.Location = new System.Drawing.Point(212, 13);
            this.lblReportByItem.Name = "lblReportByItem";
            this.lblReportByItem.Size = new System.Drawing.Size(60, 13);
            this.lblReportByItem.TabIndex = 5;
            this.lblReportByItem.Text = "Report For:";
            // 
            // cmdReport
            // 
            this.cmdReport.Location = new System.Drawing.Point(358, 113);
            this.cmdReport.Name = "cmdReport";
            this.cmdReport.Size = new System.Drawing.Size(69, 25);
            this.cmdReport.TabIndex = 4;
            this.cmdReport.Text = "&Report";
            this.cmdReport.UseVisualStyleBackColor = true;
            this.cmdReport.Click += new System.EventHandler(this.cmdReport_Click);
            // 
            // fraReportResult
            // 
            this.fraReportResult.Controls.Add(this.toolStripReportResults);
            this.fraReportResult.Controls.Add(this.lvwReportResult);
            this.fraReportResult.Location = new System.Drawing.Point(14, 144);
            this.fraReportResult.Name = "fraReportResult";
            this.fraReportResult.Size = new System.Drawing.Size(419, 155);
            this.fraReportResult.TabIndex = 3;
            this.fraReportResult.TabStop = false;
            this.fraReportResult.Text = "Result";
            this.fraReportResult.Resize += new System.EventHandler(this.fraReportResult_Resize);
            // 
            // toolStripReportResults
            // 
            this.toolStripReportResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tstripCopyResult,
            this.tStripExportResult});
            this.toolStripReportResults.Location = new System.Drawing.Point(3, 16);
            this.toolStripReportResults.Name = "toolStripReportResults";
            this.toolStripReportResults.Size = new System.Drawing.Size(413, 25);
            this.toolStripReportResults.TabIndex = 5;
            this.toolStripReportResults.Text = "toolStrip2";
            // 
            // tstripCopyResult
            // 
            this.tstripCopyResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tstripCopyResult.Image = ((System.Drawing.Image)(resources.GetObject("tstripCopyResult.Image")));
            this.tstripCopyResult.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tstripCopyResult.Name = "tstripCopyResult";
            this.tstripCopyResult.Size = new System.Drawing.Size(29, 22);
            this.tstripCopyResult.Text = "Copy";
            this.tstripCopyResult.ToolTipText = "Copy Report data to clipboard";
            // 
            // tStripExportResult
            // 
            this.tStripExportResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tStripExportResult.Image = ((System.Drawing.Image)(resources.GetObject("tStripExportResult.Image")));
            this.tStripExportResult.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tStripExportResult.Name = "tStripExportResult";
            this.tStripExportResult.Size = new System.Drawing.Size(29, 22);
            this.tStripExportResult.Text = "Export";
            // 
            // lvwReportResult
            // 
            this.lvwReportResult.FullRowSelect = true;
            this.lvwReportResult.GridLines = true;
            this.lvwReportResult.Location = new System.Drawing.Point(6, 45);
            this.lvwReportResult.Name = "lvwReportResult";
            this.lvwReportResult.Size = new System.Drawing.Size(407, 103);
            this.lvwReportResult.TabIndex = 4;
            this.lvwReportResult.UseCompatibleStateImageBehavior = false;
            this.lvwReportResult.View = System.Windows.Forms.View.Details;
            // 
            // fraDateRange
            // 
            this.fraDateRange.Controls.Add(this.cmdChangeRange);
            this.fraDateRange.Controls.Add(this.lblReportPeriod);
            this.fraDateRange.Location = new System.Drawing.Point(14, 38);
            this.fraDateRange.Name = "fraDateRange";
            this.fraDateRange.Size = new System.Drawing.Size(416, 69);
            this.fraDateRange.TabIndex = 2;
            this.fraDateRange.TabStop = false;
            this.fraDateRange.Text = "&Date Range";
            // 
            // cmdChangeRange
            // 
            this.cmdChangeRange.Location = new System.Drawing.Point(262, 23);
            this.cmdChangeRange.Name = "cmdChangeRange";
            this.cmdChangeRange.Size = new System.Drawing.Size(89, 29);
            this.cmdChangeRange.TabIndex = 1;
            this.cmdChangeRange.Text = "C&hange...";
            this.cmdChangeRange.UseVisualStyleBackColor = true;
            this.cmdChangeRange.Click += new System.EventHandler(this.cmdChangeRange_Click);
            // 
            // lblReportPeriod
            // 
            this.lblReportPeriod.AutoSize = true;
            this.lblReportPeriod.Location = new System.Drawing.Point(23, 31);
            this.lblReportPeriod.Name = "lblReportPeriod";
            this.lblReportPeriod.Size = new System.Drawing.Size(217, 13);
            this.lblReportPeriod.TabIndex = 0;
            this.lblReportPeriod.Text = "Reporting From 11/11/1111 To 11/11/1111";
            // 
            // cboReportBy
            // 
            this.cboReportBy.FormattingEnabled = true;
            this.cboReportBy.Location = new System.Drawing.Point(40, 9);
            this.cboReportBy.Name = "cboReportBy";
            this.cboReportBy.Size = new System.Drawing.Size(169, 21);
            this.cboReportBy.TabIndex = 1;
            this.cboReportBy.SelectedIndexChanged += new System.EventHandler(this.cboReportBy_SelectedIndexChanged);
            // 
            // lblReportBy
            // 
            this.lblReportBy.AutoSize = true;
            this.lblReportBy.Location = new System.Drawing.Point(12, 14);
            this.lblReportBy.Name = "lblReportBy";
            this.lblReportBy.Size = new System.Drawing.Size(22, 13);
            this.lblReportBy.TabIndex = 0;
            this.lblReportBy.Text = "By:";
            // 
            // TabPageLogs
            // 
            this.TabPageLogs.Controls.Add(this.cmdPurgeLog);
            this.TabPageLogs.Controls.Add(this.lvwLogData);
            this.TabPageLogs.Location = new System.Drawing.Point(4, 22);
            this.TabPageLogs.Name = "TabPageLogs";
            this.TabPageLogs.Size = new System.Drawing.Size(447, 302);
            this.TabPageLogs.TabIndex = 4;
            this.TabPageLogs.Text = "Logs";
            this.TabPageLogs.UseVisualStyleBackColor = true;
            // 
            // cmdPurgeLog
            // 
            this.cmdPurgeLog.Location = new System.Drawing.Point(340, 257);
            this.cmdPurgeLog.Name = "cmdPurgeLog";
            this.cmdPurgeLog.Size = new System.Drawing.Size(101, 32);
            this.cmdPurgeLog.TabIndex = 1;
            this.cmdPurgeLog.Text = "Purge Log";
            this.cmdPurgeLog.UseVisualStyleBackColor = true;
            this.cmdPurgeLog.Click += new System.EventHandler(this.cmdPurgeLog_Click);
            // 
            // lvwLogData
            // 
            this.lvwLogData.Location = new System.Drawing.Point(8, 4);
            this.lvwLogData.Name = "lvwLogData";
            this.lvwLogData.Size = new System.Drawing.Size(436, 244);
            this.lvwLogData.TabIndex = 0;
            this.lvwLogData.UseCompatibleStateImageBehavior = false;
            this.lvwLogData.View = System.Windows.Forms.View.Details;
            // 
            // tabUpdate
            // 
            this.tabUpdate.Controls.Add(this.cmdDownloadUpdate);
            this.tabUpdate.Controls.Add(this.lblLatestVersion);
            this.tabUpdate.Controls.Add(this.lblInstalledVersion);
            this.tabUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabUpdate.Name = "tabUpdate";
            this.tabUpdate.Size = new System.Drawing.Size(447, 302);
            this.tabUpdate.TabIndex = 5;
            this.tabUpdate.Text = "Update";
            this.tabUpdate.UseVisualStyleBackColor = true;
            // 
            // cmdDownloadUpdate
            // 
            this.cmdDownloadUpdate.Location = new System.Drawing.Point(17, 141);
            this.cmdDownloadUpdate.Name = "cmdDownloadUpdate";
            this.cmdDownloadUpdate.Size = new System.Drawing.Size(99, 36);
            this.cmdDownloadUpdate.TabIndex = 2;
            this.cmdDownloadUpdate.Text = "Download";
            this.cmdDownloadUpdate.UseVisualStyleBackColor = true;
            this.cmdDownloadUpdate.Click += new System.EventHandler(this.cmdDownloadUpdate_Click);
            // 
            // lblLatestVersion
            // 
            this.lblLatestVersion.AutoSize = true;
            this.lblLatestVersion.Location = new System.Drawing.Point(14, 48);
            this.lblLatestVersion.Name = "lblLatestVersion";
            this.lblLatestVersion.Size = new System.Drawing.Size(77, 13);
            this.lblLatestVersion.TabIndex = 1;
            this.lblLatestVersion.Text = "Latest Version:";
            // 
            // lblInstalledVersion
            // 
            this.lblInstalledVersion.AutoSize = true;
            this.lblInstalledVersion.Location = new System.Drawing.Point(14, 20);
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            this.lblInstalledVersion.Size = new System.Drawing.Size(87, 13);
            this.lblInstalledVersion.TabIndex = 0;
            this.lblInstalledVersion.Text = "Installed Version:";
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(368, 342);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(88, 29);
            this.cmdClose.TabIndex = 1;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(357, 367);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(264, 367);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 25);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.Location = new System.Drawing.Point(10, 342);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(86, 26);
            this.cmdRefresh.TabIndex = 4;
            this.cmdRefresh.Text = "&Refresh";
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // frmJobClockAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 380);
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.tabAdminpanel);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmJobClockAdmin";
            this.Text = "JobClock Administration";
            this.Activated += new System.EventHandler(this.frmJobClockAdmin_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmJobClockAdmin_FormClosing);
            this.Load += new System.EventHandler(this.frmJobClockAdmin_Load);
            this.Shown += new System.EventHandler(this.frmJobClockAdmin_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.l);
            this.Resize += new System.EventHandler(this.frmJobClockAdmin_Resize);
            this.tabAdminpanel.ResumeLayout(false);
            this.TabPageGeneral.ResumeLayout(false);
            this.TabPageGeneral.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.TabPageUsers.ResumeLayout(false);
            this.TabPageUsers.PerformLayout();
            this.GroupEditingUser.ResumeLayout(false);
            this.GroupEditingUser.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ToolStripUsers.ResumeLayout(false);
            this.ToolStripUsers.PerformLayout();
            this.TabPageOrders.ResumeLayout(false);
            this.TabPageOrders.PerformLayout();
            this.GroupEditingOrder.ResumeLayout(false);
            this.GroupEditingOrder.PerformLayout();
            this.tStripEditOrder.ResumeLayout(false);
            this.tStripEditOrder.PerformLayout();
            this.tStripOrders.ResumeLayout(false);
            this.tStripOrders.PerformLayout();
            this.TabPageReporting.ResumeLayout(false);
            this.TabPageReporting.PerformLayout();
            this.fraReportResult.ResumeLayout(false);
            this.fraReportResult.PerformLayout();
            this.toolStripReportResults.ResumeLayout(false);
            this.toolStripReportResults.PerformLayout();
            this.fraDateRange.ResumeLayout(false);
            this.fraDateRange.PerformLayout();
            this.TabPageLogs.ResumeLayout(false);
            this.tabUpdate.ResumeLayout(false);
            this.tabUpdate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabAdminpanel;
        private System.Windows.Forms.TabPage TabPageUsers;
        private System.Windows.Forms.ListView lvwUsers;
        private System.Windows.Forms.TabPage TabPageOrders;
        private System.Windows.Forms.ListView lvwOrders;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ToolStrip ToolStripUsers;
        private System.Windows.Forms.ToolStripButton tStripRefreshUsers;
        private System.Windows.Forms.GroupBox GroupEditingUser;
        private System.Windows.Forms.TextBox txtPINCode;
        private System.Windows.Forms.Label lblPIN;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripButton tstripNewUser;
        private System.Windows.Forms.ToolStripButton toolstripdeleteuser;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton DropDownOrders;
        private System.Windows.Forms.ToolStripMenuItem ordersGhostToolStripMenuItem;
        private System.Windows.Forms.GroupBox GroupEditingOrder;
        private System.Windows.Forms.ToolStrip tStripOrders;
        private System.Windows.Forms.ToolStripButton tStripRefreshOrders;
        private System.Windows.Forms.ToolStripButton tStripNewOrder;
        private System.Windows.Forms.ToolStripButton tStripRemoveOrder;
        private System.Windows.Forms.ToolStrip tStripEditOrder;
        private System.Windows.Forms.ToolStripDropDownButton DropDownUsers;
        private System.Windows.Forms.ToolStripMenuItem editGhostToolStripMenuItem;
        private System.Windows.Forms.TabPage TabPageReporting;
        private System.Windows.Forms.TabPage TabPageGeneral;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Button cmdApplyDB;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.Label lblDatabaseName;
        private System.Windows.Forms.TextBox txtAdminConnectionString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtClientConnectionString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdReinitialize;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtOrderID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSingleWorkOrderMode;
        private System.Windows.Forms.CheckBox chkSingleWorkorderMode;
        private System.Windows.Forms.CheckBox chkAutoAddOrders;
        private System.Windows.Forms.TabPage TabPageLogs;
        private System.Windows.Forms.Button cmdPurgeLog;
        private System.Windows.Forms.ListView lvwLogData;
        private System.Windows.Forms.ToolStripButton tStripImportUsers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripButton tStripCopyUsers;
        private System.Windows.Forms.ToolStripButton toolstripcopyorders;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cboReportBy;
        private System.Windows.Forms.Label lblReportBy;
        private System.Windows.Forms.GroupBox fraDateRange;
        private System.Windows.Forms.GroupBox fraReportResult;
        private System.Windows.Forms.ToolStrip toolStripReportResults;
        private System.Windows.Forms.ListView lvwReportResult;
        private System.Windows.Forms.Button cmdReport;
        private System.Windows.Forms.ComboBox cboReportByListing;
        private System.Windows.Forms.Label lblReportByItem;
        private System.Windows.Forms.Label lblReportPeriod;
        private System.Windows.Forms.Button cmdChangeRange;
        private System.Windows.Forms.ToolStripDropDownButton tstripCopyResult;
        private System.Windows.Forms.ToolStripDropDownButton tStripExportResult;
        private System.Windows.Forms.CheckBox chkHidePIN;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.CheckBox chkuserlisting;
        private System.Windows.Forms.TabPage tabUpdate;
        private System.Windows.Forms.Label lblLatestVersion;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.Button cmdDownloadUpdate;
        private System.Windows.Forms.Button cmSchedule;
        private System.Windows.Forms.ToolTip tTools;
        private System.Windows.Forms.Button cmdRefresh;
    }
}

