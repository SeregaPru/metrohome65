namespace Metrohome65.Settings.Controls
{
    partial class TextInputForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu;

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
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.menuOk = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.textBox = new System.Windows.Forms.TextBox();
            this.inputPanel = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuOk);
            this.mainMenu.MenuItems.Add(this.menuCancel);
            // 
            // menuOk
            // 
            this.menuOk.Text = "Ok";
            this.menuOk.Click += new System.EventHandler(this.MenuOkClick);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.MenuCancelClick);
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.White;
            this.textBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.textBox.ForeColor = System.Drawing.Color.Black;
            this.textBox.Location = new System.Drawing.Point(17, 24);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(446, 140);
            this.textBox.TabIndex = 4;
            this.textBox.GotFocus += new System.EventHandler(this.TextBoxGotFocus);
            // 
            // inputPanel
            // 
            this.inputPanel.EnabledChanged += new System.EventHandler(this.InputPanelEnabledChanged);
            // 
            // TextInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 696);
            this.ControlBox = false;
            this.Controls.Add(this.textBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu;
            this.MinimizeBox = false;
            this.Name = "TextInputForm";
            this.Text = "Input Text";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuOk;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.TextBox textBox;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel;
    }
}