<template>
    <v-container grid-list-lg>
        <v-layout row wrap>
            <v-card width="100%" color="#F2F2F2">
               
                <v-card-title class="justify-center questionaire-title">    
                        Odpovězte na následující otázky                                                                            
                </v-card-title>
                <v-card-text class="justify-center">                    
                    <div class="d-flex justify-center">
                        <label>Otázky {{currentQuestionIndex + 1}} z {{questions.length}}</label>
                    </div>
                    <div class="d-flex justify-center">
                        <v-progress-linear style="width: 50%;" background-color="#B1AFC3" color="#4CAF50" :value="getProgressBarValue"></v-progress-linear>
                    </div>
                                        
                    <v-carousel :show-arrows="false" hide-delimiters v-model="currentQuestionIndex">
                        <v-carousel-item v-for="item in approvableQuestions" :key="item.question.id">
                            <v-sheet height="100%"
                                     color="transparent"
                                     tile>

                                     <div  class="d-flex flex-column justify-center align-center fill-height">
                                        <div class="display-3">
                                                <p class="question-text" v-html="item.question.text"/>
                                            </div>                                       
                                            <questionaire-question-answer class="answer-container" v-on:click.native="item.approved = true" text="Ano" :checked="item.approved" />                                            
                                            <questionaire-question-answer class="answer-container" v-on:click.native="item.approved = false" text="Ne" :checked="!item.approved"/>   
                                     </div>                                                                               
                            </v-sheet>
                        </v-carousel-item>
                    </v-carousel>
                </v-card-text>
                <v-divider></v-divider>
                 <v-card-actions>               
                        <v-btn class="back-button" x-large @click="previousMessage">Zpět</v-btn>  
                        <v-spacer></v-spacer>
                        <v-btn @click="nextMessage">
                            <span>Pokračovat</span>
                            <v-icon style="margin-left: 20px;">mdi-arrow-right</v-icon>
                        </v-btn>   
                </v-card-actions>
            </v-card>
           
        <v-snackbar right top v-model="showValidationSnackbar" color="error">Není vybrán žádný příznak</v-snackbar>
        <v-snackbar right top v-model="showCreateFailedSnackbar" color="error">Nepodařilo se uložit rozhovor</v-snackbar>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from "vue";
    import { Component, Prop, Watch} from "vue-property-decorator";

    import SymptomQuestionDto from "@models/professionInfo/SymptomQuestionsDto";
    import SymptomQuestionsApi from "@backend/api/symptomQuestions";
    import QuestionaireApi from "@backend/api/questionaire";
    import ClientQuestionaireDTO from "@models/questionaire/ClientQuestionaireDTO"
    import ClientQuestionAnswerDTO from "@models/questionaire/ClientQuestionAnswerDTO"
    import QuestionaireQuestionAnswer from "./questionaire-question-answer.vue"
    
    @Component({
        name: "QuestionaireSQuestion",
        components: {
            QuestionaireQuestionAnswer
        },
    })

    export default class QuestionaireQuestion extends Vue {
        @Prop({default: []}) symptomsIds: number[]

        questions: SymptomQuestionDto[] = [];

        loading: boolean = false;
        showValidationSnackbar: boolean = false;
        showCreateFailedSnackbar: boolean = false;
        currentQuestionIndex = 0;

        approvableQuestions: any[] = [];

        @Watch("symptomsIds", {deep: true, immediate: true})
        onPropertyChanged(value: number[], oldValue: number[]) {
            console.log(value)
            this.getQuestions();
        }

        reset(){
            this.questions = [];         
            this.approvableQuestions = [];
            this.currentQuestionIndex = 0;
        }

        back(){
            this.$emit("goBack");
        }

        nextMessage(){
            if(this.currentQuestionIndex < this.questions.length - 1){
                this.currentQuestionIndex = this.currentQuestionIndex + 1;
                console.log(this.currentQuestionIndex)
            }
            else{
                this.createQuestionaire();
                this.reset();
            }
        }
        
        previousMessage(){
            if(this.currentQuestionIndex == 0){
                this.back()
            }else{
                this.currentQuestionIndex = this.currentQuestionIndex - 1
            }

        }

        async createQuestionaire(){
            console.log("creating questionaire-------------------------")
            console.log(this.approvableQuestions);
            let questionaire = new ClientQuestionaireDTO();
            questionaire.questionAnswers = [];

            for (let index = 0; index < this.approvableQuestions.length; index++) {
                let approvableQuestion = this.approvableQuestions[index];
                
                let questionAnswer = new ClientQuestionAnswerDTO()
                console.log(approvableQuestion);
                questionAnswer.symptomQuestionId = approvableQuestion.question.id;
                questionAnswer.approved = approvableQuestion.approved;

                questionaire.questionAnswers.push(questionAnswer)
            }

            var response = await QuestionaireApi.createQuestionaire(questionaire);

            if(response.success){
                this.showValidationSnackbar = false;
                this.currentQuestionIndex = 0;
                this.$emit("goToResult", response.data.id);
            }
            else{
                this.showCreateFailedSnackbar = true;
            }
   
        }

        async getQuestions() {
            this.loading = true;

            var result = await SymptomQuestionsApi.getSymptomQuestionsBySymptomIds(this.symptomsIds);

            if (result.success) {
               this.questions = result.data;
              
                this.approvableQuestions = this.questions.map(q=>  ({
                    question: q,
                    approved: false
		        }));
            }

            this.loading = false;
        }
        get getProgressBarValue(){
            return  (this.currentQuestionIndex + 1) / this.questions.length * 100;
        }

    }
</script>


<style scoped>
.questionaire-title{
    color: var(--dark);
    font-size: 24px;
    font-weight: 600;
}
.question-text{
    color: black;
}
.answer-container{
    margin-top: 16px;
}
.back-button{
    background-color: transparent!important;;
    border: 1px #0666EB solid;
    color: #0666EB!important;
}
</style>
