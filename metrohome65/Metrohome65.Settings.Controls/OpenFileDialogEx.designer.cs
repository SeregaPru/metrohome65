//-----------------------------------------------------------------------
// 
//  Copyright (C) MOBILE PRACTICES.  All rights reserved.
//  http://www.mobilepractices.com/
//
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//-----------------------------------------------------------------------

namespace MobilePractices.OpenFileDialogEx
{
    partial class OpenFileDialogEx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenFileDialogEx));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.fileListView = new System.Windows.Forms.ListView();
            this.FileSystemIcons = new System.Windows.Forms.ImageList();
            this.label1 = new System.Windows.Forms.Label();
            this.PathSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Up";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Cancel";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.LightGray;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 655);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(480, 41);
            this.textBox1.TabIndex = 3;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // fileListView
            // 
            this.fileListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.fileListView.BackColor = System.Drawing.Color.Black;
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.fileListView.ForeColor = System.Drawing.Color.White;
            this.fileListView.Location = new System.Drawing.Point(0, 48);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(480, 568);
            this.fileListView.SmallImageList = this.FileSystemIcons;
            this.fileListView.TabIndex = 4;
            this.fileListView.View = System.Windows.Forms.View.List;
            this.fileListView.ItemActivate += new System.EventHandler(this.fileListView_ItemActivate);
            // 
            // FileSystemIcons
            // 
            this.FileSystemIcons.ImageSize = new System.Drawing.Size(32, 32);
            this.FileSystemIcons.Images.Clear();
            this.FileSystemIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.FileSystemIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            this.FileSystemIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.FileSystemIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 616);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 39);
            this.label1.Text = "File name:";
            // 
            // PathSelectorComboBox
            // 
            this.PathSelectorComboBox.BackColor = System.Drawing.Color.LightGray;
            this.PathSelectorComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PathSelectorComboBox.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.PathSelectorComboBox.ForeColor = System.Drawing.Color.Black;
            this.PathSelectorComboBox.Location = new System.Drawing.Point(0, 0);
            this.PathSelectorComboBox.Name = "PathSelectorComboBox";
            this.PathSelectorComboBox.Size = new System.Drawing.Size(480, 48);
            this.PathSelectorComboBox.TabIndex = 6;
            this.PathSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.PathSelectorComboBox_SelectedIndexChanged);
            // 
            // OpenFileDialogEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(480, 696);
            this.ControlBox = false;
            this.Controls.Add(this.fileListView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.PathSelectorComboBox);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "OpenFileDialogEx";
            this.Text = "Select File";
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ListView fileListView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox PathSelectorComboBox;
		private System.Windows.Forms.ImageList FileSystemIcons;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
    }
}