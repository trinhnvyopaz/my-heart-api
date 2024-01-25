using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.User
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewsDetailPage : ContentPage
	{
		public NewsDetailPage (TherapyNewsDTO therapyNews)
		{
			BindingContext = new NewsDetailPageViewModel(therapyNews);
			InitializeComponent ();
		}
	}
}