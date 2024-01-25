using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Example.Mylibrary;
using MyHeart.MobileApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using static Android.Media.AudioManager;

namespace MyHeart.MobileApp.Droid.Views
{
    public class TwilioVideoViewDroid : FrameLayout, ITwilioVideoWrapperCallback, ITwilioVideoViewCallback, IOnCommunicationDeviceChangedListener
    {
        TwilioVideoWrapper twilioVideo = null;
        View remoteView;
        MainActivity activity;
        AudioManager manager;

        AudioDeviceInfo currentAudioDevice = null;
        List<AudioDeviceInfo> availableAudioDevices = new List<AudioDeviceInfo>();

        bool pollDevices = true;

        public event EventHandler<EventArgs> RoomDisconected;
        public event EventHandler<EventArgs> ParticipanConnected;
        public event EventHandler<EventArgs> JoinedRoom;
        public event EventHandler<RoomConnectionFailedEventArgs> RoomConnectionFailed;
        public event EventHandler<EventArgs> TrackSubscribed;
        public event EventHandler<EventArgs> TrackUnsubscribed;

        public TwilioVideoViewDroid(Context context) : base(context)
        {
            twilioVideo = new TwilioVideoWrapper(context, this);
            LayoutParameters = new LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent,
                GravityFlags.Center // Center the VideoView in FrameLayout
            );

            SetBackgroundColor(Android.Graphics.Color.Argb(1, 30, 30, 30));

            activity = GetActivity();
            if (activity != null)
            {
                activity.OnMinimizing += MainActivity_OnMinimazing;
            }

            manager = (AudioManager)context.GetSystemService(Context.AudioService);
            manager.AddOnCommunicationDeviceChangedListener(Context.MainExecutor, this);

            _ = AvailabelAudioListPolling();
        }

        private async Task AvailabelAudioListPolling()
        {
            while (pollDevices)
            {
                var devices = manager.AvailableCommunicationDevices;

                if (devices.Count() != availableAudioDevices.Count)
                {
                    SetUpAudio();
                }

                await Task.Delay(1000);
            }
        }

        private void SetUpAudio()
        {
            var devices = manager.AvailableCommunicationDevices;

            AudioDeviceInfo bluetoothDevice = null;
            AudioDeviceInfo wireheadsetDevice = null;
            AudioDeviceInfo speakerDevice = null;
            AudioDeviceInfo earpieceDevice = null;

            availableAudioDevices.Clear();

            foreach (var deviceInfo in devices)
            {

                if (deviceInfo.Type == AudioDeviceType.BluetoothSco)
                {
                    bluetoothDevice = deviceInfo;
                }
                else if (deviceInfo.Type == AudioDeviceType.WiredHeadset || deviceInfo.Type == AudioDeviceType.WiredHeadphones)
                {
                    wireheadsetDevice = deviceInfo;
                }
                else if (deviceInfo.Type == AudioDeviceType.BuiltinSpeaker)
                {
                    speakerDevice = deviceInfo;
                }
                else if (deviceInfo.Type == AudioDeviceType.BuiltinEarpiece)
                {
                    earpieceDevice = deviceInfo;
                }


                availableAudioDevices.Add(deviceInfo);
            }

            if (bluetoothDevice != null)
            {
                if (bluetoothDevice.Id != currentAudioDevice?.Id)
                {
                    manager.Mode = Mode.InCommunication;
                    manager.SpeakerphoneOn = false;
                    manager.BluetoothScoOn = true;
                    manager.StopBluetoothSco();
                    manager.StartBluetoothSco();
                    manager.SetCommunicationDevice(bluetoothDevice);
                }
            }
            else if (wireheadsetDevice != null)
            {
                if (wireheadsetDevice.Id != currentAudioDevice?.Id)
                {
                    manager.Mode = Mode.InCommunication;
                    manager.SpeakerphoneOn = false;
                    manager.SetCommunicationDevice(wireheadsetDevice);
                }

            }
            else if (speakerDevice != null)
            {
                if (speakerDevice.Id != currentAudioDevice?.Id)
                {
                    manager.Mode = Mode.InCommunication;
                    manager.SpeakerphoneOn = true;
                }
            }
            else if (earpieceDevice != null)
            {
                if (earpieceDevice.Id != currentAudioDevice?.Id)
                {
                    manager.Mode = Mode.InCall;
                    manager.SpeakerphoneOn = false;
                    manager.SetCommunicationDevice(earpieceDevice);
                }
            }
            else
            {
                manager.Mode = Mode.InCommunication;
                manager.SpeakerphoneOn = true;
            }
        }

        private void MainActivity_OnMinimazing(object sender, EventArgs e)
        {
            if (twilioVideo != null)
            {
                var aspectRatio = new Rational(16, 9); // Adjust to your content’s aspect ratio
                var parameters = new PictureInPictureParams.Builder()
                    .SetAspectRatio(aspectRatio)
                    .Build();

                activity?.EnterPictureInPictureMode(parameters);
            }

        }

        public MainActivity GetActivity()
        {
            Context context = Context;
            while (context is ContextWrapper)
            {
                if (context is MainActivity)
                {
                    return (MainActivity)context;
                }
                context = ((ContextWrapper)context).BaseContext;
            }
            return null;
        }


        public void JoinRoom(string token, string roomName)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(roomName))
            {
                twilioVideo.JoinRoom(token, roomName);
            }
        }

        public bool IsMicOn
        {
            get => twilioVideo == null ? false : (bool)twilioVideo.IsMicOn;
        }
        public bool IsCameraOn
        {
            get => twilioVideo == null ? false : (bool)twilioVideo.IsCameraOn;
        }

        public bool ToggleCamera(bool toggleOn)
        {
            if (toggleOn != (bool)twilioVideo.IsCameraOn)
            {
                var status = twilioVideo.ToggleCamera();
                return (bool)status;
            }

            return (bool)twilioVideo.IsCameraOn;
        }

        public bool ToggleMicrophone(bool toggleOn)
        {
            if (toggleOn != (bool)twilioVideo.IsMicOn)
            {
                var status = twilioVideo.ToggleMic();
                return !(bool)status;
            }

            return !(bool)twilioVideo.IsMicOn;
        }

        public void OnJoinedRoom()
        {
            JoinedRoom?.Invoke(this, EventArgs.Empty);

            SetUpAudio();
        }

        public void Disconnect()
        {
            activity.OnMinimizing -= MainActivity_OnMinimazing;

            if (activity.IsInPictureInPictureMode)
            {
                Intent bringToForegroundIntent = new Intent(Context, typeof(MainActivity));
                bringToForegroundIntent.SetFlags(ActivityFlags.ReorderToFront);
                activity.StartActivity(bringToForegroundIntent);
            }

            pollDevices = false;

            manager.RemoveOnCommunicationDeviceChangedListener(this);
            manager.Dispose();

            twilioVideo?.DisconnectRoom();
            twilioVideo?.Dispose();
        }

        public void OnRoomDisconected()
        {
            RoomDisconected?.Invoke(this, EventArgs.Empty);
        }

        public void OnConnectFailure(string p0)
        {
            RoomConnectionFailed?.Invoke(this, new RoomConnectionFailedEventArgs(p0));
        }

        public void OnError(Java.Lang.Exception p0)
        {

        }

        public void OnAudioTrackSubscribed()
        {

        }

        public void OnAudioTrackUnsubscribed()
        {

        }

        public void OnVideoTrackSubscribed(View view)
        {
            if (this.ChildCount == 0 && view != null)
            {
                view.LayoutParameters = new LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent,
                    GravityFlags.Center
                );

                view.SetBackgroundColor(Android.Graphics.Color.Argb(1, 30, 30, 30));

                AddView(view);
                remoteView = view;
            }

            TrackSubscribed?.Invoke(this, EventArgs.Empty);
        }

        public void OnVideoTrackUnsubscribed()
        {
            if (remoteView != null)
            {
                RemoveView(remoteView);
            }

            TrackUnsubscribed?.Invoke(this, EventArgs.Empty);
        }

        public void OnLog(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Twilio log {message}");
        }

        public void OnCommunicationDeviceChanged(AudioDeviceInfo device)
        {
            currentAudioDevice = device;
        }

        public void OnParticipantConnected()
        {
            ParticipanConnected?.Invoke(this, EventArgs.Empty);
        }
    }
}