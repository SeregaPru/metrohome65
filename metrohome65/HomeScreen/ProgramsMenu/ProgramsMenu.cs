using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using Fleux.Controls;
using MetroHome65.Routines;
using Fleux.UIElements;
using Fleux.Styles;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    /// <summary>
    /// message for notification to tiles grid to add new tile
    /// </summary>
    public class PinProgramMessage : ITinyMessage
    {
        public string Name;
        public string Path;
        public object Sender { get { return null; } }
    }


    sealed class ProgramsMenu : ListElement
    {

        // color for item name font
        private readonly Color _fontColor;

        // color for item icon background
        private readonly Color _bgColor;

        private readonly Brush _bgBrush;

        public ProgramsMenu()
        {
            EntranceAnimation = null;
            ExitAnimation = null;
            VerticalScroll = true;

            _refa = new FileRoutines.structa();

            _bgColor = MetroTheme.PhoneAccentBrush;
            _fontColor = MetroTheme.PhoneForegroundBrush;
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
            var folderInfo = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.StartMenu));
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


        private static readonly int IconSize = 64;
        private static readonly int PaddingHor = 10;
        private static readonly int BorderSize = 5;
        private static readonly int BlankSize = 5;

        private FileRoutines.structa _refa;
        private Rectangle _rect = new Rectangle(0, 0, IconSize + BorderSize * 2, IconSize + BorderSize * 2);

        private UIElement BuildItem(object aFileDescr)
        {
            var fileDescr = (FileDescr)aFileDescr;

            var canvas = new Canvas
            {
                Size = new Size(ScreenConsts.ScreenWidth, IconSize + BlankSize * 2 + BorderSize * 2)
            };

            // draw icon with border
            var image = new Bitmap(_rect.Width, _rect.Height);
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
                Style = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, MetroTheme.PhoneFontSizeNormal, _fontColor),
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

        /// <summary>
        ////shows context menu for program item
        /// </summary>
        /// <param name="fileDescr"></param>
        private void ShowPopupMenu(Point location, FileDescr fileDescr)
        {
            var mainMenu = new ContextMenu();

            var menuPinProgram = new MenuItem { Text = "Pin to start" };
            menuPinProgram.Click += (s, e) => PinProgram(fileDescr);
            mainMenu.MenuItems.Add(menuPinProgram);

            mainMenu.Show(TinyIoCContainer.Current.Resolve<FleuxControl>(), location);
        }

        /// <summary>
        /// pin selected item to start menu
        /// </summary>
        /// <param name="fileDescr"></param>
        private void PinProgram(FileDescr fileDescr)
        {
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();

            messenger.Publish(new PinProgramMessage() { Name = fileDescr.Name, Path = fileDescr.Path} );
        }

    }
}
