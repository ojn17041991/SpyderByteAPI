using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;

namespace SpyderByteResources.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
            {
                return string.Empty;
            }

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return attributes.First().Description;
        }
    }
}
