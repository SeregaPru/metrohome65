using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using Microsoft.WindowsMobile.Forms;
using MobilePractices.OpenFileDialogEx;

namespace Metrohome65.Settings.Controls
{
    /// <summary>
    /// Control panel with caption and file select dialog.
    /// For use in settings forms
    /// </summary>
    public sealed class FileSettingsControl : StackPanel, INotifyPropertyChanged
    {

        private readonly TextElement _lblCaption;

        private readonly TextBox _inputBox;


        public String Caption { set { _lblCaption.Text = value; } }

        public String Value
        {
            get { return _inputBox.Text; }
            set
            {
                if (_inputBox.Text != value)
                {
                    _inputBox.Text = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }


        public FileSettingsControl(FleuxControlPage settingsPage)
        {
            _lblCaption = new TextElement("<file selection>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            _inputBox = new TextBox(settingsPage)
            {
                Size = new Size(SettingsConsts.MaxWidth, 150),
                MultiLine = true,
            };
            _inputBox.TextChanged += (s, e) => NotifyPropertyChanged("Value");
            AddElement(_inputBox);

            AddElement(new Canvas() { Size = new Size(SettingsConsts.MaxWidth, 10), } );

            var buttonPanel = new Canvas() { Size = new Size(SettingsConsts.MaxWidth, 50), };
            AddElement(buttonPanel);

            var buttonSelectImage = new Fleux.UIElements.Button("select")
            {
                Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                Location = new Point(0, 0),
                TapHandler = p => { BrowseFile(); return true; },
            };
            buttonPanel.AddElement(buttonSelectImage);

            var buttonClearImage = new Fleux.UIElements.Button("clear")
            {
                Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                Location = new Point(SettingsConsts.MaxWidth / 2 + 10, 0),
                TapHandler = p => { Value = ""; return true; },
            };
            buttonPanel.AddElement(buttonClearImage);
        }

        private void BrowseFile()
        {
            var folder = Path.GetDirectoryName(Value);
            if (string.IsNullOrEmpty(folder))
                folder = @"\Windows\Start Menu\Programs";

            var dialog = new SelectPictureDialog
            {
                Filter = "Program files|*.exe;*.lnk",
                Title = "Select program",
                SortOrder = SortOrder.NameAscending,
                InitialDirectory = folder,
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                Value = dialog.FileName;
            /*
            var dialog = new OpenFileDialogEx { Filter = "*.exe;*.lnk", };

            if (dialog.ShowDialog() == DialogResult.OK)
                Value = dialog.FileName;
            */ 
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }
}
