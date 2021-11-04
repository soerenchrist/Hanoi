
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Hanoi.Converters
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long millis)
            {
                var timespan = new TimeSpan(millis * 10000);
                return timespan.ToString(@"hh\:mm\:ss");
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
