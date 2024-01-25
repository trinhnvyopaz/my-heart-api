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
                                    Přidat Preventivní opatření
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
                                    Upravit Preventivní opatření
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="preventiveMeasures.name"
                                                  color="error"
                                                  label="Název" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="preventiveMeasures.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="diseaseString"
                                                  color="error"
                                                  label="Indikace"
                                                  @focus="showBondDetail(disease,preventiveMeasures.indication,0)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="disease" :selectedList="preventiveMeasures.indication" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="workspaceString"
                                                  color="error"
                                                  label="Seznam pracovišť"
                                                  @focus="showBondDetail(medicalFacilities,preventiveMeasures.workspaceList,1)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="1" :currentBondType="bondType" :valueList="medicalFacilities" :selectedList="preventiveMeasures.workspaceList" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive">
                        <description-detail v-model="preventiveMeasures.description" v-on:changed="onDescriptionChanged"/>
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
                           @click="createPreventiveMeasure">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updatePreventiveMeasure">
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

@Component({
    name: "PreventiveDetailDialog",
    components: {
        BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, DescriptionHtmlOutput
    },
})
export default class PreventiveDetailDialog extends Vue {
    loading: boolean = false;
    error: string | null = null;
    saving: boolean = false;
    isPreventiveLoaded: boolean = false;
    modified: boolean = false;
    showDialogSave: boolean = false;

    @Prop({ default: "" })
    showDialog: boolean = false;

    @Prop({ default: "" })
    isNew: boolean = false;

    @Prop({ default: "" })
    preventiveMeasuresId: number = -1;

    @Prop({ default: "" })
    linkedIndication: number = -1;

    @Prop({ default: "" })
    linkedFacility: number = -1;

    onDescriptionChanged() {
        this.modified = true;
    }

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
        this.modified = true;
    }



    @Watch("showDialog", { deep: true })
    onValueChanged() {
        this.isPreventiveLoaded = false;

        this.disease = new PagedStickyList("disease", true);
        this.medicalFacilities = new PagedStickyList("medicalFacilities", true);

        if (!this.isNew) {
            if (this.showDialog) {
                this.loadPreventiveMeasure();
            }
                
        }
        else {
            this.preventiveMeasures = new PreventiveMeasuresDto;
            this.preventiveMeasures.indication = [];
            this.preventiveMeasures.workspaceList = [];
            this.isPreventiveLoaded = true;
            this.fillLinkedItems();
        }
        this.isDetailActive = false;
        this.isDescriptionDetailActive = false;
        this.detailWidth = 600;
        this.modified = false;
        this.showDialogSave = false;
    }     

    fillLinkedItems() {
        if (this.linkedIndication != -1) {
            this.preventiveMeasures.indication.push({ bondStrength: 0, diseaseId: this.linkedIndication, preventiveMeasuresId: this.preventiveMeasuresId });
            this.getDisease();
        }
        if (this.linkedFacility != -1) {
            this.preventiveMeasures.workspaceList.push({ bondStrength: 0, medicalFacilitiesId: this.linkedFacility, preventiveMeasureId: this.preventiveMeasuresId });
            this.getMedicalFacilities();
        }
    }

    async loadPreventiveMeasure() {
        this.loading = true;

        var result = await PreventiveMeasuresApi.getPreventiveMeasuresDetail(this.preventiveMeasuresId);

        if (result.success) {
            this.preventiveMeasures = result.data;  
            this.isPreventiveLoaded = true;

            this.getDisease();
            this.getMedicalFacilities();
        }

        this.loading = false;
    }

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
            var result = await MedicalFacilitiesApi.getByIds(faclities);

            if (result.success) {
                var facilities = result.data;

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

    async saveByKey() {
        if (this.isNew)
            await this.createPreventiveMeasure();
        else
            await this.updatePreventiveMeasure();
    }

    async createPreventiveMeasure() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = await PreventiveMeasuresApi.createPreventiveMeasures(this.preventiveMeasures);
        if (result.success) {
            this.preventiveMeasures = result.data;
            this.modified = false;
            this.hideDetailDialog();
            this.$emit("createPreventiveMeasure");
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

    async updatePreventiveMeasure() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = await PreventiveMeasuresApi.updatePreventiveMeasures(this.preventiveMeasures);
        if (result.success) {
            this.preventiveMeasures = result.data;
            this.modified = false;
            this.hideDetailDialog();
            this.$emit("updatePreventiveMeasure");
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
