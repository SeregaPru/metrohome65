using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using Fleux.Controls;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using Fleux.UIElements;
using Fleux.Styles;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    sealed partial class ProgramsMenu : ListElement
    {

        // color for item icon background
        private readonly Color _bgColor;

        private readonly Brush _bgBrush;

        public ProgramsMenu()
        {
            EntranceAnimation = null;
            ExitAnimation = null;
            VerticalScroll = true;

            _bgImage = TinyIoCContainer.Current.Resolve<HomeScreenBackground>();

            Size = new Size(100, 100);
            SizeChanged += (s, e) => CreateBuffer();

            _refa = new FileRoutines.structa();

            _bgColor = MetroTheme.PhoneAccentBrush;
            _bgBrush = new SolidBrush(_bgColor);

            DataTemplateSelector = item => BuildItem;
            SourceItems = GetProgramList();
        }


        private readonly ProgramsList _fileList = new ProgramsList();

        /// <summary>
        /// Fill list with applications
        /// </summary>
        private BindingList<object> GetProgramList()
        {
            _fileList.Add(new FileDescr()); // first empty item for hor padding

            var folderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            FillProgramsFromFolder(folderInfo);

            _fileList.SortByName();

            return _fileList;
        }


        /// <summary>
        /// Add to the program list shortcuts for application from current folder
        /// </summary>
        /// <param name="folderInfo"></param>
        private void FillProgramsFromFolder(DirectoryInfo folderInfo)
        {
            if (folderInfo.GetDirectories().Any())
            {
                foreach (DirectoryInfo curFolderInfo in folderInfo.GetDirectories())
                {
                    FileInfo[] files = curFolderInfo.GetFiles();
                    foreach (FileInfo t in files)
                    {
                        if (t.Name != "icon.lnk")
                        {
                            _fileList.Add(new FileDescr
                            {
                                Name = t.Name.Substring(0, t.Name.Length - 4),
                                Path = t.FullName
                            });
                        }
                    }

                    // recursive fill subfolder content
                    FillProgramsFromFolder(curFolderInfo);
                }
            }
        }


        private const int IconSize = 64;
        private const int PaddingHor = 10;
        private const int BorderSize = 5;
        private const int BlankSize = 5;

        private FileRoutines.structa _refa;
        private Rectangle _rect = new Rectangle(0, 0, IconSize + BorderSize * 2, IconSize + BorderSize * 2);

        private UIElement BuildItem(object aFileDescr)
        {
            var fileDescr = (FileDescr)aFileDescr;

            // special processing for first empty item
            if (fileDescr.Name == null)
            {
                return new Canvas() { Size = new Size(ScreenConsts.ScreenWidth, 50), };
            }

            var canvas = new Canvas
            {
                Size = new Size(ScreenConsts.ScreenWidth, IconSize + BlankSize * 2 + BorderSize * 2)
            };

            // draw icon with border
            var image = new Bitmap(_rect.Width, _rect.Height, PixelFormat.Format16bppRgb565);
            var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(_bgBrush, _rect);

            try
            {
                FileRoutines.SHGetFileInfo(ref fileDescr.Path, 0, ref _refa, Marshal.SizeOf(_refa), 0x100);
                var icon = Icon.FromHandle(_refa.a);
                graphics.DrawIcon(icon, BorderSize, BorderSize);
            }
            catch (Exception) { }

            canvas.AddElement(new ImageElement(image)
                                  {
                                      Size = image.Size,
                                      Location = new Point(0, BlankSize),
                                  });

            // draw program name
            var textHeight = ScreenRoutines.Scale(15);
            canvas.AddElement(new TextElement(fileDescr.Name)
            {
                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, 11, MetroTheme.PhoneForegroundBrush),
                Location = new Point(_rect.Width + PaddingHor, textHeight),
                Size = new Size(ScreenConsts.ScreenWidth - _rect.Width + PaddingHor, _rect.Height - textHeight)
            });

            // click handler = launch program
            canvas.TapHandler = point =>
                                    {
                                        FileRoutines.StartProcess(fileDescr.Path); return true;
                                    };

            // hold handler - show context menu
            canvas.HoldHandler = point =>
                                     {
                                         ShowPopupMenu(new Point(
                                             point.X + canvas.Bounds.Left + canvas.Parent.Bounds.Left,
                                             point.Y + canvas.Bounds.Top + canvas.Parent.Bounds.Top), fileDescr);
                                         return true;
                                     };

            return canvas;
        }

        public override bool Flick(Point from, Point to, int millisecs, Point startPoint)
        {
            // scroll some faster 
            return base.Flick(from, to, millisecs * 2 / 3, startPoint);
        }

        ////shows context menu for program item
        /// <summary>
        /// </summary>
        /// <param name="location"></param>
        /// <param name="fileDescr"></param>
        private void ShowPopupMenu(Point location, FileDescr fileDescr)
        {
            var mainMenu = new ContextMenu();

            var menuPinProgram = new MenuItem { Text = "Pin to start" };
            menuPinProgram.Click += (s, e) => PinProgram(fileDescr);
            mainMenu.MenuItems.Add(menuPinProgram);

            if (ParentControl != null)
                mainMenu.Show(ParentControl, location);
        }

        /// <summary>
        /// pin selected item to start menu
        /// </summary>
        /// <param name="fileDescr"></param>
        private void PinProgram(FileDescr fileDescr)
        {
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();

            messenger.Publish(new PinProgramMessage { Name = fileDescr.Name, Path = fileDescr.Path} );
        }

    }
}
