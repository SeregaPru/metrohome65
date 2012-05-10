using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    public sealed class ColorSettingsControl : StackPanel, INotifyPropertyChanged
    {
        #region Fields

        private readonly TextElement _lblCaption;

        private readonly ColorComboBox _comboSelect;

        #endregion


        #region Properties

        public String Caption { set { _lblCaption.Text = value; } }

        public Color Value
        {
            get
            {
                return ((ColorItem)_comboSelect.Items[_comboSelect.SelectedIndex]).Color;
            }
            set
            {
                if (((ColorItem)_comboSelect.Items[_comboSelect.SelectedIndex]).Color != value)
                {
                    var idx = 0;
                    for (int i = 0; i < _comboSelect.Items.Count; i++)
                    {
                        if (((ColorItem)_comboSelect.Items[i]).Color.ToArgb() == value.ToArgb())
                        {
                            idx = i;
                            break;
                        }
                    }
                    _comboSelect.SelectedIndex = idx;

                    NotifyPropertyChanged("Value");
                }
            }
        }

        public int ARGBValue
        {
            get { return Value.ToArgb(); }
            set
            {
                Value = Color.FromArgb(value);
                NotifyPropertyChanged("ARGBValue");
            }
        }

        #endregion


        #region Methods

        public ColorSettingsControl(bool withDefaultColor)
        {
            _lblCaption = new TextElement("<select color>")
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
            };
            AddElement(_lblCaption);

            // colors consts for wp7 - see http://msdn.microsoft.com/en-us/library/ff402557%28v=vs.92%29.aspx
            var items = new List<object>()
                            {
                                new ColorItem(Color.FromArgb(216,0,115), "magenta"),
                                new ColorItem(Color.FromArgb(162,0,255), "purple"),
                                new ColorItem(Color.FromArgb(0,171,169), "teal"),
                                new ColorItem(Color.FromArgb(162,193,57), "lime"),
                                new ColorItem(Color.FromArgb(160,80,0), "brown"),
                                new ColorItem(Color.FromArgb(230,113,184), "pink"),
                                new ColorItem(Color.FromArgb(240,150,9), "orange"),
                                new ColorItem(Color.FromArgb(27,161,226), "blue"),
                                new ColorItem(Color.FromArgb(229,20,0), "red"),
                                new ColorItem(Color.FromArgb(51,153,51), "green"),
                            };
            if (withDefaultColor)
                _comboSelect.Items.Insert(0, new ColorItem(Color.Empty, "<default>"));

            _comboSelect = new ColorComboBox
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
                Items = items,
            };

            _comboSelect.SelectedIndexChanged += (s, e) =>
                                                     {
                                                         NotifyPropertyChanged("Value");
                                                         NotifyPropertyChanged("ARGBValue");
                                                     };
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
