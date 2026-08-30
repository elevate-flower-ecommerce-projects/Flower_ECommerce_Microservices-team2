using System.ComponentModel;
namespace Cart_ServiceCart_Service.Common.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute = field?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        return attribute?.Description ?? value.ToString();
    }
}
