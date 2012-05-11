namespace PocketMail
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItemSelect = new System.Windows.Forms.MenuItem();
            this.menuItemMenu = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.treeViewMain = new System.Windows.Forms.TreeView();
            this.treeViewMessages = new System.Windows.Forms.TreeView();
            this.menuItemDeleteMsg = new System.Windows.Forms.MenuItem();
            this.menuItemEmptyFolder = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItemSelect);
            this.mainMenu1.MenuItems.Add(this.menuItemMenu);
            // 
            // menuItemSelect
            // 
            this.menuItemSelect.Text = "Select";
            this.menuItemSelect.Click += new System.EventHandler(this.treeViewFolders_AfterSelect);
            // 
            // menuItemMenu
            // 
            this.menuItemMenu.MenuItems.Add(this.menuItemExit);
            this.menuItemMenu.MenuItems.Add(this.menuItemDeleteMsg);
            this.menuItemMenu.MenuItems.Add(this.menuItemEmptyFolder);
            this.menuItemMenu.Text = "Menu";
            this.menuItemMenu.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // treeViewMain
            // 
            this.treeViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMain.Location = new System.Drawing.Point(0, 0);
            this.treeViewMain.Name = "treeViewMain";
            this.treeViewMain.Size = new System.Drawing.Size(176, 180);
            this.treeViewMain.TabIndex = 0;
            // 
            // treeViewMessages
            // 
            this.treeViewMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMessages.Location = new System.Drawing.Point(0, 0);
            this.treeViewMessages.Name = "treeViewMessages";
            this.treeViewMessages.Size = new System.Drawing.Size(176, 180);
            this.treeViewMessages.TabIndex = 1;
            this.treeViewMessages.Visible = false;
            // 
            // menuItemDeleteMsg
            // 
            this.menuItemDeleteMsg.Text = "Delete Message";
            this.menuItemDeleteMsg.Click += new System.EventHandler(this.menuItemDeleteMsg_Click);
            // 
            // menuItemEmptyFolder
            // 
            this.menuItemEmptyFolder.Text = "Empty Folder";
            this.menuItemEmptyFolder.Click += new System.EventHandler(this.menuItemEmptyFolder_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(176, 180);
            this.Controls.Add(this.treeViewMessages);
            this.Controls.Add(this.treeViewMain);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "PocketMail";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItemSelect;
        private System.Windows.Forms.MenuItem menuItemMenu;
        private System.Windows.Forms.TreeView treeViewMain;
        private System.Windows.Forms.TreeView treeViewMessages;
        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItemDeleteMsg;
        private System.Windows.Forms.MenuItem menuItemEmptyFolder;
    }
}

