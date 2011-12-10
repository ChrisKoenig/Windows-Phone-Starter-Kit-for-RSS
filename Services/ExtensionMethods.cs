using System;

namespace RssStarterKit.Services
{
    public static class ExtensionMethods
    {
        public static string ToSafeString(this object o, string defaultValue = "")
        {
            if (o == null)
                return defaultValue;
            else
                return o.ToString();
        }
    }
}