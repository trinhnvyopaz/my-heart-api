using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.Services
{
    public interface IKeyboardService
    {
        float Height { get; set; }
        event EventHandler<KeyBoardSizeChangedEventArgs> OnKeyBoardSizeChanged;
    }

    public class KeyBoardSizeChangedEventArgs : EventArgs
    {
        public float Height { get; set; }

        public KeyBoardSizeChangedEventArgs(float height)
        {
            Height = height;
        }
    }
}
