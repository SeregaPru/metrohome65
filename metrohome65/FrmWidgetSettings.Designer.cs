namespace MetroHome65.Pages
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
            this.cbSize = new System.Windows.Forms.ComboBox();
            this.lblWidgetSize = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lblWidgetType = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuApply = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // cbSize
            // 
            this.cbSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbSize.Location = new System.Drawing.Point(0, 122);
            this.cbSize.Name = "cbSize";
            this.cbSize.Size = new System.Drawing.Size(240, 22);
            this.cbSize.TabIndex = 11;
            // 
            // lblWidgetSize
            // 
            this.lblWidgetSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWidgetSize.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblWidgetSize.ForeColor = System.Drawing.Color.White;
            this.lblWidgetSize.Location = new System.Drawing.Point(0, 102);
            this.lblWidgetSize.Name = "lblWidgetSize";
            this.lblWidgetSize.Size = new System.Drawing.Size(240, 20);
            this.lblWidgetSize.Text = "Size";
            // 
            // cbType
            // 
            this.cbType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbType.Location = new System.Drawing.Point(0, 80);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(240, 22);
            this.cbType.TabIndex = 10;
            // 
            // lblWidgetType
            // 
            this.lblWidgetType.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWidgetType.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblWidgetType.ForeColor = System.Drawing.Color.White;
            this.lblWidgetType.Location = new System.Drawing.Point(0, 60);
            this.lblWidgetType.Name = "lblWidgetType";
            this.lblWidgetType.Size = new System.Drawing.Size(240, 20);
            this.lblWidgetType.Text = "Widget type";
            // 
            // lblColor
            // 
            this.lblColor.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblColor.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblColor.ForeColor = System.Drawing.Color.White;
            this.lblColor.Location = new System.Drawing.Point(0, 144);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(240, 20);
            this.lblColor.Text = "Color";
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPageTitle.Font = new System.Drawing.Font("Tahoma", 32F, System.Drawing.FontStyle.Regular);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(240, 60);
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
            // FrmWidgetSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.ControlBox = false;
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.cbSize);
            this.Controls.Add(this.lblWidgetSize);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.lblWidgetType);
            this.Controls.Add(this.lblPageTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 0);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FrmWidgetSettings";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSize;
        private System.Windows.Forms.Label lblWidgetSize;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lblWidgetType;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem mnuApply;
        private System.Windows.Forms.MenuItem menuCancel;


    }
}