﻿using System.ComponentModel;
using System.Reflection;

namespace SpyderByteAPI.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            // OJN: This needs cleaning up.
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
