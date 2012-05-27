using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Controls;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    /// <summary>
    /// Control panel with caption and string input box.
    /// For use in settings forms
    /// </summary>
    public sealed class StringSettingsControl : StackPanel, INotifyPropertyChanged
    {
        private readonly TextElement _lblCaption;

        private readonly TextBox _inputBox;

        public String Caption { set { _lblCaption.Text = value; } }
        
        public String Value
        {
            get { return _inputBox.Text; }
            set
            {
                if (_inputBox.Text == value) return;
                _inputBox.Text = value;
                NotifyPropertyChanged("Value");
            }
        }


        public StringSettingsControl(FleuxControlPage settingsPage)
        {
            _lblCaption = new TextElement("<input parameter>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            _inputBox = new TextBox(settingsPage) 
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
                MultiLine = false,
            };
            _inputBox.TextChanged += (s, e) => NotifyPropertyChanged("Value"); 
            AddElement(_inputBox);
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
