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
            this.physics = new Microsoft.WindowsMobile.Gestures.PhysicsEngine();
            this.SuspendLayout();
            // 
            // gestureRecognizer
            // 
            this.gestureRecognizer.Hold += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Hold);
            this.gestureRecognizer.End += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_End);
            this.gestureRecognizer.Select += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer1_Select);
            this.gestureRecognizer.Begin += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Begin);
            this.gestureRecognizer.Pan += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Pan);
            this.gestureRecognizer.Scroll += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureScrollEventArgs>(this.gestureRecognizer_Scroll);
            // 
            // physics
            // 
            this.physics.AnimateXAxis = false;
            this.physics.ViewportControl = null;
            this.physics.AnimateFrame += new System.EventHandler<Microsoft.WindowsMobile.Gestures.PhysicsAnimationFrameEventArgs>(this.physics_AnimateFrame);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.WindowsMobile.Gestures.GestureRecognizer gestureRecognizer;
        private Microsoft.WindowsMobile.Gestures.PhysicsEngine physics;


    }
}

