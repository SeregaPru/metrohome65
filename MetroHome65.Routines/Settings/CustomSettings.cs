using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MetroHome65.Routines.Settings
{
    [Serializable]
    public class CustomSettings : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, string propertyName)
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return;
                field = value;
                OnPropertyChanged(propertyName);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion
    }
}