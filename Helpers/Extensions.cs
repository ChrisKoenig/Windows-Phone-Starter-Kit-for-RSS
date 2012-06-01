using System;
using System.Linq;
using System.Xml.Linq;

namespace RssStarterKit.Helpers
{
    public static class Extensions
    {
        public static string OrNoneProvided(this string value, string @default = "None provided")
        {
            if (value != null && value.Trim().Length > 0)
                return value;
            else
                return @default;
        }

        public static string GetLink(this XElement item, string rel)
        {
            var links = item.Elements(item.GetDefaultNamespace() + "link");
            var link = from l in links
                       where l.Attribute("rel").Value == rel
                       select l.Attribute("href").Value;
            return link.FirstOrDefault();
        }

        public static DateTime? GetSafeElementDate(this XElement item, string elementName)
        {
            DateTime date;
            var element = item.Element(item.GetDefaultNamespace() + elementName);
            if (element == null)
                return null;
            if (DateTime.TryParse(element.Value, out date))
                return date;
            else
                return null;
        }

        public static string GetSafeElementString(this XElement item, string elementName)
        {
            if (item == null) return String.Empty;
            var value = item.Element(item.GetDefaultNamespace() + elementName);
            if (value != null)
                return value.Value;
            else
                return String.Empty;
        }

        public static string GetSafeElementString(this XElement item, XName elementName)
        {
            if (item == null) return String.Empty;
            var value = item.Element(elementName);
            if (value != null)
                return value.Value;
            else
                return String.Empty;
        }
    }
}