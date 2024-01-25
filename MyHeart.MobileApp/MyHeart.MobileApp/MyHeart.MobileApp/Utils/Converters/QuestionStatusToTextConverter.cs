using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class QuestionStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (QuestionStatus)value;
            switch (status)
            {
                case QuestionStatus.Open:
                    return "Odpověď";
                case QuestionStatus.AwaitingPatientResponse:
                    return "Čeká na vaši odpověď";
                case QuestionStatus.AwaitingDoctorResponse:
                    return "Čeká na odpověď lékaře";
                case QuestionStatus.Closed:
                    return "Ukončené";
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
