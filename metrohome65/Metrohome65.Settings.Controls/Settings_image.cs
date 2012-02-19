using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Microsoft.WindowsMobile.Forms;
using System.Runtime.InteropServices;
using MetroHome65.Routines;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_image : UserControl, INotifyPropertyChanged
    {
        private String _TileImage = null;
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();

        public Settings_image()
        {
            InitializeComponent();
        }


        public String Caption { set { lblCaption.Text = value; } }

        public String Value
        { 
            get { return _TileImage; } 
            set { 
                SetTileImage(value); 
            } 
        }

        private void SetTileImage(String value)
        {
            if (_TileImage != value)
            {
                _TileImage = value;

                try
                {
                    if (pictureBoxBG.Image != null)
                        pictureBoxBG.Image.Dispose();

                    if (_TileImage == "")
                    {
                        pictureBoxBG.Image = null;
                    }
                    else

                        if (IsExecutableIcon())
                        {
                            // get icon from file
                            if (String.IsNullOrEmpty(_TileImage))
                                return;

                            FileRoutines.structa refa = new FileRoutines.structa();
                            IntPtr ptr = FileRoutines.SHGetFileInfo(ref _TileImage, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                            Icon icon = Icon.FromHandle(refa.a);
                            pictureBoxBG.Image = GetBitmap(icon);
                        }

                        else
                        {
                            OpenNETCF.Drawing.Imaging.IImage _img;
                            _factory.CreateImageFromFile(_TileImage, out _img);

                            OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                            _img.GetImageInfo(out imageInfo);

                            OpenNETCF.Drawing.Imaging.IBitmapImage _bmp;
                            _factory.CreateBitmapFromImage(_img, imageInfo.Width, imageInfo.Height,
                                System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                                OpenNETCF.Drawing.Imaging.InterpolationHint.InterpolationHintDefault, out _bmp);

                            pictureBoxBG.Image = 
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
            return ((_TileImage.EndsWith(".exe")) || (_TileImage.EndsWith(".lnk")));
        }

        private Bitmap GetBitmap(Icon icon)
        {
            Bitmap bmp = new Bitmap(icon.Width, icon.Height);

            //Create temporary graphics
            Graphics gxMem = Graphics.FromImage(bmp);

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

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Value = "";
        }


        private void panelBorder_Paint(object sender, PaintEventArgs e)
        {
            Rectangle Rect = e.ClipRectangle;
            Rect.Inflate(-1, -1);
            e.Graphics.DrawRectangle(new Pen(Color.White, 2), Rect);
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }
}
