using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class ActivityFrequencyTypeToColorConverter : IValueConverter
    {
        public Color WarningColor = (Color)Application.Current.Resources["WarningColor"];
        public Color SuccessColor = (Color)Application.Current.Resources["SuccessColor"];
        public Color MainColor = (Color)Application.Current.Resources["MainColor"];
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PhysicalActivityFrequencyType)value)
            {
                case PhysicalActivityFrequencyType.Twice:
                    return WarningColor;
                case PhysicalActivityFrequencyType.FourTimes:
                    return SuccessColor;
                default:
                    return MainColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
