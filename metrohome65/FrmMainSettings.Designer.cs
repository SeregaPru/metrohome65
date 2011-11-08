namespace MetroHome65
{
    partial class FrmMainSettings
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.ControlBackground = new MetroHome65.Settings.Controls.Settings_image();
            this.menuApply = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuApply);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(324, 104);
            this.lblPageTitle.Text = "Settings";
            // 
            // ControlBackground
            // 
            this.ControlBackground.Location = new System.Drawing.Point(0, 124);
            this.ControlBackground.Name = "ControlBackground";
            this.ControlBackground.Size = new System.Drawing.Size(477, 300);
            this.ControlBackground.TabIndex = 2;
            // 
            // menuApply
            // 
            this.menuApply.Text = "Apply";
            this.menuApply.Click += new System.EventHandler(this.menuApply_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // FrmMainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(480, 748);
            this.ControlBox = false;
            this.Controls.Add(this.ControlBackground);
            this.Controls.Add(this.lblPageTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 0);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FrmMainSettings";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPageTitle;
        private MetroHome65.Settings.Controls.Settings_image ControlBackground;
        private System.Windows.Forms.MenuItem menuApply;
        private System.Windows.Forms.MenuItem menuCancel;
    }
}