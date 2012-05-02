using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines;
using Microsoft.WindowsMobile.Forms;

namespace Metrohome65.Settings.Controls
{
    class SettingsImageElement: ImageElement
    {
        public SettingsImageElement(Image image): base(image) { }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if (Image == null)
            {
                // draw crossed rectangle for empty image
                drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
                drawingGraphics.Color(MetroTheme.PhoneForegroundBrush);

                drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);
                drawingGraphics.DrawLine(0, 0, Size.Width, Size.Height);
                drawingGraphics.DrawLine(0, Size.Height, Size.Width, 0);
            }
            else
                base.Draw(drawingGraphics);
        }   
    }


    public sealed class ImageSettingsControl: Canvas, INotifyPropertyChanged
    {
        #region Fields

        private readonly TextElement _lblCaption;

        private readonly ImageElement _pictureBox;

        private String _tileImage;

        private readonly OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();

        #endregion


        #region Properties

        public String Caption { set { _lblCaption.Text = value; } }

        public String Value
        {
            get { return _tileImage; }
            set { SetTileImage(value); }
        }

        #endregion


        #region Methods

        public ImageSettingsControl()
        {
            Size = new Size(SettingsConsts.MaxWidth, 300);

            _lblCaption = new TextElement("<image parameter>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 40),
                Location = new Point(0, 0),
            };
            AddElement(_lblCaption);

            _pictureBox = new SettingsImageElement(new Bitmap(1, 1))
                              {
                                  Size = new Size(SettingsConsts.MaxWidth, 175),
                                  Location = new Point(0, _lblCaption.Bounds.Bottom + 10),
                              };
            AddElement(_pictureBox);

            var buttonSelectImage = new Fleux.UIElements.Button("select")
                                        {
                                            Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                                            Location = new Point(0, _pictureBox.Bounds.Bottom + 10),
                                            TapHandler = p => ButtonSelectBgClick(),

                                        };
            AddElement(buttonSelectImage);

            var buttonClearImage = new Fleux.UIElements.Button("clear")
                                       {
                                           Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                                           Location = new Point(SettingsConsts.MaxWidth / 2 + 10, _pictureBox.Bounds.Bottom + 10),
                                           TapHandler = p => { 
                                               Value = "";
                                               return true;
                                           },
                                       };
            AddElement(buttonClearImage);
        }

        private void SetTileImage(String value)
        {
            if (_tileImage == value) return;

            _tileImage = value;

            try
            {
                if (_pictureBox.Image != null)
                    _pictureBox.Image.Dispose();

                if (_tileImage == "")
                {
                    _pictureBox.Image = null;
                }
                else

                    if (IsExecutableIcon())
                    {
                        // get icon from file
                        if (String.IsNullOrEmpty(_tileImage))
                            return;

                        var refa = new FileRoutines.structa();
                        FileRoutines.SHGetFileInfo(ref _tileImage, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                        var icon = Icon.FromHandle(refa.a);
                        _pictureBox.Image = GetBitmap(icon);
                    }

                    else
                    {
                        OpenNETCF.Drawing.Imaging.IImage img;
                        _factory.CreateImageFromFile(_tileImage, out img);

                        OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                        img.GetImageInfo(out imageInfo);

                        OpenNETCF.Drawing.Imaging.IBitmapImage bmp;
                        _factory.CreateBitmapFromImage(img, imageInfo.Width, imageInfo.Height,
                                                       System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                                                       OpenNETCF.Drawing.Imaging.InterpolationHint.InterpolationHintDefault, out bmp);

                        _pictureBox.Image =
                            OpenNETCF.Drawing.Imaging.ImageUtils.IBitmapImageToBitmap(bmp);
                    }

            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetIconPath")
            }

            _pictureBox.Update();

            NotifyPropertyChanged("Value");
        }


        private bool IsExecutableIcon()
        {
            return ((_tileImage.EndsWith(".exe")) || (_tileImage.EndsWith(".lnk")));
        }

        private Bitmap GetBitmap(Icon icon)
        {
            var bmp = new Bitmap(icon.Width, icon.Height);

            //Create temporary graphics
            var gxMem = Graphics.FromImage(bmp);

            //Draw the icon
            gxMem.DrawIcon(icon, 0, 0);

            //Clean up
            gxMem.Dispose();

            return bmp;
        }

        private bool ButtonSelectBgClick()
        {
            var imgDialog = new SelectPictureDialog
                                {
                                    Filter = "Image files|*.jpg;*.png;*.bmp;*.gif",
                                    Title = "Select image",
                                    InitialDirectory =
                                        System.IO.Path.GetDirectoryName(
                                            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                                };

            if (imgDialog.ShowDialog() != DialogResult.OK)
                return false;

            SetTileImage(imgDialog.FileName);
            return true;
        }

        #endregion


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
