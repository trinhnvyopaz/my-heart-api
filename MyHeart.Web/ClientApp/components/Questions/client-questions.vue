<template>
    <div>
        <top-bar :tabNames="['Otevřené dotazy', 'Položit nový dotaz', 'Archiv']" v-model="selectedTab" />
        <v-tabs-items v-model="selectedTab">
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <QuestionDetail :questionType="'open'" ref="questions"></QuestionDetail>
                    </v-card-text>
                </v-card>
            </v-tab-item>

            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <question-ask v-on:askedQuestion="askedQuestion"></question-ask>
                    </v-card-text>
                </v-card>
            </v-tab-item>

            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <QuestionDetail :questionType="'closed'"></QuestionDetail>
                    </v-card-text>
                </v-card>
            </v-tab-item>
        </v-tabs-items>
        <v-snackbar right top v-model="showSnackbar" :color="snackBarColor">{{ snackBarMessage }}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import QuestionCard from "@components/Questions/question-card.vue";
    import DataTableRequest from "../../models/DataTableRequest";
    import QuestionDto from "../../models/question/QuestionDto";
    import QuestionApi from "@backend/api/question";
    import QuestionDetail from "./question-detail.vue";
    import QuestionAsk from "./question-ask.vue";
    import TopBar from "@components/top-bar.vue";

    @Component({
        name: "CreateQuestionPage",
        components: {
            QuestionCard, QuestionDetail, QuestionAsk, TopBar
        },
    })

    export default class ClientQuestionPage extends Vue {
        selectedTab: number = null;
        loading: boolean = false;

        OpenQuestions: QuestionDto[] = [];
        ClosedQuestions: QuestionDto[] = [];

        showSnackbar: boolean = false;
        snackBarColor: string = "";
        snackBarMessage: string = "";

        askedQuestion() {
            this.selectedTab = 0;

            this.$refs.questions.loadQuestions();
        }

        questionArchived(color: string, text: string){
            this.snackBarMessage = text;
            this.snackBarColor = color;
            this.showSnackbar = true;

            console.log(this.snackBarColor)
            console.log(this.snackBarMessage)

            this.$router.replace({ 
                path: this.$route.path, 
                query: {} 
            });
        }

        mounted() {
            if (this.$route.query.result) {
                console.log(this.$route.query.snackbarColor)
                console.log(this.$route.query.snackbarMessage)
                this.questionArchived(this.$route.query.color, this.$route.query.message)
            }
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
</style>
