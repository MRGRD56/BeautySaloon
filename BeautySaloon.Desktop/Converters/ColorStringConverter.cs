using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BeautySaloon.Desktop.Converters
{
    /// <summary>
    /// Конвертирует цвет в HEX-формате в <see cref="SolidColorBrush"/>.
    /// </summary>
    public class ColorStringConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует цвет в HEX-формате в <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="value">Цвет в HEX-формате без '#' в начале.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexColor = (string) value;
            return (SolidColorBrush) new BrushConverter().ConvertFromString($"#{hexColor}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
