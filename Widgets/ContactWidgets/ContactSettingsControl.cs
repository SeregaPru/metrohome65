using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Styles;
using Fleux.UIElements;
using Microsoft.WindowsMobile.Forms;
using Microsoft.WindowsMobile.PocketOutlook;

namespace Metrohome65.Settings.Controls
{
    /// <summary>
    /// Control panel with caption and contact select dialog.
    /// For use in settings forms
    /// </summary>
    public sealed class ContactSettingsControl : StackPanel, INotifyPropertyChanged
    {
        private const string ContactNotSelected = "<Contact not selected>";

        private readonly TextElement _lblCaption;

        private readonly TextElement _labelContactName;

        private int _value = -1;


        public String Caption { set { _lblCaption.Text = value; } }

        public int Value
        {
            get { return _value; }
            set {
                if (_value != value)
                {
                    _value = value;

                    var contact = FindContact(_value);
                    _labelContactName.Text = (contact != null) ? contact.FileAs : ContactNotSelected;

                    NotifyPropertyChanged("Value");
                }
            }
        }


        public ContactSettingsControl()
        {
            _lblCaption = new TextElement("<contact selection>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            _labelContactName = new TextElement(ContactNotSelected)
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
                Style = MetroTheme.PhoneTextAccentStyle,
            };
            AddElement(_labelContactName);

            AddElement(new Canvas { Size = new Size(SettingsConsts.MaxWidth, 10), } );

            var buttonPanel = new Canvas { Size = new Size(SettingsConsts.MaxWidth, 50), };
            AddElement(buttonPanel);

            var buttonSelectImage = new Fleux.UIElements.Button("select")
            {
                Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                Location = new Point(0, 0),
                TapHandler = p => { BrowseContact(); return true; },
            };
            buttonPanel.AddElement(buttonSelectImage);

            var buttonClearImage = new Fleux.UIElements.Button("clear")
            {
                Size = new Size(SettingsConsts.MaxWidth / 2 - 10, 50),
                Location = new Point(SettingsConsts.MaxWidth / 2 + 10, 0),
                TapHandler = p => { Value = -1; return true; },
            };
            buttonPanel.AddElement(buttonClearImage);
        }

        private void BrowseContact()
        {
            var contactDialog = new ChooseContactDialog();

            if (contactDialog.ShowDialog() == DialogResult.OK)
                Value = contactDialog.SelectedContact.ItemId.GetHashCode();
        }

        private Contact FindContact(int itemIdKey)
        {
            Contact findedContact = null;
            var mySession = new OutlookSession();

            foreach (var contact in mySession.Contacts.Items)
            {
                if (contact.ItemId.GetHashCode().Equals(itemIdKey))
                {
                    findedContact = contact;
                    break;
                }
            }

            return findedContact;
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
