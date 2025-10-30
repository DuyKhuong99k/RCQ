using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ToolsEx;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
        => value.GetType()
               .GetMember(value.ToString())[0]
               .GetCustomAttribute<DisplayAttribute>()?.Name
           ?? value.ToString();
}