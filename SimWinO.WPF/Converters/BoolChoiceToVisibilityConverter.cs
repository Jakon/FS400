using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimWinO.WPF.Converters
{
    public class BoolChoiceToVisibilityConverter : IValueConverter
    {
        public Visibility VisibilityIfTrue { get; set; }
        public Visibility VisibilityIfFalse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? VisibilityIfTrue : VisibilityIfFalse;
            }

            return VisibilityIfFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
