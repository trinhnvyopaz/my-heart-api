<template>
    <simple-dialog v-model="showSimpleDialog"
                   width="1400"
                   title="Odpovědi"
                   positive="Uložit"
                   @positiveClick="save">
        <localized-data-table :headers="headers"
                      :items="answerDetails"
                      hide-default-footer
                      disable-pagination>
            <template v-slot:item.text="{ item }">
                <span>{{removeHtmlTags(item.text)}}</span>
            </template>
            <template v-slot:item.approved="{ item }">
                <span>{{ item.approved ? "Ano" : "Ne"}}</span>
            </template>
            <template v-slot:item.symptomDisease.bondStrength="{item}">
                <v-edit-dialog :return-value="item.symptomDisease.bondStrength"
                               large
                               @save="bondStrengthEdited(item.symptomDisease.diseaseId)"
                               cancel-text="Zrušit"
                               save-text="Ok"
                               persistent>

                    <div>{{ item.symptomDisease.bondStrength }}</div>

                    <template v-slot:input>
                        <v-text-field v-model="item.symptomDisease.bondStrength"
                                      label="Edit"
                                      :rules="[v => v >= 0 && v <= 10 || 'Neplatná hodnota !']"
                                      type="number"
                                      single-line
                                      counter
                                      autofocus></v-text-field>
                    </template>
                </v-edit-dialog>
            </template>
        </localized-data-table>
        <questionaire-result-cards :questionaireId="questionaireId" :showBondStrength="true" />
    </simple-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import QuestionaireApi from "@backend/api/questionaire";
import SymptomQuestionApi from "@backend/api/symptomQuestions";
import ClientQuestionaireAnswerDetailDTO from "@models/questionaire/ClientQuestionaireAnswerDetailDTO"
import ClientQuestionAnswerDTO from "@models/questionaire/ClientQuestionAnswerDTO";
import QuestionaireResultCards from "@components/Client/Questionaire/questionaire-result-cards.vue"
import EventBus from "@models/EventBus";
import SimpleDialog from "@components/Shared/simpleDialog.vue";


@Component({
    name: "DiseaseListPage",
    components: {
        QuestionaireResultCards, SimpleDialog
    },
})
export default class DiseaseListPage extends Vue {
 
    @Prop({ default: false })
    showDialog: Boolean;

    @Prop({ default: 0})
    questionaireId: Number;
    
    showSimpleDialog: boolean = false;
    showUpdateSnackbar: boolean = false;
    showUpdateFailedSnackbar: boolean = false;

    headers = [
        { text: "Otázka", value: "text" },  
        { text: "Příznak", value: "symptomName" },
        { text: "Onemocnění", value: "symptomDisease.disease.name" },
        { text: "Odpověd", value: "approved" },
        { text: "Síla vazby", value: "symptomDisease.bondStrength" },
    ];

    answers: ClientQuestionAnswerDTO[] = []
    answerDetails: ClientQuestionaireAnswerDetailDTO[] = [];

    @Watch("showDialog",{deep: true, immediate: true})
    onShowDialogChange(value: boolean){
        console.log(value)
        this.showSimpleDialog = value
    }

    @Watch("showSimpleDialog")
    onShowSimpleDialog(value: boolean){
        if(!value){
            this.hideDialog()
        }
    }

    @Watch("questionaireId",{deep: true, immediate: true})
    onPropertyChanged(value: Number, oldValue: Number) {
        console.log("questionaireId changed")
        this.loadData();
    }
    hideDialog(){
        this.$emit("closeDialog")
    }

    async save(){
        var result = await SymptomQuestionApi.updateSymtomQuestionDiseases(this.answerDetails.map(a => a.symptomDisease));

        if(result.success){
            this.showUpdateSnackbar = true;
            this.hideDialog()
            this.loadData();
            EventBus.$emit("reload-questionaire-result");
        }
        else{
            this.showUpdateFailedSnackbar = true;
        }
    }
    bondStrengthEdited(diseaseId: number){
        console.log("bondStrengthEdited fired: " + diseaseId);
        var flattenDiseases = this.answers.filter(a => a.approved).map(a => a.symptomQuestion.diseases).flat();
        var filteredDiseass = flattenDiseases.filter(d => d.diseaseId == diseaseId);

        console.log(filteredDiseass);

        var sum = filteredDiseass.map(d => d.bondStrength).reduce((a, b) => parseInt(a) + parseInt(b));
        var avg = sum / filteredDiseass.length;  
        
        console.log("before event bus emit")

        EventBus.$emit("set-disease-avg", diseaseId, avg);
    }
    async loadData(){
        this.answerDetails = [];
        var questions = await QuestionaireApi.getQuestions(this.questionaireId);

        if(questions.success){
            this.answers = questions.data;

            console.log(this.answers)

            for (let index = 0; index < this.answers.length; index++) {
                let answer = this.answers[index];
                for (let index = 0; index < answer.symptomQuestion.diseases.length; index++) {
                    let disease = answer.symptomQuestion.diseases[index];
                    let answerDetail = new ClientQuestionaireAnswerDetailDTO
                                        
                    answerDetail.approved = answer.approved;
                    answerDetail.text = answer.symptomQuestion.text;
                    answerDetail.symptomName = answer.symptomQuestion.symptomName;
                    answerDetail.symptomDisease = disease;
                    
                    this.answerDetails.push(answerDetail)
                }
            }
        }    
    
    }

    removeHtmlTags(html: string | null){
        return html.replace(/(<([^>]+)>)/gi, "");
    }
}
</script>

<style lang="scss" scoped></style>
