using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    /// <summary>
    /// Control panel with caption and font select box.
    /// For use in settings forms
    /// </summary>
    public sealed class FontSettingsControl : StackPanel, INotifyPropertyChanged
    {
        private readonly TextElement _lblCaption;

        private readonly FontEdit _fontEdit;

        public String Caption { set { _lblCaption.Text = value; } }
        
        public TextStyle Value
        {
            get { return _fontEdit.Value; }
            set
            {
                if (_fontEdit.Value == value) return;
                _fontEdit.Value = value;
                NotifyPropertyChanged("Value");
            }
        }


        public FontSettingsControl()
        {
            _lblCaption = new TextElement("<input parameter>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            _fontEdit = new FontEdit {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            _fontEdit.Value.PropertyChanged += (s, e) => NotifyPropertyChanged("Value");
            AddElement(_fontEdit);
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
