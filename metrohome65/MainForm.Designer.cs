namespace SmartDeviceProject1
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.gestureRecognizer = new Microsoft.WindowsMobile.Gestures.GestureRecognizer();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.SuspendLayout();
            // 
            // gestureRecognizer
            // 
            this.gestureRecognizer.TargetControl = this;
            this.gestureRecognizer.Hold += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Hold);
            this.gestureRecognizer.Select += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer1_Select);
            this.gestureRecognizer.Scroll += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureScrollEventArgs>(this.gestureRecognizer_Scroll);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            resources.ApplyResources(this, "$this");
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu1;
        private Microsoft.WindowsMobile.Gestures.GestureRecognizer gestureRecognizer;


    }
}

