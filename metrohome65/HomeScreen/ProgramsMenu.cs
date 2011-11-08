using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using MetroHome65.Routines;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.UIElements.Panorama;
using Fleux.Templates;
using Fleux.Styles;
using WindowsPhone7Sample.Elements;

namespace MetroHome65.HomeScreen
{
    struct FileDescr
    {
        public String Name;
        public String Path;
    }


    class ProgramsMenu : ListElement
    {
       
        public ProgramsMenu()
            : base()
        {
            VerticalScroll = true;

            DataTemplateSelector = item => BuildItem;
            SourceItems = GetProgramList();
        }


        private BindingList<object> _FileList = new BindingList<object>();

        /// <summary>
        /// Fill list with applications
        /// </summary>
        private BindingList<object> GetProgramList()
        {
            DirectoryInfo FolderInfo = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.StartMenu));
            FillProgramsFromFolder(FolderInfo);

            //!! sort file list 
            //!!_FileList.Sort( (f1, f2) => { return f1.Name.CompareTo(f2.Name); });

            return _FileList;
        }


        /// <summary>
        /// Add to the program list shortcuts for application from current folder
        /// </summary>
        /// <param name="FolderInfo"></param>
        private void FillProgramsFromFolder(DirectoryInfo FolderInfo)
        {
            if (Enumerable.Count<DirectoryInfo>(FolderInfo.GetDirectories()) > 0)
            {
                foreach (DirectoryInfo CurFolderInfo in FolderInfo.GetDirectories())
                {
                    FileInfo[] files = CurFolderInfo.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Name != "icon.lnk")
                        {
                            _FileList.Add(new FileDescr()
                            {
                                Name = files[i].Name.Substring(0, files[i].Name.Length - 4),
                                Path = files[i].FullName
                            });
                        }
                    }

                    // recursive fill subfolder content
                    FillProgramsFromFolder(CurFolderInfo);
                }
            }
        }


        private static int _IconSize = ScreenRoutines.Scale(64);
        private static int _PaddingHor = ScreenRoutines.Scale(20);
        private static int _BorderSize = ScreenRoutines.Scale(7);
        private static int _BlankSize = ScreenRoutines.Scale(5);
        private static Color _BGColor = Color.Green;

        private FileRoutines.structa refa = new FileRoutines.structa();
        private IntPtr ptr;
        private Rectangle Rect = new Rectangle(0, _BlankSize, _IconSize + _BorderSize * 2, _IconSize + _BorderSize * 2);
        private Brush bgBrush = new System.Drawing.SolidBrush(_BGColor);

        private UIElement BuildItem(object AFileDescr)
        {
            var FileDescr = (FileDescr)AFileDescr;

            var canvas = new Canvas() { 
                    Size = new Size(480, _IconSize + _BlankSize) 
                };

            // draw icon with border
            ptr = FileRoutines.SHGetFileInfo(ref FileDescr.Path, 0, ref refa, Marshal.SizeOf(refa), 0x100);
            Icon icon = Icon.FromHandle(refa.a);

            Bitmap image = new Bitmap(Rect.Width + _BlankSize * 2, Rect.Height + _BlankSize * 2);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black);
            graphics.FillRectangle(bgBrush, Rect);
            graphics.DrawIcon(icon, _BorderSize, _BorderSize + _BlankSize);

            canvas.AddElement(new ImageElement(image) {
                Size = image.Size,
                Location = new Point(0, 0),
            } );

            // draw program name
            canvas.AddElement(new TextElement(FileDescr.Name) {
                Style = MetroTheme.PhoneTextSmallStyle,
                Location = new Point(image.Width + _PaddingHor, 0),
                Size = new Size(480 - Location.X, image.Height)
            });

            // onclick handler = launch program
            canvas.TapHandler = point => { 
                FileRoutines.StartProcess(FileDescr.Path); return true; 
            };

            return canvas;
        }

    }
}
