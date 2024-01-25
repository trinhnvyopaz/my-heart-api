<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  @click:outside="clickOutside"
                  persistent
                  @keydown.escape="clickOutside"
                  @keydown.ctrl.s.prevent="saveByKey">
            <v-card id="pharmacyCard">
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
                                    Přidat Farmaka
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
                                    Upravit Farmaka
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text v-if="isPharmacyLoaded">
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.name"
                                                  color="error"
                                                  label="Název účiné látky" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="medicamentGroupString"
                                                  color="error"
                                                  label="Léková skupina"
                                                  @focus="showListDetail(medicamentGroup, pharmacy.activeSubstance, 0)" />
                                </v-col>
                                <v-col cols="12" md="1">
                                    <bond-detail-buttons :isDetailActive="isListDetailActive"
                                                         :bondType="0"
                                                         :currentBondType="listType"
                                                         :valueList="medicamentGroup"
                                                         :selectedList="pharmacy.activeSubstance"
                                                         v-on:showBondDetail="showListDetail" />
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.suklCode"
                                                  color="error"
                                                  label="Sukl" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.strength"
                                                  color="error"
                                                  label="Síla" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.form"
                                                  color="error"
                                                  label="Forma" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.package"
                                                  color="error"
                                                  label="Balení" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.path"
                                                  color="error"
                                                  label="Cesta" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.supplement"
                                                  color="error"
                                                  label="Doplněk" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.atcWho"
                                                  color="error"
                                                  label="ATC WHO" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.dddamntWho"
                                                  color="error"
                                                  label="DDDAMNT WHO" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="pharmacy.dddunWho"
                                                  color="error"
                                                  label="DDDUN WHO" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-card-title>
                                Dávkování
                            </v-card-title>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.dosage"
                                                  color="error"
                                                  label="Dávkování" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.minimumDose"
                                                  color="error"
                                                  label="Minimální dávka" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.maximumDose"
                                                  color="error"
                                                  label="Maximální dávka" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="pharmacy.nameReg"
                                                  color="error"
                                                  label="Název Reg" 
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>

                            <v-row v-if="pil != null && pil.pilAvailable">
                                <v-btn class="error"
                                       text
                                       @click="getPilDoc">
                                    Stáhnout příbalový leták
                                </v-btn>
                            </v-row>
                        </v-card-text>
                    </v-col>

                    <v-col v-if="isListDetailActive">
                        <bond-detail :items="valueList" :bondType="listType" :strength="strength" v-on:updateBonds="updateModelList" />
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
                           @click="createPharmacy">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updatePharmacy">
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
    import PharmacyApi from "@backend/api/pharmacy";
    import DiseaseApi from "@backend/api/disease";
    import SymptomApi from "@backend/api/symptom";
    import PilApi from "@backend/api/pil";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";
    import PharmacyDto from "../../../../models/professionInfo/PharmacyDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import PilDto from "../../../../models/professionInfo/PilDto";
    import MedicamentGroupDto from "../../../../models/professionInfo/MedicamentGroupDto";

    import PharmacyDiseaseDto from "../../../../models/professionInfo/bonds/PharmacyDiseaseDto";
    import PharmacyMedicamentGroupDto from "../../../../models/professionInfo/bonds/PharmacyMedicamentGroupDto";
    import PharmacySymptomsDto from "../../../../models/professionInfo/bonds/PharmacySymptomsDto";

    import ListDetailDto from "../../../../models/professionInfo/helpClass/listDetailDto";
    import ListDetail from "../../../Shared/listDetail.vue";
    import ListDetailButton from "../../../Shared/listDetailButton.vue";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    @Component({
        name: "PharmacyDetailDialog",
        components: {
            BondDetail, BondDetailButtons, ListDetail, ListDetailButton
        },
    })
    export default class PharmacyDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        diseaseList: PagedStickyList = new PagedStickyList("disease");
        saving: boolean = false;
        isPharmacyLoaded: boolean = false;
        detailWidth: number = 600;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        showDialog: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: 0 })
        pharmacyId: number;

        onDescriptionChanged() {
            this.modified = true;
        }

        //disease: DiseaseDto[] = [];
        medicamentGroup: PagedStickyList = new PagedStickyList("medicamentgroup");
        //symptoms: SymptomDto[] = [];

        pharmacy: PharmacyDto = new PharmacyDto();
        pil: PilDto = null;

        listDetailDto: BondDetailDto[] = [];
        singleItem: BondDetailDto = new BondDetailDto;
        valueList: PagedStickyList = null;
        listType: number = -1;
        isListDetailActive: boolean = false;
        strength = false;

        get medicamentGroupString() {
            var medicamentGroup = "";
            for (var key in this.pharmacy.activeSubstance) {
                if (!this.medicamentGroup.selectedNative.find(x => x.id == this.pharmacy.activeSubstance[key].medicamentGroupId))
                    continue;
                medicamentGroup = medicamentGroup + this.medicamentGroup.selectedNative.find(x => x.id == this.pharmacy.activeSubstance[key].medicamentGroupId).name + ", ";
            }
            return medicamentGroup;
        }

        /*get symptomCombo() {
            return this.symptoms.map(x => new PharmacySymptomsDto(this.pharmacyId, x))
        }

        get medicamentGroupCombo() {
            return this.medicamentGroup.map(x => new PharmacyMedicamentGroupDto(this.pharmacyId, 10, x))
        }*/

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.isPharmacyLoaded = false;

            this.medicamentGroup = new PagedStickyList("medicamentgroup", true);

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadPharmacy();
                }
            }
            else {
                this.isPharmacyLoaded = true;
                this.pharmacy = new PharmacyDto;
                this.pharmacy.activeSubstance = [];
            }
            this.isListDetailActive = false;
            this.detailWidth = 600;
            //this.getDisease();
            //this.getSymptoms();
            this.modified = false;
            this.showDialogSave = false;
        }

        showListDetail(valueList: PagedStickyList, selectedList: object[], listType: number) {
            if (!this.isListDetailActive) {
                this.getListDetailList(valueList, selectedList, listType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.listType = listType;
                this.detailWidth = 1300;
                this.isListDetailActive = !this.isListDetailActive;
            }
            else {
                this.detailWidth = 600;
                this.isListDetailActive = !this.isListDetailActive;
            }
        }

        getListDetailList(valueList: PagedStickyList, selectedList: object[], listType: number) {
            this.listDetailDto = [];
            for (var key in valueList.nativeList) {
                this.singleItem.id = valueList.nativeList[key].id;
                this.singleItem.name = valueList.nativeList[key].name;
                switch (listType) {
                    case 0:
                        if (selectedList.length > 0 && selectedList.find(x => x.pharmacyId == valueList.nativeList[key].id) != null) {
                            this.singleItem.bondStr = 0;
                            this.singleItem.isSelected = true;
                        }
                        else {
                            this.singleItem.bondStr = 0;
                            this.singleItem.isSelected = false;
                        }
                        break;
                }
                this.listDetailDto.push(this.singleItem);
                this.singleItem = new BondDetailDto;
            }
        }

        updateModelList(item: BondDetailDto, listType: number) {
            switch (listType) {
                case 0:
                    this.pharmacy.activeSubstance = this.pharmacy.activeSubstance.filter(x => x.medicamentGroupId != item.id);
                    this.medicamentGroup.selected = this.medicamentGroup.selected.filter(x => x.id != item.id);
                    this.medicamentGroup.selectedNative = this.medicamentGroup.selectedNative.filter(x => x.id != item.id);

                    if (item.isSelected) {
                        this.pharmacy.activeSubstance.push({ pharmacyId: this.pharmacyId, medicamentGroupId: item.id, pharmacy: null, bondStrength: 10 });

                        this.medicamentGroup.selectedNative.push(this.medicamentGroup.nativeList.find(x => x.id === item.id));
                        this.medicamentGroup.selected.push(item);  
                    }
                    //this.medicamentGroup.sort();
                    break;
            }
            let temp = Object.assign({}, this.pharmacy);
            this.pharmacy = temp;
            this.modified = true;
        }

        async loadPharmacy() {
            this.loading = true;

            var result = await PharmacyApi.getPharmacyDetail(this.pharmacyId);

            if (result.success) {
                this.pharmacy = result.data;
                this.isPharmacyLoaded = true;
            }

            this.getMedicamentGroups();
            this.getPil();

            this.loading = false;
        }

        async getPilDoc() {
            PilApi.getFileBySukl(this.pil.sukl, this.pil.pilFileName);
        }

        async getPil() {
            const sukl = this.pharmacy.suklCode;
            var result = await PilApi.getPillBySukl(sukl);

            if (result.success) {
                this.pil = new PilDto();    
                this.pil = result.data;
            }

        }

        /*async getDisease() {
            this.loading = true;

            var result = await DiseaseApi.getDisease();

            if (result.success) {
                this.disease = result.data;
            }

            this.loading = false;
        }

        async getSymptoms() {
            this.loading = true;

            var result = await SymptomApi.getSymptoms();

            if (result.success) {
                this.symptoms = result.data;
            }

            this.loading = false;
        }*/

        async getMedicamentGroups() {
            if (this.pharmacy.activeSubstance!= null && this.pharmacy.activeSubstance.length > 0) {
                const diseaseIds = this.pharmacy.activeSubstance.map(x => x.medicamentGroupId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var substances = result.data;

                    var substanceBonds = this.pharmacy.activeSubstance;
                    for (var key in substanceBonds) {
                        var medicamentGroup: MedicamentGroupDto = substances.find(x => x.id == substanceBonds[key].medicamentGroupId);

                        if (medicamentGroup != null && medicamentGroup != undefined) {
                            this.diseaseList.selected.push({ bondStr: 0, id: substanceBonds[key].medicamentGroupId, isSelected: true, name: medicamentGroup.name });
                        }
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(substances);
                }
            }
        }

        async saveByKey() {
            if (this.isNew)
                await this.createPharmacy();
            else
                await this.updatePharmacy();
        }

        async createPharmacy() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = await PharmacyApi.createPharmacy(this.pharmacy);
            if (result.success) {
                this.pharmacy = result.data;
                this.modified = false;
                this.$emit("createPharmacy");
                this.hideDetailDialog();

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

        async updatePharmacy() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = await PharmacyApi.updatePharmacy(this.pharmacy);
            if (result.success) {
                this.pharmacy = result.data;
                this.modified = false;
                this.$emit("updatePharmacy");
                this.hideDetailDialog();
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
