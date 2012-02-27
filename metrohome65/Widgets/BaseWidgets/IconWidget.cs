using System;
using System.Drawing;
using System.Windows.Forms;
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
        private String _caption = "";
        private String _iconPath = "";
        private AlphaImage _bgImage = null;

        protected static int CaptionLeftOffset = ScreenRoutines.Scale(10);
        protected static int CaptionBottomOffset = ScreenRoutines.Scale(4);
        protected static int CaptionSize = ScreenRoutines.Scale(28); // approx caption height in px. 

        /// <summary>
        /// user defined caption for widget
        /// </summary>
        [TileParameter]
        public String Caption { 
            get { return _caption; } 
            set {
                if (_caption != value)
                {
                    _caption = value;
                    NotifyPropertyChanged("Caption");
                }
            } 
        }


        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        [TileParameter]
        public String IconPath
        {
            get { return _iconPath; }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
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
            if ((! String.IsNullOrEmpty(_iconPath)) && (!IsExecutableIcon()))
                _bgImage = new AlphaImage(_iconPath);
            else
                _bgImage = null;
        }

        private bool IsExecutableIcon()
        {
            return ((_iconPath.EndsWith(".exe")) || (_iconPath.EndsWith(".lnk")));
        }

        protected virtual void PaintIcon(Graphics g, Rectangle rect)
        {
            // get icon from file
            if (String.IsNullOrEmpty(_iconPath))
                return;

            int captionHeight = (Caption == "") ? 0 : (CaptionSize /* + CaptionBottomOffset */);

            // draw icon from external image file
            if (_bgImage != null)
            {
                _bgImage.PaintIcon(g, new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - captionHeight/2));
                return;
            }

            // draw main icon from executable file
            if (IsExecutableIcon())
            {
                FileRoutines.structa refa = new FileRoutines.structa();
                IntPtr ptr = FileRoutines.SHGetFileInfo(ref _iconPath, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                g.DrawIcon(icon, (rect.Left + rect.Right - icon.Width) / 2,
                    rect.Top + (rect.Height - icon.Height - captionHeight) / 2);

                icon.Dispose();
                icon = null;
            }
        }

        /// <summary>
        /// Draw caption
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        protected virtual void PaintCaption(Graphics g, Rectangle rect)
        {
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Segoe WP Light", 8, FontStyle.Bold);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush,
                    rect.Left + CaptionLeftOffset, 
                    rect.Bottom - CaptionBottomOffset - CaptionSize);
            }
        }


        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public override void Paint(Graphics g, Rectangle rect)
        {
            base.Paint(g, rect);

            PaintIcon(g, rect);
            PaintCaption(g, rect);
        }


        public override bool OnClick(Point location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}",
                this.Caption, location.X, location.Y));
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
