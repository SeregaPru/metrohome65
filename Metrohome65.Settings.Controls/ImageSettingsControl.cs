using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fleux.Core.NativeHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines;
using MetroHome65.Routines.File;
using Microsoft.WindowsMobile.Forms;

namespace Metrohome65.Settings.Controls
{
    class SettingsImageElement: ImageElement
    {
        public SettingsImageElement(Image image): base(image)
        {
            StretchType = StretchTypeOptions.Proportional;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if (Image == null)
            {
                // draw crossed rectangle for empty image
                drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
                drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);

                drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);
                drawingGraphics.DrawLine(0, 0, Size.Width, Size.Height);
                drawingGraphics.DrawLine(0, Size.Height, Size.Width, 0);
            }
            else
                base.Draw(drawingGraphics);
        }   
    }


    public sealed class ImageSettingsControl : StackPanel, INotifyPropertyChanged
    {
        #region Fields

        private readonly TextElement _lblCaption;

        private readonly ImageElement _pictureBox;

        private String _tileImage;

        #endregion


        #region Properties

        public String Caption { set { _lblCaption.Text = value; } }

        public String Value
        {
            get { return _tileImage; }
            set
            {
                SetTileImage(value);
                NotifyPropertyChanged("Value");
            }
        }

        #endregion


        #region Methods

        public ImageSettingsControl()
        {
            _lblCaption = new TextElement("<image parameter>")
                {
                    Size = new Size(SettingsConsts.MaxWidth, 50),
                };
            AddElement(_lblCaption);

            _pictureBox = new SettingsImageElement(new Bitmap(1, 1))
                {
                  Size = new Size(SettingsConsts.MaxWidth, 175),
                };
            AddElement(_pictureBox);

            AddElement(new Canvas { Size = new Size(SettingsConsts.MaxWidth, 10), });

            var buttonPanel = new Canvas { Size = new Size(SettingsConsts.MaxWidth, 50), };
            AddElement(buttonPanel);

            var buttonSelectImage = new Fleux.UIElements.Button("select".Localize())
                {
                    Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                    Location = new Point(0, 0),
                    TapHandler = p => ButtonSelectBgClick(),
                };
            buttonPanel.AddElement(buttonSelectImage);

            var buttonClearImage = new Fleux.UIElements.Button("clear".Localize())
               {
                   Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                   Location = new Point(SettingsConsts.MaxWidth / 2 + 10, 0),
                   TapHandler = p => { Value = ""; return true; },
               };
            buttonPanel.AddElement(buttonClearImage);
        }

        private void SetTileImage(String value)
        {
            if (_tileImage == value) return;

            _tileImage = value;

            try
            {
                if (_pictureBox.Image != null)
                {
                    _pictureBox.Image.Dispose();
                    _pictureBox.Image = null;
                }

                if (String.IsNullOrEmpty(_tileImage))
                    return;

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
                    var factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
                    IImage img;

                    factory.CreateImageFromFile(_tileImage, out img);

                    ImageInfo imageInfo;
                    img.GetImageInfo(out imageInfo);

                    IBitmapImage bmp;
                    factory.CreateBitmapFromImage(img, imageInfo.Width, imageInfo.Height,
                                                   PixelFormatID.PixelFormat16bppRGB565, 
                                                   InterpolationHint.InterpolationHintDefault, out bmp);

                    _pictureBox.Image = ImageHelpers.IBitmapImageToBitmap(bmp);
                }

            }
            catch (Exception)
            {
                _pictureBox.Image = null;
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
                                    Title = "Select image".Localize(),
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
