<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  @click:outside="clickOutside"
                  persistent
                  @keydown.escape="clickOutside"
                  @keydown.ctrl.s.prevent="saveByKey(true)">
            <v-card class="heightProvider">
                <strong class="red--text" v-html="error" v-if="error"></strong>
                <v-row>
                    <v-col>
                        <v-card-title primary-title v-if="isNew">
                            <v-row>
                                <v-col cols="1">
                                    <v-btn fab x-small @click="hideDetailDialog">
                                        &times;
                                    </v-btn>
                                </v-col>
                                <v-col>
                                    Přidat Lékovou skupinu
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-title primary-title v-if="!isNew">
                            <v-row>
                                <v-col cols="1">
                                    <v-btn fab x-small @click="hideDetailDialog">
                                        &times;
                                    </v-btn>
                                </v-col>
                                <v-col>
                                    Upravit Lékovou skupinu
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicamentGroup.name"
                                                  color="error"
                                                  label="Název" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <!-- DESCRIPTION -->
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="medicamentGroup.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive"
                                                                v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>

                            <!-- ATC -->
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="atcsString"
                                                  color="error"
                                                  label="ATC"
                                                  @focus="showBondDetail(atcs, medicamentGroup.atcs, 0)" />
                                </v-col>
                                <v-col cols="12" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive"
                                                         :bondType="0"
                                                         :currentBondType="bondType"
                                                         :valueList="atcs"
                                                         :selectedList="medicamentGroup.atcs"
                                                         v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>

                            <!-- PHARMACY DISCARDS -->
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field label="Farmaka"
                                                  readonly
                                                  v-model="pharmaciesString"
                                                  @focus="showPharmacyDiscardDetail" />
                                </v-col>
                                <v-col cols="12" md="1">
                                    <pharmacyDiscardButtons :medicamentGroupId="medicamentGroupId"
                                                            :isDetailActive="isPharmacyDiscardDetailActive"
                                                            v-on:showPharmacyDiscardDetail="showPharmacyDiscardDetail()" />
                                </v-col>
                            </v-row>

                            <!-- INDICATION -->
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="diseaseString"
                                                  color="error"
                                                  label="Indikace"
                                                  @focus="showBondDetail(indications,medicamentGroup.indication,1)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive"
                                                         :bondType="1"
                                                         :currentBondType="bondType"
                                                         :valueList="indications"
                                                         :selectedList="medicamentGroup.indication"
                                                         v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="contraindicationsString"
                                                  color="error"
                                                  label="Kontraindikace"
                                                  @focus="showBondDetail(contraIndications, medicamentGroup.contraindication, 2)" />
                                </v-col>
                                <v-col cols="12" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive"
                                                         :bondType="2"
                                                         :currentBondType="bondType"
                                                         :valueList="contraIndications"
                                                         :selectedList="medicamentGroup.contraindication"
                                                         v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="symptomsString"
                                                  color="error"
                                                  label="Nežádoucí účinky"
                                                  @focus="showBondDetail(symptoms,medicamentGroup.sideEffects,3)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive"
                                                         :bondType="3"
                                                         :currentBondType="bondType"
                                                         :valueList="symptoms"
                                                         :selectedList="medicamentGroup.sideEffects"
                                                         v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>

                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive && !isListDetailActive && !isPharmacyDiscardDetailActive" :max-width="600">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds" :strength="strength"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive && !isListDetailActive && !isPharmacyDiscardDetailActive" :max-width="600">
                        <description-detail v-model="medicamentGroup.description" v-on:changed="onDescriptionChanged"/>
                    </v-col>
                    <v-col v-if="!isDetailActive && !isDescriptionDetailActive && isListDetailActive && !isPharmacyDiscardDetailActive" :max-width="600">
                        <list-detail :items="valueList" :listType="listType" v-on:updateModelList="updateModelList"></list-detail>
                    </v-col>
                    <v-col v-show="isPharmacyDiscardDetailActive && !isDetailActive && !isDescriptionDetailActive && !isListDetailActive" :max-width="600">
                        <pharmacy-discard-detail :medicamentGroupId="medicamentGroupId" :isShown="isPharmacyDiscardDetailActive" @updatePharmacies="updatePharmacies" />
                    </v-col>

                </v-row>


                <v-divider></v-divider>

                <v-card-actions>
                    <v-btn class="error"
                           text
                           @click="hideDetailDialog">
                        Zavřít
                    </v-btn>
                    <v-spacer></v-spacer>
                    <v-btn v-if="isNew"
                           text
                           :disabled="loading || !modified"
                           @click="createMedicamentGroup(false)">
                        Založit
                    </v-btn>
                    <v-btn v-if="isNew"
                           text
                           :disabled="loading || !modified"
                           @click="createMedicamentGroup(true)">
                        Založit a zavřít
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateMedicamentGroup(false)">
                        Upravit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateMedicamentGroup(true)">
                        Upravit a zavřít
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <template>
            <v-dialog v-model="showDialogSave" width="300">
                <v-card>
                    <v-card-title primary-title>
                        Máte neuložené změny
                    </v-card-title>

                    <v-divider></v-divider>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn class="error"
                               text
                               @click="discardChanges">
                            Zahodit
                        </v-btn>
                        <v-btn class="error"
                               text
                               @click="saveChanges(true)">
                            Uložit a zavřít
                        </v-btn>
                        <v-spacer></v-spacer>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </template>
    </div>
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
import { data } from "jquery";

    @Component({
        name: "MedicamentGroupDetailDialog",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, ListDetail, ListDetailButton,
            PharmacyDiscardDetail, PharmacyDiscardButtons, DescriptionHtmlOutput
        },
    })
    export default class MedicamentGroupDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isMedicamentGroupLoaded: boolean = false;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        showDialog: boolean;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: 0 })
        medicamentGroupId: number = -1;

        @Prop({ default: "" })
        linkedDisease: number = -1;

        @Prop({ default: null })
        refresh: Function;

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
        detailWidth: number = 600;
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

        isPharmacyDiscardDetailActive: boolean = false;

        onDescriptionChanged() {
            this.modified = true;
        }

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

        /*get activeSubstancesString() {
            var activeSubstance = "";
            for (var key in this.medicamentGroup.activeSubstance) {
                if (!this.pharmacy.selectedNative.find(x => x.id == this.medicamentGroup.activeSubstance[key].pharmacyId))
                    continue;
                activeSubstance = activeSubstance + this.pharmacy.selectedNative.find(x => x.id == this.medicamentGroup.activeSubstance[key].pharmacyId).activeSubstanceName;
                activeSubstance = activeSubstance + " - " + this.medicamentGroup.activeSubstance[key].bondStrength.toString() + ", ";
            }
            return activeSubstance;
        }*/

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

        get pharmaciesString() {
            var pharmacies = "";
            for (var key in this.medicamentGroup.pharmacies) {
                pharmacies += (this.medicamentGroup.pharmacies[key].nameReg + ", ");
            }
            return pharmacies.substring(0, pharmacies.length - 2);
        }


        /*get contraIndicationCombo() {
            return this.diseases.map(x => new ContraindicationDto(this.medicamentGroupId, x))
        }

        get pharmacyCombo() {
            return this.pharmacy.map(x => new MedicamentGroupPharmacyDto(this.medicamentGroupId, 10, x));
        }*/
        
        showPharmacyDiscardDetail() {
            this.isListDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isDetailActive = false;

            this.isPharmacyDiscardDetailActive = !this.isPharmacyDiscardDetailActive;
            if (this.isPharmacyDiscardDetailActive) {
                this.detailWidth = 1300;
            }
            else {
                this.detailWidth = 600;
            }

        }

        /*showListDetail(valueList: object[], selectedList: object[], listType: number) {
            this.isDetailActive = false;
            this.isPharmacyDiscardDetailActive = false;
            this.isDescriptionDetailActive = false;

            if (!this.isListDetailActive) {
                this.getListDetailList(valueList, selectedList, listType);
                this.valueList = this.listDetailDto;
                this.listType = listType;
                this.detailWidth = 1300;
                this.isListDetailActive = !this.isListDetailActive;
            }
            else {
                if (listType != this.listType) {
                    this.getListDetailList(valueList, selectedList, listType);
                    this.valueList = this.listDetailDto;
                    this.listType = listType;
                }
                else {
                    this.detailWidth = 600;
                    this.isListDetailActive = !this.isListDetailActive;
                }
            }
        }*/

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number) {
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
                this.detailWidth = 1300;
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
                    this.detailWidth = 600;
                    this.isDetailActive = !this.isDetailActive;
                }
            }
        }

        showDescriptionDetail() {
            this.isDetailActive = false;
            this.isPharmacyDiscardDetailActive = false;
            this.isListDetailActive = false;

            if (!this.isDescriptionDetailActive) {
                this.detailWidth = 1300;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
            else {
                this.detailWidth = 600;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
        }

        /*getListDetailList(valueList: object[], selectedList: object[], listType: number) {
            this.listDetailDto = [];
            for (var key in valueList) {
                this.singleItem.id = valueList[key].id;
                this.singleItem.name = valueList[key].name;
                switch (listType) {
                    case 0:
                        if (selectedList.length > 0 && selectedList.find(x => x.diseaseId == valueList[key].id) != null) {
                            this.singleItem.isSelected = true;
                        }
                        else {
                            this.singleItem.isSelected = false;
                        }
                        break;
                    case 1:
                        if (selectedList.length > 0 && selectedList.find(x => x.atcId == valueList[key].id) != null) {
                            this.singleItem.isSelected = true;
                        }
                        else {
                            this.singleItem.isSelected = false;
                        }
                        break;
                }
                this.listDetailDto.push(this.singleItem);
                this.singleItem = new ListDetailDto;
            }
        }*/

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
                    case 1: case 2:{
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
                    /*case 1: {
                        if (selectedList.length > 0 && selectedList.find(x => x.pharmacyId == valueList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.pharmacyId == valueList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                    case 2: {
                        if (selectedList.length > 0 && selectedList.find(x => x.symptomsId == valueList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.symptomsId == valueList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }*/

                }
                this.bondDetailDto.push(this.singleBondDetail);

                this.singleBondDetail = new BondDetailDto;
                this.modified = true;
            }

        }

        updatePharmacies(list: PharmacyDto[]) {
            this.medicamentGroup.pharmacies = list;
            this.modified = true;
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
            this.modified = true;
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
            this.modified = true;
        }

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.indications = new PagedStickyList("disease", true);
            this.contraIndications = new PagedStickyList("disease", true);
            //this.pharmacy = new PagedStickyList("pharmacy");
            this.symptoms = new PagedStickyList("symptom", true);
            this.atcs = new PagedStickyList("atc", true);

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadMedicamentGroup();
                }
            }
            else {
                this.medicamentGroup = new MedicamentGroupDto;
                this.medicamentGroup.contraindication = [];
                this.medicamentGroup.indication = [];
                this.medicamentGroup.activeSubstance = [];
                this.medicamentGroup.sideEffects = [];
                this.medicamentGroup.atcs = [];
                this.medicamentGroup.pharmacies = [];
                this.fillLinkedItems();
            }
            this.isMedicamentGroupLoaded = false;
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isListDetailActive = false;
            this.detailWidth = 600;
            this.modified = false;
            this.showDialogSave = false;
            this.isPharmacyDiscardDetailActive = false;
        }

        fillLinkedItems() {
            if (this.linkedDisease != -1) {
                this.medicamentGroup.indication.push({ bondStrength: 0, diseaseId: this.linkedDisease, medicamentGroupId: this.medicamentGroupId });
                this.medicamentGroup.contraindication.push({ disease: null, diseaseId: this.linkedDisease, medicamentGroupId: this.medicamentGroupId });
                this.getIndications();
                this.getContraIndications();
            }
        }

        async loadMedicamentGroup() {
            this.loading = true;
            this.error = "";

            var result = await MedicamentGroupApi.getMedicamentGroupDetail(this.medicamentGroupId);

            if (result.success) {
                this.medicamentGroup = result.data;
                this.isMedicamentGroupLoaded = true;
                console.warn(result.data);

                //this.getPharmacy();
                this.getIndications();
                this.getContraIndications();
                this.getSymptoms();
                this.getAtcs();
            }

            this.loading = false;

        }

       /* async getPharmacy() {
            if (this.medicamentGroup.pharmacies != null && this.medicamentGroup.pharmacies.length > 0) {
                const pharmacyIds = this.medicamentGroup.pharmacies.map(x => x.id);
                var result = await PharmacyApi.getByIds(pharmacyIds);

                if (result.success) {
                    var parmacies = result.data;

                    var pharmacyBonds = this.medicamentGroup.pharmacies;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }*/

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

        async saveByKey(close: boolean) {
            if (this.isNew)
                await this.createMedicamentGroup(close);
            else
                await this.updateMedicamentGroup(close);
        }

        async createMedicamentGroup(close: boolean) {
            this.loading = true;
            this.error = "";
            this.saving = true;

            // create
            var result = await MedicamentGroupApi.createMedicamentGroup(this.medicamentGroup);
            if (result.success) {
                this.medicamentGroup = result.data;
                this.modified = false;
                this.$emit("createMedicamentGroup");
                
                if (close) {
                    this.hideDetailDialog();
                } else {
                    this.refresh(result.data.id);
                }
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

        async updateMedicamentGroup(close: boolean) {
            this.loading = true;
            this.error = "";
            this.saving = true;

            this.medicamentGroup.pharmacies = null;

            var result = await MedicamentGroupApi.updateMedicamentGroup(this.medicamentGroup);
            if (result.success) {
                this.medicamentGroup = result.data;
                this.modified = false;
                this.$emit("updateMedicamentGroup");

                if (close) {
                    this.hideDetailDialog();
                } else {
                    console.log(result.data);
                    this.refresh(this.medicamentGroupId);
                }
            }
            else {
                for (var key in result.errors) {
                    var errors = result.errors[key];
                    for (var index in errors) {
                        this.error += `${errors[index]} `;
                        this.error += "<br>";
                    }
                }

                console.warn(this.error);
            }

            this.saving = false;
            this.loading = false;
        }

        clickOutside() {
            //this.isPharmacyDiscardDetailActive = false;
            //this.hideDetailDialog();
        }

        hideDetailDialog() {
            if (this.modified) {
                this.showDialogSave = true;
            } else {
                this.$emit("hideDetailDialog");
            }
        }

        saveChanges(close: boolean) {
            this.showDialogSave = false;
            this.saveByKey(close);
        }

        discardChanges() {
            this.modified = false;
            this.showDialogSave = false;
            this.hideDetailDialog();
        }
    }
</script>
<style scoped>
    .dialog-footer {
        font-size: 13px;
        color: rgb(150,150,150);
    }
</style>
