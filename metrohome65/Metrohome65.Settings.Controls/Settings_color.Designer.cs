namespace MetroHome65.Settings.Controls
{
    partial class Settings_color
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
            this.lblColor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelColorSample = new System.Windows.Forms.Panel();
            this.trackRed = new System.Windows.Forms.HScrollBar();
            this.trackGreen = new System.Windows.Forms.HScrollBar();
            this.trackBlue = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // lblColor
            // 
            this.lblColor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lblColor.ForeColor = System.Drawing.Color.White;
            this.lblColor.Location = new System.Drawing.Point(8, 20);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(306, 47);
            this.lblColor.Text = "Color";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(15, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 40);
            this.label3.Text = "B";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Location = new System.Drawing.Point(15, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 39);
            this.label2.Text = "G";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(15, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 42);
            this.label1.Text = "R";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panelColorSample
            // 
            this.panelColorSample.Location = new System.Drawing.Point(369, 75);
            this.panelColorSample.Name = "panelColorSample";
            this.panelColorSample.Size = new System.Drawing.Size(75, 118);
            // 
            // trackRed
            // 
            this.trackRed.Location = new System.Drawing.Point(50, 75);
            this.trackRed.Maximum = 255;
            this.trackRed.Name = "trackRed";
            this.trackRed.Size = new System.Drawing.Size(313, 26);
            this.trackRed.TabIndex = 10;
            this.trackRed.ValueChanged += new System.EventHandler(this.trackBlue_ValueChanged);
            // 
            // trackGreen
            // 
            this.trackGreen.Location = new System.Drawing.Point(50, 121);
            this.trackGreen.Maximum = 255;
            this.trackGreen.Name = "trackGreen";
            this.trackGreen.Size = new System.Drawing.Size(313, 26);
            this.trackGreen.TabIndex = 11;
            this.trackGreen.ValueChanged += new System.EventHandler(this.trackBlue_ValueChanged);
            // 
            // trackBlue
            // 
            this.trackBlue.Location = new System.Drawing.Point(50, 167);
            this.trackBlue.Maximum = 255;
            this.trackBlue.Name = "trackBlue";
            this.trackBlue.Size = new System.Drawing.Size(313, 26);
            this.trackBlue.TabIndex = 12;
            this.trackBlue.ValueChanged += new System.EventHandler(this.trackBlue_ValueChanged);
            // 
            // Settings_color
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.trackBlue);
            this.Controls.Add(this.trackGreen);
            this.Controls.Add(this.trackRed);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelColorSample);
            this.Name = "Settings_color";
            this.Size = new System.Drawing.Size(450, 200);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelColorSample;
        private System.Windows.Forms.HScrollBar trackRed;
        private System.Windows.Forms.HScrollBar trackGreen;
        private System.Windows.Forms.HScrollBar trackBlue;
    }
}
