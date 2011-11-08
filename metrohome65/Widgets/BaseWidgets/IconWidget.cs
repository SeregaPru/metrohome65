using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MetroHome65.Routines;
using MetroHome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Class for transparent widget with icon and caption.
    /// </summary>
    public abstract class IconWidget : TransparentWidget
    {
        //Эти переменные понядобятся для загрузки изображений при запуске приложения.
        private String _Caption = "";
        private String _IconPath = "";
        private AlphaImage _BgImage = null;

        protected static int CaptionLeftOffset = ScreenRoutines.Scale(10);
        protected static int CaptionBottomOffset = ScreenRoutines.Scale(4);
        protected static int CaptionSize = ScreenRoutines.Scale(28); // approx caption height in px. 

        /// <summary>
        /// user defined caption for widget
        /// </summary>
        [WidgetParameter]
        public String Caption { 
            get { return _Caption; } 
            set {
                if (_Caption != value)
                {
                    _Caption = value;
                    NotifyPropertyChanged("Caption");
                }
            } 
        }


        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        [WidgetParameter]
        public String IconPath
        {
            get { return _IconPath; }
            set
            {
                if (_IconPath != value)
                {
                    _IconPath = value;
                    UpdateIconImage();
                    NotifyPropertyChanged("IconPath");
                }
            }
        }


        protected override Size[] GetSizes() 
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        protected virtual void UpdateIconImage()
        {
            if ((! String.IsNullOrEmpty(_IconPath)) && (!IsExecutableIcon()))
                _BgImage = new AlphaImage(_IconPath);
            else
                _BgImage = null;
        }

        private bool IsExecutableIcon()
        {
            return ((_IconPath.EndsWith(".exe")) || (_IconPath.EndsWith(".lnk")));
        }

        protected virtual void PaintIcon(Graphics g, Rectangle Rect)
        {
            // get icon from file
            if (String.IsNullOrEmpty(_IconPath))
                return;

            int CaptionHeight = (Caption == "") ? 0 : (CaptionSize /* + CaptionBottomOffset */);

            // draw icon from external image file
            if (_BgImage != null)
            {
                _BgImage.PaintIcon(g, new Rectangle(Rect.Left, Rect.Top, Rect.Width, Rect.Height - CaptionHeight/2));
                return;
            }

            // draw main icon from executable file
            if (IsExecutableIcon())
            {
                FileRoutines.structa refa = new FileRoutines.structa();
                IntPtr ptr = FileRoutines.SHGetFileInfo(ref _IconPath, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                g.DrawIcon(icon, (Rect.Left + Rect.Right - icon.Width) / 2,
                    Rect.Top + (Rect.Height - icon.Height - CaptionHeight) / 2);

                icon.Dispose();
                icon = null;
            }
        }

        /// <summary>
        /// Draw caption
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        protected virtual void PaintCaption(Graphics g, Rectangle Rect)
        {
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Segoe UI Light", 8, FontStyle.Bold);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush,
                    Rect.Left + CaptionLeftOffset, 
                    Rect.Bottom - CaptionBottomOffset - CaptionSize);
            }
        }


        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            PaintIcon(g, Rect);
            PaintCaption(g, Rect);
        }


        public override bool OnClick(Point Location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}",
                this.Caption, Location.X, Location.Y));
            return true;
        }


        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;

                Settings_string CaptionControl = new Settings_string();
                CaptionControl.Name = "CaptionControl";
                CaptionControl.Caption = "Caption";
                CaptionControl.Value = Caption;
                Controls.Add(CaptionControl);

                Settings_image ImgControl = new Settings_image();
                ImgControl.Name = "IconControl";
                ImgControl.Caption = "Icon image";
                ImgControl.Value = IconPath;
                Controls.Add(ImgControl);

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "Caption", CaptionControl, "Value");
                BindingManager.Bind(this, "IconPath", ImgControl, "Value");

                return Controls;
            }
        }

    }

}
