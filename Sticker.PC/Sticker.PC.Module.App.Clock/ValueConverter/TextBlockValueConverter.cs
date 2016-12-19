using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sticker.PC.Module.App.Clock.ValueConverter
{
    public class TextBlockValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double newValue = ((double)value + 280) * -1;
                return newValue;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int newValue = (int)value * -1;
                return newValue;
            }
            catch
            {
                return value;
            }
        }
    }
}
