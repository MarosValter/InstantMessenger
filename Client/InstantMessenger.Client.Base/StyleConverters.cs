using System;
using System.Windows.Data;

namespace InstantMessenger.Client.Base
{
    public class ScrollbarOnFarLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            return ((double)value > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollbarOnFarRightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return false;
            if (values[0] == null || values[1] == null || values[2] == null) return false;
            if (values[0].Equals(double.NaN) || values[1].Equals(double.NaN) || values[2].Equals(double.NaN)) return false;

            double dblHorizontalOffset = 0;
            double dblViewportWidth = 0;
            double dblExtentWidth = 0;

            double.TryParse(values[0].ToString(), out dblHorizontalOffset);
            double.TryParse(values[1].ToString(), out dblViewportWidth);
            double.TryParse(values[2].ToString(), out dblExtentWidth);

            bool fOnFarRight = Math.Round((dblHorizontalOffset + dblViewportWidth), 2) < Math.Round(dblExtentWidth, 2);
            return fOnFarRight;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
