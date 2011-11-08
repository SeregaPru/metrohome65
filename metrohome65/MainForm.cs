using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Status;
using Microsoft.WindowsMobile.Gestures;
using MetroHome65.Pages;

namespace MetroHome65.Main
{
    public partial class MainForm : Form, IHost
      {

        private IPageControl _ActivePage = null;
        private List<IPageControl> _Pages = new List<IPageControl>();

        // system state for receiving notifications about system events
        private SystemState _SystemState = new SystemState(0);

        public MainForm()
        {
            InitializeComponent();

            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Top;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            tcPages.Width = this.Width;
            tcPages.Height = this.Height + 50;

            _SystemState.Changed += new ChangeEventHandler(OnSystemStateChanged);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WidgetGrid WidgetGrid = new MetroHome65.Pages.WidgetGrid();
            AddPage(WidgetGrid);
            AddPage(new MetroHome65.Pages.ProgramList(WidgetGrid));

            tcPages.SelectedIndex = 0;
        }

        private void AddPage(IPageControl Page)
        {
            TabPage newPage = new TabPage()
            {
                BackColor = Color.Black
            };

            newPage.Controls.Add(Page.GetControl());
            tcPages.TabPages.Add(newPage);

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
            if (this._ActivePage != null)
                this._ActivePage.Active = true;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (this._ActivePage != null)
                this._ActivePage.Active = false;
        }

        /// <summary>
        /// Change active page to next or previous
        /// </summary>
        /// <param name="Next">if True change to Next page, else to previous</param>
        public void ChangePage(bool Next)
        {
            int CurIndex = tcPages.SelectedIndex;

            if (Next)
                CurIndex++;
            else
                CurIndex--;
            if (CurIndex < 0)
                CurIndex = 0;
            if (CurIndex > _Pages.Count - 1)
                CurIndex = _Pages.Count - 1;

            tcPages.SelectedIndex = CurIndex;
        }

        private void tcPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // unlink previous control
            if (this._ActivePage != null)
            {
                this._ActivePage.Active = false;
            }

            this._ActivePage = _Pages[tcPages.SelectedIndex];

            if (this._ActivePage != null)
            {
                this._ActivePage.GetControl().Location = new Point(0, 0);
                this._ActivePage.GetControl().Size = new Size(this.Width, this.Height);
                this._ActivePage.SetHost(this);
                this._ActivePage.SetBackColor(this.BackColor);

                this._ActivePage.Active = true;
            }
        }

        // handler for system state change event
        private void OnSystemStateChanged(object sender, ChangeEventArgs EventArgs)
        {
            string str = SystemState.ActiveApplication;
            if (str.Length > 6)
                str = str.Substring(str.Length - 7, 7);
            if (str.ToLower() == "desktop")
                this.Activate();
        }

    }
}