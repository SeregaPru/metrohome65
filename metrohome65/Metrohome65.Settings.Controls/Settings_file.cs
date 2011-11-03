﻿using System;
using System.Windows.Forms;
using System.ComponentModel;
using MobilePractices.OpenFileDialogEx;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_file : UserControl, INotifyPropertyChanged
    {
        private String _ExeFile = "";

        public String Caption { set { lblCaption.Text = value; } }

        public String Value { 
            get { return _ExeFile; } 
            set {
                if (_ExeFile != value)
                {
                    _ExeFile = value;
                    txtCommandLine.Text = _ExeFile;
                    NotifyPropertyChanged("Value");
                }
            } 
        }


        public Settings_file()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialogEx dialog = new OpenFileDialogEx();
            dialog.Filter = "*.exe;*.lnk";

            if (dialog.ShowDialog() == DialogResult.OK)
                Value = dialog.FileName;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Value = "";
        }

        private void txtCommandLine_TextChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Value");
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
