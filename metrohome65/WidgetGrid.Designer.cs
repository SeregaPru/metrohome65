    namespace MetroHome65.Pages
{
    partial class WidgetGrid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WidgetGrid));
            this._WidgetsContainer = new MetroHome65.Routines.TransparentPanel();
            this._WidgetsImage = new MetroHome65.Routines.TransparentPictureBox();
            this.buttonNextPage = new MetroHome65.Routines.TransparentPictureBox("MetroHome65.next.png", this.GetType().Assembly);
            this.gestureRecognizer = new Microsoft.WindowsMobile.Gestures.GestureRecognizer();
            this.physics = new Microsoft.WindowsMobile.Gestures.PhysicsEngine();
            this._mnuWidgetActions = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuMain = new System.Windows.Forms.ContextMenu();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.buttonUnpin = new System.Windows.Forms.PictureBox();
            this.buttonSettings = new System.Windows.Forms.PictureBox();
            this.panelButtons = new MetroHome65.Routines.TransparentPanel();
            this._WidgetsContainer.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // _WidgetsContainer
            // 
            this._WidgetsContainer.BackColor = System.Drawing.Color.Black;
            this._WidgetsContainer.Controls.Add(this._WidgetsImage);
            this._WidgetsContainer.Location = new System.Drawing.Point(30, 6);
            this._WidgetsContainer.Name = "_WidgetsContainer";
            this._WidgetsContainer.Size = new System.Drawing.Size(302, 336);
            // 
            // _WidgetsImage
            // 
            this._WidgetsImage.Location = new System.Drawing.Point(0, 0);
            this._WidgetsImage.Name = "_WidgetsImage";
            this._WidgetsImage.Size = new System.Drawing.Size(200, 100);
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.BackColor = System.Drawing.Color.Maroon;
            this.buttonNextPage.Location = new System.Drawing.Point(0, 0);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(48, 48);
            this.buttonNextPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // gestureRecognizer
            // 
            this.gestureRecognizer.TargetControl = this;
            this.gestureRecognizer.Hold += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Hold);
            this.gestureRecognizer.End += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_End);
            this.gestureRecognizer.DoubleSelect += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_DoubleSelect);
            this.gestureRecognizer.Select += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer1_Select);
            this.gestureRecognizer.Begin += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Begin);
            this.gestureRecognizer.Pan += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Pan);
            this.gestureRecognizer.Scroll += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureScrollEventArgs>(this.gestureRecognizer_Scroll);
            // 
            // physics
            // 
            this.physics.AnimateXAxis = false;
            this.physics.ExtentControl = null;
            this.physics.ViewportControl = null;
            this.physics.AnimateFrame += new System.EventHandler<Microsoft.WindowsMobile.Gestures.PhysicsAnimationFrameEventArgs>(this.physics_AnimateFrame);
            // 
            // _mnuWidgetActions
            // 
            this._mnuWidgetActions.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "";
            // 
            // mnuMain
            // 
            this.mnuMain.MenuItems.Add(this.mnuExit);
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // buttonUnpin
            // 
            this.buttonUnpin.Image = ((System.Drawing.Image)(resources.GetObject("buttonUnpin.Image")));
            this.buttonUnpin.Location = new System.Drawing.Point(0, 114);
            this.buttonUnpin.Name = "buttonUnpin";
            this.buttonUnpin.Size = new System.Drawing.Size(48, 48);
            this.buttonUnpin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.buttonUnpin.Visible = false;
            this.buttonUnpin.Click += new System.EventHandler(this.buttonUnpin_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Image = ((System.Drawing.Image)(resources.GetObject("buttonSettings.Image")));
            this.buttonSettings.Location = new System.Drawing.Point(0, 179);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(48, 48);
            this.buttonSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.buttonSettings.Visible = false;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.Black;
            this.panelButtons.Controls.Add(this.buttonSettings);
            this.panelButtons.Controls.Add(this.buttonNextPage);
            this.panelButtons.Controls.Add(this.buttonUnpin);
            this.panelButtons.Location = new System.Drawing.Point(343, 6);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(48, 237);
            // 
            // WidgetGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this._WidgetsContainer);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "WidgetGrid";
            this.Size = new System.Drawing.Size(402, 402);
            this.Resize += new System.EventHandler(this.WidgetGrid_Resize);
            this._WidgetsContainer.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroHome65.Routines.TransparentPanel _WidgetsContainer;
        private MetroHome65.Routines.TransparentPictureBox _WidgetsImage;
        private MetroHome65.Routines.TransparentPictureBox buttonNextPage;
        private Microsoft.WindowsMobile.Gestures.GestureRecognizer gestureRecognizer;
        private Microsoft.WindowsMobile.Gestures.PhysicsEngine physics;
        private System.Windows.Forms.ContextMenu _mnuWidgetActions;
        private System.Windows.Forms.ContextMenu mnuMain;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.PictureBox buttonSettings;
        private System.Windows.Forms.PictureBox buttonUnpin;
        private MetroHome65.Routines.TransparentPanel panelButtons;
        private System.Windows.Forms.MenuItem menuItem1;
    }
}
