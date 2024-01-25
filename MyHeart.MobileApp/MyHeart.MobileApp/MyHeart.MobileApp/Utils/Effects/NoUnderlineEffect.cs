using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.Effects
{
    public class NoUnderlineEffect : RoutingEffect
    {
        public NoUnderlineEffect() : base($"effects.{nameof(NoUnderlineEffect)}")
        {
        }
    }
}
