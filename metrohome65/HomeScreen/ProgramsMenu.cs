using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using MetroHome65.Routines;
using Fleux.UIElements;
using Fleux.Styles;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen
{
    struct FileDescr
    {
        public String Name;
        public String Path;
    }

    // container for programs shortcurs
    class ProgramsList : BindingList<object>
    {
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            var items = Items as List<object>;
            if (items != null)
                items.Sort((f1, f2) => String.CompareOrdinal(((FileDescr)f1).Name, ((FileDescr)f2).Name));
        }

        public void SortByName()
        {
            var propDescriptors = TypeDescriptor.GetProperties(typeof(FileDescr));
            ApplySortCore(propDescriptors["Name"], ListSortDirection.Ascending);
        }
    }


    sealed class ProgramsMenu : ListElement, IActive
    {
        public ProgramsMenu()
        //: base()
        {
            _refa = new FileRoutines.structa();
            EntranceAnimation = null;
            ExitAnimation = null;

            VerticalScroll = true;

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
        private static readonly int BorderSize = ScreenRoutines.Scale(7);
        private static readonly int BlankSize = ScreenRoutines.Scale(5);
        private static readonly Color BgColor = Color.Green;

        private FileRoutines.structa _refa;
        private Rectangle _rect = new Rectangle(0, BlankSize, IconSize + BorderSize * 2, IconSize + BorderSize * 2);
        private readonly Brush _bgBrush = new SolidBrush(BgColor);

        private UIElement BuildItem(object aFileDescr)
        {
            var fileDescr = (FileDescr)aFileDescr;

            var canvas = new Canvas
            {
                Size = new Size(ScreenRoutines.Scale(480), IconSize + BlankSize)
            };

            // draw icon with border
            FileRoutines.SHGetFileInfo(ref fileDescr.Path, 0, ref _refa, Marshal.SizeOf(_refa), 0x100);
            var icon = Icon.FromHandle(_refa.a);

            var image = new Bitmap(_rect.Width + BlankSize * 2, _rect.Height + BlankSize * 2);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black);
            graphics.FillRectangle(_bgBrush, _rect);
            graphics.DrawIcon(icon, BorderSize, BorderSize + BlankSize);

            canvas.AddElement(new ImageElement(image)
            {
                Size = image.Size,
                Location = new Point(0, 0),
            });

            // draw program name
            var textHeight = ScreenRoutines.Scale(15);
            canvas.AddElement(new TextElement(fileDescr.Name)
            {
                Style = new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal,
                    MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneForegroundBrush),
                Location = new Point(image.Width + PaddingHor, textHeight),
                Size = new Size(ScreenRoutines.Scale(480) - image.Width + PaddingHor, image.Height - textHeight)
            });

            // onclick handler = launch program
            canvas.TapHandler = point =>
            {
                FileRoutines.StartProcess(fileDescr.Path); return true;
            };

            return canvas;
        }

        // IActive
        public Boolean Active
        {
            get { return true; }
            set
            {
                if (! value)
                {
                    // stop scroll animation
                    Pressed(new Point(-1, -1));
                }
            }
        }

    }
}
