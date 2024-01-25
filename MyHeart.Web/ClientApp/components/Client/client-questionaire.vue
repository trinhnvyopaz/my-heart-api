<template>
    <v-container grid-list-lg>
        <v-layout row wrap>
            <v-flex xs12>
                <top-bar :tabNames="['Autorozhovor']" />
                <v-tabs-items v-model="selectedTab">

                    <v-tab-item>
                        <v-card style="background-color:var(--bg-color);" flat class="pepe-color">
                            <v-card-text>
                                <questionaire-symptom @reset="reset" @goToQuestions="goToQuestions" />
                            </v-card-text>
                        </v-card>
                    </v-tab-item>

                    <v-tab-item>
                        <v-card flat class="pepe-color">
                            <v-card-text>
                                <questionaire-question @goBack="goBack" @goToResult="goToResult" :symptomsIds="symptomsIds" />
                            </v-card-text>
                        </v-card>
                    </v-tab-item>

                    <v-tab-item>
                        <v-card flat class="pepe-color">
                            <v-card-text>
                                <questionaire-result @reset="reset" :questionaireId="questionaireId" />
                            </v-card-text>
                        </v-card>
                    </v-tab-item>
                </v-tabs-items>


            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import QuestionaireSymptom from './Questionaire/questionaire-symptom'
    import QuestionaireQuestion from './Questionaire/questionaire-question'
    import QuestionaireResult from './Questionaire/questionaire-result'
    import SymptomQuestionDiseaseDto from "@models/professionInfo/bonds/SymptomQuestionsDiseaseDto";
    import EventBus from "@models/EventBus";
    import TopBar from "@components/top-bar.vue";

    @Component({
        name: "ClientQuestionaire",
        components: {
            QuestionaireSymptom,
            QuestionaireQuestion,
            QuestionaireResult,
            TopBar
        },
    })

    export default class ClientQuestionaire extends Vue {
        loading: boolean = false;
        symptomsIds: number[] = [];
        questionaireId: number = 0;
        selectedTab: number = 0;

        mounted() {
            
        }
        goToQuestions(ids: number[]){
          
            this.symptomsIds = ids;

            this.goNext();
        }

        goToResult(questionaireId: number){
            console.log("go to result:" + questionaireId)
            this.questionaireId = questionaireId;

            this.goNext();
        }

        goNext(){
            this.selectedTab = this.selectedTab + 1;
        }
        goBack(){
             this.selectedTab = this.selectedTab - 1;
        }
        reset(){
            this.symptomsIds = [];
            this.selectedTab = 0;
            EventBus.$emit("reset-questionairesymptoms")
        }
    }
</script>


<style scoped>
    
</style>
