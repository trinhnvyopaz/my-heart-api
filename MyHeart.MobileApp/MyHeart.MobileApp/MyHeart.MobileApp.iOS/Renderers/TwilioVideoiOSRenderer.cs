using Foundation;
using MyHeart.MobileApp.iOS.PlatformServices;
using MyHeart.MobileApp.iOS.Renderers;
using MyHeart.MobileApp.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TwilioVideoView), typeof(TwilioVideoiOSRenderer))]
namespace MyHeart.MobileApp.iOS.Renderers
{
    internal class TwilioVideoiOSRenderer : ViewRenderer<TwilioVideoView, UIView>
    {
        TwilioVideoViewiOS controller;
        ToastServiceiOS toastService = new ToastServiceiOS();
        protected override void OnElementChanged(ElementChangedEventArgs<TwilioVideoView> e)
        {
            base.OnElementChanged(e);

            if(e.OldElement != null)
            {
                controller.RoomDisconected -= Controller_OnRoomDisconnected;
                controller.ParticipanConnected -= Controller_OnParticipantConnected;
                controller.JoinedRoom -= Controller_OnRoomJoined;
                controller.RoomConnectionFailed -= Controller_RoomConnectionFailed;
                controller.TrackSubscribed -= Controller_TrackSubscribed;
                controller.TrackUnsubscribed -= Controller_TrackUnsubscribed;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    controller = new TwilioVideoViewiOS();
                    SetNativeControl(controller.View);
               
                }

                controller.RoomDisconected += Controller_OnRoomDisconnected;
                controller.ParticipanConnected += Controller_OnParticipantConnected;
                controller.JoinedRoom += Controller_OnRoomJoined;
                controller.RoomConnectionFailed += Controller_RoomConnectionFailed;
                controller.TrackSubscribed += Controller_TrackSubscribed;
                controller.TrackUnsubscribed += Controller_TrackUnsubscribed;
            }
        }

        private void Controller_TrackUnsubscribed(object sender, EventArgs e)
        {
            Element.TrackUnsubscribedCommand?.Execute(null);
        }

        private void Controller_TrackSubscribed(object sender, EventArgs e)
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
            Debug.WriteLine("Controller_OnRoomJoined");

            Element.IsWaitingForParticipants = true;
            Element.IsJoiningRoom = false;

            Element.IsMuted = !controller.IsMicOn;
            Element.IsCameraOn = controller.IsCameraOn;
        }

        private void Controller_OnParticipantConnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Controller_OnParticipantConnected");
            Element.IsWaitingForParticipants = false;
        }

        private void Controller_OnRoomDisconnected(object sender, EventArgs e)
        {
            Element?.RoomDisconnectedCommand?.Execute(null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == TwilioVideoView.TokenProperty.PropertyName ||
                args.PropertyName == TwilioVideoView.RoomNameProperty.PropertyName)
            {
                Element.IsJoiningRoom = true;
                controller.JoinRoom(Element.Token, Element.RoomName);
            }

            if(args.PropertyName == TwilioVideoView.ShouldEndCallProperty.PropertyName)
            {
                controller.Disconnect();
            }

            if(args.PropertyName == TwilioVideoView.IsMutedProperty.PropertyName)
            {
                var isMuted = controller.ToggleMicrophone(!Element.IsMuted);
                if (Element.IsMuted != isMuted)
                {
                    Element.IsMuted = isMuted;
                }            
            }

            if(args.PropertyName == TwilioVideoView.IsCameraOnProperty.PropertyName)
            {
                var isOn = controller.ToggleCamera(Element.IsCameraOn);
                if(Element.IsCameraOn != isOn)
                {
                    Element.IsCameraOn = isOn;
                }
                
            }
        }
    }
}