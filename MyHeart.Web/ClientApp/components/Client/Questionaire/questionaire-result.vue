<template>
    <v-container grid-list-lg>
        <v-layout row wrap>
            <v-card width="100%">
               
                <v-card-title class="title justify-center">
                    Dle Vašich odpovědí se může jednat o některé z následujících onemocnění     
                </v-card-title>
                <v-card-text>    
                    <questionaire-result-cards :questionaireId="questionaireId" :showBondStrength="false" />
                </v-card-text>
                <v-card-actions class="justify-end">
                    <v-btn x-large @click="back">Zpět</v-btn>  
                </v-card-actions>
            </v-card>
            <!-- <v-snackbar right top v-model="showSuccessSnackbar" color="success">Automatický rozhovor vyhodnocen</v-snackbar> -->
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch} from "vue-property-decorator";
    import QuestionaireResultDto from "@models/questionaire/ClientQuestionaireResultDTO";
    import QuestionaireResultCards from "./questionaire-result-cards.vue"
    import QuestionaireApi from "@backend/api/questionaire";

    @Component({
        name: "QuestionaireResult",
        components: {
            QuestionaireResultCards
        },
    })

    export default class QuestionaireResult extends Vue {
        @Prop()
        questionaireId: number;

        back(){
            this.$emit("reset");
        }
   
    }
</script>


<style scoped>
.question-text{
    color: black;
}
.title{
    font-size: 24px;
    font-weight: 600;
    color: #47474A;
}
</style>
