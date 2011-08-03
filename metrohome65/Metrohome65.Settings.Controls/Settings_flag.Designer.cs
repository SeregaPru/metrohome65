namespace MetroHome65.Settings.Controls
{
    partial class Settings_flag
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
            this.cbFlag = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbFlag
            // 
            this.cbFlag.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.cbFlag.ForeColor = System.Drawing.Color.White;
            this.cbFlag.Location = new System.Drawing.Point(8, 20);
            this.cbFlag.Name = "cbFlag";
            this.cbFlag.Size = new System.Drawing.Size(436, 51);
            this.cbFlag.TabIndex = 5;
            this.cbFlag.Text = "<Flag parameter>";
            this.cbFlag.CheckStateChanged += new System.EventHandler(this.cbFlag_CheckStateChanged);
            // 
            // Settings_flag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.cbFlag);
            this.Name = "Settings_flag";
            this.Size = new System.Drawing.Size(450, 78);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbFlag;
    }
}
