using HotelFood.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace HotelFood.Helpers
{
    public static class EnumHelper<T> 
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }
        public static IList<T> GetValues()
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(typeof(T), fi.Name, false));
            }
            return enumValues;
        }
        public static IEnumerable<SelectListItem> GetSelectListWithIntegerValues(T value, SharedViewLocalizer localizer = null)
        {
           return EnumHelper<T>.GetValues().Select(s => new SelectListItem() { Text = EnumHelper<T>.GetDisplayValue(s, localizer), 
               Value = Convert.ToInt32(s).ToString(), Selected = Convert.ToInt32(value) == Convert.ToInt32(s)
           });
        }
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value, SharedViewLocalizer localizer=null)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj), localizer)).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value, SharedViewLocalizer localizer=null)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false);
            
            if (descriptionAttributes.Length > 0)
            {
                var dispattr = descriptionAttributes[0] as DisplayAttribute;
                
                if (dispattr.ResourceType != null)
                    return lookupResource(dispattr.ResourceType, dispattr.Name);
                if(localizer!=null)
                 return localizer[dispattr.Name];
            }
            if (localizer != null)
                return localizer[value.ToString()];
            return value.ToString();
        }
    }
}