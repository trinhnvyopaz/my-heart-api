using MyHeart.MobileApp.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class ChartDateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;
            var type = (ChartType)parameter;

            switch (type)
            {
                case ChartType.Day:
                    return date.ToString("dd.MM");;
                case ChartType.Weak:
                    var uiCulture = Thread.CurrentThread.CurrentUICulture;
                    return uiCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);                                   
                case ChartType.Month:
                    return date.ToString("MM.yy"); ;
                case ChartType.Year:
                    return date.ToString("yyyy"); ;
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum ChartType
    {
        Day,
        Weak,
        Month,
        Year
    }
}
