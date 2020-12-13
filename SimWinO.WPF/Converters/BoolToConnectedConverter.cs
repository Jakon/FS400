using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SimWinO.WPF.Converters
{
    public class BoolToConnectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
            {
                return isConnected ? new SolidColorBrush(Colors.ForestGreen) : new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
