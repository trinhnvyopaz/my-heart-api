<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="saveSymptom"
                  @delete="deleteSymptom"
                  ref="dialog"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat příznak' : 'Upravit příznak'"
                  :focusedDetail="focusedDetail">
        <div v-html="error"></div>
        <v-text-field ref="symtomNameField" label="Název" v-model="symptom.name" @focus="changeFocus(-1)" :rules="requiredRules" />
        <v-text-field label="Synonyma" :value="synonymString" @focus="changeFocus(3)" readonly/>
        <html-text-field label="Popis" :value="symptom.description" @focus="changeFocus(0)" />
        <v-text-field label="Onemocnění" :value="diseaseString" @focus="showBondDetail(diseaseList,symptom.diseases,0, 'Přidat onemocnění', showDiseaseAdd);changeFocus(1)" readonly />
        <label @click="changeFocus(2)" class="question-header">Otázky</label>
        <v-container>
            <div class="d-flex align-items-center" v-for="question in questionList" :key="question.id" @click="changeFocus(2)">           
                <v-icon color="error" @click.stop="deleteQuestion(question.id)">mdi-delete</v-icon>          
                <v-icon class="mr-2" color="primary" @click.stop="editQuestion(question.id)">mdi-pencil</v-icon>
                <span class="question-text">{{ formatQuestion(question.text) }}</span>                  
            </div>
        </v-container>
        <template #rightPanel >
            <v-tab-item><vue-editor ref="descriptionEditor" v-model="symptom.description" /></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" /></v-tab-item>
            <v-tab-item><disease-question-detail @add-question="addQuestion" :assignedQuestionIds="assignedQuestionIds" :key="questionDetailComponentKey" :items="selectedDiseases" ref="questionComponent" /></v-tab-item>
            <v-tab-item>
                <synonym-detail ref="synonymDetail" :items="symptom.synonyms" @delete-item="deleteSynonym" @add-item="addSynonym"  />
            </v-tab-item>
        </template>

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease"/>

        <symptom-questions-detail-dialog-deep :isNew="deepNew"
                                              v-model="isSymptomQuestionAddActive"
                                              :symptomId="deepId"
                                              :openFromSymptomId="symptom.id"
                                              :symptomQuestionId="symptomQuestionId"
                                              :levelsDeep="levelsDeep + 1"
                                              @updateSymptomQuestions="updateSymptomQuestions" />


    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import SymptomApi from "@backend/api/symptom";
    import DiseaseApi from "@backend/api/disease";
    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import SymptomDiseaseDto from "../../../../models/professionInfo/bonds/SymptomDiseaseDto";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";

    import DiseaseQuestionDetail from "../../../Shared/diseaseQuestionDetail.vue";
    import SymptomQuestionsSymptomDto from "../../../../models/professionInfo/bonds/SymptomQuestionsSymptomDto";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import SymptomQuestionsDto from "../../../../models/professionInfo/SymptomQuestionsDto";
    import SymptomQuestionsApi from "@backend/api/symptomQuestions";
    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"

    import ArrayFunctions from "@helpers/ArrayFunctions"
    import symptomQuestions from "@backend/api/symptomQuestions";
    import QuestionDto from "@models/question/QuestionDto";
    import SymptomSynonymDto from "@models/professionInfo/SymptomSynonymDto";

    import SynonymDetail from "../../../Shared/synonymDetail.vue";

    @Component({
        name: "SymptomDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
            DiseaseQuestionDetail, DescriptionHtmlOutput,SplitDialog, HtmlTextField, SynonymDetail
        },
    })
    export default class SymptomDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        diseaseList: PagedStickyList = new PagedStickyList("disease");
        questionList: SymptomQuestionsDto[] = [];
        isSymptomLoaded: boolean = false;
        isModified: boolean = false;
        showDialogSave: boolean = false;
        isDiseaseAddActive: boolean = false;
        isSymptomQuestionAddActive: boolean = false;        

        questionDetailComponentKey: number = 0;

        saving: boolean = false;
        symptom: SymptomDto = new SymptomDto();

        dis: SymptomDiseaseDto = new SymptomDiseaseDto;
        shortQuestions: String[] = [];

        //bond detail variables
        detailWidth: number = 600;
        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        isTaken: boolean = false;
        isDescriptionDetailActive: boolean = false;

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null

        symptomQuestionId: Number = 0

        //questions variables
        isQuestionDetailActive: boolean = false;

        focusedDetail = -1;

        requiredRules = [v => (v != null && v !="") || 'Povinné pole']

        @Prop({ default: false }) value: boolean = false; // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        @Prop({default: 0})
        levelsDeep: number 

        @Prop({ default: false })
        isNew: boolean;

        @Prop({ default: -1 })
        symptomId: number;

        @Watch("symptom", { deep: true })
        onModified() {
            this.isModified = true;
        }

        get synonymString(){
            return this.symptom?.synonyms?.map(x => x.name)?.join(', ')
        }

        get assignedQuestionIds() {
            if (this.symptom && this.symptom.symptomQuestions) {
                var questionIds =this.symptom.symptomQuestions.map(x => x.symptomQuestionsId);
                console.log(questionIds)
                return questionIds;
            }
            return [];
        }

        async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)

            if(this.focusedDetail == -1){
                this.showAddButton = false;
            }
            if(this.focusedDetail == 0){
                this.showAddButton = false; 
                FocusHelper.focusEditor(this.$refs.descriptionEditor.quill)
            }                          
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()
                
            }else if(this.focusedDetail == 2){
                this.showAddButton = true; 
                this.addButtonText = "Přidat otázku";
                this.addButtonAction = this.showSymptomQuestionAdd;
                this.$refs.questionComponent.$refs.searchBar.focus()
            }else if(this.focusedDetail == 3){
                this.showAddButton = false;
                this.$refs.synonymDetail.$refs.synonymField.focus()
            }                               
        }

        mounted(){
            console.log(this.$refs.dialog.$el.style)
            // this.$refs.dialog.style.height = '800px'; 
        }

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.isSymptomLoaded = false;
   
           

            this.diseaseList = new PagedStickyList("disease", true);

            this.symptom = new SymptomDto;
            this.questionList = [];
            this.changeFocus(-1);
            this.error = "";

            if (this.$refs.symtomNameField) {
                this.$refs.symtomNameField.resetValidation();
            }
            
            if (!this.isNew) {
                if (this.dialogShown) {
                    await this.loadSymptom();
                }
            }
            else {
                this.symptom = new SymptomDto;
                this.symptom.diseases = [];
                this.symptom.symptomQuestions = [];
                this.$set(this.symptom, 'synonyms', []);
                this.isSymptomLoaded = true;
            }
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isQuestionDetailActive = false;
            this.isModified = false;
            this.showDialogSave = false
        }

        get selectedDiseases() {
            var selected = [];

            for (var key in this.symptom.diseases) {
                if (!this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId))
                    continue;
                selected.push(this.diseaseList.selectedNative.find(x => x.id === this.symptom.diseases[key].diseaseId));
            }

            return selected;
        }

        //TODO refactor
        get diseaseString() {
            var disease = "";
            for (var key in this.symptom.diseases) {
                if (!this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId))
                    continue;
                disease = disease + this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId).name
                disease = disease + " - " + this.symptom.diseases[key].bondStrength.toString() + ", ";
            }

            return disease.substring(0, disease.length - 2);
        }
        
        get questionsString(){
            var question = "";


            for(var key in this.symptom.symptomQuestions){
                if(!this.questionList.find(x => x.id == this.symptom.symptomQuestions[key].symptomQuestionsId))
                    continue;

                question = question + this.questionList.find(x => x.id == this.symptom.symptomQuestions[key].symptomQuestionsId)?.text
                question = question + " - " + this.symptom.symptomQuestions[key].bondStrength.toString() + ", ";
            }

            return question.substring(0, question.length - 2);
        }

        get questionString() {
            var questions = "";

            for (var question of this.shortQuestions) {
                questions = questions + `${question}, `;
            }

            return questions.substring(0, questions.length - 2);
        }

        deleteSynonym(name: string){
            const index = this.symptom.synonyms.findIndex(s => s.name == name);
            if (index !== -1) {
                this.symptom.synonyms.splice(index, 1);
            }
        }

        addSynonym(synonym){
            const dto ={                
                symptomId: this.symptom.id,
                isManual: synonym.isManual,
                name: synonym.name
            }

            this.symptom.synonyms.push(dto);
            // this.symptom.synonyms = [...this.symptom.synonyms];
        }

        formatQuestion(text){
            return text.replace(/(<([^>]+)>)/gi, "");
        }

        addNew(){
            this.addButtonAction()
        }

        beforeCreate(){
            this.$options.components.DiseaseDetailDialogDeep = require('@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue').default
            this.$options.components.SymptomQuestionsDetailDialogDeep = require('@components/Manager/ProfessionInformation/SymptomQuestions/symptom-questions-detail-dialog-deep.vue').default                                                                               
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number, addButtonText: string, addNewFunction) {
            if (!this.isDetailActive) {
                this.getBondDetailList(valueList, selectedList, bondType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.bondType = bondType;
                this.isDetailActive = !this.isDetailActive;
                this.isDescriptionDetailActive = false;
                this.isQuestionDetailActive = false;
            }
            else {
                if (bondType != this.bondType) {
                    this.getBondDetailList(valueList, selectedList, bondType);
                    this.valueList = valueList;
                    if (valueList.page === 0) {
                        this.valueList.page = 1;
                    }
                    this.bondType = bondType;
                }
                else {
                    this.isDetailActive = !this.isDetailActive;
                }
            }

            if(this.levelsDeep <= 1){
                this.showAddButton = true;
            }

            this.addButtonText = addButtonText;
            this.addButtonAction = addNewFunction;
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];
            for (var key in valueList.nativeList) {
                this.singleBondDetail.id = valueList.nativeList[key].id;
                this.singleBondDetail.name = valueList.nativeList[key].name;
                switch (bondType) {
                    case 0: {
                        if (selectedList.length > 0 && selectedList.find(x => x.diseaseId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.diseaseId == valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                }
                this.bondDetailDto.push(this.singleBondDetail);

                this.singleBondDetail = new BondDetailDto;

            }
        }

        updateBonds(item: BondDetailDto, bondType: number) {
            this.isTaken = false;            
            switch (bondType) {
                case 0: {
                 
                    this.symptom.diseases = this.symptom.diseases.filter(x => x.diseaseId != item.id);
                    this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                  
                    if (item.isSelected) {
                        this.symptom.diseases.push({ diseaseId: item.id, bondStrength: item.bondStr, symptomsId: this.symptomId });
                        this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));
                        this.diseaseList.selected.push(item);
                     
                    }
                    //this.diseaseList.sort();
                    break;
                }
            }

            let temp = Object.assign({}, this.symptom);
            this.symptom = temp;
            this.isModified = true;
        }

        showDiseaseAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isDiseaseAddActive = true;
        }

        showSymptomQuestionAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isSymptomQuestionAddActive = true;
        }

        showEditQuestion(){
            this.deepNew = false;
            symptomQuestionId
        }

        async updateDeepDisease(message: string, updatedDisease){
            await this.diseaseList.loadPage(updatedDisease);
         
            var bondDetail = this.diseaseList.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 0)
            this.showBondDetail(this.diseaseList, this.symptom.diseases,0, 'Přidat příčinu', this.showDiseaseAdd);  
        }

        updateSymptomQuestions(message: string, symptomQuestion){
            console.log(symptomQuestion)
            if(symptomQuestion.symptoms.find(x => x.id = this.symptomId)){
                this.addQuestion(symptomQuestion)
            }
            this.questionDetailComponentKey +=1;
        }

        async loadSymptom() {
            this.loading = true;
           
            var result = await SymptomApi.getSymptomDetail(this.symptomId);

            if (result.success) {
                this.symptom = result.data;
      
                await Promise.all([
                    this.getDisease(),
                    this.getQuestions(),
                    this.getSymptomQuestions()
                ]);

                this.isSymptomLoaded = true;
            }

            this.loading = false;
        }

        async getDisease() {
            if (this.symptom.diseases != null && this.symptom.diseases.length > 0) {
                const diseaseIds = this.symptom.diseases.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.symptom.diseases;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }

        async getSymptomQuestions() {
            if (this.symptom.symptomQuestions != null && this.symptom.symptomQuestions.length > 0) {
                const questionsIds = this.symptom.symptomQuestions.map(x => x.symptomQuestionsId);
                var result = await SymptomQuestionsApi.getByIds(questionsIds);
                if (result.success) {
                    this.questionList = result.data;                  
                }
            }
        }

        async getDiseaseQuestionDetailData(id: number): Promise<String[]> {
            this.loading = true;

            var result = await SymptomQuestionsApi.getSymptomQuestionsByDiseaseId(id);

            if (result.success) {
                var data = result.data as SymptomQuestionsDto[];

                var strings: String[] = [];

                for (var single of data) {
                    var noHtml = single.text.replace(/(<([^>]+)>)/gi, "");
                    strings.push(noHtml.substring(0, 100));
                }

                return strings;
            }

            return [];
        }

        async addQuestionShorts(id: number) {
            var shorts = await this.getDiseaseQuestionDetailData(id);
            this.shortQuestions = this.shortQuestions.concat(shorts);
        }

        async getQuestions() {
            this.shortQuestions = [];

            for (var disease of this.symptom.diseases) {
                this.addQuestionShorts(disease.diseaseId);
            }
        }


        async saveSymptom() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = this.isNew ? await SymptomApi.addSymptom(this.symptom)
                : await SymptomApi.updateSymptom(this.symptom);
            if (result.success) {
                this.symptom = result.data;
                this.isModified = false;
                this.$emit("updateSymptom", "Záznam uložen.", this.symptom);
                this.dialogShown = false;
            }
            else {
                for (var key in result.errors) {
                    const errors = result.errors[key];
                    this.error += `${errors} `;
                    this.error += "<br>";
                }
            }
            this.saving = false;
            this.loading = false;
        }

        async deleteSymptom() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await SymptomApi.deleteSymptom(this.symptomId);
            if (result.success) {
                this.$emit("updateSymptom", "Záznam odstraněn.");
            }
            this.saving = false;
            this.loading = false;
        }

        deleteQuestion(id: Number){
            this.questionList = this.questionList.filter((question) => question.id != id);
            this.symptom.symptomQuestions = this.symptom.symptomQuestions.filter((question) => question.symptomQuestionsId != id);         
        }
        editQuestion(id: Number){
            this.symptomQuestionId = id;
            this.deepNew = false;
            this.isSymptomQuestionAddActive = true;
        }
        addQuestion(question: SymptomQuestionsDto){
            console.log(this.questionList)
            var index = this.questionList.findIndex(x => x.id == question.id)

            if(index === -1){
                this.questionList.push(question)

                var symptomQuestion = {
                    symptomQuestionsId: question.id,
                    symptomId: this.symptomId,
                    bondStrength: 0
                }

                this.symptom.symptomQuestions.push(symptomQuestion)
                console.log(this.symptom.symptomQuestions)
            }
            else {
                // This will ensure the changes are reactive
                this.$set(this.questionList, index, question);
                console.log(this.questionList)
            }
        }
    }
</script>
<style scoped>
.question-header{
    color: #707070;
    font-size: 14px;
    font-family: nunito;
    padding-left: 15px;
}
.question-text{
    color: #47474A;
    font-size: 18px;
    font-family: nunito;
}
</style>