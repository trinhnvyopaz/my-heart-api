<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"                
                  @save="saveDisease"
                  @delete="deleteDisease"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat onemocnění' : 'Upravit onemocnění'"
                  :focusedDetail="focusedDetail">
        <div v-html="error"/>
        <v-form ref="form" v-model="valid" lazy-validation>
            <v-text-field label="Název" v-model="disease.name" @focus="changeFocus(-1)" :rules="requiredRules" />
            <v-text-field label="Synonyma" :value="synonymString" @focus="changeFocus(3)" readonly/>
            <html-text-field label="Popis" :value="disease.description" @focus="changeFocus(0)" />
            <v-text-field label="Prevalence na 100 000 obyvatel" v-model="disease.frequency" @focus="changeFocus(-1)" />
            <v-text-field label="Příčiny" :value="bondString('causes')" @focus="showBondDetail(causes,disease.causes,0, showDiseaseAdd, 'Přidat příčinu');changeFocus(1)" readonly />
            <v-text-field label="Příznaky" :value="bondString('symptoms')" @focus="showBondDetail(symptoms,disease.symptoms,1, showSymptomAdd, 'Přidat příznak');changeFocus(1)" readonly />
            <v-text-field label="Lékové skupiny" :value="bondString('medicamentGroup')" @focus="showBondDetail(medicamentGroup,disease.medicamentGroup,2, showMedicamentAdd, 'Přidat skupinu');changeFocus(1)" readonly />
            <v-text-field label="Nefarmakologické léčby" :value="bondString('nonpharmaticTherapy')" @focus="showBondDetail(nonpharmaticTherapy,disease.nonpharmaticTherapy,3, showNonpharmacyAdd, 'Přidat léčbu');changeFocus(1)" readonly />
            <v-text-field label="Preventivní opatření" :value="bondString('preventiveMeasures')" @focus="showBondDetail(preventiveMeasures,disease.preventiveMeasures,4, showPreventiveAdd, 'Přidat opatření');changeFocus(1)" readonly />

            <h3>Cílové hodnoty</h3>
            <v-text-field label="Hmotnost" v-model="disease.targetWeight" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" step="0.1" @focus="changeFocus(-1)" />
            <v-text-field label="Systolický tlak" v-model="disease.targetSystolicPressure" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" @focus="changeFocus(-1)" />
            <v-text-field label="Diastolický tlak" v-model="disease.targetDiastolicPressure" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" @focus="changeFocus(-1)" />
            <v-text-field label="Tepová frekvence" v-model="disease.targetHeartRate" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" @focus="changeFocus(-1)" />
            <v-text-field label="Hladina sérového LDL" v-model="disease.targetLdl" type="number" step="0.01" @wheel="$event.target.blur()" @keydown="handleKeys($event)" @focus="changeFocus(-1)" />

            <html-text-field label="Doporučení" :value="disease.systemicMeasures" @focus="changeFocus(2)" />
        </v-form>


        <template #rightPanel>
                <v-tab-item>
                    <vue-editor  ref="descriptionEditor" v-model="disease.description" />
                </v-tab-item>
                <v-tab-item>
                    <bond-detail  :items="valueList" ref="bondDetail" :bondType="bondType" @updateBonds="updateBonds"  />
                </v-tab-item>
                <v-tab-item>
                    <vue-editor  ref="measuresEditor" v-model="disease.systemicMeasures" />
                </v-tab-item>
                <v-tab-item>
                    <synonym-detail ref="synonymDetail" :items="disease.synonyms" @delete-item="deleteSynonym" @add-item="addSynonym"  />
                </v-tab-item>
        </template>

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease"/>

        <symptom-detail-dialog-deep :isNew="deepNew"
                                    v-model="isSymptomAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateSymptom="updateSymptom" />

        <medicament-group-detail-dialog-deep :isNew="deepNew"
                                            v-model="isMedicamentAddActive"
                                            :symptomId="deepId"
                                            :levelsDeep="levelsDeep + 1"
                                            @updateMedicamentGroup="updateMedicamentGroup" />

        <preventive-detail-dialog-deep :isNew="deepNew"
                                            v-model="isPreventiveAddActive"
                                            :symptomId="deepId"
                                            :levelsDeep="levelsDeep + 1"
                                            @updatePreventiveMeasure="updatePreventiveMeasure" />

        <nonpharmacy-detail-dialog-deep :isNew="deepNew"
                                        v-model="isNonpharmaAddActive"
                                        :symptomId="deepId"
                                        :levelsDeep="levelsDeep + 1"
                                        @updateNonpharmacy="updateNonpharmacy" />

        <v-snackbar right top v-model="snackbarShown" color="success" v-html="snackbarText" />

    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import MedicamentGroupDto from "../../../../models/professionInfo/MedicamentGroupDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import SymptomDto from "../../../../models/professionInfo//SymptomDto";
    import NonpharmacyDto from "../../../../models/professionInfo//NonpharmacyDto";
    import PreventiveMeasuresDto from "../../../../models/professionInfo//PreventiveMeasuresDto";

    import DiseaseApi from "@backend/api/disease";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";
    import SymptomApi from "@backend/api/symptom";
    import NonpharmacyApi from "@backend/api/nonpharmacy";
    import PreventiveMeasuresApi from "@backend/api/preventiveMeasures";

    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import DiseaseDiseaseDto from "../../../../models/professionInfo/bonds/DiseaseDiseaseDto";
    import DiseaseMedicaDto from "../../../../models/professionInfo/bonds/DiseaseMedicamentGroupDto";
    import DiseaseNonpharma from "../../../../models/professionInfo/bonds/DiseaseNonpharmaticTherapyDto";
    import DiseasePreventive from "../../../../models/professionInfo/bonds/DiseasePreventiveMeasuresDto";
    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";

    import SymptomDiseaseDto from "../../../../models/professionInfo/bonds/DiseaseSymptomDto";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"
    import ArrayFunctions from "@helpers/ArrayFunctions"
    import { update } from "lodash";
    import SynonymDetail from "@components/Shared/synonymDetail.vue"
    import DiseaseSynonymDto from "../../models/professionInfo/bonds/DiseaseSynonymDto";

    @Component({
        name: "DiseaseDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
            DescriptionHtmlOutput, SplitDialog, HtmlTextField, SynonymDetail
        },
    })
    export default class DiseaseDetailDialogDeep extends Vue {
        deepId: number = -1;        
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        medicamentGroup: PagedStickyList = new PagedStickyList("medicamentgroup");
        causes: PagedStickyList = new PagedStickyList("disease");
        disease: DiseaseDto = new DiseaseDto;
        symptoms: PagedStickyList = new PagedStickyList("symptom");
        nonpharmaticTherapy: PagedStickyList = new PagedStickyList("nonpharmacy");
        preventiveMeasures: PagedStickyList = new PagedStickyList("preventive");
        saving: boolean = false;
        isDiseaseLoaded: boolean = false;
        isDescriptionDetailActive: boolean = false;
        isModified: boolean = false;
        isDiseaseAddActive: boolean = false;
        isSymptomAddActive: boolean = false;
        isMedicamentAddActive: boolean = false;
        isNonpharmaAddActive: boolean = false;
        isPreventiveAddActive: boolean = false;

        cause: DiseaseDiseaseDto = new DiseaseDiseaseDto;
        medica: DiseaseMedicaDto = new DiseaseMedicaDto;
        symptom: SymptomDiseaseDto = new SymptomDiseaseDto;
        nonpharma: DiseaseNonpharma = new DiseaseNonpharma;
        preventive: DiseasePreventive = new DiseasePreventive;

        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;

        isDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null

        focusedDetail = -1;
        snackbarShown = false
        snackbarText = ""
        requiredRules = [v => (v != null && v !="") || 'Povinné pole']
        valid: boolean = true
    
        @Prop({ default: false }) value: boolean = false; // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        @Prop({ default: false })
        isNew: boolean;

        @Prop({default: 0})
        levelsDeep: number 

        @Prop({ default: -1 })
        diseaseId: number;


        get synonymString(){
            return this.disease?.synonyms?.map(x => x.name)?.join(', ')
        }

        toggleFocus(event){
            console.log(event)
        }

        deleteSynonym(name: string){
            const index = this.disease.synonyms.findIndex(s => s.name == name);
            if (index !== -1) {
                this.disease.synonyms.splice(index, 1);
            }
        }

        addSynonym(disease){
            this.disease.synonyms.push({                
                diseaseId: this.disease.id,
                isManual: disease.isManual,
                name: disease.name
            })
        }

        async changeFocus(index){
            this.focusedDetail = index;
            await FocusHelper.delay(100)

            if(this.focusedDetail == 0){             
                this.showAddButton = false     
                FocusHelper.focusEditor(this.$refs.descriptionEditor.quill)
            }                        
            else if(this.focusedDetail == -1){
                this.showAddButton = false
            }  
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()
                
            }else if(this.focusedDetail == 2){
                this.showAddButton = false
                FocusHelper.focusEditor(this.$refs.measuresEditor.quill)
            }else if(this.focusedDetail == 3){
                this.showAddButton = false;
                this.$refs.synonymDetail.$refs.synonymField.focus()
            }                
        }

       
        @Watch("disease", { deep: true })
        onModified() {
            this.isModified = true;
        }

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.isDiseaseLoaded = false;

            this.medicamentGroup = new PagedStickyList("medicamentgroup", true);
            this.symptoms = new PagedStickyList("symptom", true);
            this.causes = new PagedStickyList("disease", true);
            this.nonpharmaticTherapy = new PagedStickyList("nonpharmacy", true);
            this.preventiveMeasures = new PagedStickyList("preventive", true);

            this.error = "";
            this.changeFocus(-1);

            if (this.$refs.form) {
                this.$refs.form.resetValidation();
            }
      
            if (!this.isNew) {
                if (this.dialogShown)
                    await this.loadDisease();
                
            }
            else {
                this.isDiseaseLoaded = true;
                this.disease = new DiseaseDto;
                this.disease.causes = [];
                this.disease.symptoms = [];
                this.disease.medicamentGroup = [];
                this.disease.nonpharmaticTherapy = [];
                this.disease.preventiveMeasures = [];
                this.$set(this.disease, 'synonyms', []);
            }

            this.isModified = false;
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
        }

        beforeCreate(){
            this.$options.components.DiseaseDetailDialog = require('@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue').default
            this.$options.components.NonpharmacyDetailDialogDeep = require('@components/Manager/ProfessionInformation/NonpharmaticTherapy/nonpharmatic-detail-dialog-deep.vue').default
            this.$options.components.PreventiveDetailDialogDeep = require('@components/Manager/ProfessionInformation/PreventiveMeasures/preventive-detail-dialog-deep.vue').default
            this.$options.components.MedicamentGroupDetailDialogDeep = require('@components/Manager/ProfessionInformation/MedicamentGroup/medicament-group-detail-dialog-deep.vue').default
            this.$options.components.SymptomDetailDialogDeep = require('@components/Manager/ProfessionInformation/Symptoms/symptom-detail-dialog-deep.vue').default
        }

        showSymptomAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isSymptomAddActive = true;
        }
        showMedicamentAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isMedicamentAddActive = true;
        }
        showPreventiveAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isPreventiveAddActive = true;
        }

        showNonpharmacyAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isNonpharmaAddActive = true;
        }
        showDiseaseAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isDiseaseAddActive = true;
        }
        addNew(){
            this.addButtonAction()
        }

        updateDisease(status) {
            this.$refs.dataTable.refresh();
            this.showSnackbar(status.message, status.color);
        }
        hideSymptomAddDialog() {
            this.isSymptomAddActive = false;
            this.symptoms.loadPage();
        }

        bondString(type: string) {
            var result = "";
            for (var key in this.disease[type]) {
                // fields are named inconsistently so we gotta do this unfortunate thing:
                var idField = type == "causes" ? "causeId" : type + "Id";
                if (!this[type].selectedNative.find(x => x.id == this.disease[type][key][idField]))
                    continue;
                result += this[type].selectedNative.find(x => x.id == this.disease[type][key][idField]).name
                result += " - " + this.disease[type][key].bondStrength.toString() + ", ";
            }
     
            return result.substring(0, result.length - 2);
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number, addNewFunction, addButtonText: string) {
            if (!this.isDetailActive) {
                this.getBondDetailList(valueList, selectedList, bondType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.bondType = bondType;
                this.isDetailActive = !this.isDetailActive;
                this.isDescriptionDetailActive = false;
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

            console.log(this.valueList)
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];

            if (!selectedList)
                selectedList = [];

            console.log(selectedList)

            let idField = ["causeId", "symptomsId", "medicamentGroupId", "nonpharmaticTherapyId", "preventiveMeasuresId"][bondType];
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
            let idField = ["causeId", "symptomsId", "medicamentGroupId", "nonpharmaticTherapyId", "preventiveMeasuresId"][bondType];
            let field = ["causes", "symptoms", "medicamentGroup", "nonpharmaticTherapy", "preventiveMeasures"][bondType];
            this.disease[field] = this.disease[field].filter(x => x[idField] != item.id);
            this[field].selected = this[field].selected.filter(x => x.id != item.id);
            this[field].selectedNative = this[field].selectedNative.filter(x => x.id !== item.id);
            if (item.isSelected) {
                this[field].selectedNative.push(this[field].nativeList.find(x => x.id === item.id));
                let bonded = { idField: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId };
                bonded[idField] = item.id;
                this.disease[field].push(bonded);

                this[field].selected.push(item);
            }
            let temp = Object.assign({}, this.disease);
            this.disease = temp;
            this.isModified = true;
        }

        async getBonds(api: any, type: string) {
            // fields are named inconsistently so we gotta do this unfortunate thing:
            var idField = type == "causes" ? "causeId" : type + "Id";

            if (this.disease[type] != null && this.disease[type].length > 0) {
                var result = await api.getByIds(this.disease[type].map(x => x[idField]));

                if (result.success) {
                    var bondedData = result.data;

                    var bonds = this.disease[type];
                    for (var key in bonds) {
                        var bonded = bondedData.find(x => x.id == bonds[key][idField]);

                        if (bonded != null && bonded != undefined) {
                            this[type].selected.push({ bondStr: bonds[key].bondStrength, id: bonds[key][idField], isSelected: true, name: bonded.name });
                        }
                    }

                    this[type].selectedNative = this[type].selectedNative.concat(bondedData);
                }
            }
        }
        
        async updatePreventiveMeasure(message: string, updatedMeasure){                                   
            await this.preventiveMeasures.loadPage(updatedMeasure);
            var bondDetail = this.preventiveMeasures.displayList.find(x => x.id == updatedMeasure.id);
            this.updateBonds(bondDetail, 4)
            this.showBondDetail(this.preventiveMeasures,this.disease.preventiveMeasures,4, this.showPreventiveAdd, 'Přidat opatření')
        }

        async updateMedicamentGroup(message: string, updatedMedicamentGroup){
            await this.medicamentGroup.loadPage(updatedMedicamentGroup);
            var bondDetail = this.medicamentGroup.displayList.find(x => x.id == updatedMedicamentGroup.id);
            this.updateBonds(bondDetail, 2)
            this.showBondDetail(this.medicamentGroup, this.disease.medicamentGroup,2, this.showMedicamentAdd, 'Přidat skupinu')
        }

        async updateSymptom(message: string, updatedSymptom){
            await this.symptoms.loadPage(updatedSymptom);
            var bondDetail = this.symptoms.displayList.find(x => x.id == updatedSymptom.id);
            this.updateBonds(bondDetail, 1)
            this.showBondDetail(this.symptoms, this.disease.symptoms,1, this.showSymptomAdd, 'Přidat příznak');
        }

        async updateDeepDisease(message: string, updatedDisease){
            await this.causes.loadPage(updatedDisease);
            console.log(this.causes)
            console.log(updatedDisease.id)
            var bondDetail = this.causes.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 0)
            this.showBondDetail(this.causes, this.disease.causes,0, this.showDiseaseAdd, 'Přidat příčinu');  
        }
        
        async updateNonpharmacy(message: string, updatedNonpharmacy){           
            await this.nonpharmaticTherapy.loadPage(updatedNonpharmacy)
            console.log(updatedNonpharmacy)
            var bondDetail = this.nonpharmaticTherapy.displayList.find(x => x.id == updatedNonpharmacy.id);
            this.updateBonds(bondDetail, 3)
            this.showBondDetail(this.nonpharmaticTherapy, this.disease.nonpharmaticTherapy,3, this.showNonpharmacyAdd, 'Přidat léčbu');
        }

        async loadDisease() {
            this.isDiseaseLoaded = false;
            this.loading = true;

            var result = await DiseaseApi.getDiseaseDetail(this.diseaseId);
            if (result.success) {
                this.disease = result.data;
                
                this.isDiseaseLoaded = true;
            }
            
            console.log('loadDisease')

            await Promise.all([
                this.getBonds(PreventiveMeasuresApi, "preventiveMeasures"),
                this.getBonds(NonpharmacyApi, "nonpharmaticTherapy"),
                this.getBonds(SymptomApi, "symptoms"),
                this.getBonds(DiseaseApi, "causes"),
                this.getBonds(MedicamentGroupApi, "medicamentGroup")
            ]);

            this.loading = false;
            this.isModified = false;
        }

        async saveDisease() {
            console.warn('SAVING DISEASE', this.disease);

            if(!this.$refs.form.validate()){
                return
            }

            this.loading = true;
            this.error = "";
            this.saving = true;

            console.log('save disease', this.disease)

            var result = this.isNew ?
                await DiseaseApi.createDisease(this.disease) :
                await DiseaseApi.updateDisease(this.disease);

            if (result.success) {
                this.disease = result.data;
                this.isModified = false;
                this.$emit("updateDisease", { message: "Záznam uložen.", color: "success" }, this.disease);
                this.dialogShown = false;
            }
            else {
                this.$emit("updateDisease", { message: "Záznam se nepodařilo uložit.", color: "error" });
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

        async deleteDisease() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await DiseaseApi.deleteDisease(this.diseaseId);
            if (result.success) {
                this.$emit("updateDisease", {message: "Záznam odstraněn.", color: "success"});
            }
            this.saving = false;
            this.loading = false;
        }

        handleKeys(e) {
            if (e.keyCode === 38 || e.keyCode === 40) {
                e.target.blur();
            }
        }
    }
</script>
<style scoped>
.v-dialog{
    max-height: 400px;
}
.small-input{
    scale: .7;
    width: 142%;
    transform-origin: 0 0;
    margin-bottom: -10px;
    /* font-size: 10px!important; */
}
</style>