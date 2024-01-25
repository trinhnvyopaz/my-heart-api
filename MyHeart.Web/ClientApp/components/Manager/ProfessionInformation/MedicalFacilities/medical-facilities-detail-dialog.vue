<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  @click:outside="clickOutside"
                  persistent
                  @keydown.escape="hideDetailDialog"
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
                                    Přidat Zdravotnické zařízení
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
                                    Upravit Zdravotnické zařízení
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text v-if="isMedicalFacilitiesLoaded">
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.name"
                                                  color="error"
                                                  label="Název" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.facilityId"
                                                  color="error"
                                                  label="Id zařízení" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.facilityCode"
                                                  color="error"
                                                  label="Kód zařízení" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.facilityTypeCode"
                                                  color="error"
                                                  label="Kód typu zařízení" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.character"
                                                  color="error"
                                                  label="Charakter" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.characterSecondary"
                                                  color="error"
                                                  label="Druhotný charakter" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.ico"
                                                  color="error"
                                                  label="IČO" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.pcz"
                                                  color="error"
                                                  label="PCZ" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.pcdp"
                                                  color="error"
                                                  label="PCDP" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.region"
                                                  color="error"
                                                  label="Kraj" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.regionCode"
                                                  color="error"
                                                  label="Kód kraje" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.district"
                                                  color="error"
                                                  label="Okres" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.districtCode"
                                                  color="error"
                                                  label="Kód okresu" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.administrativeDistrict"
                                                  color="error"
                                                  label="Správní obvod" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.municipality"
                                                  color="error"
                                                  label="Obec" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="7">
                                    <v-text-field v-model="medicalFacilities.street"
                                                  color="error"
                                                  label="Ulice" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col cols="3">
                                    <v-text-field v-model="medicalFacilities.buildingNumber"
                                                  color="error"
                                                  label="Číslo popisné" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col cols="2">
                                    <v-text-field v-model="medicalFacilities.zip"
                                                  color="error"
                                                  label="PSČ" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.telephone"
                                                  color="error"
                                                  label="Telefon" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.email"
                                                  color="error"
                                                  label="Email" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.website"
                                                  color="error"
                                                  label="Webová stránka" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="medicalFacilities.fax"
                                                  color="error"
                                                  label="Fax" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.providerType"
                                                  color="error"
                                                  label="Typ poskytovatele" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.providerName"
                                                  color="error"
                                                  label="Jméno poskytovatele" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.careField"
                                                  color="error"
                                                  label="Obor péče" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.careForm"
                                                  color="error"
                                                  label="Forma péče" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.careType"
                                                  color="error"
                                                  label="Typ péče" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="medicalFacilities.representative"
                                                  color="error"
                                                  label="Zástupci" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-card-title>
                                Prováděné výkony
                            </v-card-title>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="preventiveMeasuresString"
                                                  color="error"
                                                  label="Preventivní opatření"
                                                  @focus="showBondDetail(preventiveMeasures,medicalFacilities.preventiveMeasures,0)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="preventiveMeasures" :selectedList="medicalFacilities.preventiveMeasures" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="nonpharmaString"
                                                  color="error"
                                                  label="Nefarmakologické léčby"
                                                  @focus="showBondDetail(nonpharmacy,medicalFacilities.nonpharmaticTherapy,1)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="1" :currentBondType="bondType" :valueList="nonpharmacy" :selectedList="medicalFacilities.nonpharmaticTherapy" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row v-show="hasCoords">
                                <v-col>
                                    <map-container :gps="medicalFacilities.gps">

                                    </map-container>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds"></bond-detail>
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
                           @click="createMedicalFacilities">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           text
                           :disabled="loading || !modified"
                           @click="updateMedicalFacilities">
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
    @Component({
        name: "MedicalFacilitiesDetailDialog",
        components: {
            BondDetail, BondDetailButtons, MapContainer
        },
    })
    export default class MedicalFacilitiesDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isMedicalFacilitiesLoaded: boolean = false;
        detailWidth: number = 600;
        isDetailActive: boolean = false;
        hasCoords: boolean = false;
        isMapLoaded: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        showDialog: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: 0 })
        medicalFacilitiesId: number = -1;

        bondDetailDto: BondDetailDto[] = [];
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isTaken: boolean = false;

        medicalFacilities: MedicalFacilitiesDto = new MedicalFacilitiesDto();
        nonpharmacy: PagedStickyList = new PagedStickyList("nonpharmacy", true);
        preventiveMeasures: PagedStickyList = new PagedStickyList("preventive", true);

        onDescriptionChanged() {
            this.modified = true;
        }

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

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.isMedicalFacilitiesLoaded = false;

            this.nonpharmacy = new PagedStickyList("nonpharmacy", true);
            this.preventiveMeasures = new PagedStickyList("preventive", true);
            this.isMapLoaded = false;

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadMedicalFacilities();
                }

            }
            else {
                this.medicalFacilities = new MedicalFacilitiesDto;
                this.medicalFacilities.nonpharmaticTherapy = [];
                this.medicalFacilities.preventiveMeasures = [];
                this.isMedicalFacilitiesLoaded = true;
            }
            this.isDetailActive = false;
            this.detailWidth = 600;
            this.modified = false;
            this.showDialogSave = false;
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
            for (var key in valueList.nativeList) {
                this.singleBondDetail.id = valueList.nativeList[key].id;
                this.singleBondDetail.name = valueList.nativeList[key].name;
                switch (bondType) {
                    case 0: {
                        if (selectedList.length > 0 && selectedList.find(x => x.preventiveMeasureId == valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.preventiveMeasureId == valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }

                    case 1: {
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

                }
                this.bondDetailDto.push(this.singleBondDetail);

                this.singleBondDetail = new BondDetailDto;

            }
            console.log(this.bondDetailDto);

        }



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
            this.error = "";

            var result = await MedicalFacilitiesApi.getMedicalFacilitiesDetail(this.medicalFacilitiesId);

            if (result.success) {
                this.medicalFacilities = result.data;
                this.isMedicalFacilitiesLoaded = true;

                this.getNonpharmacy();
                this.getPreventiveMeasures();
                this.fillHasCoords();
            }

            this.loading = false;

        }

        fillHasCoords() {
            this.hasCoords = false;
            const gps = this.medicalFacilities.gps;
            if (gps == null || gps == "") return;

            this.hasCoords = true;
        }

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
            this.modified = true;
        }

        async saveByKey() {
            if (this.isNew)
                await this.createMedicalFacilities();
            else
                await this.updateMedicalFacilities();
        }

        async createMedicalFacilities() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            // create

            var result = await MedicalFacilitiesApi.createMedicalFacilities(this.medicalFacilities);
            if (result.success) {
                this.medicalFacilities = result.data;
                this.modified = false;
                this.hideDetailDialog();
                this.$emit("createMedicalFacilities");
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

        async updateMedicalFacilities() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            // update

            var result = await MedicalFacilitiesApi.updateMedicalFacilities(this.medicalFacilities);
            if (result.success) {
                this.medicalFacilities = result.data;
                this.modified = false;
                this.hideDetailDialog();
                this.$emit("updateMedicalFacilities");
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
    .dialog-footer {
        font-size: 13px;
        color: rgb(150,150,150);
    }
</style>
