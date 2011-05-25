using System;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    public partial class Settings_image : UserControl
    {
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private String _ButtonImage = null;

        public Settings_image()
        {
            InitializeComponent();
        }

        public String Caption { set { lblCaption.Text = value; } }

        public String Value { get { return _ButtonImage; } set { SetButtonImage(value); } }

        private void SetButtonImage(String value)
        {
            if (_ButtonImage != value)
            {
                _ButtonImage = value;

                if (pictureBoxBG.Image != null)
                    pictureBoxBG.Image.Dispose();

                if (_ButtonImage != "")
                {
                    OpenNETCF.Drawing.Imaging.IImage _img;
                    _factory.CreateImageFromFile(_ButtonImage, out _img);

                    OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                    _img.GetImageInfo(out imageInfo);

                    OpenNETCF.Drawing.Imaging.IBitmapImage _bmp;
                    _factory.CreateBitmapFromImage(_img, imageInfo.Width, imageInfo.Height,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                        OpenNETCF.Drawing.Imaging.InterpolationHint.InterpolationHintDefault, out _bmp);

                    pictureBoxBG.Image = OpenNETCF.Drawing.Imaging.ImageUtils.IBitmapImageToBitmap(_bmp);
                }
                else
                    pictureBoxBG.Image = null;
            }
        }

        private void buttonSelectBG_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                SetButtonImage(openFileDialog1.FileName);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            SetButtonImage("");
        }

    }
}
