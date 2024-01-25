using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.iOS.Extensions
{
	public static class CGPointExtensions
	{
		public static CGPoint Scaled(this CGPoint self, CGSize size)
		{
			return new CGPoint(self.X * size.Width, self.Y * size.Height);
		}
	}
}
