using System;
using System.Windows.Forms;
using System.IO;

namespace MobilePractices.OpenFileDialogEx
{
	/// <summary>
	/// This is a custom implementation of OpenFileDialog
	/// allowing the user to browse the full filesystem.
	/// </summary>
    public partial class OpenFileDialogEx : Form
    {
		//Some constants here
		const string ROOT = "\\";
		const string GOBACK = "..";

		//private members
		private string currentPath = ROOT;
		private bool updating = false;
		private string filter = string.Empty;
		private string fileName = string.Empty;


		//enum for the file system ListViewItems ImageIndex
		//This is just for readibility
		private enum FileSystemIcon : int
		{
			File=0,
			Directory,
			StorageCard,
			Back
		}
		
		//enum for the filesystem type
		private enum FileSystemKind
		{
			File = 1,
			Directory,
			StorageCard
		}

		// Only the basic constructor is implemented
		// Feel free to implement your own overrides
		public OpenFileDialogEx()
        {
            InitializeComponent();
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdateControls(); //Update the controls
		}

		/// <summary>
		/// Sets/Gets the file search pattern. 
		/// You can use values like "*.*" or "*.txt"
		/// </summary>
		public string Filter
		{
			set { filter = value; }
			get { return filter; }
		}
		
		/// <summary>
		/// Sets/Gets the selected file name to open.
		/// This is the full path.
		/// </summary>
		public string FileName
		{
			set { fileName = value; }
			get { return fileName; }
		}

		// Refill the ComboBox with the path
		// step by step
		private void UpdateSelector()
		{
			PathSelectorComboBox.Items.Clear();
			string[] pathParts = currentPath.Split(Path.DirectorySeparatorChar);

			foreach (string part in pathParts)
			{
				if (part == string.Empty)
					PathSelectorComboBox.Items.Add("\\");
				else
					PathSelectorComboBox.Items.Add(part);
			}
			PathSelectorComboBox.SelectedIndex = pathParts.Length - 1;
		}

		// Refill the file list
		private void UpdateList()
		{
			fileListView.SuspendLayout();
			
			// 1) Clear the list and normalize the current path
			fileListView.Items.Clear();
			if (currentPath == string.Empty) 
				currentPath = ROOT;

			// 2) Add 'Back' if needed
			if (currentPath != ROOT)
			{
				ListViewItem item = new ListViewItem(GOBACK);
				item.ImageIndex = (int) FileSystemIcon.Back;
				fileListView.Items.Add(item);
			}

			// 3) Add directories and/or Storage Cards
			foreach (string entry in Directory.GetDirectories(currentPath))
			{
				ListViewItem item = new ListViewItem(Path.GetFileName(entry));
				FileInfo info = new FileInfo(entry);
				FileSystemKind kind = GetKindFor(info);
				switch(kind)
				{
					case FileSystemKind.StorageCard: 
						item.ImageIndex = (int) FileSystemIcon.StorageCard; 
						break;
					case FileSystemKind.Directory: 
						item.ImageIndex = (int) FileSystemIcon.Directory; 
						break;
				}
				item.Tag = kind;
				fileListView.Items.Add(item);
			}

			// 4) Add Files using filter search pattern
			foreach (string entry in Directory.GetFiles(currentPath, "*.*"))
			{
                if ((filter+";").Contains(Path.GetExtension(entry)))
                {
				    ListViewItem item = new ListViewItem(Path.GetFileName(entry));
				    FileInfo info = new FileInfo(entry);
				    item.ImageIndex = (int) FileSystemIcon.File;
				    item.Tag = FileSystemKind.File;
				    fileListView.Items.Add(item);
                }
			}

			this.ResumeLayout();
		}

		//Helper method which find out the kind of file system entry
		//based on the attributes
		private FileSystemKind GetKindFor(FileSystemInfo info)
		{
			if ((info.Attributes & (FileAttributes.Directory | FileAttributes.Temporary))
				== (FileAttributes.Directory | FileAttributes.Temporary))
				return FileSystemKind.StorageCard;
			else if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
				return FileSystemKind.Directory;
			else
				return FileSystemKind.File;
		}

		//When the user activates an item
		//we can browse the directory or open the file....
		private void fileListView_ItemActivate(object sender, EventArgs e)
		{
			ListView list = sender as ListView;
			
			//We'll continue only if there is a selected item
			if (list != null && list.SelectedIndices.Count >0)
			{
				ListViewItem currentItem = list.Items[list.SelectedIndices[0]];
				string currentFileName = currentItem.Text;

				//If it's not a file we should change the current path
				if ((currentItem.Tag as FileSystemKind?) != FileSystemKind.File)
				{
					if (currentFileName == GOBACK)
						currentPath = currentPath.Substring(0, currentPath.Length - Path.GetFileName(currentPath).Length - 1);
					else
						currentPath = Path.Combine(currentPath, currentFileName);
					UpdateControls();
				}
				else
				{
					//If it's a file, we should return the selected filename
					fileName = Path.Combine(currentPath, currentFileName);
					EndOk();
				}
			}
		}

		// We can change the current path using the PathSelectorComboBox
		private void PathSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Only do this if the view is not being updated
			if (!updating)
			{
				//1) Build the new path
				currentPath = string.Empty;
				for (int i = 0; i <= (sender as ComboBox).SelectedIndex; i++)
				{
					currentPath = Path.Combine(currentPath, PathSelectorComboBox.Items[i].ToString());
				}

				//2) Update the dialog!
				UpdateControls();
			}
		}

		//Update the dialog!
		private void UpdateControls()
		{
			if (updating) return;
			updating = true;
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				UpdateList();
				UpdateSelector();
			}
			finally
			{
				updating = false;
				Cursor.Current = Cursors.Default;
			}
		}


		//We shoud react when the user press ENTER
		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			//When the user press ENTER it could be:
			// 1. Entering the filename to be attached to the current path
			// 2. Entering a new full path (starting with '\' )
			// In both cases it can be a file or a directory, if it's a directory
			// we need to stay on the dialog but showing that directory

			// This is a risky code because we cannot be sure that the user has entered
			// a valid path. Try / Catch for any problem with that.
			if (e.KeyChar == '\r')
			{
				try
				{
					string tempPath = textBox1.Text.Trim(); //Clear white-spaces

					if (!tempPath.StartsWith("\\"))
					{
						tempPath = Path.Combine(currentPath, tempPath);
					}

					//Check whether is a file or a directory
					FileInfo fi = new FileInfo(tempPath);
					if ((fi.Attributes & FileAttributes.Directory) 
						== FileAttributes.Directory)
					{
						//if it's a directory/storagecard
						//try to change the current path
						currentPath = fi.FullName;
						UpdateControls();
					}
					else
					{
						//if it's a file, set the filename and return ok!
						fileName = tempPath;
						EndOk();
					}
				}
				catch
				{ }
			}
		}

		//Returns OK
		private void EndOk()
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		//UP
		//Go to the parent folder by softkey1
		private void menuItem1_Click(object sender, EventArgs e)
		{
			if (currentPath.Length == 0 || currentPath == ROOT)
				return;
			currentPath = currentPath.Substring(0, currentPath.Length - Path.GetFileName(currentPath).Length - 1);
			UpdateControls();
		}

		//CANCEL
		//Cancel by softkey2
		private void menuItem2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

    }
}