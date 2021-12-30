// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;

namespace Kikitori.GUI
{
    [ValueConversion(typeof(string), typeof(string))]
    public class WindowSizeConverter : MarkupExtension, IValueConverter
    {
        private static WindowSizeConverter instance;

        public WindowSizeConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance ?? (instance = new WindowSizeConverter());
        }

    }
}
