using Android.Content;
using Android.Widget;
using Com.Example.Mylibrary;
using MyHeart.MobileApp.Droid.PlatformServices;
using MyHeart.MobileApp.Droid.Renderers;
using MyHeart.MobileApp.Droid.Views;
using MyHeart.MobileApp.Views.Controls;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(TwilioVideoView), typeof(TwilioVideoDroidRenderer))]
namespace MyHeart.MobileApp.Droid.Renderers
{
    internal class TwilioVideoDroidRenderer : ViewRenderer<TwilioVideoView, FrameLayout>
    {
        TwilioVideoViewDroid view = null;
        ToastServiceDroid toastService = new ToastServiceDroid();
        public TwilioVideoDroidRenderer(Context context) : base(context)
        {

        }



        protected override void OnElementChanged(ElementChangedEventArgs<TwilioVideoView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                view.RoomDisconected -= Controller_OnRoomDisconnected;
                view.ParticipanConnected -= Controller_OnParticipantConnected;
                view.JoinedRoom -= Controller_OnRoomJoined;
                view.RoomConnectionFailed -= Controller_RoomConnectionFailed;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    view = new TwilioVideoViewDroid(Context);
                    SetNativeControl(view);
                }

                view.RoomDisconected += Controller_OnRoomDisconnected;
                view.ParticipanConnected += Controller_OnParticipantConnected;
                view.JoinedRoom += Controller_OnRoomJoined;
                view.RoomConnectionFailed += Controller_RoomConnectionFailed;
                view.TrackSubscribed += View_TrackSubscribed;
                view.TrackUnsubscribed += View_TrackUnsubscribed;
            }
        }

        private void View_TrackUnsubscribed(object sender, EventArgs e)
        {
            Element.TrackUnsubscribedCommand?.Execute(null);
        }

        private void View_TrackSubscribed(object sender, EventArgs e)
        {
            Element.TrackSubscribedCommand?.Execute(null);
        }

        private void Controller_RoomConnectionFailed(object sender, Utils.RoomConnectionFailedEventArgs e)
        {
            toastService.ShowError(e.Error, Enums.NotificationPosition.Bottom);
            Element?.RoomDisconnectedCommand?.Execute(null);
        }

        private void Controller_OnRoomJoined(object sender, EventArgs e)
        {

            Element.IsWaitingForParticipants = true;
            Element.IsJoiningRoom = false;

            Element.IsMuted = !view.IsMicOn;
            Element.IsCameraOn = view.IsCameraOn;
        }

        private void Controller_OnParticipantConnected(object sender, EventArgs e)
        {
            Element.IsWaitingForParticipants = false;
        }

        private void Controller_OnRoomDisconnected(object sender, EventArgs e)
        {
            Element.RoomDisconnectedCommand?.Execute(null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == TwilioVideoView.TokenProperty.PropertyName ||
                args.PropertyName == TwilioVideoView.RoomNameProperty.PropertyName)
            {
                Element.IsJoiningRoom = true;
                view?.JoinRoom(Element.Token, Element.RoomName);
            }

            if (args.PropertyName == TwilioVideoView.ShouldEndCallProperty.PropertyName)
            {
                view.Disconnect();
            }

            if (args.PropertyName == TwilioVideoView.IsMutedProperty.PropertyName)
            {
                var isMuted = view.ToggleMicrophone(!Element.IsMuted);
                if (Element.IsMuted != isMuted)
                {
                    Element.IsMuted = isMuted;
                }
            }

            if (args.PropertyName == TwilioVideoView.IsCameraOnProperty.PropertyName)
            {
                var isOn = view.ToggleCamera(Element.IsCameraOn);
                if (Element.IsCameraOn != isOn)
                {
                    Element.IsCameraOn = isOn;
                }

            }
        }
    }
}