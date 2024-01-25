using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels.Messages
{
    public class InitVideoMessage
    {
        public VideoRoom VideoRoom { get; set; } 
        public UserDTO User { get; set; } 
    }
}
