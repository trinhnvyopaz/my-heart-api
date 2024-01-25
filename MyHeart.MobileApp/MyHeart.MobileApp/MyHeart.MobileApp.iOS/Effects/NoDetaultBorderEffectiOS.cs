using Foundation;
using MyHeart.MobileApp.iOS.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("effects")]
[assembly: ExportEffect(typeof(NoDetaultBorderEffectiOS), nameof(NoDetaultBorderEffectiOS))]
namespace MyHeart.MobileApp.iOS.Effects
{
    public class NoDetaultBorderEffectiOS : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is UITextField textField)
            {
                textField.BorderStyle = UITextBorderStyle.None;
            }
        }

        protected override void OnDetached()
        {

        }
    }
}