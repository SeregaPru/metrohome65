using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Gestures;
using MetroHome65.Pages;

namespace MetroHome65.Main
{
    public partial class MainForm : Form, IHost
      {

        private IPageControl _PageControl = null;
        private List<IPageControl> _Pages = new List<IPageControl>();

        public MainForm()
        {
            InitializeComponent();

            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Top;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WidgetGrid WidgetGrid = new MetroHome65.Pages.WidgetGrid();
            AddPage(WidgetGrid);
            AddPage(new MetroHome65.Pages.ProgramList(WidgetGrid));

            SetPageControl(_Pages[0]);
        }

        private void AddPage(IPageControl Page)
        {
            _Pages.Add(Page);
        }


        /// <summary>
        /// When app activated, switch on widgets update,
        /// and bring focus to current control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (this._PageControl != null)
            {
                this._PageControl.Active = true;
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (this._PageControl != null)
                this._PageControl.Active = false;
        }

        private void SetPageControl(IPageControl APageControl)
        {
            // unlink previous control
            if (this._PageControl != null)
            {
                this._PageControl.Active = false;
                this.Controls.Remove(this._PageControl.GetControl());
            }

            this._PageControl = APageControl;

            if (this._PageControl != null)
            {
                this._PageControl.GetControl().Location = new Point(0, 0);
                this._PageControl.GetControl().Size = new Size(this.Width, this.Height);
                this._PageControl.SetHost(this);
                this._PageControl.SetBackColor(this.BackColor);

                this.Controls.Add(this._PageControl.GetControl());

                this._PageControl.Active = true;
            }
        }

        /// <summary>
        /// Change active page to next or previous
        /// </summary>
        /// <param name="Next">if True change to Next page, else to previous</param>
        public void ChangePage(bool Next)
        {
            int CurIndex = _Pages.IndexOf(this._PageControl);

            if (Next)
                CurIndex++;
            else
                CurIndex--;
            if (CurIndex < 0)
                CurIndex = 0;
            if (CurIndex > _Pages.Count - 1)
                CurIndex = _Pages.Count - 1;

            SetPageControl(_Pages[CurIndex]);
        }

    }
}