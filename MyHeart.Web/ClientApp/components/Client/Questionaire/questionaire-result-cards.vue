<template>
    <v-container>
        <v-layout>
             <v-row>
                <v-flex xs12 md4>
                    <v-card style="border: 1px solid #EA4444;" class="mr-2 pa-2" color="#EA4444B3">
                        <v-card-title>
                            <v-flex> 
                                <label class="title">Pravděpodobné</label>           
                            </v-flex>                   
                        </v-card-title>
                        <v-card-text> 
                            <v-flex v-for="probableDisease in questionaireResult.probableDiseases" :key="probableDisease.id">
                                <v-row align="center" justify="center">
                                    <p class="disease">{{probableDisease.disease.name}}</p><p class="disease" v-if="showBondStrength">&nbsp; - {{probableDisease.averageBondStrength}}</p>
                                </v-row>
                            </v-flex>
                        </v-card-text> 
                    </v-card>
                </v-flex>
                <v-flex xs12 md4>
                    <v-card class="mr-2 pa-2"  color="rgba(234, 68, 68, 0.5)">
                        <v-card-title>
                            <v-flex> 
                                <label class="title title-probable">Možné</label>           
                            </v-flex>                   
                        </v-card-title>
                        <v-card-text> 
                            <v-flex v-for="possibleDisease in questionaireResult.possibleDiseases" :key="possibleDisease.id">
                                <v-row align="center" justify="center">
                                    <p class="disease">{{possibleDisease.disease.name}}</p><p class="disease" v-if="showBondStrength">&nbsp; - {{possibleDisease.averageBondStrength}}</p>
                                </v-row>
                            </v-flex>
                        </v-card-text> 
                    </v-card>
                </v-flex>
                <v-flex xs12 md4>
                    <v-card class="mr-2 pa-2"  color="rgba(234, 68, 68, 0.2)">
                        <v-card-title>
                            <v-flex> 
                                <label class="title title-unlikely">Nepravděpodobné</label>           
                            </v-flex>                   
                        </v-card-title>
                        <v-card-text> 
                            <v-flex v-for="improbableDisease in questionaireResult.improbableDiseases" :key="improbableDisease.id">
                                <v-row align="center" justify="center">
                                    <p class="disease">{{improbableDisease.disease.name}}</p><p class="disease" v-if="showBondStrength">&nbsp;- {{improbableDisease.averageBondStrength}}</p>
                                </v-row>                           
                            </v-flex>
                        </v-card-text> 
                    </v-card>
                </v-flex>
            </v-row>                    
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch} from "vue-property-decorator";
    import QuestionaireResultDTO from "@models/questionaire/ClientQuestionaireResultDTO";
    import QuestionaireApi from "@backend/api/questionaire";
    import EventBus from "@models/EventBus";
import { indexOf } from "lodash";

    @Component({
        name: "QuestionaireResultCards",
        components: {

        },
    })


    export default class QuestionaireResultCards extends Vue {
        @Prop()
        questionaireId: number;

        @Prop()
        showBondStrength: boolean;

        questionaireResult: QuestionaireResultDTO = new QuestionaireResultDTO;

        probableThreshold: number = 7;
        possibleThreshold: number = 3;

        @Watch("questionaireId", {deep: true, immediate: true})
        onPropertyChanged(newValue: number, oldValue: number) {      
            console.log(newValue)      
            this.loadData();            
        }

        async loadData(){
            var response = await QuestionaireApi.getResult(this.questionaireId);

            if(response.success){
                console.log(response.data)
                this.questionaireResult = response.data;
            }
        }
        mounted(){
            EventBus.$on("set-disease-avg", this.setAverage);
            EventBus.$on("reload-questionaire-result", this.loadData);
        }

        setAverage(diseaseId: number, average: number){
            console.log("setAverage fired")

            var probableDisease = this.questionaireResult.probableDiseases.find(d => d.disease.id == diseaseId )
            if(probableDisease != null){

                probableDisease.averageBondStrength = Math.round(average * 100) / 100 

                if(average <= this.probableThreshold){

                    var index = this.questionaireResult.probableDiseases.indexOf(probableDisease)
                    this.questionaireResult.probableDiseases.splice(index, 1)

                    if(average <= this.possibleThreshold){

                        this.questionaireResult.improbableDiseases.push(probableDisease)
                    }
                    else{

                        this.questionaireResult.possibleDiseases.push(probableDisease)
                    }                                                            
                }
                return;
            }

            var possibleDisease = this.questionaireResult.possibleDiseases.find(d => d.disease.id == diseaseId)
            if(possibleDisease != null){

                possibleDisease.averageBondStrength = Math.round(average * 100) / 100 

                if(average > this.probableThreshold || average < this.possibleThreshold){

                    var index = this.questionaireResult.possibleDiseases.indexOf(possibleDisease)
                    this.questionaireResult.possibleDiseases.splice(index, 1)

                    if(average > this.probableThreshold){

                        this.questionaireResult.probableDiseases.push(possibleDisease)
                    }
                    else{

                         this.questionaireResult.improbableDiseases.push(possibleDisease)
                    }
                    
                }

                return;
            }

            var improbableDisease = this.questionaireResult.improbableDiseases.find(d => d.disease.id == diseaseId)
            if(improbableDisease != null){

                improbableDisease.averageBondStrength = Math.round(average * 100) / 100 

                 if(average > this.possibleThreshold){

                    var index = this.questionaireResult.improbableDiseases.indexOf(improbableDisease)
                    this.questionaireResult.improbableDiseases.splice(index, 1)

                    if(average > this.probableThreshold){

                         this.questionaireResult.probableDiseases.push(improbableDisease)
                    }
                    else{

                        this.questionaireResult.possibleDiseases.push(improbableDisease)

                    }
                                    
                }

                return;
            }
        }
   }
</script>
<style scoped>
.v-card{
    border-radius: 20px!important;
}
.title{
    font-size: 24px;
    font-weight: bold;
    color: var(--dark);
}
.title-probable{
    opacity: 90%;
}
.title-unlikely{
    opacity: 80%;
}
.disease{
    color: #47474A;
    font-size: 24px;
}
</style>