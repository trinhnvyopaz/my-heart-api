<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="savePreventiveMeasure"
                  @delete="deletePreventiveMeasure"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat opatření' : 'Upravit opatření'"
                  :focusedDetail="focusedDetail">
        <div>{{error}}</div>
        <v-form ref="form">
            <v-text-field label="Název" v-model="preventiveMeasures.name" @focus="changeFocus(-1)" :rules="requiredRules" />
            <v-text-field label="Synonyma" :value="synonymString" @focus="changeFocus(2)" readonly/>
            <html-text-field label="Popis" :value="preventiveMeasures.description" @focus="changeFocus(0)" />
            <v-text-field label="Indikace" :value="diseaseString" @focus="showBondDetail(disease,preventiveMeasures.indication,0, showDiseaseAdd, 'Přidat indikaci');changeFocus(1)" readonly />
            <v-text-field label="Seznam pracovišť" :value="workspaceString" @focus="showBondDetail(medicalFacilities,preventiveMeasures.workspaceList,1, ShowFacilityAdd, 'Přidat pracoviště');changeFocus(1)" readonly />
        </v-form>

        <template #rightPanel>
            <v-tab-item><vue-editor ref="descriptionEditor" v-model="preventiveMeasures.description" /></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" /></v-tab-item>
            <v-tab-item>
                <synonym-detail ref="synonymDetail" :items="preventiveMeasures.synonyms" @delete-item="deleteSynonym" @add-item="addSynonym"  />
            </v-tab-item>
        </template>


        <!-- <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease"/> -->

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :diseaseId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease" />


        <medical-facilities-detail-dialog-deep :isNew="deepNew"
                                                v-model="IsFacilityAddActive"
                                                :medicalFacilitiesId="deepId"
                                                :levelsDeep="levelsDeep + 1"
                                                @updateMedicalFacility="updateMedicalFacilities"/>
    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import PreventiveMeasuresApi from "@backend/api/preventiveMeasures";
    import DiseaseApi from "@backend/api/disease";
    import MedicalFacilitiesApi from "@backend/api/medicalFacilitie";
    import PreventiveMeasuresDto from "../../../../models/professionInfo/PreventiveMeasuresDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import MedicalFacilitiesDto from "../../../../models/professionInfo/MedicalFacilitiesDto";

    import PreventiveMeasuresDiseaseDto from "../../../../models/professionInfo/bonds/PreventiveMeasuresDiseaseDto";
    import PreventiveMeasuresMedicalFacilitiesDto from "../../../../models/professionInfo/bonds/PreventiveMeasuresMedicalFacilitiesDto";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import DiseaseDetailDialogDeep from "@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue";
    import MedicalFacilitiesDetailDialogDeep from "@components/Manager/ProfessionInformation/MedicalFacilities/medical-facilities-detail-dialog-deep.vue"
    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"
    import SynonymDetail from "@components/Shared/synonymDetail.vue"
@Component({
    name: "PreventiveDetailDialogDeep",
    components: {
        BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
        DescriptionHtmlOutput, DiseaseDetailDialogDeep, SplitDialog, HtmlTextField, MedicalFacilitiesDetailDialogDeep,
        SynonymDetail    
    },
})
export default class PreventiveDetailDialogDeep extends Vue {
    deepId: number = -1;
    deepNew: boolean = true;
    loading: boolean = false;
    error: string | null = null;
    saving: boolean = false;
    isPreventiveLoaded: boolean = false;
    isModified: boolean = false;
    showDialogSave: boolean = false;
    isDiseaseAddActive: boolean = false;
    IsFacilityAddActive: boolean = false;

    disease: PagedStickyList = new PagedStickyList("disease");
    medicalFacilities: PagedStickyList = new PagedStickyList("medicalFacilities");
    preventiveMeasures: PreventiveMeasuresDto = new PreventiveMeasuresDto();

    workspace: PreventiveMeasuresMedicalFacilitiesDto = new PreventiveMeasuresMedicalFacilitiesDto;
    indication: PreventiveMeasuresDiseaseDto = new PreventiveMeasuresDiseaseDto;

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
    preventiveMeasuresId: number;

    @Watch("preventiveMeasures", { deep: true })
    onModified() {
        this.isModified = true;
    }

    @Watch("dialogShown", { deep: true })
    async dialogShownChanged() {
        this.isPreventiveLoaded = false;

        this.disease = new PagedStickyList("disease", true);
        this.medicalFacilities = new PagedStickyList("medicalFacilities", true);

        this.changeFocus(-1);
        this.error = ""

        if(this.$refs.form){
            this.$refs.form.resetValidation()
        }

        if (!this.isNew) {
            if (this.dialogShown) {
                await this.loadPreventiveMeasure();
            }

        }
        else {
            this.preventiveMeasures = new PreventiveMeasuresDto;
            this.preventiveMeasures.indication = [];
            this.preventiveMeasures.workspaceList = [];
            this.$set(this.preventiveMeasures, 'synonyms', []);
            this.isPreventiveLoaded = true;
        }
        this.isModified = false;
        this.isDetailActive = false;
        this.isDescriptionDetailActive = false;
    }

    get synonymString(){
        return this.preventiveMeasures?.synonyms?.map(x => x.name)?.join(', ')
    }

    deleteSynonym(name: string){
            const index = this.preventiveMeasures.synonyms.findIndex(s => s.name == name);
            if (index !== -1) {
                this.preventiveMeasures.synonyms.splice(index, 1);
            }
        }

    addSynonym(preventiveMeasure){
        this.preventiveMeasures.synonyms.push({                
            preventiveMeasureId: this.preventiveMeasures.id,
            isManual: preventiveMeasure.isManual,
            name: preventiveMeasure.name
        })
    }

    async changeFocus(index){
            this.focusedDetail = index
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
                this.showAddButton = false;
                this.$refs.synonymDetail.$refs.synonymField.focus()
            }   
        }

    //TODO refactor
    get diseaseString() {
        var disease = "";
        for (var key in this.preventiveMeasures.indication) {
            if (!this.disease.selectedNative.find(x => x.id == this.preventiveMeasures.indication[key].diseaseId))
                continue;
            disease = disease + this.disease.selectedNative.find(x => x.id == this.preventiveMeasures.indication[key].diseaseId).name
            disease = disease + " - " + this.preventiveMeasures.indication[key].bondStrength.toString() + ", ";
        }

        return disease.substring(0, disease.length - 2);
    }

    get workspaceString() {
        var workspace = "";
        for (var key in this.preventiveMeasures.workspaceList) {
            if (!this.medicalFacilities.selectedNative.find(x => x.id == this.preventiveMeasures.workspaceList[key].medicalFacilitiesId))
                continue;

            workspace = workspace + this.medicalFacilities.selectedNative.find(x => x.id == this.preventiveMeasures.workspaceList[key].medicalFacilitiesId).name
            workspace = workspace + " - " + this.preventiveMeasures.workspaceList[key].bondStrength.toString() + ", ";
        }

        return workspace.substring(0, workspace.length - 2);
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
    }

    getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
        this.bondDetailDto = [];
        let idField = ["diseaseId", "medicalFacilitiesId"][bondType];
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

    addNew(){
        this.addButtonAction()
    }
    
    showDiseaseAdd() {
        console.log(this.isDiseaseAddActive)
            this.deepNew = true;
            this.deepId = -1;
            this.isDiseaseAddActive = true;
    }
    ShowFacilityAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.IsFacilityAddActive = true;
    }
    async updateDeepDisease(message: string, updatedDisease){
        await this.disease.loadPage(updatedDisease);
        var bondDetail = this.disease.displayList.find(x => x.id == updatedDisease.id);
        this.updateBonds(bondDetail, 0)
        this.showBondDetail(this.disease, this.preventiveMeasures.indication,0, this.showDiseaseAdd, 'Přidat indikaci');  
    }
    async updateMedicalFacilities(message: string, updatedFacilities) {
        await this.medicalFacilities.loadPage(updatedFacilities);
        var bondDetail = this.medicalFacilities.displayList.find(x => x.id == updatedFacilities.id);
        this.updateBonds(bondDetail, 1)
        this.showBondDetail(this.medicalFacilities, this.preventiveMeasures.workspaceList,0, this.ShowFacilityAdd, 'Přidat pracoviště');  
    }
    
    //TODO refactor
    updateBonds(item: BondDetailDto, bondType: number) {
        this.isTaken = false;
        switch (bondType) {
            case 0: {
                this.preventiveMeasures.indication = this.preventiveMeasures.indication.filter(x => x.diseaseId != item.id);
                this.disease.selected = this.disease.selected.filter(x => x.id != item.id);
                this.disease.selectedNative = this.disease.selectedNative.filter(x => x.id !== item.id);
                if (item.isSelected) {
                    this.preventiveMeasures.indication.push({ diseaseId: item.id, bondStrength: item.bondStr, preventiveMeasuresId: this.preventiveMeasuresId });
                    this.disease.selectedNative.push(this.disease.nativeList.find(x => x.id === item.id));
                    this.disease.selected.push(item);
                }
                //this.disease.sort();
                break;
            }

            case 1: {
                this.preventiveMeasures.workspaceList = this.preventiveMeasures.workspaceList.filter(x => x.medicalFacilitiesId != item.id);
                this.medicalFacilities.selected = this.medicalFacilities.selected.filter(x => x.id != item.id);
                this.medicalFacilities.selectedNative = this.medicalFacilities.selectedNative.filter(x => x.id !== item.id);
                if (item.isSelected) {
                    this.preventiveMeasures.workspaceList.push({ medicalFacilitiesId: item.id, bondStrength: item.bondStr, preventiveMeasureId: this.preventiveMeasuresId });
                    this.medicalFacilities.selectedNative.push(this.medicalFacilities.nativeList.find(x => x.id === item.id));
                    this.medicalFacilities.selected.push(item);
                }
                //this.medicalFacilities.sort();
                break;
            }
        }

        let temp = Object.assign({}, this.preventiveMeasures);
        this.preventiveMeasures = temp;
        this.isModified = true;
    }

    async loadPreventiveMeasure() {
        this.loading = true;

        var result = await PreventiveMeasuresApi.getPreventiveMeasuresDetail(this.preventiveMeasuresId);

        if (result.success) {
            this.preventiveMeasures = result.data;  
            this.isPreventiveLoaded = true;

            await Promise.all([
                this.getDisease(),
                this.getMedicalFacilities()
            ]);
        }

        this.loading = false;
        this.isModified = false;
    }

    // TODO refactor
    async getDisease() {
        if (this.preventiveMeasures.indication != null && this.preventiveMeasures.indication.length > 0) {
            const diseaseIds = this.preventiveMeasures.indication.map(x => x.diseaseId);
            var result = await DiseaseApi.getByIds(diseaseIds);

            if (result.success) {
                var diseases = result.data;

                var diseaseBonds = this.preventiveMeasures.indication;
                for (var key in diseaseBonds) {
                    var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                    if (disease != null && disease != undefined) {
                        this.disease.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                    }
                }

                this.disease.selectedNative = this.disease.selectedNative.concat(diseases);
            }
        }
    }

    async getMedicalFacilities() {
        if (this.preventiveMeasures.workspaceList != null && this.preventiveMeasures.workspaceList.length > 0) {
            const faclities = this.preventiveMeasures.workspaceList.map(x => x.medicalFacilitiesId);
            console.log(facilities)
            var result = await MedicalFacilitiesApi.getByIds(faclities);

            if (result.success) {
                var facilities = result.data;
                console.log(facilities)
                var facilityBonds = this.preventiveMeasures.workspaceList;
                for (var key in facilityBonds) {
                    var facility: DiseaseDto = facilities.find(x => x.id == facilityBonds[key].medicalFacilitiesId);

                    if (facility != null && facility != undefined) {
                        this.medicalFacilities.selected.push({ bondStr: facilityBonds[key].bondStrength, id: facilityBonds[key].medicalFacilitiesId, isSelected: true, name: facility.name });
                    }
                }

                this.medicalFacilities.selectedNative = this.medicalFacilities.selectedNative.concat(facilities);
            }
        }
    }

    async savePreventiveMeasure() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = this.isNew ? await PreventiveMeasuresApi.createPreventiveMeasures(this.preventiveMeasures)
            : await PreventiveMeasuresApi.updatePreventiveMeasures(this.preventiveMeasures);
        if (result.success) {
            this.preventiveMeasures = result.data;
            this.isModified = false;
            this.$emit("updatePreventiveMeasure", "Záznam uložen.", this.preventiveMeasures);
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

    async deletePreventiveMeasure() {
        this.loading = true;
        this.error = "";
        this.saving = true;
        var result = await PreventiveMeasuresApi.deletePreventiveMeasures(this.preventiveMeasuresId);
        console.log(result)
        if (result.success) {
            this.$emit("deletePreventiveMeasure", "Záznam odstraněn.");
        }
        this.saving = false;
        this.loading = false;
    }

}
</script>