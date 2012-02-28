using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace MetroHome65.Routines
{
    public class BindingManager
    {
        public class BindingInfo
        {
            public INotifyPropertyChanged source;
            public string sourcePropertyName;
            public INotifyPropertyChanged target;
            public string targetPropertyName;
        }

        private List<BindingInfo> Binding = new List<BindingInfo>();

        public BindingManager()
        {
        }

        public void Bind(
            INotifyPropertyChanged source, string sourcePropertyName,
            INotifyPropertyChanged target, string targetPropertyName)
        {
            BindingInfo BindingInfo = new BindingInfo();
            BindingInfo.source = source;
            BindingInfo.sourcePropertyName = sourcePropertyName;
            BindingInfo.target = target;
            BindingInfo.targetPropertyName = targetPropertyName;

            Binding.Add(BindingInfo);

            var sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            var targetProperty = target.GetType().GetProperty(targetPropertyName);

            source.PropertyChanged += PropertyChangedEventHandler;
            target.PropertyChanged += PropertyChangedEventHandler;
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            foreach (BindingInfo BindingInfo in Binding)
            {
                if ((BindingInfo.source == sender) && (BindingInfo.sourcePropertyName == e.PropertyName))
                {
                    var sourceProperty = BindingInfo.source.GetType().GetProperty(BindingInfo.sourcePropertyName);
                    var sourceValue = sourceProperty.GetValue(BindingInfo.source, null);
                    var targetProperty = BindingInfo.target.GetType().GetProperty(BindingInfo.targetPropertyName);
                    var targetValue = targetProperty.GetValue(BindingInfo.target, null);
                    if (!Object.Equals(sourceValue, targetValue))
                    {
                        targetProperty.SetValue(BindingInfo.target, sourceValue, null);
                    }
                    return;
                }

                if ((BindingInfo.target == sender) && (BindingInfo.targetPropertyName == e.PropertyName))
                {
                    var sourceProperty = BindingInfo.source.GetType().GetProperty(BindingInfo.sourcePropertyName);
                    var sourceValue = sourceProperty.GetValue(BindingInfo.source, null);
                    var targetProperty = BindingInfo.target.GetType().GetProperty(BindingInfo.targetPropertyName);
                    var targetValue = targetProperty.GetValue(BindingInfo.target, null);
                    if (!Object.Equals(sourceValue, targetValue))
                    {
                        sourceProperty.SetValue(BindingInfo.source, targetValue, null);
                    }
                    return;
                }
            }
        }
    }
}
