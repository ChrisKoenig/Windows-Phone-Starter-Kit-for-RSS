using System;
using System.Windows.Data;

namespace RssStarterKit.Converters
{
    public class ImageNotAvailableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new Uri("/Images/FeedType/RSS.jpg", UriKind.Relative);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}