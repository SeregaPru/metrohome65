using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using MetroHome65.Routines;
using Fleux.UIElements;
using Fleux.Styles;

namespace MetroHome65.HomeScreen.ProgramsMenu
{

    sealed class ProgramsMenu : ListElement
    {

        // color for item name font
        private readonly Color _fontColor;

        // color for item icon background
        private readonly Color _bgColor;

        private readonly Brush _bgBrush;

        public ProgramsMenu(MainSettings mainSettings)
        {
            EntranceAnimation = null;
            ExitAnimation = null;
            VerticalScroll = true;

            _refa = new FileRoutines.structa();

            _bgColor = mainSettings.TileColor;
            _fontColor = mainSettings.ListFontColor;
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


        private static readonly int IconSize = ScreenRoutines.Scale(64);
        private static readonly int PaddingHor = ScreenRoutines.Scale(7);
        private static readonly int BorderSize = ScreenRoutines.Scale(4);
        private static readonly int BlankSize = ScreenRoutines.Scale(4);

        private FileRoutines.structa _refa;
        private Rectangle _rect = new Rectangle(0, 0, IconSize + BorderSize * 2, IconSize + BorderSize * 2);

        private UIElement BuildItem(object aFileDescr)
        {
            var fileDescr = (FileDescr)aFileDescr;

            var canvas = new Canvas
            {
                Size = new Size(ScreenRoutines.Scale(480), IconSize + BlankSize * 2 + BorderSize * 2)
            };

            // draw icon with border
            FileRoutines.SHGetFileInfo(ref fileDescr.Path, 0, ref _refa, Marshal.SizeOf(_refa), 0x100);
            var icon = Icon.FromHandle(_refa.a);

            var image = new Bitmap(_rect.Width, _rect.Height);
            var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(_bgBrush, _rect);
            graphics.DrawIcon(icon, BorderSize, BorderSize);

            canvas.AddElement(new ImageElement(image)
            {
                Size = image.Size,
                Location = new Point(0, BlankSize),
            });

            // draw program name
            var textHeight = ScreenRoutines.Scale(15);
            canvas.AddElement(new TextElement(fileDescr.Name)
            {
                Style = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, MetroTheme.PhoneFontSizeNormal, _fontColor),
                Location = new Point(image.Width + PaddingHor, textHeight),
                Size = new Size(ScreenRoutines.Scale(480) - image.Width + PaddingHor, image.Height - textHeight)
            });

            // click handler = launch program
            canvas.TapHandler = point =>
            {
                FileRoutines.StartProcess(fileDescr.Path); return true;
            };

            return canvas;
        }

    }
}
