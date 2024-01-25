using Foundation;
using MyHeart.MobileApp.Utils;
using System;
using System.Diagnostics;
using UIKit;
using TwilioVideoWrapperiOS;

namespace MyHeart.MobileApp.iOS
{
    public class TwilioVideoViewiOS : UIViewController, ITwilioVideoWrapperCallback, ITwilioVideoViewCallback
    {
        TwilioVideoWrapper twilioVideo = null;

        public event EventHandler<EventArgs> RoomDisconected;
        public event EventHandler<EventArgs> ParticipanConnected;
        public event EventHandler<EventArgs> JoinedRoom;
        public event EventHandler<RoomConnectionFailedEventArgs> RoomConnectionFailed;
        public event EventHandler<EventArgs> TrackSubscribed;
        public event EventHandler<EventArgs> TrackUnsubscribed;

        public bool IsMicOn
        {
            get => twilioVideo == null ? false : twilioVideo.IsMicOn;
        }
        public bool IsCameraOn
        {
            get => twilioVideo == null ? false : twilioVideo.IsCameraOn;
        }

        public void JoinRoom(string token, string roomName)
        {
            if (twilioVideo == null && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(roomName))
            {
                twilioVideo = new TwilioVideoWrapper(token, roomName, View.Frame, this);
            }
        }

        public void Disconnect()
        {
            twilioVideo.Disconnect();
            twilioVideo.Dispose();
        }

        public bool ToggleCamera(bool toggleOn)
        {
            if (toggleOn != twilioVideo.IsCameraOn)
            {
                var status = twilioVideo.ToggleCamera();
                return status;
            }

            return twilioVideo.IsCameraOn;
        }

        public bool ToggleMicrophone(bool toggleOn)
        {
            if (toggleOn != twilioVideo.IsMicOn)
            {
                var status = twilioVideo.ToggleMicrophone();
                return !status;
            }

            return !twilioVideo.IsMicOn;
        }

        public void OnRoomDisconected()
        {
            RoomDisconected?.Invoke(this, EventArgs.Empty);
        }

        public void OnJoinedRoom()
        {
            JoinedRoom?.Invoke(this, EventArgs.Empty);
        }

        public void OnVideoTrackSubscribed(UIView view)
        {
            if (View.Subviews.Length == 0 && view != null)
            {
                View.AddSubview(view);
            }

            TrackSubscribed?.Invoke(this, EventArgs.Empty);
        }

        public void OnVideoTrackUnsubscribed()
        {

        }

        public void OnAudioTrackSubscribed(bool isEnabled, bool isPlaybackEnabled)
        {

        }

        public void OnAudioTrackUnsubscribed()
        {
            TrackUnsubscribed?.Invoke(this, EventArgs.Empty);
        }

        public void OnParticipantConnected()
        {
            ParticipanConnected?.Invoke(this, EventArgs.Empty);
        }

        public void OnRoomFailedToConnect(string error)
        {
            RoomConnectionFailed?.Invoke(this, new RoomConnectionFailedEventArgs(error));
        }

        public void OnError(NSError error)
        {

        }

        public void OnLog(string message)
        {
            Debug.WriteLine(message);
        }
    }

}