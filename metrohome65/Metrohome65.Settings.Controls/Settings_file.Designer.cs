namespace MetroHome65.Settings.Controls
{
    partial class Settings_file
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.txtCommandLine = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.DarkGray;
            this.buttonBrowse.Location = new System.Drawing.Point(308, 67);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(105, 37);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Location = new System.Drawing.Point(8, 20);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(436, 47);
            this.lblCaption.Text = "<File parameter>";
            // 
            // txtCommandLine
            // 
            this.txtCommandLine.BackColor = System.Drawing.Color.LightGray;
            this.txtCommandLine.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.txtCommandLine.Location = new System.Drawing.Point(8, 110);
            this.txtCommandLine.Multiline = true;
            this.txtCommandLine.Name = "txtCommandLine";
            this.txtCommandLine.Size = new System.Drawing.Size(440, 113);
            this.txtCommandLine.TabIndex = 0;
            this.txtCommandLine.Text = "<empty>";
            this.txtCommandLine.TextChanged += new System.EventHandler(this.txtCommandLine_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Executable files|*.exe;*.lnk";
            this.openFileDialog1.InitialDirectory = "/";
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.Gray;
            this.buttonClear.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.buttonClear.Location = new System.Drawing.Point(416, 67);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(32, 37);
            this.buttonClear.TabIndex = 8;
            this.buttonClear.TabStop = false;
            this.buttonClear.Text = "x";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Settings_file
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.txtCommandLine);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.lblCaption);
            this.Name = "Settings_file";
            this.Size = new System.Drawing.Size(450, 226);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.TextBox txtCommandLine;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonClear;
    }
}
