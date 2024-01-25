<template>   
<div class="main-container">         
    <top-bar :middleTitle="remoteUserFullName" class="top-bar"></top-bar>
    <div class="loading-container" v-if="connectingToCall">
        <v-progress-circular indeterminate  color="#2474e3"></v-progress-circular>
        <span>Připojování k hovoru</span>
    </div>
    <div class="loading-container" v-if="waitingForParticipant">
        <v-progress-circular indeterminate  color="#2474e3"></v-progress-circular>
        <span>Čekání na dalšího účastníka</span>
    </div>

    <div v-show="!waitingForParticipant && !connectingToCall" id="remote-media"/>
<!--             
    <div v-if="showPlacehodler" :class="{'video-placeholder-mobile': $vuetify.breakpoint.mobile, 'video-placeholder-desktop': !$vuetify.breakpoint.mobile }" class="video-placeholder">
        <v-icon color="white" x-large>mdi-video-off</v-icon>
    </div>     -->

    <div v-if="!connectingToCall" class="bottom-menu">
        <v-btn x-large depressed fab class="bottom-menu-item">                    
            <v-icon v-if="!isCameraOff" @click="toggleCamera(true)" x-large>mdi-video</v-icon>
            <v-icon v-if="isCameraOff" @click="toggleCamera(false)" x-large>mdi-video-off</v-icon>
        </v-btn>

        <v-btn x-large depressed fab class="bottom-menu-item">
            <v-icon v-if="!isMuted" @click="toggleMicrophone(true)" x-large>mdi-microphone</v-icon>
            <v-icon v-if="isMuted" @click="toggleMicrophone(false)" x-large>mdi-microphone-off</v-icon>
        </v-btn>

        <v-btn @click="toQuestionList" x-large depressed fab color="error" class="bottom-menu-item">
            <v-icon x-large>mdi-phone-hangup</v-icon>
        </v-btn>
    </div>
    <v-snackbar right top v-model="showSnackbar" :color="snackBarColor">{{snackBarMessage}}</v-snackbar>       
</div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";

import QuestionApi from "@backend/api/question";
import AuthStore from "@backend/store/authStore";

import UserDto from "../../models/user/UserDto";
import VideoRoomDto from "../../models/question/VideoRoomDto";

import Events from "@models/shared/Events";
import EventBus from "@models/EventBus";
import Twilio, { connect, createLocalTracks, createLocalVideoTrack, LocalTrack, Logger, Participant, RemoteParticipant } from 'twilio-video'
import TopBar from "@components/top-bar.vue";
import UserAPi from "@backend/api/user"

@Component({
    name: "QuestionVideo",
    components: {
        TopBar
    },
})

export default class QuestionVideo extends Vue {

    currentUser: UserDto | null = null;
    remoteUserFullName: string = "";
    questionId: number = 0;
    room: Twilio.Room |null = null;
    isMuted: boolean = false;
    isCameraOff: boolean = false;
    connectingToCall: boolean = true;
    waitingForParticipant: boolean = false;  
    localAudioTrack: Twilio.LocalAudioTrack | null = null;
    localVideoTrack: Twilio.LocalVideoTrack | null = null;

    showSnackbar: boolean = false;
    snackBarColor: string = "";
    snackBarMessage: string = "";

    remoteParticipantHasCamera: boolean = true;

    mounted() {
        this.getQuestionId()
        this.currentUser = AuthStore.getCurrentUser() as UserDto;

        const logger = Twilio.Logger.getLogger('twilio-video')
        logger.disableAll();

        this.joinRoom();

        EventBus.$emit(Events.Video);
    }

    getQuestionId() {
        var path = window.location.pathname;
        var chunks = path.split('/');

        if (chunks.length == 4) {
            this.questionId = Number.parseInt(chunks.pop());
            console.log(this.questionId);
        } else {
            this.questionId = -1;
        }
    }

    get showPlacehodler() : Boolean {
        const show = !this.waitingForParticipant && !this.connectingToCall && this.remoteParticipantHasCamera == false
        console.log('showPlacehodler', show)
        return show
    }

    async joinRoom() {
        if (this.questionId >= 0) {
            
            var result = await QuestionApi.joinRoom(this.questionId);
            if (result.success) {
                console.log(result.data.accessToken)
                        
                try{
                    this.localAudioTrack = await Twilio.createLocalAudioTrack()
                }catch(error){
                    this.setAndShowSnackBar('Není dostupné žádné zvukové zařízení s mikrofonem', "warning")
                }

                try{
                    let videoTrackoptions: Twilio.CreateLocalTracksOptions = {};

                    if(this.$vuetify.breakpoint.mobile){
                        videoTrackoptions.video ={ 
                            height: 480, frameRate: 24, width: 680 
                        }
                    }else{
                        videoTrackoptions.video ={ 
                            height: 720, frameRate: 24, width: 1280 
                        }
                    }

                    this.localVideoTrack = await Twilio.createLocalVideoTrack(videoTrackoptions)
                }catch(error){
                    console.log(error)
                    this.setAndShowSnackBar('Není dostupná žádná videokamera', "warning")
                }
                
                let localTracks: LocalTrack[]= [];
                if(this.localAudioTrack){
                    localTracks.push(this.localAudioTrack)
                }

                if(this.localVideoTrack){
                    localTracks.push(this.localVideoTrack)
                }

                let connectOptions: Twilio.ConnectOptions = {
                    name: result.data.name,   
                    tracks: localTracks                               
                };

                Twilio.connect(result.data.accessToken, connectOptions).then(room => {
                    this.room = room

                    this.connectingToCall = false;
                    this.waitingForParticipant = true;

                    console.log(room)
                    
                    room.on('participantConnected', participant => {
                        this.getParticipantFullname(participant)

                        participant.on('trackSubscribed', (track) => this.trackSubscribed(track, participant));
                        participant.on('trackUnsubscribed', this.trackUnsubscribed);  
                        
                        this.remoteParticipantHasCamera = participant.videoTracks != null && participant.videoTracks.size > 0

                        this.attachRemoteParticipant(participant)

                        this.waitingForParticipant = false;  
                    });
                    
                    room.participants.forEach(participant => {
                        this.getParticipantFullname(participant)

                        participant.on('trackSubscribed', (track) => this.trackSubscribed(track, participant));
                        participant.on('trackUnsubscribed', this.trackUnsubscribed);

                        this.remoteParticipantHasCamera = participant.videoTracks != null && participant.videoTracks.size > 0
                   
                        this.attachRemoteParticipant(participant)
                        this.waitingForParticipant = false;  
                    });

                    room.on('participantDisconnected', this.toQuestionList)

                }).catch(error => {
                    this.setAndShowSnackBar(error, "error")
                    
                    console.log(error)
                });        
            }
        }   
    }

    getParticipantFullname(participant: Participant){
        console.log(participant.identity)
        const identityParts = participant.identity.split(':')
                        
        if(identityParts.length > 2){
            this.remoteUserFullName = identityParts[1] + " " + identityParts[2]
        }                        
    }

    setAndShowSnackBar(error: string, color: string){
        this.snackBarColor = color;
        this.snackBarMessage = error;
        this.showSnackbar = true;
    }

    toggleMicrophone(mute: boolean){     
        if(this.room){
            this.room.localParticipant.audioTracks.forEach(publication => {
                console.log(publication)
                if(mute){
                    publication.track.disable();
                }else{
                    publication.track.enable();
                }
                console.log(publication)
                this.isMuted = !this.isMuted
            });
        }
    }

    toggleCamera(turnOff: boolean){    
        if(this.room){
            this.room.localParticipant.videoTracks.forEach(publication => {
                console.log(publication)
                if(turnOff){
                    publication.track.disable();
                }else{
                    publication.track.enable();
                }
                this.isCameraOff = !this.isCameraOff
            });
        }   

    }

    trackSubscribed(track: Twilio.RemoteTrack, participant: RemoteParticipant) {
        const div = document.getElementById('remote-media');
        const el = track.attach();
        console.log(track);
        console.log(el);
        div?.appendChild(el);
        console.log(div);
        this.waitingForParticipant = false;

        this.remoteParticipantHasCamera = participant.videoTracks != null && participant.videoTracks.size > 0
    };

    trackUnsubscribed(track: Twilio.RemoteTrack, participant: RemoteParticipant){
        console.log(track)
        track.detach().forEach((element) => {
            console.log(element)
            element.remove()
            this.remoteParticipantHasCamera = participant.videoTracks != null && participant.videoTracks.size > 0
        });
    }

    attachRemoteParticipant(participant: Twilio.RemoteParticipant){
        let remoteContainer = document.getElementById('remote-media');
        this.attachParticipantTracks(participant, remoteContainer);
    }

    attachParticipantTracks(participant: Twilio.RemoteParticipant, container) {
       console.log(participant.tracks)
       
       participant.tracks.forEach(publication => {
            let track = publication.track
            console.log(publication)
            if(publication.isSubscribed){
                this.trackSubscribed(track, participant)
            }
       });
    }

    toQuestionList() {
        EventBus.$emit(Events.Navigated);
        this.$router.push("/client/question/" + this.questionId);
    }

    beforeDestroy() {
        console.log("beforeDestroy")
        if (this.room) {
            this.room.disconnect();
        }

        this.localAudioTrack?.stop();
        this.localVideoTrack?.stop();

        this.localAudioTrack = null;
        this.localVideoTrack = null;
    }
}
</script>

<style>
#remote-media{
    height: 100%;
}
video{
    height: 100%;
    width: 100% !important;
}
.video-placeholder{
    background-color: #1e1e1e;
    display: flex;
    align-items: center;
    justify-content: center;
}
</style>
<style scoped>
.main-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    height: 100%;
}
.bottom-menu-item{
    height: 60px!important;
    width: 60px!important;
    border-radius: 50%!important;
}
.top-bar{
    width: 100%;
}
.video-placeholder-mobile{
    width: 480px;
    height: 680px;
}
.video-placeholder-desktop{
    width: 1280px;
    height: 720px;
}
.loading-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    position: absolute;
    top: 50%;
    gap: 10px;
}


#local-media {
    height: 250px;
    display: flex;
    justify-content: center;
    padding: 10px;
    width: 100%;
    background-color: lightgray;
}

.bottom-menu {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 75px;
    gap: 10px;
    width: 100%;
    background-color: #F7F7F7;
}

.bottom-menu-item{
    cursor: pointer;
}
</style>