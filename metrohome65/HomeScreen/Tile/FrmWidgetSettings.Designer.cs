namespace MetroHome65.HomeScreen.Tile
{
    partial class FrmWidgetSettings
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuApply = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.panelSize = new System.Windows.Forms.Panel();
            this.cbSize = new System.Windows.Forms.ComboBox();
            this.lblWidgetSize = new System.Windows.Forms.Label();
            this.panelType = new System.Windows.Forms.Panel();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lblWidgetType = new System.Windows.Forms.Label();
            this.panelSize.SuspendLayout();
            this.panelType.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe WP", 28F, System.Drawing.FontStyle.Regular);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(324, 104);
            this.lblPageTitle.Text = "Settings";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuApply);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // mnuApply
            // 
            this.mnuApply.Text = "Apply";
            this.mnuApply.Click += new System.EventHandler(this.mnuApply_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // panelSize
            // 
            this.panelSize.BackColor = System.Drawing.Color.Black;
            this.panelSize.Controls.Add(this.cbSize);
            this.panelSize.Controls.Add(this.lblWidgetSize);
            this.panelSize.Location = new System.Drawing.Point(0, 234);
            this.panelSize.Name = "panelSize";
            this.panelSize.Size = new System.Drawing.Size(450, 78);
            // 
            // cbSize
            // 
            this.cbSize.BackColor = System.Drawing.Color.LightGray;
            this.cbSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.cbSize.Location = new System.Drawing.Point(99, 20);
            this.cbSize.Name = "cbSize";
            this.cbSize.Size = new System.Drawing.Size(260, 51);
            this.cbSize.TabIndex = 13;
            // 
            // lblWidgetSize
            // 
            this.lblWidgetSize.Font = new System.Drawing.Font("Segoe WP", 12F, System.Drawing.FontStyle.Regular);
            this.lblWidgetSize.ForeColor = System.Drawing.Color.White;
            this.lblWidgetSize.Location = new System.Drawing.Point(8, 20);
            this.lblWidgetSize.Name = "lblWidgetSize";
            this.lblWidgetSize.Size = new System.Drawing.Size(435, 51);
            this.lblWidgetSize.Text = "Size";
            // 
            // panelType
            // 
            this.panelType.BackColor = System.Drawing.Color.Black;
            this.panelType.Controls.Add(this.cbType);
            this.panelType.Controls.Add(this.lblWidgetType);
            this.panelType.Location = new System.Drawing.Point(0, 104);
            this.panelType.Name = "panelType";
            this.panelType.Size = new System.Drawing.Size(450, 130);
            // 
            // cbType
            // 
            this.cbType.BackColor = System.Drawing.Color.LightGray;
            this.cbType.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.cbType.Location = new System.Drawing.Point(8, 75);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(440, 51);
            this.cbType.TabIndex = 12;
            this.cbType.SelectedValueChanged += new System.EventHandler(this.cbType_SelectedValueChanged);
            // 
            // lblWidgetType
            // 
            this.lblWidgetType.Font = new System.Drawing.Font("Segoe WP", 12F, System.Drawing.FontStyle.Regular);
            this.lblWidgetType.ForeColor = System.Drawing.Color.White;
            this.lblWidgetType.Location = new System.Drawing.Point(8, 20);
            this.lblWidgetType.Name = "lblWidgetType";
            this.lblWidgetType.Size = new System.Drawing.Size(435, 52);
            this.lblWidgetType.Text = "Tile type";
            // 
            // FrmWidgetSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(480, 748);
            this.ControlBox = false;
            this.Controls.Add(this.panelSize);
            this.Controls.Add(this.panelType);
            this.Controls.Add(this.lblPageTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 0);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FrmWidgetSettings";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelSize.ResumeLayout(false);
            this.panelType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem mnuApply;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.Panel panelSize;
        private System.Windows.Forms.ComboBox cbSize;
        private System.Windows.Forms.Label lblWidgetSize;
        private System.Windows.Forms.Panel panelType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lblWidgetType;


    }
}