using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fleux.UIElements;
using MetroHome65.Routines;
using Microsoft.WindowsMobile.Forms;

namespace Metrohome65.Settings.Controls
{
    public class ImageSettingsControl: Canvas, INotifyPropertyChanged
    {
        private TextElement _lblCaption;

        private ImageElement _pictureBox;

        private String _tileImage = null;

        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();


        public ImageSettingsControl()
        {
            Size = new Size(450, 300);

            _lblCaption = new TextElement("<image parameter>");
            AddElement(_lblCaption);

            _pictureBox = new ImageElement(new Bitmap(1, 1))
                              {
                                  Size = new Size(427, 173),
                                  Location = new Point(7, 6),
                              };
            AddElement(_pictureBox);

            var buttonSelectImage = new Fleux.UIElements.Button("...")
                                        {
                                            Location = new Point(308, 67),
                                        };
            AddElement(buttonSelectImage);

            var buttonClearImage = new Fleux.UIElements.Button("x")
                                       {
                                           Location = new Point(416, 67),
                                       };
            AddElement(buttonClearImage);
        }


        public String Caption { set { _lblCaption.Text = value; } }

        public String Value
        {
            get { return _tileImage; }
            set
            {
                SetTileImage(value);
            }
        }

        private void SetTileImage(String value)
        {
            if (_tileImage != value)
            {
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

                            FileRoutines.structa refa = new FileRoutines.structa();
                            IntPtr ptr = FileRoutines.SHGetFileInfo(ref _tileImage, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                            Icon icon = Icon.FromHandle(refa.a);
                            _pictureBox.Image = GetBitmap(icon);
                        }

                        else
                        {
                            OpenNETCF.Drawing.Imaging.IImage _img;
                            _factory.CreateImageFromFile(_tileImage, out _img);

                            OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                            _img.GetImageInfo(out imageInfo);

                            OpenNETCF.Drawing.Imaging.IBitmapImage _bmp;
                            _factory.CreateBitmapFromImage(_img, imageInfo.Width, imageInfo.Height,
                                System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                                OpenNETCF.Drawing.Imaging.InterpolationHint.InterpolationHintDefault, out _bmp);

                            _pictureBox.Image =
                                OpenNETCF.Drawing.Imaging.ImageUtils.IBitmapImageToBitmap(_bmp);
                        }

                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "SetIconPath")
                }

                NotifyPropertyChanged("Value");
            }
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

        private void buttonSelectBG_Click(object sender, EventArgs e)
        {
            SelectPictureDialog ImgDialog = new SelectPictureDialog();
            ImgDialog.Filter = "Image files|*.jpg;*.png;*.bmp;*.gif";
            ImgDialog.Title = "Select image";
            ImgDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (ImgDialog.ShowDialog() == DialogResult.OK)
                SetTileImage(ImgDialog.FileName);
        }


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
