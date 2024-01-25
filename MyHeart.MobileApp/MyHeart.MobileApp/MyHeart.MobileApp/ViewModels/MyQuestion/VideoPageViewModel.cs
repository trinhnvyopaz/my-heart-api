using MyHeart.DTO.Questions;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class VideoPageViewModel : BaseViewModel
    {
        private QuestionsService questionsService;
        private int questionId;
        private bool isJoiningRoom;
        private bool isWaitingForParticipants;
        private string token;
        private string roomName;
        private bool isMuted = true;
        private bool isCameraOn = true;
        private bool shouldEndVideo;
        private bool isInPictureInPictureMode;
        private bool isRemoteVideoOn;

        public ICommand GoBackCommand { get; }
        public ICommand ToggleCameraCommand { get; }
        public ICommand ToggleMicrophoneCommand { get; }
        public ICommand EndCallCommand { get; }
        public ICommand TrackSubscribedCommand { get; }
        public ICommand TrackUnsubscribedCommand { get; }

        public bool IsMuted
        {
            get => isMuted;
            set => SetProperty(ref isMuted, value);
        }

        public bool IsCameraOn
        {
            get => isCameraOn;
            set => SetProperty(ref isCameraOn, value);
        }

        public string Token
        {
            get => token;
            set => SetProperty(ref token, value);
        }

        public string RoomName
        {
            get => roomName;
            set => SetProperty(ref roomName, value);
        }

        public bool IsJoiningRoom
        {
            get => isJoiningRoom;
            set
            {
                SetProperty(ref isJoiningRoom, value);
                OnPropertyChanged(nameof(ShowVideo));
                OnPropertyChanged(nameof(ShowVideoControls));
            }
        }

        public bool ShowVideo
        {
            get => !IsBusy && !IsJoiningRoom && !IsWaitingForParticipants;
        }

        public bool ShowVideoControls
        {
            get => !IsBusy && !IsJoiningRoom && !IsInPictureInPictureMode;
        }


        public bool IsWaitingForParticipants
        {
            get => isWaitingForParticipants;
            set
            {
                SetProperty(ref isWaitingForParticipants, value);
                OnPropertyChanged(nameof(ShowVideo));
                OnPropertyChanged(nameof(ShowPlaceholderImage));
            }
        }

        public bool ShouldEndVideo
        {
            get => shouldEndVideo;
            set => SetProperty(ref shouldEndVideo, value);
        }

        public bool IsInPictureInPictureMode
        {
            get => isInPictureInPictureMode;
            set
            {
                SetProperty(ref isInPictureInPictureMode, value);
                OnPropertyChanged(nameof(ShowVideoControls));
            }
        }

        public bool IsRemoteVideoOn
        {
            get => isRemoteVideoOn;
            set
            {
                SetProperty(ref isRemoteVideoOn, value);
                OnPropertyChanged(nameof(ShowPlaceholderImage));
            }
        }

        public bool ShowPlaceholderImage
        {
            get => ShowVideo && !IsRemoteVideoOn;
        }

        public VideoPageViewModel(int questionId)
        {
            questionsService = DependencyService.Resolve<QuestionsService>();
            this.questionId = questionId;

            GoBackCommand = new Command(() =>
            {
                MessagingCenter.Unsubscribe<Application>(this, "OnConfigurationChanged");
                Application.Current.MainPage.Navigation.PopModalAsync();
            });
            ToggleCameraCommand = new Command(() => IsCameraOn = !IsCameraOn);
            ToggleMicrophoneCommand = new Command(() => IsMuted = !IsMuted);
            EndCallCommand = new Command(() =>
            {
                ShouldEndVideo = true;
                GoBackCommand.Execute(null);
            });

            TrackSubscribedCommand = new Command(() => IsRemoteVideoOn = true);
            TrackUnsubscribedCommand = new Command(() => IsRemoteVideoOn = false);

            MessagingCenter.Subscribe<Application, bool>(this, "OnConfigurationChanged", (sender, isPip) => IsInPictureInPictureMode = isPip);

            _ = LoadData();
        }

        private async Task LoadData()
        {
            UpdateIsBusy(true);

            var videoRoom = await questionsService.JoinRoom(questionId);
            if (videoRoom != null)
            {
                Token = videoRoom.AccessToken;
                RoomName = videoRoom.Name;
            }
            else
            {
                toastService.ShowSuccess("Nepodařilo se spustit video");
            }

            UpdateIsBusy(false);
        }

        private void UpdateIsBusy(bool value)
        {
            IsBusy = value;
            OnPropertyChanged(nameof(ShowVideo));
            OnPropertyChanged(nameof(ShowVideoControls));
        }
    }
}
