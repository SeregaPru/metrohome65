using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    /// <summary>
    /// Control panel with caption and combobox.
    /// For use in settings forms
    /// </summary>
    public sealed class PopupSettingsControl : StackPanel, INotifyPropertyChanged
    {
        #region Fields

        private readonly TextElement _lblCaption;

        private readonly ComboBox _comboSelect;

        #endregion


        #region Properties

        public String Caption { set { _lblCaption.Text = value; } }

        public List<object> Items
        {
            get { return _comboSelect.Items; }
            set { _comboSelect.Items = value; }
        }

        public int SelectedIndex
        {
            get { return _comboSelect.SelectedIndex; }
            set
            {
                if (_comboSelect.SelectedIndex != value)
                {
                    _comboSelect.SelectedIndex = value;
                    NotifyPropertyChanged("SelectedIndex");
                }
            }
        }

        #endregion


        #region Methods

        public PopupSettingsControl()
        {
            _lblCaption = new TextElement("<select parameter>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            _comboSelect = new ComboBox
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            _comboSelect.SelectedIndexChanged += (s, e) => NotifyPropertyChanged("SelectedIndex"); 
            AddElement(_comboSelect);
        }

        #endregion


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
