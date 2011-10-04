namespace MetroHome65.Settings.Controls
{
    partial class Settings_string
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
            this.lblCaption = new System.Windows.Forms.Label();
            this.textValue = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Location = new System.Drawing.Point(8, 20);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(436, 47);
            this.lblCaption.Text = "<String parameter>";
            // 
            // textValue
            // 
            this.textValue.BackColor = System.Drawing.Color.LightGray;
            this.textValue.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.textValue.Location = new System.Drawing.Point(8, 68);
            this.textValue.Name = "textValue";
            this.textValue.Size = new System.Drawing.Size(391, 51);
            this.textValue.TabIndex = 2;
            this.textValue.TextChanged += new System.EventHandler(this.textValue_TextChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.Gray;
            this.buttonClear.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.buttonClear.Location = new System.Drawing.Point(399, 68);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(45, 51);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "x";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Settings_string
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.textValue);
            this.Controls.Add(this.lblCaption);
            this.Name = "Settings_string";
            this.Size = new System.Drawing.Size(450, 125);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.TextBox textValue;
        private System.Windows.Forms.Button buttonClear;
    }
}
