using DevExpress.XamarinForms.Charts;
using MyHeart.MobileApp.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Views.PopUp
{
    public class AxisLabelFormatter : BindableObject, IAxisLabelTextFormatter
    {
        public static readonly BindableProperty MeasureUnitProperty = BindableProperty.Create(nameof(MeasureUnit), typeof(DateTimeMeasureUnit), typeof(AxisLabelFormatter));
        object value;
        public DateTimeMeasureUnit MeasureUnit
        {
            get => (DateTimeMeasureUnit)GetValue(MeasureUnitProperty);
            set
            {
                SetValue(MeasureUnitProperty, value);
                if (this.value != null)
                {
                    Format(this.value);
                }
            }
        }

        public string Format(object value)
        {
            var date = (DateTime)value;
            this.value = value;
            switch (MeasureUnit)
            {
                case DateTimeMeasureUnit.Day:
                    return date.ToString("dd"); ;
                case DateTimeMeasureUnit.Week:
                    var uiCulture = Thread.CurrentThread.CurrentUICulture;
                    return uiCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                case DateTimeMeasureUnit.Month:
                    return date.ToString("MM"); ;
                case DateTimeMeasureUnit.Year:
                    return date.ToString("yyyy"); ;
                default:
                    return "";
            }
        }
    }
}
