using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Core;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines;
using Microsoft.WindowsCE.Forms;
using OpenNETCF.Drawing.Imaging;

namespace Metrohome65.Settings.Controls
{
    public class TextBoxInputPage : FleuxControlPage
    {

        public TextBoxInputPage() : base(true)
        {
            CreateControls();
        }

        private InputPanel _sip;

        private void CreateControls()
        {
            ScreenRoutines.CursorWait();
            try
            {
                //Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                var appBar = new ApplicationBar
                {
                    Size = new Size(Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.ok.bmp"));
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.cancel.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Control.AddElement(appBar/*.AnimateHorizontalEntrance(false)*/);

                var input = new System.Windows.Forms.TextBox()
                                {
                                    Location = new Point(20, 100),
                                    Size = new Size(400, 100),
                                    Text = "12345",
                                };
                this.TheForm.Controls.Add(input);
                input.Parent = this.TheForm;
                input.BringToFront();

                _sip = new InputPanel();
                _sip.EnabledChanged += (sender, args) =>
                                          {
                                              _sip.Enabled = input.Focused; HideIt(); 
                                          };
                //SipShowIM(SIPF_ON);
                //MoveKeyboardDown(25);

                input.Focus();
                _sip.Enabled = true;
                HideIt();

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        ~TextBoxInputPage()
        {
            _sip = null;
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // ok button
            {
                ApplySettings();

                Close();
            }
            else
                // close button
                Close();
        }

        private void ApplySettings()
        {
            //
        }


        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string caption, string className);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hwnd, int state);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        [DllImport("coredll.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private void button1_Click(object sender, EventArgs e)
        {
            HideIt();
        }

        private void HideIt()
        {
            IntPtr hSipWindow = FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");
            if (hSipWindow != IntPtr.Zero)
            {
                IntPtr hSipButton = GetWindow(hSipWindow, 5);
                if (hSipButton != IntPtr.Zero)
                {
                    bool res = ShowWindow(hSipButton, SW_HIDE);
                }
            }
        }

    }
}