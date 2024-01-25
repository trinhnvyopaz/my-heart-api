<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  @click:outside="clickOutside"
                  persistent
                  @keydown.escape="clickOutside"
                  @keydown.ctrl.s.prevent="saveByKey">
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
                                    Přidat Nefarmakologická léčba
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
                                    Upravit Nefarmakologická léčba
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text v-if="isNonpharmacyLoaded">
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="nonpharmacy.name"
                                                  color="error"
                                                  label="Název" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="nonpharmacy.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="indicationString"
                                                  color="error"
                                                  label="Indikace"
                                                  @focus="showBondDetail(disease,nonpharmacy.indication,0)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="indicationList" :selectedList="nonpharmacy.indication" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="complicationString"
                                                  color="error"
                                                  label="Komplikace"
                                                  @focus="showBondDetail(disease,nonpharmacy.complication,1)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="1" :currentBondType="bondType" :valueList="complicationList" :selectedList="nonpharmacy.complication" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="nonpharmacy.efficiency"
                                                  color="error"
                                                  label="Účinnost" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="nonpharmacy.hospitalizationLength"
                                                  color="error"
                                                  label="Délka hospitalizace" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="medicalFacilitiesString"
                                                  color="error"
                                                  label="zdravotnická zařízení"
                                                  @focus="showBondDetail(medicalFacilities,nonpharmacy.medicalIntervention,2)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="2" :currentBondType="bondType" :valueList="medicalFacilities" :selectedList="nonpharmacy.medicalIntervention" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive">
                        <description-detail v-model="nonpharmacy.description" v-on:changed="onDescriptionChanged"/>
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
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="createNonpharmacy">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateNonpharmacy">
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
                        <v-btn class="error"
                               text
                               @click="discardChanges">
                            Zahodit
                        </v-btn>
                        <v-btn class="error"
                               text
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

@Component({
    name: "NonpharmacyDetailDialog",
    components: {
        BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, DescriptionHtmlOutput
    },
})
export default class NonpharmacyDetailDialog extends Vue {
    loading: boolean = false;
    error: string | null = null;
    saving: boolean = false;
    isNonpharmacyLoaded: boolean = false;
    modified: boolean = false;
    showDialogSave: boolean = false;

    @Prop({ default: "" })
    showDialog: boolean = false;

    @Prop({ default: "" })
    isNew: boolean = false;

    @Prop({ default: "" })
    nonpharmacyId: number = -1;

    @Prop({ default: "" })
    linkedDisease: number = -1;

    @Prop({ default: "" })
    linkedFacility: number = -1;

    onDescriptionChanged() {
        this.modified = true;
    }

    indicationList: PagedStickyList = new PagedStickyList("disease");
    complicationList: PagedStickyList = new PagedStickyList("disease");
    medicalFacilities: PagedStickyList = new PagedStickyList("medicalFacilities");

    nonpharmacy: NonpharmacyDto = new NonpharmacyDto();
    indication: NonpharmaticTherapyDiseaseDto = new NonpharmaticTherapyDiseaseDto;
    medicalFacilitie: NonpharmaticTherapyMedicalFaclitiesDto = new NonpharmaticTherapyMedicalFaclitiesDto;

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
        this.modified = true;
    }


    @Watch("showDialog", { deep: true })
    onValueChanged() {
        this.isNonpharmacyLoaded = false;

        this.indicationList = new PagedStickyList("disease", true);
        this.complicationList = new PagedStickyList("disease", true);
        this.medicalFacilities = new PagedStickyList("medicalFacilities", true);

        if (!this.isNew) {
            if (this.showDialog) {
                this.loadNonpharmacy();
            }
                
        }
        else {
            this.isNonpharmacyLoaded = true;
            this.nonpharmacy = new NonpharmacyDto;
            this.nonpharmacy.complication = [];
            this.nonpharmacy.indication = [];
            this.nonpharmacy.medicalIntervention = [];
            this.fillLinkedItems();
        }

        this.isDetailActive = false;
        this.detailWidth = 600;
        this.isDescriptionDetailActive = false;
        this.modified = false;
        this.showDialogSave = false;
    }     

    fillLinkedItems() {
        if (this.linkedDisease != -1) {
            this.nonpharmacy.indication.push({ bondStrength: 0, diseaseId: this.linkedDisease, NonpharmaticTherapyId: this.nonpharmacyId });
            this.nonpharmacy.complication.push({ bondStrength: 0, diseaseId: this.linkedDisease, NonpharmaticTherapyId: this.nonpharmacyId });
            this.getIndications();
            this.getComplications();
        }
        if (this.linkedFacility != -1) {
            this.nonpharmacy.medicalIntervention.push({ bondStrength: 0, medicalFacilitiesId: this.linkedFacility, NonpharmaticTherapyId: this.nonpharmacyId });
            this.getMedicalFacilities();
        }
    }

    async loadNonpharmacy() {
        this.loading = true;

        var result = await NonpharmacyApi.getNonpharmacyDetail(this.nonpharmacyId);

        if (result.success) {
            this.nonpharmacy = result.data;
            this.isNonpharmacyLoaded = true;

            this.getIndications();
            this.getComplications();
            this.getMedicalFacilities();
        }

        this.loading = false;
    }

    async getIndications() {
        if (this.nonpharmacy.indication!= null && this.nonpharmacy.indication.length > 0) {
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

            if (result.success) {
                var facilities = result.data;

                var facilityBonds = this.nonpharmacy.medicalIntervention;
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

    async saveByKey() {
        if (this.isNew)
            await this.createNonpharmacy();
        else
            await this.updateNonpharmacy();
    }

    async createNonpharmacy() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = await NonpharmacyApi.createNonpharmacy(this.nonpharmacy);
        if (result.success) {
            this.nonpharmacy = result.data;
            this.modified = false;
            this.hideDetailDialog();
            this.$emit("createNonpharmacy");
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

    async updateNonpharmacy() {
        this.loading = true;
        this.error = "";
        this.saving = true;


        var result = await NonpharmacyApi.updateNonpharmacy(this.nonpharmacy);
        if (result.success) {
            this.nonpharmacy = result.data;
            this.modified = false;
            this.hideDetailDialog();
            this.$emit("updateNonpharmacy");
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

    clickOutside() {
        //this.hideDetailDialog();
    }

    hideDetailDialog() {
        if (this.modified) {
            this.showDialogSave = true;
        } else {
            this.$emit("hideDetailDialog");
        }
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

}
</script>
<style scoped>
    .dialog-footer{
        font-size:13px;
        color:rgb(150,150,150);
    }

</style>
