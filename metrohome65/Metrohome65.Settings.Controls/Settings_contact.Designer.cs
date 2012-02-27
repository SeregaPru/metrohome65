namespace MetroHome65.Settings.Controls
{
    partial class Settings_contact
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
            this.labelContactName = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.Gray;
            this.buttonClear.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.buttonClear.Location = new System.Drawing.Point(416, 67);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(32, 37);
            this.buttonClear.TabIndex = 9;
            this.buttonClear.TabStop = false;
            this.buttonClear.Text = "x";
            // 
            // lblCaption
            // 
            this.lblCaption.Font = new System.Drawing.Font("Segoe WP", 12F, System.Drawing.FontStyle.Regular);
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Location = new System.Drawing.Point(8, 20);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(436, 47);
            this.lblCaption.Text = "Contact";
            // 
            // labelContactName
            // 
            this.labelContactName.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.labelContactName.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.labelContactName.Location = new System.Drawing.Point(8, 67);
            this.labelContactName.Name = "labelContactName";
            this.labelContactName.Size = new System.Drawing.Size(304, 37);
            this.labelContactName.Text = "<empty>";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.DarkGray;
            this.buttonBrowse.Location = new System.Drawing.Point(308, 67);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(105, 37);
            this.buttonBrowse.TabIndex = 13;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // Settings_contact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.labelContactName);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.lblCaption);
            this.Name = "Settings_contact";
            this.Size = new System.Drawing.Size(450, 110);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label labelContactName;
        private System.Windows.Forms.Button buttonBrowse;
    }
}
