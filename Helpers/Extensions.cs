using System;
using System.Xml.Linq;

namespace RssStarterKit.Helpers
{
    public static class Extensions
    {
        public static DateTime? GetSafeElementDate(this XElement item, string elementName)
        {
            DateTime date;
            var element = item.Element(elementName);
            if (element == null)
                return null;
            if (DateTime.TryParse(element.Value, out date))
                return date;
            else
                return null;
        }

        public static string GetSafeElementString(this XElement item, string elementName)
        {
            var value = item.Element(elementName);
            if (value != null)
                return value.Value;
            else
                return String.Empty;
        }

        public static string GetSafeElementString(this XElement item, XName elementName)
        {
            var value = item.Element(elementName);
            if (value != null)
                return value.Value;
            else
                return String.Empty;
        }
    }
}