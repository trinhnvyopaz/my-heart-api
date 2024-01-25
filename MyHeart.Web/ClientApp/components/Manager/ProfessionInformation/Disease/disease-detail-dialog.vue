<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  persistent
                  @keydown.ctrl.s.prevent="saveByKey">
            <v-card class="heightProvider">
                <strong class="red--text" v-html="error" v-if="error"></strong>
                <v-row>
                    <v-col>
                        <v-card-title primary-title v-if="isNew">
                            <v-row>
                                <v-col cols="1">
                                    <v-btn fab @click="hideDetailDialog">
                                        &times;
                                    </v-btn>
                                </v-col>
                                <v-col>
                                    Přidat Onemocnění
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
                                    Upravit Onemocnění
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text v-if="isDiseaseLoaded">
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="disease.name"
                                                  color="error"
                                                  label="Název"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                            </v-row>      
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="disease.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="disease.frequency"
                                                  color="error"
                                                  label="Prevalence na 100.000 obyvatel"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="causesString"
                                                  color="error"
                                                  label="Příčiny"
                                                  @focus="showBondDetail(diseaseList,disease.causes,0)"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="diseaseList" :selectedList="disease.causes" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="symptomsString"
                                                  color="error"
                                                  label="Příznaky"
                                                  @focus="showBondDetail(symptoms,disease.symptoms,1)"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="1" :currentBondType="bondType" :valueList="symptoms" :selectedList="disease.symptoms" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-card-title>
                                Léčba
                            </v-card-title>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="medicamentGroupsString"
                                                  color="error"
                                                  label="Lékové skupiny"
                                                  @focus="showBondDetail(medicamentGroups,disease.medicamentGroup,2)"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="2" :currentBondType="bondType" :valueList="medicamentGroups" :selectedList="disease.medicamentGroup" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="nonpharmaticTherapysString"
                                                  color="error"
                                                  label="Nefarmakologické léčby"
                                                  @focus="showBondDetail(nonpharmacy,disease.nonpharmaticTherapy,3)"
                                                  dense
                                                  @change="onDescriptionChanged" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="3" :currentBondType="bondType" :valueList="nonpharmacy" :selectedList="disease.nonpharmaticTherapy" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="preventiveMeasuresString"
                                                  color="error"
                                                  label="Preventivní opatření"
                                                  @focus="showBondDetail(preventiveMeasures,disease.preventiveMeasures,4)"
                                                  dense />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="4" :currentBondType="bondType" :valueList="preventiveMeasures" :selectedList="disease.preventiveMeasures" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>

                            <v-card-title>
                                Cílové hodnoty
                            </v-card-title>

                            <v-row cols="12">
                                <v-col>
                                    <v-text-field label="Hmotnost" color="error" v-model="disease.targetWeight" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" step="0.1" />
                                </v-col>
                            </v-row>

                            <v-row cols="12">
                                <v-col>
                                    <v-text-field label="Systolický tlak" color="error" v-model="disease.targetSystolicPressure" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" />
                                </v-col>
                                <v-col>
                                    <v-text-field label="Diastolický tlak" color="error" v-model="disease.targetDiastolicPressure" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" />
                                </v-col>
                            </v-row>

                            <v-row cols="12">
                                <v-col>
                                    <v-text-field label="Tepová frekvence" color="error" v-model="disease.targetHeartRate" type="number" @wheel="$event.target.blur()" @keydown="handleKeys($event)" />
                                </v-col>
                            </v-row>

                            <v-row cols="12">
                                <v-col>
                                    <v-text-field label="Hladina sérového LDL" color="error" v-model="disease.targetLdl" type="number" step="0.01" @wheel="$event.target.blur()" @keydown="handleKeys($event)" />
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail(1)">
                                    <description-html-output :description="disease.systemicMeasures" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail(1)" />
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive">
                        <description-detail v-model="disease.description" v-on:changed="onDescriptionChanged"/>
                    </v-col>
                </v-row>


                <v-divider></v-divider>

                <v-card-actions>
                    <v-btn text
                           @click="hideDetailDialog">
                        Zavřít
                    </v-btn>
                    <v-spacer></v-spacer>
                    <v-btn v-if="isNew"
                           text
                           @click="createDisease"
                           :disabled="loading || !modified">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           text
                           :disabled="loading || !modified"
                           @click="updateDisease">
                        Upravit
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
                        <v-btn text
                               @click="discardChanges">
                            Zahodit
                        </v-btn>
                        <v-btn text
                               @click="saveChanges">
                            Uložit
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

    @Component({
        name: "DiseaseDetailDialog",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, DescriptionHtmlOutput
        },
    })
    export default class DiseaseDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        medicamentGroups: PagedStickyList = new PagedStickyList("medicamentgroup");
        diseaseList: PagedStickyList = new PagedStickyList("disease");
        disease: DiseaseDto = new DiseaseDto;
        symptoms: PagedStickyList = new PagedStickyList("symptom");
        nonpharmacy: PagedStickyList = new PagedStickyList("nonpharmacy");
        preventiveMeasures: PagedStickyList = new PagedStickyList("preventive");
        saving: boolean = false;
        isDiseaseLoaded: boolean = false;
        isDescriptionDetailActive: boolean = false;
        modified: boolean = false;
        showDialogSave: boolean = false;

        detailWidth: number = 600;
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

        @Prop({ default: "" })
        showDialog: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: "" })
        diseaseId: number = -1;

        @Prop({ default: "" })
        linkedCause: number = -1;

        @Prop({ default: "" })
        linkedSymptom: number = -1;

        @Prop({ default: "" })
        linkedMedicamentGroup: number = -1;

        @Prop({ default: "" })
        linkedNonpharma: number = -1;

        @Prop({ default: "" })
        linkedPreventive: number = -1;

        onDescriptionChanged() {
            this.modified = true;
        }

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.isDiseaseLoaded = false;

            this.medicamentGroups = new PagedStickyList("medicamentgroup", true);
            this.symptoms = new PagedStickyList("symptom", true);
            this.diseaseList = new PagedStickyList("disease", true);
            this.nonpharmacy = new PagedStickyList("nonpharmacy", true);
            this.preventiveMeasures = new PagedStickyList("preventive", true);

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadDisease();
                }
            }
            else {
                this.isDiseaseLoaded = true;
                this.disease = new DiseaseDto;
                this.disease.causes = [];
                this.disease.symptoms = [];
                this.disease.medicamentGroup = [];
                this.disease.nonpharmaticTherapy = [];
                this.disease.preventiveMeasures = [];
                this.fillLinkedItems();
            }

            this.isDetailActive = false;
            this.detailWidth = 600;
            this.isDescriptionDetailActive = false;
            this.modified = false;
            this.showDialogSave = false;
        }

        fillLinkedItems() {
            if (this.linkedCause != -1) {
                this.disease.causes.push({ bondStrength: 0, causeId: this.linkedCause, diseaseId: this.diseaseId });
                this.getDisease();
            }
            if (this.linkedSymptom != -1) {
                this.disease.symptoms.push({ bondStrength: 0, symptomsId: this.linkedSymptom, diseaseId: this.diseaseId });
                this.getSymptoms();
            }
            if (this.linkedMedicamentGroup != -1) {
                this.disease.medicamentGroup.push({ bondStrength: 0, medicamentGroupId: this.linkedMedicamentGroup, diseaseId: this.diseaseId });
                this.getMedicamentGroups();
            }
            if (this.linkedNonpharma != -1) {
                this.disease.nonpharmaticTherapy.push({ bondStrength: 0, nonpharmaticTherapyId: this.linkedNonpharma, diseaseId: this.diseaseId });
                this.getNonpharmacy();
            }
            if (this.linkedPreventive != -1) {
                this.disease.preventiveMeasures.push({ bondStrength: 0, preventiveMeasuresId: this.linkedPreventive, diseaseId: this.diseaseId });
                this.getPreventiveMeasures();
            }
        }

        showDescriptionDetail() {
            if (!this.isDescriptionDetailActive) {
                this.detailWidth = 1300;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
                this.isDetailActive = false;
            }
            else {
                this.detailWidth = 600;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
        }

        get causesString() {
            var causes = "";
            for (var key in this.disease.causes) {
                if (!this.diseaseList.selectedNative.find(x => x.id == this.disease.causes[key].causeId))
                    continue;
                causes = causes + this.diseaseList.selectedNative.find(x => x.id == this.disease.causes[key].causeId).name;
                causes = causes + " - " + this.disease.causes[key].bondStrength.toString() + ", ";
            }
            return causes.substring(0, causes.length - 2);
        }

        get symptomsString() {
            var symptoms = "";
            for (var key in this.disease.symptoms) {
                if (!this.symptoms.selectedNative.find(x => x.id == this.disease.symptoms[key].symptomsId))
                    continue;
                symptoms = symptoms + this.symptoms.selectedNative.find(x => x.id == this.disease.symptoms[key].symptomsId).name
                symptoms = symptoms + " - " + this.disease.symptoms[key].bondStrength.toString() + ", ";
            }

            return symptoms.substring(0, symptoms.length - 2);
        }

        get medicamentGroupsString() {
            var medicamentGroups = "";
            for (var key in this.disease.medicamentGroup) {
                if (!this.medicamentGroups.selectedNative.find(x => x.id == this.disease.medicamentGroup[key].medicamentGroupId))
                    continue;
                medicamentGroups = medicamentGroups + this.medicamentGroups.selectedNative.find(x => x.id == this.disease.medicamentGroup[key].medicamentGroupId).name
                medicamentGroups = medicamentGroups + " - " + this.disease.medicamentGroup[key].bondStrength.toString() + ", ";
            }

            return medicamentGroups;
        }

        get nonpharmaticTherapysString() {
            var nonphrama = "";
            for (var key in this.disease.nonpharmaticTherapy) {
                if (!this.nonpharmacy.selectedNative.find(x => x.id == this.disease.nonpharmaticTherapy[key].nonpharmaticTherapyId))
                    continue;
                nonphrama = nonphrama + this.nonpharmacy.selectedNative.find(x => x.id == this.disease.nonpharmaticTherapy[key].nonpharmaticTherapyId).name
                nonphrama = nonphrama + " - " + this.disease.nonpharmaticTherapy[key].bondStrength.toString() + ", ";
            }

            return nonphrama;
        }

        get preventiveMeasuresString() {
            var preventives = "";
            for (var key in this.disease.preventiveMeasures) {
                if (!this.preventiveMeasures.selectedNative.find(x => x.id == this.disease.preventiveMeasures[key].preventiveMeasuresId))
                    continue;
                preventives = preventives + this.preventiveMeasures.selectedNative.find(x => x.id == this.disease.preventiveMeasures[key].preventiveMeasuresId).name
                preventives = preventives + " - " + this.disease.preventiveMeasures[key].bondStrength.toString() + ", ";
            }

            return preventives;
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            if (!this.isDetailActive) {
                this.getBondDetailList(valueList, selectedList, bondType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.bondType = bondType;
                this.detailWidth = 1300;
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
                    this.detailWidth = 600;
                    this.isDetailActive = !this.isDetailActive;
                }
            }
        }

        getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.bondDetailDto = [];

            if (!selectedList)
                selectedList = [];

            for (var key in valueList.nativeList) {
                this.singleBondDetail.id = valueList.nativeList[key].id;
                this.singleBondDetail.name = valueList.nativeList[key].name;
                switch (bondType) {
                    case 0: {
                        if (selectedList.length > 0 && selectedList.find(x => x.causeId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.causeId == valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                    case 1: {
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

                    case 2: {
                        if (selectedList.length > 0 && selectedList.find(x => x.medicamentGroupId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.medicamentGroupId == valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                    case 3: {
                        if (selectedList.length > 0 && selectedList.find(x => x.nonpharmaticTherapyId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.nonpharmaticTherapyId == valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                    case 4: {
                        if (selectedList.length > 0 && selectedList.find(x => x.preventiveMeasuresId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.preventiveMeasuresId == valueList.nativeList[key].id).bondStrength;
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
            console.log(this.bondDetailDto);

        }

        async getPreventiveMeasures() {
            if (this.disease.preventiveMeasures != null && this.disease.preventiveMeasures.length > 0) {
                var result = await PreventiveMeasuresApi.getByIds(this.disease.preventiveMeasures.map(x => x.preventiveMeasuresId));

                if (result.success) {
                    var measures = result.data;

                    var preventiveBonds = this.disease.preventiveMeasures;
                    for (var key in preventiveBonds) {
                        var measure: PreventiveMeasuresDto = measures.find(x => x.id == preventiveBonds[key].preventiveMeasuresId);

                        if (measure != null && measure != undefined) {
                            this.preventiveMeasures.selected.push({ bondStr: preventiveBonds[key].bondStrength, id: preventiveBonds[key].preventiveMeasuresId, isSelected: true, name: measure.name });
                        }
                    }

                    this.preventiveMeasures.selectedNative = this.preventiveMeasures.selectedNative.concat(measures);
                }
            }
        }

        async getNonpharmacy() {
            if (this.disease.nonpharmaticTherapy != null && this.disease.nonpharmaticTherapy.length > 0) {
                var result = await NonpharmacyApi.getByIds(this.disease.nonpharmaticTherapy.map(x => x.nonpharmaticTherapyId));

                if (result.success) {
                    var nonPharmacies = result.data;

                    var nonPharmacyBonds = this.disease.nonpharmaticTherapy;
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

        async getSymptoms() {
            if (this.disease.symptoms != null && this.disease.symptoms.length > 0) {
                var result = await SymptomApi.getByIds(this.disease.symptoms.map(x => x.symptomsId));

                if (result.success) {
                    const symp = result.data;

                    var preventiveBonds = this.disease.symptoms;
                    for (var key in preventiveBonds) {
                        const symptom: SymptomDto = symp.find(x => x.id == preventiveBonds[key].symptomsId);

                        if (symptom != null && symptom != undefined) {
                            this.symptoms.selected.push({ bondStr: preventiveBonds[key].bondStrength, id: preventiveBonds[key].symptomsId, isSelected: true, name: symptom.name });
                        }
                    }

                    this.symptoms.selectedNative = this.symptoms.selectedNative.concat(symp);
                }
            }
        }

        async loadDisease() {
            this.isDiseaseLoaded = false;
            this.loading = true;
            this.error = "";

            var result = await DiseaseApi.getDiseaseDetail(this.diseaseId);
            if (result.success) {
                this.disease = result.data;
                this.isDiseaseLoaded = true;
            }

            this.getMedicamentGroups();
            this.getSymptoms();
            this.getDisease();
            this.getNonpharmacy();
            this.getPreventiveMeasures();

            this.loading = false;
        }

        async getDisease() {
            if (this.disease.causes != null && this.disease.causes.length > 0) {
                const diseaseIds = this.disease.causes.map(x => x.causeId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.disease.causes;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].causeId);

                        if (disease != null && disease != undefined) {
                            this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].causeId, isSelected: true, name: disease.name });
                        }
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }

        async getMedicamentGroups() {
            if (this.disease.medicamentGroup != null && this.disease.medicamentGroup.length > 0) {
                var result = await MedicamentGroupApi.getByIds(this.disease.medicamentGroup.map(x => x.medicamentGroupId));

                if (result.success) {
                    var medicamentGroups = result.data;

                    var preventiveBonds = this.disease.medicamentGroup;
                    for (var key in preventiveBonds) {
                        var medicamentGroup: MedicamentGroupDto = medicamentGroups.find(x => x.id == preventiveBonds[key].medicamentGroupId);

                        if (medicamentGroup != null && medicamentGroup != undefined) {
                            this.medicamentGroups.selected.push({ bondStr: preventiveBonds[key].bondStrength, id: preventiveBonds[key].medicamentGroupId, isSelected: true, name: medicamentGroup.name });
                        }
                    }

                    this.medicamentGroups.selectedNative = this.medicamentGroups.selectedNative.concat(medicamentGroups);
                }
            }
        }

        async saveByKey() {
            if (this.isNew)
                await this.createDisease();
            else
                await this.updateDisease();
        }

        async createDisease() {
            console.warn('SAVING DISEASE', this.disease);

            this.loading = true;
            this.error = "";

            this.saving = true;

            // create
            var result = await DiseaseApi.createDisease(this.disease);
            if (result.success) {
                this.disease = result.data;
                this.modified = false;
                this.hideDetailDialog();
                this.$emit("addDisease");
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

        async updateDisease() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = await DiseaseApi.updateDisease(this.disease);
            if (result.success) {
                this.disease = result.data;
                this.modified = false;
                this.hideDetailDialog();
                this.$emit("updateDisease");
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

            //this.$emit("hideDetailDialog");

            this.saving = false;
            this.loading = false;
        }

        hideDetailDialog() {
            if (this.modified) {
                this.showDialogSave = true;
            } else {
                this.$emit("hideDetailDialog");
            }
        }

        updateBonds(item: BondDetailDto, bondType: number) {
            switch (bondType) {
                case 0: {
                    this.disease.causes = this.disease.causes.filter(x => x.causeId != item.id);
                    this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));

                        this.disease.causes.push({ causeId: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId });

                        this.diseaseList.selected.push(item);
                    }
                    //this.diseaseList.sort();
                    break;
                }

                case 1: {
                    this.disease.symptoms = this.disease.symptoms.filter(x => x.symptomsId != item.id);
                    this.symptoms.selected = this.symptoms.selected.filter(x => x.id != item.id);
                    this.symptoms.selectedNative = this.symptoms.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.symptoms.selectedNative.push(this.symptoms.nativeList.find(x => x.id === item.id));

                        this.disease.symptoms.push({ symptomsId: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId });

                        this.symptoms.selected.push(item);
                    }
                    //this.symptoms.sort();
                    break;
                }

                case 2: {
                    this.disease.medicamentGroup = this.disease.medicamentGroup.filter(x => x.medicamentGroupId != item.id);
                    this.medicamentGroups.selected = this.medicamentGroups.selected.filter(x => x.id != item.id);
                    this.medicamentGroups.selectedNative = this.medicamentGroups.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.medicamentGroups.selectedNative.push(this.medicamentGroups.nativeList.find(x => x.id === item.id));

                        this.disease.medicamentGroup.push({ medicamentGroupId: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId });

                        this.medicamentGroups.selected.push(item);
                    }
                    //this.medicamentGroups.sort();
                    break;
                }

                case 3: {
                    this.disease.nonpharmaticTherapy = this.disease.nonpharmaticTherapy.filter(x => x.nonpharmaticTherapyId != item.id);
                    this.nonpharmacy.selected = this.nonpharmacy.selected.filter(x => x.id != item.id);
                    this.nonpharmacy.selectedNative = this.nonpharmacy.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.nonpharmacy.selectedNative.push(this.nonpharmacy.nativeList.find(x => x.id === item.id));

                        this.disease.nonpharmaticTherapy.push({ nonpharmaticTherapyId: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId });

                        this.nonpharmacy.selected.push(item);
                    }
                    //this.nonpharmacy.sort();
                    break;
                }

                case 4: {
                    this.disease.preventiveMeasures = this.disease.preventiveMeasures.filter(x => x.preventiveMeasuresId != item.id);
                    this.preventiveMeasures.selected = this.preventiveMeasures.selected.filter(x => x.id != item.id);
                    this.preventiveMeasures.selectedNative = this.preventiveMeasures.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.preventiveMeasures.selectedNative.push(this.preventiveMeasures.nativeList.find(x => x.id === item.id));

                        this.disease.preventiveMeasures.push({ preventiveMeasuresId: item.id, bondStrength: item.bondStr, diseaseId: this.diseaseId });

                        this.preventiveMeasures.selected.push(item);
                    }
                    //this.preventiveMeasures.sort();
                    break;
                }
            }
            let temp = Object.assign({}, this.disease);
            this.disease = temp;
            this.modified = true;
        }

        saveChanges() {
            this.showDialogSave = false;
            this.saveByKey();
        }

        discardChanges() {
            this.modified = false;
            this.showDialogSave = false;
            this.hideDetailDialog();
        }
        handleKeys(e) {
            console.log(e.keyCode)
            if (e.keyCode === 38 || e.keyCode === 40) {
                e.target.blur();
            }
        }
    }
</script>
<style scoped>
    .dialog-footer {
        font-size: 13px;
        color: rgb(150,150,150);
    }
</style>
