using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class ElapsedDaysConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var elapsedDays = (DateTime.Now - (DateTime)value).Days;

            if (elapsedDays == 0)
            {
                return "Dnes";
            }
            else
            {
                return $"Před {elapsedDays} dny";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
