using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.File;
using Metrohome65.Settings.Controls;

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
        private AlphaImage _iconImage;
        private TextStyle _captionFont = new TextStyle(MetroTheme.PhoneFontFamilyNormal, 8, Color.White);

        protected static int CaptionLeftOffset = 10;
        protected static int CaptionBottomOffset = 4;
        protected int CaptionHeight = 30; 

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

        [TileParameter]
        public TextStyle CaptionFont
        {
            get { return _captionFont; }
            set
            {
                if (_captionFont != value)
                {
                    _captionFont = value;

                    NotifyPropertyChanged("CaptionFont");
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
            var sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        protected virtual void UpdateIconImage()
        {
            if ((! String.IsNullOrEmpty(_iconPath)) && (!IsExecutableIcon()))
                _iconImage = new AlphaImage(_iconPath);
            else
                _iconImage = null;
        }

        private bool IsExecutableIcon()
        {
            return ((_iconPath.EndsWith(".exe")) || (_iconPath.EndsWith(".lnk")));
        }


        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);

            CaptionHeight = FleuxApplication.DummyDrawingGraphics.Style(_captionFont)
                .CalculateMultilineTextHeight("Xg", 1000);

            PaintIcon(g, rect);
            PaintCaption(g, rect);
        }

        protected virtual void PaintIcon(Graphics g, Rectangle rect)
        {
            // get icon from file
            if (String.IsNullOrEmpty(_iconPath))
                return;

            int captionHeight = (Caption == "") ? 0 : 
                (CaptionHeight + ((GridSize.Height == 1) ? 1 : 0) /* + CaptionBottomOffset */);

            // draw icon from external image file
            if (_iconImage != null)
            {
                _iconImage.PaintIcon(g, new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - captionHeight/2));
                return;
            }

            // draw main icon from executable file
            if (IsExecutableIcon())
            {
                var refa = new FileRoutines.structa();
                FileRoutines.SHGetFileInfo(ref _iconPath, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                var icon = Icon.FromHandle(refa.a);

                g.DrawIcon(icon, (rect.Left + rect.Right - icon.Width) / 2,
                    rect.Top + (rect.Height - icon.Height - captionHeight) / 2);

                icon.Dispose();
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
                var captionFont = new Font(_captionFont.FontFamily, _captionFont.FontSize.ToLogic(), FontStyle.Regular);
                var captionBrush = new SolidBrush(_captionFont.Foreground);
                g.DrawString(Caption, captionFont, captionBrush,
                    rect.Left + CaptionLeftOffset - ((GridSize.Height == 1) ? 2 : 0),
                    rect.Bottom - CaptionBottomOffset - CaptionHeight + ((GridSize.Height == 1) ? 2.ToLogic() : 0));
            }
        }


        public override bool OnClick(Point location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}", Caption, location.X, location.Y));
            return true;
        }


        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var captionControl = new StringSettingsControl(settingsPage)
                                     {
                                         Caption = "Caption", 
                                         Name = "Caption",
                                     };
            controls.Add(captionControl);
            bindingManager.Bind(this, "Caption", captionControl, "Value", true);

            var captionFontControl = new FontSettingsControl
            {
                Caption = "Caption Font",
                Name = "CaptionFont",
            };
            controls.Add(captionFontControl);
            bindingManager.Bind(this, "CaptionFont", captionFontControl, "Value", true);

            var imgControl = new ImageSettingsControl()
                                 {
                                     Caption = "Icon image", 
                                     Name = "Icon",
                                 };
            controls.Add(imgControl);
            bindingManager.Bind(this, "IconPath", imgControl, "Value", true);

            return controls;
        }

    }

}
