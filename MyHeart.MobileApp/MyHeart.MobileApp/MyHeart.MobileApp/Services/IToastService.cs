using MyHeart.MobileApp.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.Services
{
    public interface IToastService
    {
        void ShowSuccess(string message, NotificationPosition position = NotificationPosition.Bottom);
        void ShowError(string message, NotificationPosition position = NotificationPosition.Bottom);
    }
}
