using System;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.UIElements;

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
    /// Mark field or property of lockscreen, that it is user defined parameter,
    /// and it should be stored in lockscreen settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class LockScreenParameterAttribute : Attribute
    {
    }


    /// <summary>
    /// Lock screen base interface
    /// </summary>
    public interface ILockScreen
    {
        ICollection<UIElement> EditControls(FleuxControlPage settingsPage);
    }

}
