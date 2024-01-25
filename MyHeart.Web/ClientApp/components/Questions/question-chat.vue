<template>
    <div>
        <v-container style="align-content:center;">
            <v-col cols="12"  lg="8">
                <v-card flat class="elevation-3">
                <v-card-title>
                    <v-tooltip bottom>
                        <template v-slot:activator="{ on, attrs }">
                            <v-btn elevation="0"
                                   icon @click="toQuestionList"
                                   v-bind="attrs"
                                   v-on="on">
                                <font-awesome-icon icon="arrow-left" size="lg"></font-awesome-icon>
                            </v-btn>

                        </template>
                        <span>zpět</span>
                    </v-tooltip>
                    <v-divider vertical class="my-0 mx-2" />
                    <v-tooltip bottom>
                        <template v-slot:activator="{ on, attrs }">
                            <v-btn elevation="0"
                                   icon @click="toVideo"
                                   v-bind="attrs"
                                   v-on="on">
                                <font-awesome-icon icon="video" size="lg"></font-awesome-icon>
                            </v-btn>

                        </template>
                        <span>video hovor</span>
                    </v-tooltip>
                    <v-divider vertical class="my-0 mx-2" />
                    {{question.subject}}
                    <v-spacer/>
                    <v-btn @click="archiveQuestion">Archivovat</v-btn>
                </v-card-title>
                <v-card-text>
                    <perfect-scrollbar id="scroll"  ref="scroll" v-on:ps-y-reach-start="logUp()" :options="options">
                        <v-row>---</v-row>
                        <v-row v-for="(item, index) in comments" :key="item.id" :accesskey="index"
                                class="d-flex" v-bind:class="{ 'justify-end': isMyMessage(item), 'justify-start': !isMyMessage(item)}"
                                style="max-width: 90%;">
                            <div class="ml-6" style="max-width: 80%">
                                <v-card class="pl-2 pr-2" v-bind:class="{ 'blue': isMyMessage(item), 'gray': !isMyMessage(item)}">
                                    {{item.text}}
                                </v-card>
                                <div v-if="!isMyMessage(item)" class="text--secondary" style="font-size: x-small" align="left" justify="left">
                                    {{item.lastUpdateUser}} - {{getReadableDate(item.creationDate)}}
                                </div>
                                <div v-if="isMyMessage(item)" class="text--secondary" style="font-size: x-small" align="right" justify="right">
                                    {{getReadableDate(item.creationDate)}}
                                </div>
                            </div>
                        </v-row>
                    </perfect-scrollbar>

                    <v-row align="center">
                        <v-col cols="12" md="9">
                            <v-textarea id="askQuestion"
                                v-model="askQuestion"
                                label="Odpověď"
                                auto-grow
                                :rows="numberOfRows"
                                v-on:keyup.enter="handleSend" />
                        </v-col>
                        <v-col cols="12" md="3" class="mt-2 text-right">
          <v-btn @click="handleSend">Odeslat</v-btn>
      </v-col>
                    </v-row>
                </v-card-text>
            </v-card>
            </v-col>          
        </v-container>
        <v-snackbar right top v-model="showSnackbar" :color="snackBarColor">{{snackBarMessage}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import QuestionCommentRequest from "../../models/QuestionCommentRequest";
    import QuestionDto, { QuestionStatus } from "../../models/question/QuestionDto";
    import QuestionCommentDto from "../../models/question/QuestionCommentDto";
    import QuestionApi from "@backend/api/question";
    import UserApi from "@backend/api/user";
    import { HubConnectionBuilder } from '@aspnet/signalr';
    import AuthStore from "@backend/store/authStore";
    import ChatBubble from "@components/Questions/chat-bubble.vue";

    import Events from "@models/shared/Events";
    import EventBus from "@models/EventBus";
    import UserDto, { UserType } from "../../models/user/UserDto";
import { forEach } from "lodash";

    @Component({
        name: "QuestionChat",
        components: {
            ChatBubble
        },
    })
    export default class QuestionChat extends Vue {
        questionId: number = 0;
        curUser: UserDto = null;

        chatHub: any = null;
        question: QuestionDto = new QuestionDto();
        comments: QuestionCommentDto[] = [];
        request: QuestionCommentRequest = { page: 1, pageSize: 10, QuestionId: -1 }
        totalResponseNumber: number = -1;
        askQuestion: string = "";

        isImpersonating: boolean = false;
        impersonated: UserDto = null;

        loading: boolean = false;
        initLoad: boolean = true;
        disposing: boolean = false;

        showSnackbar: boolean = false;
        snackBarColor: string = "";
        snackBarMessage: string = ""

        options = {
            suppressScrollX: true
        }

        get numberOfRows() {
        // If text is empty, show as one line
        if (!this.askQuestion) return 1;

        // Otherwise, calculate the number of lines based on newline characters
            return this.askQuestion.split(/\r\n|\r|\n/).length;
        }

        mounted() {
            console.log(this.options);
            $(".ps").css({ "height": window.innerHeight-280 + "px" });

            console.log("mounted Question chat");
            this.getQuestionId();
            this.curUser = AuthStore.getCurrentUser();
            this.isImpersonating = AuthStore.isImpersonating();
            if (this.isImpersonating) {
                this.impersonated = AuthStore.getImpersonatedUser() as UserDto;
            }

            if (this.questionId != -1) {
                this.loadQuestion();
                this.request.QuestionId = this.questionId;
                this.loadComments();

                if (this.question.status != QuestionStatus.Closed.toString()) {
                    const hubUrl = this.buildUrl();

                    this.chatHub = new HubConnectionBuilder().withUrl(hubUrl, {
                        accessTokenFactory: () => AuthStore.getToken()
                    }).build();

                    this.start();
                }
            }

            window.addEventListener('resize', () => {
                $(".ps").css({ "height": window.innerHeight - 280 + "px" });
            })

            EventBus.$on(Events.Navigated, () => {
                if (this.chatHub != null) {
                    console.log("stopping on side menu navigation");
                    this.disposing = true;
                    this.chatHub.stop();
                    this.chatHub = null;
                }
            });

            let objDiv = document.getElementById('scroll');
            console.log("ScrollTop " + objDiv.scrollTop);
        }

        async start() {
            if (!this.disposing) {
                try {
                    this.chatHub.start().then(() => {
                        this.join();
                    });
                } catch (err) {
                    setTimeout(this.start, 1000);
                }

                this.chatHub.on("ReceiveMessage", this.handleReceiveMessage);
                this.chatHub.onclose(this.start);
            }
        }

        buildUrl() {
            const base: string = Vue.prototype.$http.defaults.baseURL;
            const split = base.split("/");
            const filtered = split.filter(function (x) {
                return x != null && x != "";
            });
            /*console.log(base);
            console.log(split);
            console.log(filtered);*/
            return filtered[0] + "//" + filtered[1] + "/hub/chat";
        }

        scrollDown() {
            let objDiv = document.getElementById('scroll');
            this.$nextTick(() => {
                objDiv.scrollTop = objDiv.scrollHeight;
            })
        }

        getReadableDate(date: string): string {
            const d = new Date(date);
            return d.getHours() + ":" + d.getMinutes() + " " + d.getDate() + "." + d.getMonth() + "." + d.getFullYear(); 
        }

        isMyMessage(message: QuestionCommentDto): boolean {
            if (this.curUser.id == message.senderId) {
                return true;
            }
            return false;
        }

        getQuestionId() {
            var path = window.location.pathname;
            var chunks = path.split('/');
            /*console.log("chunks - " + chunks.length);
            console.log(chunks);*/

            if (chunks.length == 4) {
                this.questionId = Number.parseInt(chunks.pop());
                console.log(this.questionId);
            } else {
                this.questionId = -1;
                this.question.subject = "tady nic není";
                this.question.body = "Nesprávné ID otázky, jak jste se sem dostali?";
            }
        }

        private handleReceiveMessage(message: QuestionCommentDto) {
            console.log("received message");
            console.log(message);
            console.log(this.comments);
            this.comments.push(message);

            this.scrollDown();
        }

        private join() {
            this.chatHub.invoke("JoinGroup", this.questionId);
        }

        handleSend() {
            if (this.askQuestion != "") {
                
                var message = { message: this.askQuestion, group: this.questionId };
                this.chatHub.invoke("SendMessage", message);
                
                this.askQuestion = "";
            }
        }

        async loadQuestion() {
            var result = await QuestionApi.getQuestion(this.questionId);

            if (result.success) {
                this.question = result.data;

                this.startImpersonating();
            }
        }

        async startImpersonating() {
            var result = await UserApi.getUserById(this.question.userId);

            if (result.success) {
                AuthStore.setImpersonating(result.data);

                EventBus.$emit(Events.StartedImpersonating);

                this.isImpersonating = AuthStore.isImpersonating();
                if (this.isImpersonating) {
                    this.impersonated = AuthStore.getImpersonatedUser() as UserDto;
                }
            }
        }

        async loadComments() {
            if (!this.loading && (this.totalResponseNumber < 0 || (this.request.page - 1)*this.request.pageSize <= this.totalResponseNumber)) {
                this.loading = true;

                var result = await QuestionApi.getResponses(this.request);

                if (result.success) {
                    this.comments = result.data.data.concat(this.comments);

                    this.totalResponseNumber = result.data.totalCount;
                    this.request.page = result.data.page + 1;
                }

                this.loading = false;
            }

            if (this.initLoad) {
                this.$nextTick(() => {
                    this.scrollDown();
                    console.log("current up is " + document.getElementById('scroll').scrollTop)
                    if (document.getElementById('scroll').scrollTop == 0 && ((this.request.page - 1) * this.request.pageSize) <= this.totalResponseNumber) {
                        this.loadComments();
                    } else {
                        this.initLoad = false;
                    }
                });
            }
            
        }

        async toVideo() {
            this.disposing = true;
            this.chatHub.stop();
            this.chatHub = null;

            this.$router.push("/client/video/" + this.questionId);
        }

        toQuestionList() {
            this.disposing = true;
            this.chatHub.stop();
            this.chatHub = null;

            if (this.curUser.userType == UserType.Patient) {
                this.$router.push("/client/questions");
            } else {
                this.$router.push("/admin/questions");
            }
        }

        logUp() {
            if (!this.initLoad) {
                //console.log("firing up, resNumber = " + this.totalResponseNumber + ", page = " + this.request.page + ", pageSize = " + this.request.pageSize);
                this.loadComments();
                document.getElementById('scroll').scrollTop = 1;
            }
        }
        async archiveQuestion(){
            var response = await QuestionApi.archiveQuestion(this.questionId)
            if(response.success){
                if (this.curUser.userType == UserType.Patient) {
                    this.$router.push({
                        path:"/client/questions",
                        query: {
                            result: true,
                            message: 'Dotaz úspěšně archivován',
                            color: 'success'
                        }
                    });
                } else {
                    this.$router.push({
                        path:"/admin/questions",
                        query: {
                            result: true,
                            snackbarMessage: 'Dotaz úspěšně archivován',
                            snackbarColor: 'success'
                        }
                    });
                }                
            }else{
                this.setAndShowSnackBar("error", "Nepodařilo se archivovat dotaz")
            }
        }

        setAndShowSnackBar(color: string, message: string){
            this.snackBarColor = color
            this.snackBarMessage = message
            this.showSnackbar = true
        }
    }
</script>

<style scoped>
    .open {
        background-color: rgb(245,245,245) !important;
    }

    .closed {
        background-color: rgb(245,245,245) !important;
    }

    .awaitingClient {
        background-color: rgb(245,245,245) !important;
    }

    .awaitingDoctor {
        background-color: rgb(245,245,245) !important;
    }

    .lightgrey-color {
        background-color: rgb(245,245,245) !important;
    }

    .blue {
        background-color: lightblue !important;
    }

    .gray {
        background-color: rgb(245,245,245) !important;
    }

    .align-right{
        float: right;
    }

    .align-left {
        float: left;
    }
</style>
