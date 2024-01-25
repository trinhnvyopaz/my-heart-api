using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MyHeart.MobileApp.iOS
{
    public class SnackbarView : UIView
    {
        private const double AnimationDuration = 0.3;
        private const double DisplayDuration = 3.0;

        private UILabel headlineLabel;
        private UILabel descriptionLabel;

        private UITapGestureRecognizer tapGestureRecognizer;
        private NSTimer hideTimer;

        public SnackbarView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.FromRGB(227, 227, 225); // Customize the background color

            headlineLabel = new UILabel
            {
                Frame = new CGRect(20, 40, Frame.Width - 40, 30),
                Font = UIFont.BoldSystemFontOfSize(20),
                Text = "Headline",
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
            };

            AddSubview(headlineLabel);

            descriptionLabel = new UILabel
            {
                Frame = new CGRect(20, 80, Frame.Width - 40, 20),
                Font = UIFont.SystemFontOfSize(14),
                Text = "This is a smaller text label.",
                TextColor = UIColor.FromRGB(46, 46, 45),
                TextAlignment = UITextAlignment.Left,
            };
            AddSubview(descriptionLabel);

            tapGestureRecognizer = new UITapGestureRecognizer(Hide);
            AddGestureRecognizer(tapGestureRecognizer);
        }

        public void Show(string title, string message)
        {
            headlineLabel.Text = title;
            descriptionLabel.Text = message;

            UIView.Animate(AnimationDuration, () =>
            {
                Frame = new CGRect(Frame.Left, 0, Frame.Width, Frame.Height);
            }, () =>
            {
                hideTimer = NSTimer.CreateScheduledTimer(DisplayDuration, _ => Hide(null));
            });
        }

        private void Hide(UITapGestureRecognizer gestureRecognizer)
        {
            hideTimer?.Invalidate();
            hideTimer = null;

            UIView.Animate(AnimationDuration, () =>
            {
                Frame = new CGRect(Frame.Left, -Frame.Height, Frame.Width, Frame.Height);
            }, () =>
            {
                RemoveFromSuperview();
            });
        }
    }
}