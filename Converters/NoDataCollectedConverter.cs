using System;
using System.Windows.Data;
using RssStarterKit.Localization;

namespace RssStarterKit.Converters
{
    public class NoDataCollectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return AppResources.NoDataCollectedConverterText;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}