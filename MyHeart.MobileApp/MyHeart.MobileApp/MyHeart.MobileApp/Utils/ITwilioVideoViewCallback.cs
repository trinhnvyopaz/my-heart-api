using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.Utils
{
    public interface ITwilioVideoViewCallback
    {
        event EventHandler<EventArgs> RoomDisconected;
        event EventHandler<EventArgs> ParticipanConnected;
        event EventHandler<EventArgs> JoinedRoom;
        event EventHandler<RoomConnectionFailedEventArgs> RoomConnectionFailed;
        event EventHandler<EventArgs> TrackSubscribed;
        event EventHandler<EventArgs> TrackUnsubscribed;
    }

    public class RoomConnectionFailedEventArgs : EventArgs
    {
        public string Error { get; set; }
        public RoomConnectionFailedEventArgs(string error)
        {
            Error = error;
        }
    }
}
