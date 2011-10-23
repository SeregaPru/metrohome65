namespace MetroHome65.Settings.Controls
{
    partial class Settings_image
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
            this.buttonSelectBG = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.pictureBoxBG = new System.Windows.Forms.PictureBox();
            this.panelBorder = new System.Windows.Forms.Panel();
            this.panelBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Location = new System.Drawing.Point(8, 20);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(436, 47);
            this.lblCaption.Text = "<Image parameter>";
            // 
            // buttonSelectBG
            // 
            this.buttonSelectBG.BackColor = System.Drawing.Color.DarkGray;
            this.buttonSelectBG.Location = new System.Drawing.Point(308, 67);
            this.buttonSelectBG.Name = "buttonSelectBG";
            this.buttonSelectBG.Size = new System.Drawing.Size(105, 37);
            this.buttonSelectBG.TabIndex = 4;
            this.buttonSelectBG.TabStop = false;
            this.buttonSelectBG.Text = "Browse";
            this.buttonSelectBG.Click += new System.EventHandler(this.buttonSelectBG_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.Gray;
            this.buttonClear.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.buttonClear.Location = new System.Drawing.Point(416, 67);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(32, 37);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.TabStop = false;
            this.buttonClear.Text = "x";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // pictureBoxBG
            // 
            this.pictureBoxBG.Location = new System.Drawing.Point(7, 6);
            this.pictureBoxBG.Name = "pictureBoxBG";
            this.pictureBoxBG.Size = new System.Drawing.Size(427, 173);
            // 
            // panelBorder
            // 
            this.panelBorder.BackColor = System.Drawing.Color.Black;
            this.panelBorder.Controls.Add(this.pictureBoxBG);
            this.panelBorder.Location = new System.Drawing.Point(8, 110);
            this.panelBorder.Name = "panelBorder";
            this.panelBorder.Size = new System.Drawing.Size(440, 184);
            this.panelBorder.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder_Paint);
            // 
            // Settings_image
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panelBorder);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.buttonSelectBG);
            this.Name = "Settings_image";
            this.Size = new System.Drawing.Size(450, 300);
            this.panelBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Button buttonSelectBG;
        private System.Windows.Forms.PictureBox pictureBoxBG;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Panel panelBorder;
    }
}
