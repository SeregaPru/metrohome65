using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenNETCF.Drawing;
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
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private String _Caption = "";
        private String _IconPath = "";


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
            try
            {
                if ((_IconPath != "") && (!IsExecutableIcon()))
                    _factory.CreateImageFromFile(_IconPath, out _img);
                else
                    _img = null;
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetIconPath")
            }
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

            int CaptionHeight = (Caption == "") ? 0 : Rect.Height / 20;

            // draw icon from file
            if (_img != null)
            {
                try
                {
                    OpenNETCF.Drawing.Imaging.ImageInfo ImageInfo;
                    int tmp = _img.GetImageInfo(out ImageInfo);

                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        (Rect.Left + Rect.Right - (int)ImageInfo.Width) / 2,
                        (Rect.Top + Rect.Bottom - (int)ImageInfo.Height) / 2 - CaptionHeight,
                        (int)ImageInfo.Width, (int)ImageInfo.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintIcon")
                }
                return;
            }

            if (IsExecutableIcon())
            {
                FileRoutines.structa refa = new FileRoutines.structa();
                IntPtr ptr = FileRoutines.SHGetFileInfo(ref _IconPath, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                g.DrawIcon(icon, (Rect.Left + Rect.Right - icon.Width) / 2, (Rect.Top + Rect.Bottom - icon.Height) / 2 - CaptionHeight);

                icon.Dispose();
                icon = null;
            }
        }

        protected virtual void PaintCaption(Graphics g, Rectangle Rect)
        {
            // draw caption
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Segoe UI Light", 8, FontStyle.Bold);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush,
                    Rect.Left + 10, Rect.Bottom - 6 - g.MeasureString(Caption, captionFont).Height);
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


        public override void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}",
                this.Caption, Location.X, Location.Y));
        }


        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;

                Settings_string CaptionControl = new Settings_string();
                CaptionControl.Caption = "Caption";
                CaptionControl.Value = Caption;
                Controls.Add(CaptionControl);

                Settings_image ImgControl = new Settings_image();
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
