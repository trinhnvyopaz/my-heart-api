using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class QuestionStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (QuestionStatus)value;
            switch (status)
            {
                case QuestionStatus.Open:
                    return Application.Current.Resources["SuccessColor"];
                case QuestionStatus.AwaitingPatientResponse:
                    return Application.Current.Resources["WarningColor"];
                case QuestionStatus.AwaitingDoctorResponse:
                    return Application.Current.Resources["WarningColor"];
                case QuestionStatus.Closed:
                    return Application.Current.Resources["ErrorColor"];
                default:
                    return Application.Current.Resources["SuccessColor"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
