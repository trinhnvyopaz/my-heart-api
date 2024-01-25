using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class PersonalRecordViewModel : BaseViewModel
    {
        private string value;
        private DateTime datetime;
        private TimeSpan time;

        public int Id { get; set; }
        public AnamnesisType Type { get; set; }
        public string Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
        public DateTime Date
        {
            get => datetime;
            set => SetProperty(ref datetime, value);
        }

        public TimeSpan Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }
        public PersonalAnamnesisCreatorType IsPersonal_CreatorType { get; set; }
        public PersonalRecordViewModel()
        {

        }
        public PersonalRecordViewModel(UserAnamnesisDTO userAnamnesis)
        {
            Id = userAnamnesis.Id;
            Type = (AnamnesisType)userAnamnesis.IsPersonal_Type;
            Value = userAnamnesis.IsPersonal_Value;
            Date = userAnamnesis.IsPersonal_Date;
            Time = userAnamnesis.IsPersonal_Date.TimeOfDay;
            IsPersonal_CreatorType = userAnamnesis.IsPersonal_CreatorType;
        }
    }
}
