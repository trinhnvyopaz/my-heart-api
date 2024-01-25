<template>
    <div>
        <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="saveNonpharmacy"
                  @delete="deleteNonpharmacy"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat léčbu' : 'Upravit léčbu'"
                  :focusedDetail="focusedDetail">
            <div v-html="error"/>
            <v-form ref="form" v-model="valid" lazy-validation>
                <v-text-field label="Název" v-model="nonpharmacy.name" @focus="changeFocus(-1)" :rules="requiredRules" />
                <v-text-field label="Synonyma" :value="synonymString" @focus="changeFocus(2)" readonly/>
                <html-text-field label="Popis" :value="nonpharmacy.description" @focus="changeFocus(0)"  />
                <v-text-field label="Indikace" :value="indicationString" @focus="showBondDetail(indicationList,nonpharmacy.indication,0, showIndicationAdd, 'Přidat indikaci');changeFocus(1)" readonly />
                <v-text-field label="Komplikace" :value="complicationString" @focus="showBondDetail(complicationList,nonpharmacy.complication,1, showComplicationmAdd, 'Přidat komplikaci');changeFocus(1)" readonly />
                <v-text-field label="Účinnost" v-model="nonpharmacy.efficiency" @focus="changeFocus(-1)" />
                <v-text-field label="Délka hospitalizace" v-model="nonpharmacy.hospitalizationLength" @focus="changeFocus(-1)" />
                <v-text-field label="Zdravotnická zařízení" :value="medicalFacilitiesString" @focus="showBondDetail(medicalFacilities,nonpharmacy.medicalIntervention,2, showMedicalFacilities, 'Přidat zařízení');changeFocus(1)" readonly />
            </v-form>



            <template #rightPanel>
                <v-tab-item><vue-editor ref="descriptionEditor" v-model="nonpharmacy.description" /></v-tab-item>
                <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" /></v-tab-item>
                <v-tab-item>
                    <synonym-detail ref="synonymDetail" :items="nonpharmacy.synonyms" @delete-item="deleteSynonym" @add-item="addSynonym"  />
                </v-tab-item>
            </template>

            <disease-detail-dialog-deep :isNew="deepNew"
                                        v-model="isIndicationAddActive"
                                        :symptomId="deepId"
                                        :levelsDeep="levelsDeep + 1"
                                        @updateDisease="updateIndicationDisease"/>

            <disease-detail-dialog-deep :isNew="deepNew"
                                        v-model="isComplicationAddActive"
                                        :symptomId="deepId"
                                        :levelsDeep="levelsDeep + 1"
                                        @updateDisease="updateComplicationDisease"/>

            <medical-facilities-detail-dialog-deep :isNew="deepNew"
                                        v-model="isFacilityAddActive"
                                        :medicalFacilitiesId="deepId"
                                        :levelsDeep="levelsDeep + 1"
                                        @updateMedicalFacility="updateFacility" />
        </split-dialog>

        <v-snackbar right top v-model="snackbarShown" color="error">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import NonpharmacyApi from "@backend/api/nonpharmacy";
    import DiseaseApi from "@backend/api/disease";
    import MedicalFacilitiesApi from "@backend/api/medicalFacilitie";
    import NonpharmacyDto from "../../../../models/professionInfo/NonpharmacyDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import MedicalFacilitiesDto from "../../../../models/professionInfo/MedicalFacilitiesDto";

    import NonpharmaticTherapyDiseaseDto from "../../../../models/professionInfo/bonds/NonpharmaticTherapyDiseaseDto";
    import NonpharmaticTherapyMedicalFaclitiesDto from "../../../../models/professionInfo/bonds/NonpharmaticTherapyMedicalFaclitiesDto";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"
    import { update } from "lodash";
    import SynonymDetail from "@components/Shared/synonymDetail.vue"

    @Component({
        name: "NonpharmacyDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
            DescriptionHtmlOutput, SplitDialog, HtmlTextField, SynonymDetail
        },
    })
    export default class NonpharmacyDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isNonpharmacyLoaded: boolean = false;
        isModified: boolean = false;
        showDialogSave: boolean = false;
        isIndicationAddActive: boolean = false;
        isComplicationAddActive: boolean = false;
        isFacilityAddActive: boolean = false;

        indicationList: PagedStickyList = new PagedStickyList("disease");
        complicationList: PagedStickyList = new PagedStickyList("disease");
        medicalFacilities: PagedStickyList = new PagedStickyList("medicalFacilities");

        nonpharmacy: NonpharmacyDto = new NonpharmacyDto();
        indication: NonpharmaticTherapyDiseaseDto = new NonpharmaticTherapyDiseaseDto;
        medicalFacilitie: NonpharmaticTherapyMedicalFaclitiesDto = new NonpharmaticTherapyMedicalFaclitiesDto;

        //bond detail variables
        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        isTaken: boolean = false;
        isDescriptionDetailActive: boolean = false;
        requiredRules = [v => (v != null && v !="") || 'Povinné pole'];

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null
        valid: boolean = true

        snackbarShown: boolean = false
        snackbarText: string = "";

        focusedDetail = -1;

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
        nonpharmacyId: number;

        @Watch("nonpharmacy", { deep: true })
        onModified() {
            this.isModified = true;
        }

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.isNonpharmacyLoaded = false;

            this.indicationList = new PagedStickyList("disease", true);
            this.complicationList = new PagedStickyList("disease", true);
            this.medicalFacilities = new PagedStickyList("medicalFacilities", true);

            this.changeFocus(-1);
            this.error = ""

            if(this.$refs.form){
                this.$refs.form.resetValidation()
            }

            if (!this.isNew) {
                if (this.dialogShown) {
                    await this.loadNonpharmacy();
                }

            }
            else {
                this.isNonpharmacyLoaded = true;
                this.nonpharmacy = new NonpharmacyDto;
                this.nonpharmacy.complication = [];
                this.nonpharmacy.indication = [];
                this.nonpharmacy.medicalIntervention = [];
                this.$set(this.nonpharmacy, 'synonyms', []);
            }

            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isModified = false;
            this.showDialogSave = false;
        }

        get synonymString(){
            return this.nonpharmacy?.synonyms?.map(x => x.name)?.join(', ')
        }

        beforeCreate(){
            this.$options.components.DiseaseDetailDialogDeep = require('@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue').default;
            this.$options.components.MedicalFacilitiesDetailDialogDeep = require("@components/Manager/ProfessionInformation/MedicalFacilities/medical-facilities-detail-dialog-deep.vue").default;
        }

        async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)

            if(this.focusedDetail == 0){                  
                this.showAddButton =false;
                FocusHelper.focusEditor(this.$refs.descriptionEditor.quill)
            }                          
            else if(this.focusedDetail == -1){
                this.showAddButton =false;
            }
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()
                
            }else if(this.focusedDetail == 2){
                this.showAddButton = false;
                this.$refs.synonymDetail.$refs.synonymField.focus()
            }    
        }

        //TODO refactor
        get indicationString() {
            var disease = "";
            for (var key in this.nonpharmacy.indication) {
                if (!this.indicationList.selectedNative.find(x => x.id == this.nonpharmacy.indication[key].diseaseId))
                    continue;
                disease = disease + this.indicationList.selectedNative.find(x => x.id == this.nonpharmacy.indication[key].diseaseId).name
                disease = disease + " - " + this.nonpharmacy.indication[key].bondStrength.toString() + ", ";
            }
            return disease.substring(0, disease.length - 2);
        }

        get complicationString() {
            var disease = "";
            for (var key in this.nonpharmacy.complication) {
                if (!this.complicationList.selectedNative.find(x => x.id == this.nonpharmacy.complication[key].diseaseId))
                    continue;
                disease = disease + this.complicationList.selectedNative.find(x => x.id == this.nonpharmacy.complication[key].diseaseId).name
                disease = disease + " - " + this.nonpharmacy.complication[key].bondStrength.toString() + ", ";
            }
            return disease.substring(0, disease.length - 2);
        }

        get medicalFacilitiesString() {
            var medicalFacilities = "";
            for (var key in this.nonpharmacy.medicalIntervention) {
                if (!this.medicalFacilities.selectedNative.find(x => x.id == this.nonpharmacy.medicalIntervention[key].medicalFacilitiesId))
                    continue;
                medicalFacilities = medicalFacilities + this.medicalFacilities.selectedNative.find(x => x.id == this.nonpharmacy.medicalIntervention[key].medicalFacilitiesId).name
                medicalFacilities = medicalFacilities + " - " + this.nonpharmacy.medicalIntervention[key].bondStrength.toString() + ", ";
            }

            return medicalFacilities.substring(0, medicalFacilities.length - 2);
        }


        showIndicationAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isIndicationAddActive = true;
        }

        showComplicationmAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isComplicationAddActive = true;
        }

        showMedicalFacilities() {
            this.deepNew = true;
            this.deepId = -1;
            this.isFacilityAddActive = true;
        }

        deleteSynonym(name: number){
            const index = this.nonpharmacy.synonyms.findIndex(s => s.name == name);
            if (index !== -1) {
                this.nonpharmacy.synonyms.splice(index, 1);
            }
        }

        addSynonym(nonpharmacy){
            this.nonpharmacy.synonyms.push({                
                nonpharmaticTherapyId: this.nonpharmacy.id,
                isManual: nonpharmacy.isManual,
                name: nonpharmacy.name
            })
        }

        async updateIndicationDisease(message: string, updatedDisease){
            await this.indicationList.loadPage(updatedDisease);
            var bondDetail = this.indicationList.displayList.find(x => x.id == updatedDisease.id);
            console.log(updatedDisease)
            console.log(this.indicationList.displayList)
            console.log(bondDetail)
            this.updateBonds(bondDetail, 0)
            this.showBondDetail(this.indicationList, this.nonpharmacy.indication,0, this.showIndicationAdd, 'Přidat indikaci');  
        }

        async updateComplicationDisease(message: string, updatedDisease){
            await this.complicationList.loadPage(updatedDisease);
            var bondDetail = this.complicationList.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 1)
            this.showBondDetail(this.complicationList, this.nonpharmacy.complication,1, this.showComplicationmAdd, 'Přidat komplikaci');  
        }
        
        async updateFacility(message: string, updatedFacility){
            await this.medicalFacilities.loadPage(updatedFacility);
            var bondDetail = this.medicalFacilities.displayList.find(x => x.id == updatedFacility.id);
            this.updateBonds(bondDetail, 2)
            this.showBondDetail(this.medicalFacilities, this.nonpharmacy.medicalIntervention,2, this.showMedicalFacilities, 'Přidat zařízení');  
        }

        addNew(){
            this.addButtonAction()
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

        //TODO refactor
        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];
            let idField = ["diseaseId", "diseaseId", "medicalFacilitiesId"][bondType];
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

                    case 1: {
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

                    case 2: {
                        if (selectedList.length > 0 && selectedList.find(x => x.medicalFacilitiesId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.medicalFacilitiesId == valueList.nativeList[key].id).bondStrength;
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

        //TODO refactor
        updateBonds(item: BondDetailDto, bondType: number) {
            this.isTaken = false;
            switch (bondType) {
                case 0: {
                    this.nonpharmacy.indication = this.nonpharmacy.indication.filter(x => x.diseaseId != item.id);
                    this.indicationList.selected = this.indicationList.selected.filter(x => x.id != item.id);
                    this.indicationList.selectedNative = this.indicationList.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.nonpharmacy.indication.push({ diseaseId: item.id, bondStrength: item.bondStr, NonpharmaticTherapyId: this.nonpharmacyId });
                        this.indicationList.selectedNative.push(this.indicationList.nativeList.find(x => x.id === item.id));
                        this.indicationList.selected.push(item);
                    }
                    //this.indicationList.sort();
                    break;
                }

                case 1: {
                    this.nonpharmacy.complication = this.nonpharmacy.complication.filter(x => x.diseaseId != item.id);
                    this.complicationList.selected = this.complicationList.selected.filter(x => x.id != item.id);
                    this.complicationList.selectedNative = this.complicationList.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.nonpharmacy.complication.push({ diseaseId: item.id, bondStrength: item.bondStr, NonpharmaticTherapyId: this.nonpharmacyId });
                        this.complicationList.selectedNative.push(this.complicationList.nativeList.find(x => x.id === item.id));
                        this.complicationList.selected.push(item);
                    }
                    //this.complicationList.sort();
                    break;
                }

                case 2: {
                    this.nonpharmacy.medicalIntervention = this.nonpharmacy.medicalIntervention.filter(x => x.medicalFacilitiesId != item.id);
                    this.medicalFacilities.selected = this.medicalFacilities.selected.filter(x => x.id != item.id);
                    this.medicalFacilities.selectedNative = this.medicalFacilities.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.nonpharmacy.medicalIntervention.push({ medicalFacilitiesId: item.id, bondStrength: item.bondStr, NonpharmaticTherapyId: this.nonpharmacyId });
                        this.medicalFacilities.selectedNative.push(this.medicalFacilities.nativeList.find(x => x.id === item.id));
                        this.medicalFacilities.selected.push(item);
                    }
                    //this.medicalFacilities.sort();
                    break;
                }
            }

            let temp = Object.assign({}, this.nonpharmacy);
            this.nonpharmacy = temp;
            this.isModified = true;
        }




        async loadNonpharmacy() {
            this.loading = true;

            var result = await NonpharmacyApi.getNonpharmacyDetail(this.nonpharmacyId);

            if (result.success) {
                this.nonpharmacy = result.data;
                this.isNonpharmacyLoaded = true;

                            
                this.$refs.form.resetValidation()

                await Promise.all([
                    this.getIndications(),
                    this.getComplications(),
                    this.getMedicalFacilities()
                ]);
            }

            this.loading = false;
            this.isModified = false;
        }

        //TODO refactor
        async getIndications() {
            if (this.nonpharmacy.indication != null && this.nonpharmacy.indication.length > 0) {
                const diseaseIds = this.nonpharmacy.indication.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.nonpharmacy.indication;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.indicationList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.indicationList.selectedNative = this.indicationList.selectedNative.concat(diseases);
                }
            }
        }

        async getComplications() {
            if (this.nonpharmacy.complication != null && this.nonpharmacy.complication.length > 0) {
                const diseaseIds = this.nonpharmacy.complication.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.nonpharmacy.complication;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.complicationList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.complicationList.selectedNative = this.complicationList.selectedNative.concat(diseases);
                }
            }
        }

        async getMedicalFacilities() {
            if (this.nonpharmacy.medicalIntervention != null && this.nonpharmacy.medicalIntervention.length > 0) {
                const faclities = this.nonpharmacy.medicalIntervention.map(x => x.medicalFacilitiesId);
                var result = await MedicalFacilitiesApi.getByIds(faclities);
                console.log(result)
                if (result.success) {
                    var facilities = result.data;

                    var facilityBonds = this.nonpharmacy.medicalIntervention;
                    for (var key in facilityBonds) {
                        var facility: MedicalFacilitiesDto = facilities.find(x => x.id == facilityBonds[key].medicalFacilitiesId);

                        if (facility != null && facility != undefined) {
                            this.medicalFacilities.selected.push({ bondStr: facilityBonds[key].bondStrength, id: facilityBonds[key].medicalFacilitiesId, isSelected: true, name: facility.name });
                        }
                    }

                    this.medicalFacilities.selectedNative = this.medicalFacilities.selectedNative.concat(facilities);
                }
            }
        }

        async saveNonpharmacy() {
            if(!this.$refs.form.validate()){
                return
            }

            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = this.isNew ? await NonpharmacyApi.createNonpharmacy(this.nonpharmacy)
                : await NonpharmacyApi.updateNonpharmacy(this.nonpharmacy);
                console.log(result)
            if (result.success) {
                this.nonpharmacy = result.data;
                this.isModified = false;
                this.$emit("updateNonpharmacy", "Záznam uložen.", this.nonpharmacy);
                this.dialogShown = false;
            }
            else {
                for (var key in result.errors) {
                    var error = result.errors[key];
                    this.snackbarText = error
                    this.snackbarShown = true;
                }
            }
            this.saving = false;
            this.loading = false;
        }

        async deleteNonpharmacy() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await NonpharmacyApi.deleteNonpharmacy(this.nonpharmacyId);
            if (result.success) {
                this.$emit("updateNonpharmacy", "Záznam odstraněn.");
            }
            this.saving = false;
            this.loading = false;
        }
    }
</script>