using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace MetroHome65.Pages
{
    struct FileDescr
    {
        public String Name;
        public String Path;
    }


    public partial class ProgramList : UserControl, IPageControl
    {
        private Size _IconSize = new Size(64, 64);
        private int _BorderSize = 7;
        private int _BlankSize = 5;
        private Color _BGColor = Color.Green;
        private List<FileDescr> _FileList = new List<FileDescr>();

        
        public ProgramList()
        {
            InitializeComponent();

            FillProgramList();
        }

        private void ProgramList_Resize(object sender, EventArgs e)
        {
            this.lvApps.Width = this.Width - this.lvApps.Left + 30;
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
            Routines.structa refa = new Routines.structa();
            IntPtr ptr;

            Brush bgBrush = new System.Drawing.SolidBrush(_BGColor);
            Rectangle Rect = new Rectangle(0, _BlankSize, _IconSize.Width + _BorderSize * 2, _IconSize.Height + _BorderSize * 2);

            lvApps.Items.Clear();
            ilAppIcons.Images.Clear();

            foreach (FileDescr FileDescr in _FileList)
            {
                // get icon from file
                fname = FileDescr.Path;
                ptr = Routines.SHGetFileInfo(ref fname, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                Bitmap image = new Bitmap(Rect.Width + _BlankSize*2, Rect.Height + _BlankSize*2);
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

                MetroHome65.Routines.StartProcess(FileName);
            }
        }


        // IPageControl
        public void SetScrollPosition(Point Location) { }
        public Point GetScrollPosition() { return new Point(0, 0); }

        public Size GetViewportSize() { return this.Size; }
        public Size GetExtentSize() { return this.Size; }

        public void ClickAt(Point Location) { }
        public void DblClickAt(Point Location) { }
        public Boolean ShowPopupMenu(Point Location) { return false; }

        public Boolean Active { set { } }

        public void SetBackColor(Color value) { }

        public Control GetControl() { return this; }


        public event EventHandler ChangePage = null;

        public void OnChangePage(EventArgs e)
        {
            if (this.ChangePage != null)
                this.ChangePage(this, e);
        }


        private void buttonBack_Click(object sender, EventArgs e)
        {
            OnChangePage(EventArgs.Empty);
        }

    }
    
}
