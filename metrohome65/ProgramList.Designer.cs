namespace MetroHome65.Pages
{
    partial class ProgramList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgramList));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem();
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem();
            this.buttonBack = new System.Windows.Forms.PictureBox();
            this.lvApps = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ilAppIcons = new System.Windows.Forms.ImageList();
            this.gestureRecognizer = new Microsoft.WindowsMobile.Gestures.GestureRecognizer();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Image = ((System.Drawing.Image)(resources.GetObject("buttonBack.Image")));
            this.buttonBack.Location = new System.Drawing.Point(19, 6);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(45, 45);
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // lvApps
            // 
            this.lvApps.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvApps.BackColor = System.Drawing.Color.Black;
            this.lvApps.Columns.Add(this.columnHeader1);
            this.lvApps.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lvApps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem1.BackColor = System.Drawing.Color.Black;
            listViewItem1.ForeColor = System.Drawing.Color.White;
            listViewItem1.Text = "item";
            listViewItem2.BackColor = System.Drawing.Color.Black;
            listViewItem2.ForeColor = System.Drawing.Color.White;
            listViewItem2.Text = "item";
            this.lvApps.Items.Add(listViewItem1);
            this.lvApps.Items.Add(listViewItem2);
            this.lvApps.Location = new System.Drawing.Point(82, 0);
            this.lvApps.Name = "lvApps";
            this.lvApps.Size = new System.Drawing.Size(460, 679);
            this.lvApps.SmallImageList = this.ilAppIcons;
            this.lvApps.TabIndex = 1;
            this.lvApps.View = System.Windows.Forms.View.Details;
            this.lvApps.SelectedIndexChanged += new System.EventHandler(this.lvApps_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ColumnHeader";
            this.columnHeader1.Width = 395;
            // 
            // ilAppIcons
            // 
            this.ilAppIcons.ImageSize = new System.Drawing.Size(88, 88);
            // 
            // gestureRecognizer
            // 
            this.gestureRecognizer.TargetControl = this.lvApps;
            this.gestureRecognizer.Hold += new System.EventHandler<Microsoft.WindowsMobile.Gestures.GestureEventArgs>(this.gestureRecognizer_Hold);
            // 
            // ProgramList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.lvApps);
            this.Controls.Add(this.buttonBack);
            this.Name = "ProgramList";
            this.Size = new System.Drawing.Size(480, 679);
            this.Resize += new System.EventHandler(this.ProgramList_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox buttonBack;
        private System.Windows.Forms.ListView lvApps;
        private System.Windows.Forms.ImageList ilAppIcons;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Microsoft.WindowsMobile.Gestures.GestureRecognizer gestureRecognizer;
    }
}
