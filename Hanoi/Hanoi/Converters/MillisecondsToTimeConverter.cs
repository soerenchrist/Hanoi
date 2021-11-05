
using Hanoi.Util;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Hanoi.Converters
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timespan)
            {
                return timespan.Format();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
