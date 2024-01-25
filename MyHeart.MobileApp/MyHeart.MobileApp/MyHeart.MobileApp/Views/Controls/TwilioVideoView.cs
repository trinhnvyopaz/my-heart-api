using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Views.Controls
{
    public class TwilioVideoView : View
    {
        public static readonly BindableProperty TokenProperty =
            BindableProperty.Create(nameof(Token), typeof(string), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty RoomNameProperty =
            BindableProperty.Create(nameof(RoomName), typeof(string), typeof(TwilioVideoView));

        public static readonly BindableProperty IsJoiningRoomProperty =
            BindableProperty.Create(nameof(IsJoiningRoom), typeof(bool), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty RoomDisconnectedCommandProperty =
            BindableProperty.Create(nameof(RoomDisconnectedCommand), typeof(ICommand), typeof(TwilioVideoView));

        public static readonly BindableProperty TrackSubscribedCommandProperty =
            BindableProperty.Create(nameof(TrackSubscribedCommand), typeof(ICommand), typeof(TwilioVideoView));

        public static readonly BindableProperty TrackUnsubscribedCommandProperty =
            BindableProperty.Create(nameof(TrackUnsubscribedCommand), typeof(ICommand), typeof(TwilioVideoView));

        public static readonly BindableProperty IsWaitingForParticipantsProperty =
            BindableProperty.Create(nameof(IsWaitingForParticipants), typeof(bool), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty ShouldEndCallProperty =
            BindableProperty.Create(nameof(ShouldEndCall), typeof(bool), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsCameraOnProperty =
            BindableProperty.Create(nameof(IsCameraOn), typeof(bool), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsMutedProperty =
        BindableProperty.Create(nameof(IsMuted), typeof(bool), typeof(TwilioVideoView), defaultBindingMode: BindingMode.TwoWay);

        public bool IsJoiningRoom
        {
            get => (bool)GetValue(IsJoiningRoomProperty);
            set => SetValue(IsJoiningRoomProperty, value);
        }
        public ICommand RoomDisconnectedCommand
        {
            get => (ICommand)GetValue(RoomDisconnectedCommandProperty);
            set => SetValue(RoomDisconnectedCommandProperty, value);
        }
        public ICommand TrackSubscribedCommand
        {
            get => (ICommand)GetValue(TrackSubscribedCommandProperty);
            set => SetValue(TrackSubscribedCommandProperty, value);
        }

        public ICommand TrackUnsubscribedCommand
        {
            get => (ICommand)GetValue(TrackUnsubscribedCommandProperty);
            set => SetValue(TrackUnsubscribedCommandProperty, value);
        }

        public bool IsWaitingForParticipants
        {
            get => (bool)GetValue(IsWaitingForParticipantsProperty);
            set => SetValue(IsWaitingForParticipantsProperty, value);
        }

        public string Token
        {
            get => (string)GetValue(TokenProperty);
            set => SetValue(TokenProperty, value);
        }

        public string RoomName
        {
            get => (string)GetValue(RoomNameProperty);
            set => SetValue(RoomNameProperty, value);
        }

        public bool ShouldEndCall
        {
            get => (bool)GetValue(ShouldEndCallProperty);
            set => SetValue(ShouldEndCallProperty, value);
        }

        public bool IsCameraOn
        {
            get => (bool)GetValue(IsCameraOnProperty);
            set => SetValue(IsCameraOnProperty, value);
        }
        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }
    }
}
