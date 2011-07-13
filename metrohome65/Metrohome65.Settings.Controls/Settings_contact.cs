﻿using System;
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.WindowsMobile.Forms;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_contact : UserControl, INotifyPropertyChanged
    {
        private int _Value = -1;

        public Settings_contact()
        {
            InitializeComponent();
        }

        public int Value
        {
            get { return _Value; }
            set {
                if (_Value != value)
                {
                    _Value = value;

                    Contact contact = FindContact(this._Value);
                    labelContactName.Text = (contact != null) ? contact.FileAs : "<Contact not selected>";

                    NotifyPropertyChanged("Value");
                }
            }
        }


        private void buttonClear_Click(object sender, EventArgs e)
        {
            Value = -1;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            Microsoft.WindowsMobile.Forms.ChooseContactDialog ContactDialog = new ChooseContactDialog()
            {
            };

            if (ContactDialog.ShowDialog() == DialogResult.OK)
                Value = ContactDialog.SelectedContact.ItemId.GetHashCode();
        }


        Contact FindContact(int ItemIdKey)
        {
            Contact FindedContact = null;
            OutlookSession mySession = new OutlookSession();

            ContactCollection collection = mySession.Contacts.Items;
            foreach (Contact contact in collection)
            {
                if (contact.ItemId.GetHashCode().Equals(ItemIdKey))
                {
                    FindedContact = contact;
                    break;
                }
            }

            return FindedContact;
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