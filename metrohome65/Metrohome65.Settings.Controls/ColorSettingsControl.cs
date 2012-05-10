﻿using System;
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

            _comboSelect = new ColorComboBox
            {
                Size = new Size(SettingsConsts.MaxWidth, 50),
                Items = new List<object>()
                                    {
                                        new ColorItem(Color.Magenta, "magenta"), 
                                        new ColorItem(Color.Purple, "purple"),  
                                        new ColorItem(Color.Teal, "teal"), 
                                        new ColorItem(Color.Lime, "lime"), 
                                        new ColorItem(Color.Brown, "brown"), 
                                        new ColorItem(Color.Pink, "pink"), 
                                        new ColorItem(Color.Orange, "orange"), 
                                        new ColorItem(Color.Blue, "blue"), 
                                        new ColorItem(Color.Red, "red"), 
                                        new ColorItem(Color.Green, "green")
                                    },
            };

            if (withDefaultColor)
            {
                _comboSelect.Items.Insert(0, new ColorItem(Color.Empty, "<default>"));
                _comboSelect.SelectedIndex = 0;
            }

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
