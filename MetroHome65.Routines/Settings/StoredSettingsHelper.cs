using System;
using System.Collections.Generic;
using System.Linq;
using MetroHome65.Routines.File;

namespace MetroHome65.Routines.Settings
{
    public static class StoredSettingsHelper  
    {
        public static void StoredSettingsToObject(List<StoredParameter> settingsList, object targetObject, Type attributeType)
        {
            if (settingsList == null) return;

            // get object properties info
            var propertyInfos = (targetObject.GetType().GetProperties()).Where(
                propertyInfo => propertyInfo.GetCustomAttributes(attributeType, true).Length > 0).ToList();

            // copy properties values
            foreach (var param in settingsList)
            {
                foreach (var propertyInfo in propertyInfos.Where(propertyInfo => propertyInfo.Name == param.Name))
                {
                    propertyInfo.SetValue(targetObject, Convert.ChangeType(param.Value, propertyInfo.PropertyType, null), null);
                    break;
                }
            }
        }

        public static void StoredSettingsFromObject(ref List<StoredParameter> settingsList, object sourceObject, Type attributeType)
        {
            if (settingsList == null)
                settingsList = new List<StoredParameter>();
            else
                settingsList.Clear();

            try
            {
                var srcProps = sourceObject.GetType().GetProperties();
                foreach (var srcPropInfo in srcProps)
                {
                    // set only properties that are marked as tile parameter
                    var attributes = srcPropInfo.GetCustomAttributes(attributeType, true);
                    if (attributes.Length > 0)
                    {
                        settingsList.Add(new StoredParameter(srcPropInfo.Name, srcPropInfo.GetValue(sourceObject, null).ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "serialize settings error");
            }

        }
        
    }
}