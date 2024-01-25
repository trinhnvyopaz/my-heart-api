<template>
    <v-container grid-list-lg >
        <v-flex v-for="(item, index) in questions" :key="index" style="margin-bottom: 2rem;align-items: center;">
            <v-card v-on:click="goto(item.id)">               
                <v-card-title>
                    <span>
                        <span>
                            <font-awesome-icon v-if="item.status == 2" icon="clock" :style="{ color: 'rgb(255, 106, 0)' }"></font-awesome-icon>
                            <font-awesome-icon v-if="item.status == 1" icon="reply" :style="{ color: 'rgb(0, 38, 255)' }"></font-awesome-icon>
                            <font-awesome-icon v-if="item.status == 0" icon="lock-open" :style="{ color: 'rgb(32, 167, 0)' }"></font-awesome-icon>
                            <font-awesome-icon v-if="item.status == 3" icon="lock" :style="{ color: 'rgb(255, 0, 0)' }"></font-awesome-icon>
                        </span>
                        <span class="ml-1" style="font-size:medium;"> {{item.subject}}</span>
                    </span>
                </v-card-title>

                <v-card-subtitle>           
                    <div class="d-flex flex-row justify-space-between">
                        <span class="grey--text align-self-center" style="font-weight:bold;">{{getQuestionStatus(item.status)}}</span>
                        <div>
                            <v-btn v-if="item.status != 3" @click.stop="archive(item.id)">Archivovat</v-btn>
                            <v-btn @click.stop="getChatReport(item)">Exportovat</v-btn>
                        </div>
                        
                    </div>                     
                </v-card-subtitle>

                <v-card-text>
                    <div class="black--text" v-html="item.body"></div>
                    <div class="small">{{item.name}} - {{item.creationDate}}</div>
                </v-card-text>
            </v-card>
        </v-flex>
        <v-pagination @input="loadQuestions" v-model="request.page" :length.sync="pageCount" :total-visible="9" color="error"/>
        <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{snackbarText}}</v-snackbar>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch} from "vue-property-decorator";
    import DataTableRequest from "../../models/DataTableRequest";
    import QuestionDto, { QuestionStatus } from "../../models/question/QuestionDto";
    import QuestionApi from "@backend/api/question";
    import PdfReportApi from "@backend/api/pdfReports";

    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../models/user/UserDto";

@Component({
    name: "QuestionDetail",
    components: {
    },
})
export default class QuestionDetail extends Vue {
    @Prop({ default: "" })
    questionType: string;

    @Watch("questionType", { deep: true })
    onValueChanged() {
        this.$nextTick(() => {
            this.$forceUpdate();
            this.loadQuestions();
        })
    }

    request: DataTableRequest = { page: 1, pageSize: 5, filter: "" };
    pageCount: number = 0;
    loading: boolean = false;
    questions: QuestionDto[] = [];
    snackbarShown: boolean = false;
    snackbarText: string = "";
    snackBarColor: string = "";
    
    getQuestionStatus(num) {
        switch (num) {
            case 0:
                return "Otevřená otázka";
            case 1:
                return "Čeká na vaši odpověď";
            case 2:
                return "Čeká na odpověď lékaře";
            case 3:
                return "Archivovaná otázka";
        }
    }

    getHeaderClass(num) {
        switch (num) {
            case 0:
                return "card-header open";
            case 1:
                return "card-header awaitingClient";
            case 2:
                return "card-header awaitingDoctor";
            case 3:
                return "card-header closed";
        }
    }

    goto(id) {
        this.$router.push("/client/question/" + id);
    }

    async getChatReport(question: QuestionDto){
        var user = AuthStore.getCurrentUser();
        await PdfReportApi.downloadChatReport(user.id, question.id, question.subject)
    }

    async loadQuestions() {
        this.loading = true;

        if (this.questionType == "open") {
            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                var result = await QuestionApi.getOpenQuestionsForUser(impersonated.id, this.request);
            } else {
                var result = await QuestionApi.getOpenQuestions(this.request);
            }
        } else {
            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                var result = await QuestionApi.getClosedQuestionsForUser(impersonated.id, this.request);
            } else {
                var result = await QuestionApi.getClosedQuestions(this.request);
            }
        }

        if (result.success) {
            this.questions = result.data.data;
            this.pageCount = Math.ceil(result.data.totalCount / this.request.pageSize);
        }

        this.loading = false;
    }

    mounted() {
        this.loadQuestions();
    }

    async archive(id: Number){
        var response = await QuestionApi.archiveQuestion(id)
        if(response.success){
            this.showSnackBar("Záznam úspěšně archivován", "success")
            this.loadQuestions()
        }else{
            this.showSnackBar("Záznam se nepodařilo archivovat", "error")
        }
     
    }

    showSnackBar(message: string, color: string){
        this.snackbarShown = true;
        this.snackBarColor = color;
        this.snackbarText = message
    }
}
</script>

<style scoped>
    .open {
        background-color: rgb(32, 167, 0) !important;
        color: black !important;
    }
    .closed {
        background-color: rgb(255, 0, 0) !important;
        color: black !important;
    }
    .awaitingClient {
        background-color: rgb(0, 38, 255) !important;
        color: black !important;
    }
    .awaitingDoctor {
        background-color: rgb(255, 106, 0) !important;
        color: black !important;
    }
    .card-body {
        margin-top: 2rem;
    }
    .card-header {
        margin-left: 1.5rem;
        margin-right: 1.5rem;
        bottom: 1.5rem;
    }
    .small{
        font-size: x-small;
    }
    .black {
        color: black;
    }
    .bold{
        font-weight: bold;
    }
</style>
