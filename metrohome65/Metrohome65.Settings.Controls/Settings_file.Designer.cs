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
            this.buttonClear = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.labelAppName = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.DarkGray;
            this.buttonClear.Location = new System.Drawing.Point(343, 67);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(105, 37);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.Text = "Browse";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
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
            // labelAppName
            // 
            this.labelAppName.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.labelAppName.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.labelAppName.Location = new System.Drawing.Point(8, 67);
            this.labelAppName.Name = "labelAppName";
            this.labelAppName.Size = new System.Drawing.Size(327, 37);
            this.labelAppName.Text = "<empty>";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Executable files|*.exe;*.lnk";
            this.openFileDialog1.InitialDirectory = "/";
            // 
            // Settings_file
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.labelAppName);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.lblCaption);
            this.Name = "Settings_file";
            this.Size = new System.Drawing.Size(450, 110);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label labelAppName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
