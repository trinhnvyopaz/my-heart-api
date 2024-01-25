using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.Models
{
    public enum MenuItemType
    {
        Dashboard,
        MyIllneesses,
        MyTreatments,
        NonpharmaticTherapy,
        MyReports,
        MyQuestions,
        News,
        Profile,
        LogOut
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
