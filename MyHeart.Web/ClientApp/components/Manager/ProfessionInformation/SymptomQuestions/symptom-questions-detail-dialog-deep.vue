<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="saveSymptomQuestions"
                  @delete="deleteSymptomQuestions"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat otázku' : 'Upravit otázku'"
                  :focusedDetail="focusedDetail">
        <div>{{error}}</div>

        <html-text-field label="Popis" :value="symptomQuestion.text" @focus="changeFocus(0)" :rules="requiredRules" />
        <v-text-field label="Příznaky" :value="symptomsString" @focus="showBondDetail(symptomsList,symptomsSelectedList,0, showSymptomAdd, 'Přidat příznak');changeFocus(1)" readonly />
        <v-text-field label="Nemoci" :value="diseaseString" @focus="showBondDetail(diseaseList, diseaseSelectedList, 1, showDiseaseAdd, 'Přidat onemocnění');changeFocus(1)" readonly />

        <template #rightPanel>
            <v-tab-item><vue-editor ref="questionEditor" v-model="symptomQuestion.text" /></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" :strength="strength" /></v-tab-item>
        </template>

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease"/>

        <symptom-detail-dialog-deep :isNew="deepNew"
                                    v-model="isSymptomAddActive"
                                    :symptomId="deepId"
                                    @updateSymptom="updateSymptom" />

    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import SymptomQuestionsApi from "@backend/api/symptomQuestions";
    import SymptomQuestionsDto from "../../../../models/professionInfo/SymptomQuestionsDto";
    import SymptomQuestionsSymptomDto from "../../../../models/professionInfo/bonds/SymptomQuestionsSymptomDto";
    import SymptomQuestionsDiseaseDto from "../../../../models/professionInfo/bonds/SymptomQuestionsDiseaseDto";

    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import SymptomApi from "@backend/api/symptom";

    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto"
    import DiseaseApi from "@backend/api/disease"

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import MathTools from '../../../../backend/tools/MathTools';

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    //import autocompleteBondStrength from "../../../Shared/AutocompleteBondStrength";
    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"
import symptom from "@backend/api/symptom";


    @Component({
        name: "SymptomQuestionsDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, DescriptionHtmlOutput, DescriptionDetail,
            DescriptionDetailButtons, SplitDialog, HtmlTextField
        },
    })
    export default class SymptomQuestionsDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isModified: boolean = false;
                
        checkbox: boolean = false;
        symptomQuestion: SymptomQuestionsDto = new SymptomQuestionsDto();
        symptoms: SymptomDto[] = [];
        combo: SymptomQuestionsSymptomDto[] = [];
        diseases: DiseaseDto[] = [];

        diseaseSelectedList: SymptomQuestionsDiseaseDto[] = [];
        symptomsSelectedList: SymptomQuestionsSymptomDto[] = [];
        symptomsList: PagedStickyList = new PagedStickyList("symptom");
        diseaseList: PagedStickyList = new PagedStickyList("disease");

        bondStrength: number = 0;
        bondDetailDto: BondDetailDto[] = [];
        singleBond: BondDetailDto = new BondDetailDto;
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isDetailActive: boolean = false;
        isDiseaseDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        strength: boolean = false;
        multi: boolean = false;
        isDescriptionDetailActive: boolean = false;
        isDiseaseAddActive: boolean = false;
        isSymptomAddActive: boolean = false;

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null;

        
        focusedDetail = -1;
        
        requiredRules = [v => (v != null && v !="") || 'Povinné pole']
        
        @Prop({default: 0})
        openFromSymptomId: number;

        @Prop({default: 0})
        levelsDeep: number 

        @Prop({ default: false }) value: boolean = false; // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        @Prop({ default: false })
        isNew: boolean;

        @Prop({ default: -1 })
        symptomQuestionId: number;

        @Watch("symptomQuestion", { deep: true })
        onModified() {
            this.isModified = true;
        }

        async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)

            if(this.focusedDetail == 0){  
                this.showAddButton = false;
                FocusHelper.focusEditor(this.$refs.questionEditor.quill)
            }else if(this.focusedDetail == -1){
                this.showAddButton = false;
            }                          
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()              
            }                
        }

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.diseaseSelectedList = [];
            this.symptomsSelectedList = [];

            this.changeFocus(-1);
            this.error = "";

            this.symptomsList = new PagedStickyList("symptom");
            this.diseaseList = new PagedStickyList("disease", true);

            if (!this.isNew) {
                if (this.dialogShown) {
                    await this.loadSymptomQuestions();
                }
                this.isModified = false;
            }
            else {
             
                this.symptomQuestion = new SymptomQuestionsDto;
                this.symptomsSelectedList = [];
                this.diseaseSelectedList = [];

                if(this.openFromSymptomId > 0){
                    var result = await SymptomApi.getByIds([this.openFromSymptomId])
                    if(result.success){
                      
                        const symptom = result.data[0]
                        this.symptomQuestion.symptoms =[{
                            symptomId: this.openFromSymptomId,
                            symptom: symptom,
                            symptomQuestionsId: 0,
                            bondStrength: 0
                        }] 
                        
               

                        this.getSymptoms()
                    }
                }       
            }

            this.combo = [];
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
        }

        beforeCreate(){
            this.$options.components.DiseaseDetailDialogDeep = require("@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue").default;
            this.$options.components.SymptomDetailDialogDeep = require("@components/Manager/ProfessionInformation/Symptoms/symptom-detail-dialog-deep.vue").default;
        }

        get symptomsString() {
            var symptoms = "";
            for (var key in this.symptomsList.selectedNative) {
                if(symptoms != ""){
                    symptoms += ", "
                }

                
                if(this.symptomsList.selectedNative[key]){
                    symptoms += this.symptomsList.selectedNative[key].name;      
                }                       
            }

            return symptoms;
        }

        get diseaseString() {
            var diseases = "";
            for (var key in this.diseaseList.selectedNative) {
                diseases += this.diseaseList.selectedNative[key].name;
                diseases += " - " + this.diseaseList.selected.find(x => x.id == this.diseaseList.selectedNative[key].id).bondStr.toString() + ", ";
            }

            return diseases.substring(0, diseases.length - 2);
        }

        changeDefaultBondStrength() {
            this.bondStrength = MathTools.clamp(this.bondStrength, 0, 10);
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];

            if (!selectedList)
                selectedList = [];
                
            let idField = ["symptomsId", "diseaseId"][bondType];
            
            for (var key in valueList.nativeList) {
                this.singleBondDetail.id = valueList.nativeList[key].id;
                this.singleBondDetail.name = valueList.nativeList[key].name;
                if (selectedList.length > 0 && selectedList.find(x => x[idField] == valueList.nativeList[key].id) != null) {
                    this.singleBondDetail.bondStr = selectedList.find(x => x[idField] == valueList.nativeList[key].id).bondStrength;
                    this.singleBondDetail.isSelected = true;
                }
                else {
                    this.singleBondDetail.bondStr = 0;
                    this.singleBondDetail.isSelected = false;
                }
                this.bondDetailDto.push(this.singleBondDetail);

                this.singleBondDetail = new BondDetailDto;

            }
       
        }

        updateBonds(item: BondDetailDto, bondType: number) {
      
            switch (bondType) {
                case 0: {

        
                    
                    this.symptomsSelectedList = this.symptomsSelectedList.filter(x => x.symptomId != item.id);
                    this.symptomsList.selected = this.symptomsList.selected.filter(x => x.id != item.id);
                    this.symptomsList.selectedNative = this.symptomsList.selectedNative.filter(x => x.id !== item.id);
          
                    if (item.isSelected) {
             
                        this.symptomsList.selectedNative.push(this.symptomsList.nativeList.find(x => x.id === item.id));

                        this.symptomsSelectedList.push({ symptomId: item.id, bondStrength: item.bondStr, symptomQuestionsId: this.symptomQuestionId, symptom: this.symptomsList.selectedNative.find(x => x.id === item.id) });

                        this.symptomsList.selected.push(item);
                    }
              
                    break;
                }
                case 1: {
                    this.diseaseSelectedList = this.diseaseSelectedList.filter(x => x.diseaseId != item.id);
                    this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));

                        this.diseaseSelectedList.push({ diseaseId: item.id, bondStrength: item.bondStr, symptomQuestionsId: this.symptomQuestionId, disease: this.diseaseList.selectedNative.find(x => x.id === item.id) });

                        this.diseaseList.selected.push(item);
                    }
                    break;
                }
            }



            this.symptomQuestion = Object.assign({}, this.symptomQuestion);
            this.isModified = true;
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number, addNewFunction, addButtonText: string) {
            this.isDescriptionDetailActive = false;
            this.isDiseaseDetailActive = false;

            if (bondType === 1) {
                this.strength = true;
                this.multi = true;
            } else {
                this.strength = false;
                this.multi = false;
            }
      
            if (!this.isDetailActive) {
                this.getBondDetailList(valueList, selectedList, bondType);

                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }

                this.bondType = bondType;
                this.isDetailActive = !this.isDetailActive;
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



            this.showAddButton = true;        
            this.addButtonText = addButtonText;
            this.addButtonAction = addNewFunction;
        }

        
        async loadSymptomQuestions() {
            this.loading = true;

            var result = await SymptomQuestionsApi.getSymptomQuestionDetail(this.symptomQuestionId);

            if (result.success) {
  
                this.symptomQuestion = result.data;
                if (this.symptomQuestion.diseases != null) {
                    const fullDiseases = await this.getFullDiseases(this.symptomQuestion.diseases);

                    this.diseaseSelectedList = fullDiseases;
                }
                if (this.symptomQuestion.symptoms != null) {
                    const fullSymptoms = await this.getFullSymptoms(this.symptomQuestion.symptoms)
                    
                    this.symptomsSelectedList = fullSymptoms;
        
                    // var s = this.symptomQuestion.symptom;
                    // this.symptomsSelectedList.push({ bondStrength: s.bondStrength, symptomQuestionsId: s.symptomQuestionsId, symptoms: s.symptoms, symptomsId: s.symptomsId });
                }
            }

            await Promise.all([
                this.getSymptoms(),
                this.getDiseases()
            ])
            
            this.loading = false;
            this.isModified = false;
        }

        async getFullSymptoms(symptoms: SymptomQuestionsSymptomDto[]) : Promise<SymptomQuestionsSymptomDto[]>{
            var ids = symptoms.map(x => x.symptomId)

            var result = await SymptomApi.getByIds(ids)

            if(result.success){
                var disArray = result.data as SymptomDto[];

                symptoms.forEach(d =>{
                    d.symptom = disArray.find(x => x.id == d.symptomId)
                })
            }

            return symptoms;
        }

        async getFullDiseases(diseases: SymptomQuestionsDiseaseDto[]): Promise<SymptomQuestionsDiseaseDto[]> {
            var ids = diseases.map(x => x.diseaseId);

            var result = await DiseaseApi.getByIds(ids);

            if (result.success) {
                var disArray = result.data as DiseaseDto[];

                diseases.forEach(d => {
                    d.disease = disArray.find(x => x.id == d.diseaseId);
                })
            }

            return diseases;
        }

        async getSymptoms() {
            if (this.symptomQuestion.symptoms != null && this.symptomQuestion.symptoms.length > 0) {
                var result = await SymptomApi.getByIds(this.symptomQuestion.symptoms.map(x => x.symptomId));
       
                if (result.success) {
                    var sDetail = result.data;

                    var symptomBond = this.symptomQuestion.symptoms;
                    for (var key in symptomBond) {
                        var symptom: SymptomDto = sDetail.find(x => x.id == symptomBond[key].symptomId);

                        this.symptomsList.selected.push({ bondStr: symptomBond[key].bondStrength, id: symptomBond[key].symptomId, isSelected: true, name: symptom.name });
                    }

                    this.symptomsList.selectedNative = this.symptomsList.selectedNative.concat(sDetail);

                    // this.symptomsList.selected.push({ bondStr: s.bondStrength, id: s.symptomsId, name: sDetail.name, isSelected: true });

                    // this.symptomsList.selectedNative.push(sDetail);
                }
            }
        }

        async getDiseases() {
            if (this.symptomQuestion.diseases != null && this.symptomQuestion.diseases.length > 0) {
                var result = await DiseaseApi.getByIds(this.symptomQuestion.diseases.map(x => x.diseaseId));

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.symptomQuestion.diseases;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }

        async updateDeepDisease(message: string, updatedDisease){
            await this.diseaseList.loadPage(updatedDisease);
            var bondDetail = this.diseaseList.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 1)
            this.showBondDetail(this.diseaseList, this.diseaseSelectedList, 0, this.showDiseaseAdd, 'Přidat příčinu');  
        }

        async updateSymptom(message: string, updatedSymptom){
            await this.symptomsList.loadPage(updatedSymptom);
   
            var bondDetail = this.symptomsList.displayList.find(x => x.id == updatedSymptom.id);
            this.updateBonds(bondDetail, 0)
            this.showBondDetail(this.symptomsList, this.symptomsSelectedList,1, this.showSymptomAdd, 'Přidat příznak');
        }

        addNew(){
            this.addButtonAction()
        }

        showSymptomAdd() {

            this.deepNew = true;
            this.deepId = -1;
            this.isSymptomAddActive = true;
        }

        showDiseaseAdd() {
     
            this.deepNew = true;
            this.deepId = -1;
            this.isDiseaseAddActive = true;
        }

        async saveSymptomQuestions() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            // create
            if (this.symptomQuestion == null) this.symptomQuestion = new SymptomQuestionsDto();

            if (this.diseaseSelectedList.length > 0) {
                var selected = [];
                for (var sel in this.diseaseSelectedList) {
                    var select = this.diseaseSelectedList[sel];
                    selected.push(new SymptomQuestionsDiseaseDto(select.symptomQuestionsId, select.bondStrength, select.disease));
                }

                this.symptomQuestion.diseases = selected;
            }

      
            if (this.symptomsSelectedList.length > 0) {
                var selectedSymptoms = [];
                for(var sel in this.symptomsSelectedList){
                    var selectedSymptom = this.symptomsSelectedList[sel];
          
                    selectedSymptoms.push(new SymptomQuestionsSymptomDto(selectedSymptom.symptomQuestionsId, selectedSymptom.bondStrength, selectedSymptom.symptom))
                }

                this.symptomQuestion.symptoms = selectedSymptoms
                // if (this.symptomQuestion.symptom == null) {
                //     this.symptomQuestion.symptom = new SymptomQuestionsSymptomDto();
                // } else {
                //     this.symptomQuestion.symptom.bondStrength = s.bondStrength;
                //     this.symptomQuestion.symptom.symptomQuestionsId = s.symptomQuestionsId;
                //     this.symptomQuestion.symptom.symptoms = s.symptoms;
                //     this.symptomQuestion.symptom.symptomsId = s.symptomsId
                //     if (this.isNew) this.symptomQuestion.symptom.symptomsId = s.symptomsId;
                // }
            }

      

            var result = this.isNew ? await SymptomQuestionsApi.createSymptomQuestions(this.symptomQuestion) :
                await SymptomQuestionsApi.updateSymptomQuestions(this.symptomQuestion);
            if (result.success) {
                this.symptomQuestion = result.data;
                this.isModified = false;
                console.log(this.symptomQuestion)
                this.$emit("updateSymptomQuestions", "Záznam uložen.", this.symptomQuestion);
                this.dialogShown = false;
            }
            else {
                for (var key in result.errors) {
                    var errors = result.errors[key];
                    for (var index in errors) {
                        this.error += `${errors[index]} `;
                        this.error += "<br>";
                    }
                }
            }
            this.saving = false;
            this.loading = false;
        }

        async deleteSymptomQuestions() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await SymptomQuestionsApi.deleteSymptomQuestions(this.symptomQuestionId);
            if (result.success) {
                this.$emit("updateSymptomQuestions", "Záznam odstraněn.");
            }
            this.saving = false;
            this.loading = false;
        }
    }
</script>
<style scoped>
    .dialog-footer {
        font-size: 13px;
        color: rgb(150,150,150);
    }
</style>
