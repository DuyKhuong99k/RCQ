using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToolsEx
{

    public static class ObjectExtensions
    {
        public static T GetPropValueOrDefault<T>(this object? obj, string propertyPath, T defaultValue = default!)
        {
            if (string.IsNullOrEmpty(propertyPath))
                return defaultValue;

            var parts = propertyPath.Split('.');
            object? currentObj = obj;

            foreach (var part in parts)
            {
                if (currentObj == null) return defaultValue;
                if (currentObj is IDictionary<string, object> dict)
                {
                    dict.TryGetValue(part, out currentObj);
                }
                else
                {
                    var prop = currentObj.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop == null)
                        return defaultValue;

                    currentObj = prop.GetValue(currentObj);
                }
            }

            if (currentObj == null) return defaultValue;

            try
            {
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), currentObj.ToString()!);

                return (T)Convert.ChangeType(currentObj, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }

    

}
