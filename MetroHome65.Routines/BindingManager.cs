﻿using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace MetroHome65.Routines
{
    public class BindingManager
    {
        public class BindingInfo
        {
            public INotifyPropertyChanged Source;
            public string SourcePropertyName;
            public INotifyPropertyChanged Target;
            public string TargetPropertyName;
        }


        private readonly List<BindingInfo> _binding = new List<BindingInfo>();

        public bool MultiBind = false;

        public void Bind(
            INotifyPropertyChanged source, string sourcePropertyName,
            INotifyPropertyChanged target, string targetPropertyName)
        {
            var bindingInfo = new BindingInfo
                                  {
                                      Source = source,
                                      SourcePropertyName = sourcePropertyName,
                                      Target = target,
                                      TargetPropertyName = targetPropertyName
                                  };

            _binding.Add(bindingInfo);

            source.PropertyChanged += PropertyChangedEventHandler;
            target.PropertyChanged += PropertyChangedEventHandler;
        }

        public void Bind(
            INotifyPropertyChanged source, string sourcePropertyName,
            INotifyPropertyChanged target, string targetPropertyName, bool initialSet)
        {
            Bind(source, sourcePropertyName, target, targetPropertyName);

            // initial set target property value to source value
            if (initialSet)
                PropertyChangedEventHandler(source, new PropertyChangedEventArgs(sourcePropertyName));
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                foreach (var bindingInfo in _binding)
                {
                    if ((bindingInfo.Source == sender) && (bindingInfo.SourcePropertyName == e.PropertyName))
                    {
                        var sourceProperty = bindingInfo.Source.GetType().GetProperty(bindingInfo.SourcePropertyName);
                        var sourceValue = sourceProperty.GetValue(bindingInfo.Source, null);
                        var targetProperty = bindingInfo.Target.GetType().GetProperty(bindingInfo.TargetPropertyName);
                        var targetValue = targetProperty.GetValue(bindingInfo.Target, null);
                        if (!Object.Equals(sourceValue, targetValue))
                        {
                            targetProperty.SetValue(bindingInfo.Target, sourceValue, null);
                        }
                        if (!MultiBind) return;
                        continue;
                    }

                    if ((bindingInfo.Target == sender) && (bindingInfo.TargetPropertyName == e.PropertyName))
                    {
                        var sourceProperty = bindingInfo.Source.GetType().GetProperty(bindingInfo.SourcePropertyName);
                        var sourceValue = sourceProperty.GetValue(bindingInfo.Source, null);
                        var targetProperty = bindingInfo.Target.GetType().GetProperty(bindingInfo.TargetPropertyName);
                        var targetValue = targetProperty.GetValue(bindingInfo.Target, null);
                        if (!Object.Equals(sourceValue, targetValue))
                        {
                            sourceProperty.SetValue(bindingInfo.Source, targetValue, null);
                        }
                        if (!MultiBind) return;
                    }
                }
            }
            catch (Exception) { }
        }

    }
}
