using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using MetroHome65.Routines;

namespace MetroHome65.Pages
{
    struct FileDescr
    {
        public String Name;
        public String Path;
    }


    public partial class ProgramList : UserControl, IPageControl
    {
        private int _IconSize = ScreenRoutines.Scale(64);
        private static int PaddingHor = ScreenRoutines.Scale(30);
        private int _BorderSize = ScreenRoutines.Scale(7);
        private int _BlankSize = ScreenRoutines.Scale(5);

        private Color _BGColor = Color.Green;
        private List<FileDescr> _FileList = new List<FileDescr>();
        private WidgetGrid _WidgetGrid;
        private MetroHome65.Main.IHost _Host;


        public ProgramList(WidgetGrid WidgetGrid)
        {
            InitializeComponent();

            // change sizes and position for QVGA display
            if (ScreenRoutines.IsQVGA)
            {
                int ImgSize = _IconSize + _BorderSize * 2 + _BlankSize * 2;
                ilAppIcons.ImageSize = new Size(ImgSize, ImgSize);
            }

            this._WidgetGrid = WidgetGrid;

            FillProgramList();
        }

        private void ProgramList_Resize(object sender, EventArgs e)
        {
            this.lvApps.Width = this.Width - this.lvApps.Left + PaddingHor;
            this.lvApps.Height = this.Height;
        }


        /// <summary>
        /// Fill list with applications
        /// </summary>
        private void FillProgramList()
        {
            Cursor.Current = Cursors.WaitCursor;
            
            _FileList.Clear();
            DirectoryInfo FolderInfo = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.StartMenu));
            FillProgramsFromFolder(FolderInfo);

            // sort file list 
            _FileList.Sort(
                delegate(FileDescr File1, FileDescr File2) { return File1.Name.CompareTo(File2.Name); } );

            FillListView();

            Cursor.Current = Cursors.Default;
        }


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
                            _FileList.Add(new FileDescr() { 
                                Name = files[i].Name.Substring(0, files[i].Name.Length - 4),
                                Path = files[i].FullName                             
                            } );
                        }
                    }

                    // recursive fill subfolder content
                    FillProgramsFromFolder(CurFolderInfo);
                }
            }
        }

        private void FillListView()
        {
            String fname;
            FileRoutines.structa refa = new FileRoutines.structa();
            IntPtr ptr;

            Brush bgBrush = new System.Drawing.SolidBrush(_BGColor);
            Rectangle Rect = new Rectangle(0, _BlankSize, _IconSize + _BorderSize * 2, _IconSize + _BorderSize * 2);

            lvApps.Items.Clear();
            ilAppIcons.Images.Clear();

            foreach (FileDescr FileDescr in _FileList)
            {
                // get icon from file
                fname = FileDescr.Path;
                ptr = FileRoutines.SHGetFileInfo(ref fname, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                Bitmap image = new Bitmap(Rect.Width + _BlankSize * 2, Rect.Height + _BlankSize * 2);
                Graphics graphics = Graphics.FromImage(image);
                graphics.Clear(Color.Black);
                graphics.FillRectangle(bgBrush, Rect);
                graphics.DrawIcon(icon, _BorderSize, _BorderSize + _BlankSize);

                ilAppIcons.Images.Add(image);
                graphics.Dispose();
                graphics = null;

                // add new program item to program list
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = FileDescr.Name;
                listViewItem.ForeColor = Color.LightGray;
                listViewItem.ImageIndex = ilAppIcons.Images.Count - 1; // last added image index

                lvApps.Items.Add(listViewItem);

                icon.Dispose();
                icon = null;
                listViewItem = null;
            }
        }


        /// <summary>
        /// Launch selected program in list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvApps.SelectedIndices.Count > 0)
            {
                String FileName = _FileList[lvApps.SelectedIndices[0]].Path;
                lvApps.Items[lvApps.SelectedIndices[0]].Selected = false;

                FileRoutines.StartProcess(FileName);
            }
        }


        // IPageControl
        public Boolean Active { set { } }
        public void SetBackColor(Color value) { }
        public Control GetControl() { return this; }

        public void SetHost(MetroHome65.Main.IHost Host)
        {
            _Host = Host;
        }


        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (_Host != null)
                _Host.ChangePage(false);
        }


        /// <summary>
        /// Shows popup menu - pin to Start
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public Boolean ShowPopupMenu(Point Location) {
            if (! lvApps.Bounds.Contains(Location))
                return false; 

            ContextMenu _mnuWidgetActions = new ContextMenu();
            _mnuWidgetActions.MenuItems.Clear();

            MenuItem menuAddWidget = new System.Windows.Forms.MenuItem();
            menuAddWidget.Text = "Pin to start";
            menuAddWidget.Click += AddWidget_Click;
            _mnuWidgetActions.MenuItems.Add(menuAddWidget);

            _mnuWidgetActions.Show(this, Location);
            return true;
        }

        private void AddWidget_Click(object sender, EventArgs e)
        {
            _WidgetGrid.AddWidget(new Point(0, 90), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget");
            if (_Host != null)
                _Host.ChangePage(false);
        }

        private void gestureRecognizer_Hold(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            ShowPopupMenu(e.Location);
        }

    }
    
}
