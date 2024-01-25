using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Converters
{
    public class QuestionStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (QuestionStatus)value;
            switch (status)
            {
                case QuestionStatus.Open:
                    return ImageSource.FromResource("MyHeart.MobileApp.Resources.Icons.done.png", typeof(QuestionStatusToIconConverter).GetTypeInfo().Assembly);
                case QuestionStatus.AwaitingPatientResponse:
                    return ImageSource.FromResource("MyHeart.MobileApp.Resources.Icons.hourglass.png", typeof(QuestionStatusToIconConverter).GetTypeInfo().Assembly);
                case QuestionStatus.AwaitingDoctorResponse:
                    return "";
                case QuestionStatus.Closed:
                    return "";
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
