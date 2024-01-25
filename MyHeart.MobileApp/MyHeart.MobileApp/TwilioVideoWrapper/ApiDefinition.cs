using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using System;
using NativeHandle = System.IntPtr;

namespace TwilioVideoWrapperiOS
{
    // @interface TwilioVideoWrapper : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface TwilioVideoWrapper
    {
        // @property (readonly, nonatomic) BOOL isMicOn;
        [Export("isMicOn")]
        bool IsMicOn { get; }

        // @property (readonly, nonatomic) BOOL isCameraOn;
        [Export("isCameraOn")]
        bool IsCameraOn { get; }

        // -(instancetype _Nonnull)initWithToken:(NSString * _Nonnull)token roomName:(NSString * _Nonnull)roomName frame:(CGRect)frame callback:(id<TwilioVideoWrapperCallback> _Nonnull)callback __attribute__((objc_designated_initializer));
        [Export("initWithToken:roomName:frame:callback:")]
        [DesignatedInitializer]
        NativeHandle Constructor(string token, string roomName, CGRect frame, ITwilioVideoWrapperCallback callback);

        // -(BOOL)toggleMicrophone __attribute__((warn_unused_result("")));
        [Export("toggleMicrophone")]
        bool ToggleMicrophone();

        // -(BOOL)toggleCamera __attribute__((warn_unused_result("")));
        [Export("toggleCamera")]
        bool ToggleCamera();

        // -(void)disconnect;
        [Export("disconnect")]
        void Disconnect();
    }

    // @protocol TwilioVideoWrapperCallback
    /*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/
    interface ITwilioVideoWrapperCallback { }

    [Protocol(Name = "_TtP18TwilioVideoWrapper26TwilioVideoWrapperCallback_")]
    [BaseType(typeof(NSObject))]
    interface TwilioVideoWrapperCallback
    {
        // @required -(void)onRoomDisconected;
        [Abstract]
        [Export("onRoomDisconected")]
        void OnRoomDisconected();

        // @required -(void)onJoinedRoom;
        [Abstract]
        [Export("onJoinedRoom")]
        void OnJoinedRoom();

        [Abstract]
        [Export("onVideoTrackSubscribedWithView:")]
        void OnVideoTrackSubscribed([NullAllowed] UIView view);

        // @required -(void)onVideoTrackUnsubscribed;
        [Abstract]
        [Export("onVideoTrackUnsubscribed")]
        void OnVideoTrackUnsubscribed();

        // @required -(void)onAudioTrackSubscribedWithIsEnabled:(BOOL)isEnabled isPlaybackEnabled:(BOOL)isPlaybackEnabled;
        [Abstract]
        [Export("onAudioTrackSubscribedWithIsEnabled:isPlaybackEnabled:")]
        void OnAudioTrackSubscribed(bool isEnabled, bool isPlaybackEnabled);

        // @required -(void)onAudioTrackUnsubscribed;
        [Abstract]
        [Export("onAudioTrackUnsubscribed")]
        void OnAudioTrackUnsubscribed();

        // @required -(void)onRoomFailedToConnectWithError:(NSString * _Nonnull)error;
        [Abstract]
        [Export("onRoomFailedToConnectWithError:")]
        void OnRoomFailedToConnect(string error);

        // @required -(void)onParticipantConnected;
        [Abstract]
        [Export("onParticipantConnected")]
        void OnParticipantConnected();

        [Abstract]
        [Export("OnErrorWithError:")]
        void OnError(NSError error);

        [Abstract]
        [Export("onLogWithMessage:")]
        void OnLog(string message);
    }
}
