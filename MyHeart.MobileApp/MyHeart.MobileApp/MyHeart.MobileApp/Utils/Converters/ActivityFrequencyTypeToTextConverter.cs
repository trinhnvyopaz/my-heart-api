using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class ActivityFrequencyTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PhysicalActivityFrequencyType)value)
            {
                case PhysicalActivityFrequencyType.None:
                case PhysicalActivityFrequencyType.Once:
                    return "Nízká";  
                case PhysicalActivityFrequencyType.Twice:
                case PhysicalActivityFrequencyType.FourTimes:
                    return "Střední";
                case PhysicalActivityFrequencyType.SixTimes:
                    return "Vysoká";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
