namespace SVC_ORACLE
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.gbConnection = new System.Windows.Forms.GroupBox();
            this.txtSchemas = new System.Windows.Forms.TextBox();
            this.lblSchemas = new System.Windows.Forms.Label();
            this.numAutoRefresh = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.btnPath = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.lblSN = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.bnProfiles = new System.Windows.Forms.BindingNavigator(this.components);
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.cbProfiles = new System.Windows.Forms.ToolStripComboBox();
            this.txtUpdated = new System.Windows.Forms.TextBox();
            this.lblUpdated = new System.Windows.Forms.Label();
            this.btnFastRefresh = new System.Windows.Forms.Button();
            this.btnFullRefresh = new System.Windows.Forms.Button();
            this.fbPath = new System.Windows.Forms.FolderBrowserDialog();
            this.pbStatus = new System.Windows.Forms.ProgressBar();
            this.gbConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnProfiles)).BeginInit();
            this.bnProfiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbConnection
            // 
            this.gbConnection.Controls.Add(this.txtSchemas);
            this.gbConnection.Controls.Add(this.lblSchemas);
            this.gbConnection.Controls.Add(this.numAutoRefresh);
            this.gbConnection.Controls.Add(this.label1);
            this.gbConnection.Controls.Add(this.txtPassword);
            this.gbConnection.Controls.Add(this.lblPassword);
            this.gbConnection.Controls.Add(this.txtUser);
            this.gbConnection.Controls.Add(this.lblUser);
            this.gbConnection.Controls.Add(this.txtSN);
            this.gbConnection.Controls.Add(this.btnPath);
            this.gbConnection.Controls.Add(this.lblPath);
            this.gbConnection.Controls.Add(this.lblSN);
            this.gbConnection.Controls.Add(this.txtPort);
            this.gbConnection.Controls.Add(this.lblPort);
            this.gbConnection.Controls.Add(this.txtHost);
            this.gbConnection.Controls.Add(this.lblHost);
            this.gbConnection.Location = new System.Drawing.Point(12, 37);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Size = new System.Drawing.Size(207, 242);
            this.gbConnection.TabIndex = 0;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "Properties";
            // 
            // txtSchemas
            // 
            this.txtSchemas.Location = new System.Drawing.Point(89, 153);
            this.txtSchemas.Name = "txtSchemas";
            this.txtSchemas.Size = new System.Drawing.Size(100, 20);
            this.txtSchemas.TabIndex = 13;
            // 
            // lblSchemas
            // 
            this.lblSchemas.AutoSize = true;
            this.lblSchemas.Location = new System.Drawing.Point(6, 156);
            this.lblSchemas.Name = "lblSchemas";
            this.lblSchemas.Size = new System.Drawing.Size(78, 13);
            this.lblSchemas.TabIndex = 12;
            this.lblSchemas.Text = "Schemas, CSV";
            // 
            // numAutoRefresh
            // 
            this.numAutoRefresh.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAutoRefresh.Location = new System.Drawing.Point(89, 211);
            this.numAutoRefresh.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numAutoRefresh.Name = "numAutoRefresh";
            this.numAutoRefresh.Size = new System.Drawing.Size(100, 20);
            this.numAutoRefresh.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Auto refresh, s";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(89, 127);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(6, 130);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "Password";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(89, 101);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(100, 20);
            this.txtUser.TabIndex = 7;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(6, 104);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(29, 13);
            this.lblUser.TabIndex = 6;
            this.lblUser.Text = "User";
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(89, 75);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(100, 20);
            this.txtSN.TabIndex = 5;
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(114, 182);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(75, 23);
            this.btnPath.TabIndex = 3;
            this.btnPath.Text = "Browse";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(6, 187);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(89, 13);
            this.lblPath.TabIndex = 2;
            this.lblPath.Text = "Destination folder";
            // 
            // lblSN
            // 
            this.lblSN.AutoSize = true;
            this.lblSN.Location = new System.Drawing.Point(6, 78);
            this.lblSN.Name = "lblSN";
            this.lblSN.Size = new System.Drawing.Size(72, 13);
            this.lblSN.TabIndex = 4;
            this.lblSN.Text = "Service name";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(89, 49);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.TabIndex = 3;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 52);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(89, 23);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(100, 20);
            this.txtHost.TabIndex = 1;
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(6, 26);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(29, 13);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host";
            // 
            // bnProfiles
            // 
            this.bnProfiles.AddNewItem = this.btnAdd;
            this.bnProfiles.CountItem = null;
            this.bnProfiles.DeleteItem = this.btnDelete;
            this.bnProfiles.Dock = System.Windows.Forms.DockStyle.None;
            this.bnProfiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDelete,
            this.btnSave,
            this.cbProfiles});
            this.bnProfiles.Location = new System.Drawing.Point(15, 9);
            this.bnProfiles.MoveFirstItem = null;
            this.bnProfiles.MoveLastItem = null;
            this.bnProfiles.MoveNextItem = null;
            this.bnProfiles.MovePreviousItem = null;
            this.bnProfiles.Name = "bnProfiles";
            this.bnProfiles.PositionItem = null;
            this.bnProfiles.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.bnProfiles.Size = new System.Drawing.Size(204, 25);
            this.bnProfiles.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.RightToLeftAutoMirrorImage = true;
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Text = "Add new";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.RightToLeftAutoMirrorImage = true;
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbProfiles
            // 
            this.cbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfiles.Name = "cbProfiles";
            this.cbProfiles.Size = new System.Drawing.Size(121, 25);
            this.cbProfiles.SelectedIndexChanged += new System.EventHandler(this.cbProfiles_SelectedIndexChanged);
            // 
            // txtUpdated
            // 
            this.txtUpdated.Location = new System.Drawing.Point(84, 285);
            this.txtUpdated.Name = "txtUpdated";
            this.txtUpdated.ReadOnly = true;
            this.txtUpdated.Size = new System.Drawing.Size(117, 20);
            this.txtUpdated.TabIndex = 5;
            // 
            // lblUpdated
            // 
            this.lblUpdated.AutoSize = true;
            this.lblUpdated.Location = new System.Drawing.Point(18, 288);
            this.lblUpdated.Name = "lblUpdated";
            this.lblUpdated.Size = new System.Drawing.Size(66, 13);
            this.lblUpdated.TabIndex = 4;
            this.lblUpdated.Text = "Last update:";
            // 
            // btnFastRefresh
            // 
            this.btnFastRefresh.Location = new System.Drawing.Point(21, 340);
            this.btnFastRefresh.Name = "btnFastRefresh";
            this.btnFastRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnFastRefresh.TabIndex = 7;
            this.btnFastRefresh.Text = "Fast Refresh";
            this.btnFastRefresh.UseVisualStyleBackColor = true;
            this.btnFastRefresh.Click += new System.EventHandler(this.btnFastRefresh_Click);
            // 
            // btnFullRefresh
            // 
            this.btnFullRefresh.Location = new System.Drawing.Point(126, 340);
            this.btnFullRefresh.Name = "btnFullRefresh";
            this.btnFullRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnFullRefresh.TabIndex = 8;
            this.btnFullRefresh.Text = "Full Refresh";
            this.btnFullRefresh.UseVisualStyleBackColor = true;
            this.btnFullRefresh.Click += new System.EventHandler(this.btnFullRefresh_Click);
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(21, 307);
            this.pbStatus.Maximum = 1;
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(180, 23);
            this.pbStatus.Step = 1;
            this.pbStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbStatus.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 376);
            this.Controls.Add(this.btnFullRefresh);
            this.Controls.Add(this.btnFastRefresh);
            this.Controls.Add(this.pbStatus);
            this.Controls.Add(this.txtUpdated);
            this.Controls.Add(this.lblUpdated);
            this.Controls.Add(this.bnProfiles);
            this.Controls.Add(this.gbConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Oracle source exporter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnProfiles)).EndInit();
            this.bnProfiles.ResumeLayout(false);
            this.bnProfiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConnection;
        private System.Windows.Forms.NumericUpDown numAutoRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblSN;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.BindingNavigator bnProfiles;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripComboBox cbProfiles;
        private System.Windows.Forms.TextBox txtUpdated;
        private System.Windows.Forms.Label lblUpdated;
        private System.Windows.Forms.Button btnFastRefresh;
        private System.Windows.Forms.Button btnFullRefresh;
        private System.Windows.Forms.FolderBrowserDialog fbPath;
        private System.Windows.Forms.TextBox txtSchemas;
        private System.Windows.Forms.Label lblSchemas;
        private System.Windows.Forms.ProgressBar pbStatus;
    }
}

