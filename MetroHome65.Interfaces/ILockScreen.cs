using System;
using System.Collections.Generic;
using System.ComponentModel;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Routines;

namespace MetroHome65.Interfaces
{
    /// <summary>
    /// Attribute - tile description, that will be diplayed in properties page
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LockScreenInfoAttribute : Attribute
    {
        private readonly String _caption = "";

        public String Caption { get { return _caption; } }


        public LockScreenInfoAttribute() { }

        public LockScreenInfoAttribute(String caption)
        {
            _caption = caption;
        }
    }

    /// <summary>
    /// Mark field or property of lockscreen, that holds settings for this plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class LockScreenSettingsAttribute : Attribute
    { }

    /// <summary>
    /// Mark field or property of settings that should be stored in file
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class LockScreenParameterAttribute : Attribute
    { }


    public interface ILockScreenSettings : INotifyPropertyChanged
    {
        ICollection<UIElement> EditControls(FleuxControlPage settingsPage, BindingManager bindingManager);
    }

    /// <summary>
    /// Lock screen base interface
    /// </summary>
    public interface ILockScreen
    {
        void ApplySettings(ILockScreenSettings settings);
    }

}
