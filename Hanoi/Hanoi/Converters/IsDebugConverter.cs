using System;
using System.Globalization;
using Xamarin.Forms;

namespace Hanoi.Converters
{
    public class IsDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
