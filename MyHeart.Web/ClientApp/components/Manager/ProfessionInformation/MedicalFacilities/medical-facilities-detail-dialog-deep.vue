<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="saveMedicalFacility"
                  @delete="deleteMedicalFacility"
                  :title="isNew ? 'Přidat zařízení' : 'Upravit zařízení'"
                  :focusedDetail="focusedDetail"
                  @focus="focusedDetail=0"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton">
        <div v-html="error" />
        <v-form ref="form" v-model="valid" lazy-validation>
            <v-text-field label="Název" ref="nameInput" v-model="medicalFacilities.name" @focus="changeFocus(0)" :rules="requiredRules"/>
            <v-row>
                <v-col cols="4"><v-text-field label="Id zařízení" v-model="medicalFacilities.facilityId" @focus="changeFocus(0)" readonly /></v-col>
                <v-col cols="4"><v-text-field label="Kód zařízení" v-model="medicalFacilities.facilityCode" @focus="changeFocus(0)" readonly /></v-col>
                <v-col cols="4"><v-text-field label="Kód typu zařízení" v-model="medicalFacilities.facilityTypeCode" @focus="changeFocus(0)" readonly /></v-col>
            </v-row>
            <v-text-field label="Charakter" v-model="medicalFacilities.character" @focus="changeFocus(0)" />
            <v-text-field label="Druhotný charakter" v-model="medicalFacilities.characterSecondary" @focus="changeFocus(0)" />
            <v-text-field label="Druhotný charakter" v-model="medicalFacilities.characterSecondary" @focus="changeFocus(0)" />
            <v-row>
                <v-col cols="4"><v-text-field label="IČO" v-model="medicalFacilities.ico" @focus="changeFocus(0)"/></v-col>
                <v-col cols="4"><v-text-field label="PCZ" v-model="medicalFacilities.pcz" @focus="changeFocus(0)"/></v-col>
                <v-col cols="4"><v-text-field label="PCDP" v-model="medicalFacilities.pcdp" @focus="changeFocus(0)"/></v-col>
            </v-row>
            <v-row>
                <v-col cols="6"><v-text-field label="Kraj" v-model="medicalFacilities.region" @focus="changeFocus(0)" /></v-col>
                <v-col cols="6"><v-text-field label="Kód kraje" v-model="medicalFacilities.regionCode" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="6"><v-text-field label="Okres" v-model="medicalFacilities.district" @focus="changeFocus(0)" /></v-col>
                <v-col cols="6"><v-text-field label="Kód okresu" v-model="medicalFacilities.districtCode" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="6"><v-text-field label="Správní obvod" v-model="medicalFacilities.administrativeDistrict" @focus="changeFocus(0)" /></v-col>
                <v-col cols="6"><v-text-field label="Obec" v-model="medicalFacilities.municipality" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="4"><v-text-field label="Ulice" v-model="medicalFacilities.street" @focus="changeFocus(0)" /></v-col>
                <v-col cols="4"><v-text-field label="Číslo popisné" v-model="medicalFacilities.buildingNumber" @focus="changeFocus(0)" /></v-col>
                <v-col cols="4"><v-text-field label="PSČ" v-model="medicalFacilities.zip" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="6"><v-text-field label="Telefon" v-model="medicalFacilities.telephone" @focus="changeFocus(0)" /></v-col>
                <v-col cols="6"><v-text-field label="Email" v-model="medicalFacilities.email" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="6"><v-text-field label="Webová stránka" v-model="medicalFacilities.website" @focus="changeFocus(0)" /></v-col>
                <v-col cols="6"><v-text-field label="Fax" v-model="medicalFacilities.fax" @focus="changeFocus(0)" /></v-col>
            </v-row>
            <v-text-field label="Typ poskytovatele" v-model="medicalFacilities.providerType" @focus="changeFocus(0)" />
            <v-text-field label="Jméno poskytovatele" v-model="medicalFacilities.providerName" @focus="changeFocus(0)" />
            <v-text-field label="Obor péče" v-model="medicalFacilities.careField" @focus="changeFocus(0)" />
            <v-text-field label="Forma péče" v-model="medicalFacilities.careForm" @focus="changeFocus(0)" />
            <v-text-field label="Typ péče" v-model="medicalFacilities.careType" @focus="changeFocus(0)" />
            <v-text-field label="Zástupci" v-model="medicalFacilities.representative" @focus="changeFocus(0)" />
            <v-text-field label="Preventivní opatření" :value="preventiveMeasuresString" @focus="showBondDetail(preventiveMeasures,medicalFacilities.preventiveMeasures,0, showPreventiveAdd, 'Přidat opatření');changeFocus(1)" readonly />
            <v-text-field label="Nefarmakologické léčby" :value="nonpharmaString" @focus="showBondDetail(nonpharmacy,medicalFacilities.nonpharmaticTherapy,1, showNonpharmacyAdd, 'Přidat léčbu');changeFocus(1)" readonly />
        </v-form>
        
        <template #rightPanel>
            <v-tab-item><map-container ref="facilityMap" :gps="medicalFacilities.gps" v-show="!!medicalFacilities.gps"/></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" /></v-tab-item>
        </template>

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

    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import MedicalFacilitiesApi from "@backend/api/medicalFacilitie";
    import NonpharmacyApi from "@backend/api/nonpharmacy";
    import PreventiveMeasuresApi from "@backend/api/preventiveMeasures";
    import MedicalFacilitiesDto from "../../../../models/professionInfo/MedicalFacilitiesDto";
    import NonpharmacyDto from "@models/ProfessionInformation/NonpharmacyDto";
    import PreventiveMeasuresDto from "@models/ProfessionInformation/PreventiveMeasuresDto";

    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";
    import MapContainer from "../../../Shared/MapContainer";


    import SplitDialog from "@components/Shared/splitDialog.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import FocusHelper from "@utils/FocusHelper"

    @Component({
        name: "MedicalFacilitiesDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, MapContainer, SplitDialog, HtmlTextField
        },
    })
    export default class MedicalFacilitiesDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isMedicalFacilitiesLoaded: boolean = false;
        isDetailActive: boolean = false;
        hasCoords: boolean = false;
        isMapLoaded: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        isModified: boolean = false;
        isNonpharmaAddActive: boolean = false;
        isPreventiveAddActive: boolean = false;

        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isTaken: boolean = false;

        medicalFacilities: MedicalFacilitiesDto = new MedicalFacilitiesDto();
        nonpharmacy: PagedStickyList = new PagedStickyList("nonpharmacy", true);
        preventiveMeasures: PagedStickyList = new PagedStickyList("preventive", true);

        focusedDetail: number = 0;
        valid: boolean = true
        requiredRules = [v => (v != null && v !="") || 'Povinné pole']
 
        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null

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
        medicalFacilitiesId: number;

        @Prop({default: 0})
        levelsDeep: number 

        @Watch("medicalFacilities", { deep: true })
        onModified() {
            this.isModified = true;
        }

         async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)
                    
            if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()               
            }else if(this.focusedDetail == 0 || this.focusedDetail == -1){
                this.showAddButton = false
            }
        }
        @Watch("dialogShown")
        async dialogShownChanged() {

            this.isMedicalFacilitiesLoaded = false;

            this.nonpharmacy = new PagedStickyList("nonpharmacy", true);
            this.preventiveMeasures = new PagedStickyList("preventive", true);
            this.isMapLoaded = false;

            this.error = "";
            this.changeFocus(-1);
       
            
            if(this.$refs.form){
                this.$refs.form.resetValidation()
            }

            if (!this.isNew) {
                if (this.dialogShown) {
                    await FocusHelper.delay(100)
                    this.$refs.nameInput.focus()
                    await this.loadMedicalFacilities();
                    await this.$refs.facilityMap.getMap();
                }

            }
            else {
                this.medicalFacilities = new MedicalFacilitiesDto;
                this.medicalFacilities.nonpharmaticTherapy = [];
                this.medicalFacilities.preventiveMeasures = [];
                this.isMedicalFacilitiesLoaded = true;
            }
            this.isDetailActive = false;
            this.isModified = false;
        }

        //TODO refactor
        get nonpharmaString() {
            var nonpharma = "";
            for (var key in this.medicalFacilities.nonpharmaticTherapy) {
                if (!this.nonpharmacy.selectedNative.find(x => x.id == this.medicalFacilities.nonpharmaticTherapy[key].nonpharmaticTherapyId))
                    continue;
                nonpharma = nonpharma + this.nonpharmacy.selectedNative.find(x => x.id == this.medicalFacilities.nonpharmaticTherapy[key].nonpharmaticTherapyId).name
                nonpharma = nonpharma + " - " + this.medicalFacilities.nonpharmaticTherapy[key].bondStrength.toString() + ", ";
            }

            return nonpharma.substring(0, nonpharma.length - 2);
        }

        get preventiveMeasuresString() {
            var preventives = "";
            for (var key in this.medicalFacilities.preventiveMeasures) {
                if (!this.preventiveMeasures.selectedNative.find(x => x.id == this.medicalFacilities.preventiveMeasures[key].preventiveMeasureId))
                    continue;
                preventives = preventives + this.preventiveMeasures.selectedNative.find(x => x.id == this.medicalFacilities.preventiveMeasures[key].preventiveMeasureId).name
                preventives = preventives + " - " + this.medicalFacilities.preventiveMeasures[key].bondStrength.toString() + ", ";
            }

            return preventives.substring(0, preventives.length - 2);
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

        beforeCreate(){
            this.$options.components.NonpharmacyDetailDialogDeep = require('@components/Manager/ProfessionInformation/NonpharmaticTherapy/nonpharmatic-detail-dialog-deep').default
            this.$options.components.PreventiveDetailDialogDeep = require('@components/Manager/ProfessionInformation/PreventiveMeasures/preventive-detail-dialog-deep').default
        }

        addNew(){
            this.addButtonAction()
        }

        async updatePreventiveMeasure(message: string, updatedMeasure){                       
            await this.preventiveMeasures.loadPage(updatedMeasure);
            var bondDetail = this.preventiveMeasures.displayList.find(x => x.id == updatedMeasure.id);
            this.updateBonds(bondDetail, 0)
            this.showBondDetail(this.preventiveMeasures,this.medicalFacilities.preventiveMeasures,4, this.showPreventiveAdd, 'Přidat opatření')
        }

        async updateNonpharmacy(message: string, updatedNonpharmacy){            
            await this.nonpharmacy.loadPage(updatedNonpharmacy)
            var bondDetail = this.nonpharmacy.displayList.find(x => x.id == updatedNonpharmacy.id);
            this.updateBonds(bondDetail, 1)
            this.showBondDetail(this.nonpharmacy, this.medicalFacilities.nonpharmaticTherapy,3, this.showNonpharmacyAdd, 'Přidat léčbu');
        }

        showPreventiveAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isPreventiveAddActive = true;
            console.log(this.isPreventiveAddActive)
        }

        showNonpharmacyAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isNonpharmaAddActive = true;
            console.log(this.isNonpharmaAddActive)
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];
            let idField = ["preventiveMeasureId", "nonpharmaticTherapyId"][bondType];
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
            console.log(this.bondDetailDto);
        }


        // TODO refactor
        async getPreventiveMeasures() {
            if (this.medicalFacilities.preventiveMeasures != null && this.medicalFacilities.preventiveMeasures.length > 0) {
                var result = await PreventiveMeasuresApi.getByIds(this.medicalFacilities.preventiveMeasures.map(x => x.preventiveMeasureId));

                if (result.success) {
                    var measures = result.data;

                    var preventiveBonds = this.medicalFacilities.preventiveMeasures;
                    for (var key in preventiveBonds) {
                        var measure: PreventiveMeasuresDto = measures.find(x => x.id == preventiveBonds[key].preventiveMeasureId);

                        if (measure != null && measure != undefined) {
                            this.preventiveMeasures.selected.push({ bondStr: preventiveBonds[key].bondStrength, id: preventiveBonds[key].preventiveMeasureId, isSelected: true, name: measure.name });
                        }
                    }

                    this.preventiveMeasures.selectedNative = this.preventiveMeasures.selectedNative.concat(measures);
                }
            }
        }

        async getNonpharmacy() {
            if (this.medicalFacilities.nonpharmaticTherapy != null && this.medicalFacilities.nonpharmaticTherapy.length > 0) {
                var result = await NonpharmacyApi.getByIds(this.medicalFacilities.nonpharmaticTherapy.map(x => x.nonpharmaticTherapyId));

                if (result.success) {
                    var nonPharmacies = result.data;

                    var nonPharmacyBonds = this.medicalFacilities.nonpharmaticTherapy;
                    for (var key in nonPharmacyBonds) {
                        var nonPharmacy: NonpharmacyDto = nonPharmacies.find(x => x.id == nonPharmacyBonds[key].nonpharmaticTherapyId);

                        if (nonPharmacy != null && nonPharmacy != undefined) {
                            this.nonpharmacy.selected.push({ bondStr: nonPharmacyBonds[key].bondStrength, id: nonPharmacyBonds[key].nonpharmaticTherapyId, isSelected: true, name: nonPharmacy.name });
                        }
                    }

                    this.nonpharmacy.selectedNative = this.nonpharmacy.selectedNative.concat(nonPharmacies);
                }
            }
        }

        async loadMedicalFacilities() {
            this.loading = true;

            var result = await MedicalFacilitiesApi.getMedicalFacilitiesDetail(this.medicalFacilitiesId);

            if (result.success) {
                this.medicalFacilities = result.data;
                this.isMedicalFacilitiesLoaded = true;
                
                await Promise.all([
                    this.getNonpharmacy(),
                    this.getPreventiveMeasures()
                ])
            }

            this.loading = false;
        }

        //TODO refactor
        updateBonds(item: BondDetailDto, bondType: number) {
            switch (bondType) {
                case 0:
                    this.medicalFacilities.preventiveMeasures = this.medicalFacilities.preventiveMeasures.filter(x => x.preventiveMeasureId != item.id);
                    this.preventiveMeasures.selected = this.preventiveMeasures.selected.filter(x => x.id != item.id);
                    this.preventiveMeasures.selectedNative = this.preventiveMeasures.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.medicalFacilities.preventiveMeasures.push({ preventiveMeasureId: item.id, bondStrength: item.bondStr, medicalFacilitiesId: this.medicalFacilitiesId, preventiveMeasures: null });
                        this.preventiveMeasures.selectedNative.push(this.preventiveMeasures.nativeList.find(x => x.id === item.id));
                        this.preventiveMeasures.selected.push(item);
                    }
                    //this.preventiveMeasures.sort();
                    break;
                case 1:
                    this.medicalFacilities.nonpharmaticTherapy = this.medicalFacilities.nonpharmaticTherapy.filter(x => x.nonpharmaticTherapyId != item.id);
                    this.nonpharmacy.selected = this.nonpharmacy.selected.filter(x => x.id != item.id);
                    this.nonpharmacy.selectedNative = this.nonpharmacy.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.medicalFacilities.nonpharmaticTherapy.push({ nonpharmaticTherapyId: item.id, bondStrength: item.bondStr, medicalFacilitiesId: this.medicalFacilitiesId, nonpharmacy: null });
                        this.nonpharmacy.selectedNative.push(this.nonpharmacy.nativeList.find(x => x.id === item.id));
                        this.nonpharmacy.selected.push(item);
                    }
                    //this.preventiveMeasures.sort();
                    break;
            }


            this.medicalFacilities = Object.assign({}, this.medicalFacilities);
        }

        async saveMedicalFacility() {
            if(!this.$refs.form.validate()){
                return
            }

            this.loading = true;
            this.error = "";
            this.saving = true;
            // create

            console.log(this.isNew)

            var result = this.isNew ? await MedicalFacilitiesApi.createMedicalFacilities(this.medicalFacilities)
                : await MedicalFacilitiesApi.updateMedicalFacilities(this.medicalFacilities);
            if (result.success) {
                this.medicalFacilities = result.data;
                this.isModified = false;
                this.$emit("updateMedicalFacility", "Záznam uložen.", this.medicalFacilities);
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

        async deleteMedicalFacility() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await MedicalFacilitiesApi.deleteDisease(this.medicalFacilitiesId);
            if (result.success) {
                this.$emit("updateMedicalFacility", "Záznam odstraněn.");
            }
            this.saving = false;
            this.loading = false;
        }
    }
</script>