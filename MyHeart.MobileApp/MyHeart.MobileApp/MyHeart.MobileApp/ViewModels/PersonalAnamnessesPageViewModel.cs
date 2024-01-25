using MyHeart.MobileApp.ViewModels;
using System.Collections.Generic;
using MyHeart.DTO.User;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using MyHeart.MobileApp.Services;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Views
{
    public class PersonalAnamnessesPageViewModel : BaseViewModel
    {
        private AnamnesisService anamnesisService;

        public ObservableCollection<PersonalRecordViewModel> Records { get; } // = new ObservableCollection<PersonalRecordViewModel>();
        public PersonalAnamnessesPageViewModel(ObservableCollection<PersonalRecordViewModel> data)
        {
            Records = data;
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
            //foreach (var record in data.Select(ua => new PersonalRecordViewModel() {
            //            Type = PersonalMapper.mapType(ua.IsPersonal_Type),
            //            Datetime = ua.IsPersonal_Date,
            //            Value = ua.IsPersonal_Value
            //        })
            //    )
            //{
            //    Records.Add(record);
            //}
        }
    }
}