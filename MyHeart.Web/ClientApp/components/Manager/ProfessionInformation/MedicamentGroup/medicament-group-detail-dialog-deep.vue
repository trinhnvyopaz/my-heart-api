<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="saveMedicamentGroup"
                  @delete="deleteMedicamentGroup"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat skupinu' : 'Upravit skupinu'"
                  :focusedDetail="focusedDetail">
        <div>{{error}}</div>
        <v-form ref="form">
            <v-text-field label="Název" :rules="requiredRules" v-model="medicamentGroup.name" @focus="changeFocus(-1)" />
            <html-text-field label="Popis" :value="medicamentGroup.description" @focus="changeFocus(0)" />
            <v-text-field label="ATC" :value="atcsString" @focus="showBondDetail(atcs, medicamentGroup.atcs, 0, null, '');changeFocus(1)" readonly />
            <v-text-field label="Indikace" :value="diseaseString" @focus="showBondDetail(indications,medicamentGroup.indication,1, showIndicationAdd, 'Přidat indikaci');changeFocus(1)" readonly />
            <v-text-field label="Kontraindikace" :value="contraindicationsString" @focus="showBondDetail(contraIndications, medicamentGroup.contraindication, 2, showContraIndicationAdd, 'Přidat Kontraindikaci');changeFocus(1)" readonly />
            <v-text-field label="Nežádoucí účinky" :value="symptomsString" @focus="showBondDetail(symptoms,medicamentGroup.sideEffects,3, showSymptomAdd, 'Přidat příznak');changeFocus(1)" readonly />
        </v-form>


        <template #rightPanel>
            <v-tab-item><vue-editor ref="descriptionEditor" v-model="medicamentGroup.description" /></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" :strength="strength" /></v-tab-item>
        </template>


        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isIndicationAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateIndications"/>

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isContraIndicationAdActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateContracIndication"/>

        <symptom-detail-dialog-deep :isNew="deepNew"
                                    v-model="isSymptomAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateSymptom="updateSymptom" />
    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";
    import DiseaseApi from "@backend/api/disease";
    import PharmacyApi from "@backend/api/pharmacy";
    import SymptomApi from "@backend/api/symptom";
    import AtcApi from "@backend/api/atc"
    import MedicamentGroupDto from "../../../../models/professionInfo/MedicamentGroupDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import PharmacyDto from "../../../../models/professionInfo/PharmacyDto";
    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import AtcDto from "../../../../models/professionInfo/AtcDto";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import ListDetailDto from "../../../../models/professionInfo/helpClass/listDetailDto";
    import ListDetail from "../../../Shared/listDetail.vue";
    import ListDetailButton from "../../../Shared/listDetailButton.vue";

    import PharmacyDiscardDetail from "../../../Shared/pharmacyDiscardDetail.vue"
    import PharmacyDiscardButtons from '../../../Shared/pharmacyDiscardButtons.vue'

    import ContraindicationDto from "../../../../models/professionInfo/bonds/MedicamentGroupDiseaseContraindicationDto";
    import IndicationDto from "../../../../models/professionInfo/bonds/MedicamentGroupDiseaseIndicationDto";
    import MedicamentGroupPharmacyDto from "../../../../models/professionInfo/bonds/MedicamentGroupPharmacyDto";
    import SideEffectsDto from "../../../../models/professionInfo/bonds/MedicamentGroupSymptomsSideEffectsDto";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"
    
    @Component({
        name: "MedicamentGroupDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, ListDetail, ListDetailButton,
            PharmacyDiscardDetail, PharmacyDiscardButtons, DescriptionHtmlOutput, SplitDialog, HtmlTextField
        },
    })
    export default class MedicamentGroupDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isMedicamentGroupLoaded: boolean = false;
        isModified: boolean = false;
        isAtcAddActive: boolean = false;
        isIndicationAddActive: boolean = false;
        isContraIndicationAdActive: boolean = false;
        isSymptomAddActive: boolean = false;

        medicamentGroup: MedicamentGroupDto = new MedicamentGroupDto();
        //diseases: PagedStickyList = new PagedStickyList("disease");
        indications: PagedStickyList = new PagedStickyList("disease");
        contraIndications: PagedStickyList = new PagedStickyList("disease");
        pharmacy: PagedStickyList = new PagedStickyList("pharmacy");
        symptoms: PagedStickyList = new PagedStickyList("symptom");
        //activeSubstances: any[] = [];
        atcs: PagedStickyList = new PagedStickyList("atc");

        symptom: SideEffectsDto = new SideEffectsDto;
        disease: IndicationDto = new IndicationDto;

        //bond detail variables
        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        isTaken: boolean = false;
        isDescriptionDetailActive: boolean = false;
        strength: boolean = false;

        listDetailDto: ListDetailDto[] = [];
        singleItem: ListDetailDto = new ListDetailDto;
        listType: number = -1;
        isListDetailActive: boolean = false;

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null

        isPharmacyDiscardDetailActive: boolean = false;

        focusedDetail = -1;

        requiredRules = [v => (v != null && v !="") || 'Povinné pole']

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
        medicamentGroupId: number;

        @Watch("medicamentGroup", { deep: true })
        onModified() {
            this.isModified = true;
        }

        @Prop({default: 0})
        levelsDeep: number 

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.indications = new PagedStickyList("disease", true);
            this.contraIndications = new PagedStickyList("disease", true);
            //this.pharmacy = new PagedStickyList("pharmacy");
            this.symptoms = new PagedStickyList("symptom", true);
            this.atcs = new PagedStickyList("atc", true);

            this.error = "";
            this.changeFocus(-1);

            if(this.$refs.form){
                this.$refs.form.resetValidation()
            }

            if (!this.isNew) {
                if (this.dialogShown) {
                    await this.loadMedicamentGroup();
                }
                this.isModified = false;
            }
            else {
                this.medicamentGroup = new MedicamentGroupDto;
                this.medicamentGroup.contraindication = [];
                this.medicamentGroup.indication = [];
                this.medicamentGroup.activeSubstance = [];
                this.medicamentGroup.sideEffects = [];
                this.medicamentGroup.atcs = [];
                this.medicamentGroup.pharmacies = [];

            }
            this.isMedicamentGroupLoaded = false;
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isListDetailActive = false;
            this.isPharmacyDiscardDetailActive = false;
        }

        beforeCreate(){
            this.$options.components.DiseaseDetailDialogDeep = require('@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue').default
            this.$options.components.SymptomDetailDialogDeep = require('@components/Manager/ProfessionInformation/Symptoms/symptom-detail-dialog-deep.vue').default
        }

        async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)

            if(this.focusedDetail == 0){
                this.showAddButton = false;                  
                FocusHelper.focusEditor(this.$refs.descriptionEditor.quill)
            }else if(this.focusedDetail == -1){
                this.showAddButton = false;
            }                         
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()                
            }               
        }

        //TODO abstract this out
        get diseaseString() {
            var disease = "";
            for (var key in this.medicamentGroup.indication) {
                if (!this.indications.selectedNative.find(x => x.id == this.medicamentGroup.indication[key].diseaseId))
                    continue;
                disease = disease + this.indications.selectedNative.find(x => x.id == this.medicamentGroup.indication[key].diseaseId).name
                disease = disease + " - " + this.medicamentGroup.indication[key].bondStrength.toString() + ", ";
            }

            return disease.substring(0, disease.length - 2);
        }

        get contraindicationsString() {
            var result = "";
            for (var key in this.medicamentGroup.contraindication) {
                if (!this.contraIndications.selectedNative.find(x => x.id == this.medicamentGroup.contraindication[key].diseaseId))
                    continue;
                result = result + this.contraIndications.selectedNative.find(x => x.id == this.medicamentGroup.contraindication[key].diseaseId).name + ", ";
            }
            return result.substring(0, result.length - 2);
        }

        get symptomsString() {
            var symptoms = "";
            for (var key in this.medicamentGroup.sideEffects) {
                if (!this.symptoms.selectedNative.find(x => x.id == this.medicamentGroup.sideEffects[key].symptomsId))
                    continue;
                symptoms = symptoms + this.symptoms.selectedNative.find(x => x.id == this.medicamentGroup.sideEffects[key].symptomsId).name
                symptoms = symptoms + " - " + this.medicamentGroup.sideEffects[key].bondStrength.toString() + ", ";
            }

            return symptoms.substring(0, symptoms.length - 2);
        }

        get atcsString() {
            var result = "";
            for (var key in this.medicamentGroup.atcs) {
                if (!this.atcs.selectedNative.find(x => x.id == this.medicamentGroup.atcs[key].atcId))
                    continue;
                result = result + this.atcs.selectedNative.find(x => x.id == this.medicamentGroup.atcs[key].atcId).name + ", ";
            }
            return result.substring(0, result.length - 2);
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number, addNewFunction, addButtonText: string) {
            this.isPharmacyDiscardDetailActive = false;
            this.isListDetailActive = false;
            this.isDescriptionDetailActive = false;

            if (!this.isDetailActive) {
                this.getBondDetailList(valueList, selectedList, bondType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                if (bondType === 0 || bondType === 2) {
                    this.strength = false;
                } else {
                    this.strength = true;
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
                    if (bondType === 0 || bondType === 2) {
                        this.strength = false;
                    } else {
                        this.strength = true;
                    }
                    this.bondType = bondType;
                }
                else {
                    this.isDetailActive = !this.isDetailActive;
                }
            }

            if(this.addButtonAction != null){
                if(this.levelsDeep <= 1){
                this.showAddButton = true;
            }

                this.addButtonText = addButtonText;
                this.addButtonAction = addNewFunction;
            }
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];
            for (var key in valueList.nativeList) {
                this.singleBondDetail.id = valueList.nativeList[key].id;
                this.singleBondDetail.name = valueList.nativeList[key].name;
                switch (bondType) {
                    case 0:
                        if (selectedList.length > 0 && selectedList.find(x => x.atcId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleItem.isSelected = false;
                            this.singleBondDetail.bondStr = 0;
                        }
                        break;
                    case 1: case 2: {
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
                    case 3: {
                        if (selectedList.length > 0 && selectedList.find(x => x.symptomsId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.symptomsId == valueList.nativeList[key].id).bondStrength;
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

        updatePharmacies(list: PharmacyDto[]) {
            this.medicamentGroup.pharmacies = list;
        }

        updateModelList(item: ListDetailDto, listType: number) {
            switch (listType) {
                case 0:
                    this.medicamentGroup.contraindication = this.medicamentGroup.contraindication.filter(x => x.diseaseId != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.contraindication.push({ diseaseId: item.id, medicamentGroupId: this.medicamentGroupId, disease: null })
                    }
                    break;
                case 1:
                    this.medicamentGroup.atcs = this.medicamentGroup.atcs.filter(x => x.atcId != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.atcs.push({ atcId: item.id, medicamentGroupId: this.medicamentGroupId, atc: null })
                    }
                    break;
            }
            let temp = Object.assign({}, this.medicamentGroup);
            this.medicamentGroup = temp;
        }

        updateBonds(item: BondDetailDto, bondType: number) {
            this.isTaken = false;
            switch (bondType) {
                case 0:
                    this.medicamentGroup.atcs = this.medicamentGroup.atcs.filter(x => x.atcId != item.id);
                    this.atcs.selected = this.atcs.selected.filter(x => x.id != item.id);
                    this.atcs.selectedNative = this.atcs.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.atcs.push({ atcId: item.id, medicamentGroupId: this.medicamentGroupId, atc: null });
                        this.atcs.selectedNative.push(this.atcs.nativeList.find(x => x.id == item.id));
                        this.atcs.selected.push(item);
                    }
                    //this.atcs.sort();
                    break;
                case 1: {
                    this.medicamentGroup.indication = this.medicamentGroup.indication.filter(x => x.diseaseId != item.id);
                    this.indications.selected = this.indications.selected.filter(x => x.id != item.id);
                    this.indications.selectedNative = this.indications.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.indication.push({ diseaseId: item.id, bondStrength: item.bondStr, medicamentGroupId: this.medicamentGroupId });
                        this.indications.selectedNative.push(this.indications.nativeList.find(x => x.id == item.id));
                        this.indications.selected.push(item);
                    }
                    //this.indications.sort();
                    break;
                }
                case 2: {
                    this.medicamentGroup.contraindication = this.medicamentGroup.contraindication.filter(x => x.diseaseId != item.id);
                    this.contraIndications.selected = this.contraIndications.selected.filter(x => x.id != item.id);
                    this.contraIndications.selectedNative = this.contraIndications.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.contraindication.push({ diseaseId: item.id, medicamentGroupId: this.medicamentGroupId, disease: null });
                        this.contraIndications.selectedNative.push(this.contraIndications.nativeList.find(x => x.id == item.id));
                        this.contraIndications.selected.push(item);
                    }
                    //this.contraIndications.sort();
                    break;
                }
                case 3: {
                    this.medicamentGroup.sideEffects = this.medicamentGroup.sideEffects.filter(x => x.symptomsId != item.id);
                    this.symptoms.selected = this.symptoms.selected.filter(x => x.id != item.id);
                    this.symptoms.selectedNative = this.symptoms.selectedNative.filter(x => x.id != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.sideEffects.push({ symptomsId: item.id, bondStrength: item.bondStr, medicamentGroupId: this.medicamentGroupId });
                        this.symptoms.selectedNative.push(this.symptoms.nativeList.find(x => x.id == item.id));
                        this.symptoms.selected.push(item);
                    }
                    //this.symptoms.sort();
                    break;
                }

                /*case 0: {
                    this.medicamentGroup.indication = this.medicamentGroup.indication.filter(x => x.diseaseId != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.indication.push({ diseaseId: item.id, bondStrength: item.bondStr, medicamentGroupId: this.medicamentGroupId });
                    }
                    break;
                }

                case 1:
                    this.medicamentGroup.activeSubstance = this.medicamentGroup.activeSubstance.filter(x => x.pharmacyId != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.activeSubstance.push({ pharmacyId: item.id, bondStrength: item.bondStr, medicamentGroupId: this.medicamentGroupId, pharmacy: null });
                    }
                    break;

                case 2: {
                    this.medicamentGroup.sideEffects = this.medicamentGroup.sideEffects.filter(x => x.symptomsId != item.id);
                    if (item.isSelected) {
                        this.medicamentGroup.sideEffects.push({ symptomsId: item.id, bondStrength: item.bondStr, medicamentGroupId: this.medicamentGroupId });
                    }
                    break;
                }*/
            }
            let temp = Object.assign({}, this.medicamentGroup);
            this.medicamentGroup = temp;
        }

        async updateSymptom(message: string, updatedSymptom){
            await this.symptoms.loadPage(updatedSymptom);
            var bondDetail = this.symptoms.displayList.find(x => x.id == updatedSymptom.id);
            this.updateBonds(bondDetail, 3)
            this.showBondDetail(this.symptoms, this.medicamentGroup.sideEffects,1, this.showSymptomAdd, 'Přidat příznak');
        }

        async updateIndications(message: string, updatedDisease){
            await this.indications.loadPage(updatedDisease);
            var bondDetail = this.indications.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 1)
            this.showBondDetail(this.indications, this.medicamentGroup.indication,0, this.showIndicationAdd, 'Přidat indikaci');  
        }

        async updateContracIndication(message: string, updatedDisease){
            await this.contraIndications.loadPage(updatedDisease);
            var bondDetail = this.contraIndications.displayList.find(x => x.id == updatedDisease.id);
            this.updateBonds(bondDetail, 2)
            this.showBondDetail(this.contraIndications, this.medicamentGroup.contraindication, 0, this.showContraIndicationAdd, 'Přidat kontraindikaci');  
        }

        addNew(){
            this.addButtonAction()
        }

        showAtcAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isAtcAddActive = true;
        }

        showIndicationAdd(){
            this.deepNew = true;
            this.deepId = -1;
            this.isIndicationAddActive = true;
        }

        showContraIndicationAdd(){
            this.deepNew = true;
            this.deepId = -1;
            this.isContraIndicationAdActive = true;
        }

        showSymptomAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isSymptomAddActive = true;
        }

        async loadMedicamentGroup() {
            this.loading = true;

            var result = await MedicamentGroupApi.getMedicamentGroupDetail(this.medicamentGroupId);

            if (result.success) {
                this.medicamentGroup = result.data;
                this.isMedicamentGroupLoaded = true;

                await Promise.all([
                    this.getIndications(),
                    this.getContraIndications(),
                    this.getSymptoms(),
                    this.getAtcs()
                ]);
            }

            this.loading = false;
            this.isModified = false;
        }

        //TODO refactor this (see disease page)
        async getIndications() {
            if (this.medicamentGroup.indication != null && this.medicamentGroup.indication.length > 0) {
                const indicationIds = this.medicamentGroup.indication.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(indicationIds);

                if (result.success) {
                    var indications = result.data;

                    var indicationBonds = this.medicamentGroup.indication;
                    for (var key in indicationBonds) {
                        var disease: DiseaseDto = indications.find(x => x.id == indicationBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.indications.selected.push({ bondStr: indicationBonds[key].bondStrength, id: indicationBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.indications.selectedNative = this.indications.selectedNative.concat(indications);
                }
            }
        }

        async getContraIndications() {
            if (this.medicamentGroup.contraindication != null && this.medicamentGroup.contraindication.length > 0) {
                const contraIndicationIds = this.medicamentGroup.contraindication.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(contraIndicationIds);

                if (result.success) {
                    var contraIndications = result.data;

                    var indicationBonds = this.medicamentGroup.contraindication;
                    for (var key in indicationBonds) {
                        var disease: DiseaseDto = contraIndications.find(x => x.id == indicationBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.contraIndications.selected.push({ bondStr: 0, id: indicationBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.contraIndications.selectedNative = this.contraIndications.selectedNative.concat(contraIndications);
                }
            }
        }

        async getSymptoms() {
            if (this.medicamentGroup.sideEffects != null && this.medicamentGroup.sideEffects.length > 0) {
                const sideEffectsIds = this.medicamentGroup.sideEffects.map(x => x.symptomsId);
                var result = await SymptomApi.getByIds(sideEffectsIds);

                if (result.success) {
                    var sideEffects = result.data;

                    var sideEffectBonds = this.medicamentGroup.sideEffects;
                    for (var key in sideEffectBonds) {
                        var disease: SymptomDto = sideEffects.find(x => x.id == sideEffectBonds[key].symptomsId);

                        if (disease != null && disease != undefined) {
                            this.symptoms.selected.push({ bondStr: sideEffectBonds[key].bondStrength, id: sideEffectBonds[key].symptomsId, isSelected: true, name: disease.name });
                        }
                    }

                    this.symptoms.selectedNative = this.symptoms.selectedNative.concat(sideEffects);
                }
            }
        }

        async getAtcs() {
            if (this.medicamentGroup.atcs != null && this.medicamentGroup.atcs.length > 0) {
                const atcIds = this.medicamentGroup.atcs.map(x => x.atcId);
                var result = await AtcApi.getByIds(atcIds);

                if (result.success) {
                    var atcs = result.data;

                    var atcBonds = this.medicamentGroup.atcs;
                    for (var key in atcBonds) {
                        var disease: AtcDto = atcs.find(x => x.id == atcBonds[key].atcId);

                        if (disease != null && disease != undefined) {
                            this.atcs.selected.push({ bondStr: 0, id: atcBonds[key].atcId, isSelected: true, name: disease.atcCode + ' - ' + disease.name });
                        }
                    }

                    this.atcs.selectedNative = this.atcs.selectedNative.concat(atcs.map(obj => { return { id: obj.id, name: obj.atcCode + ' - ' + obj.name } }));
                }
            }
        }

        async saveMedicamentGroup() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            console.log(this.isNew)

            if(!this.isNew) this.medicamentGroup.pharmacies = null;

            var result = this.isNew ? await MedicamentGroupApi.createMedicamentGroup(this.medicamentGroup)
                : await MedicamentGroupApi.updateMedicamentGroup(this.medicamentGroup);

            console.log(result)
            if (result.success) {
                this.medicamentGroup = result.data;
                this.isModified = false;
                this.$emit("updateMedicamentGroup", {message: "Záznam uložen.", color: "success"}, this.medicamentGroup);
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

        async deleteMedicamentGroup() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await MedicamentGroupApi.deleteMedicamentGroup(this.medicamentGroupId);
            if (result.success) {
                this.$emit("updateMedicamentGroup", {message: "Záznam odstraněn.", color: "success"});
            }
            this.saving = false;
            this.loading = false;
        }
    }
</script>