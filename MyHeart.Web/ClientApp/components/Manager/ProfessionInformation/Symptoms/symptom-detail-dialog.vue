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
                                    Přidat Příznak
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
                                    Upravit Příznak
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text v-if="isSymptomLoaded">
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="symptom.name"
                                                  color="error"
                                                  label="Název"
                                                  @change="onDescriptionChanged"/>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="symptom.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive"
                                                                v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="diseaseString"
                                                  color="error"
                                                  label="Onemocnění"
                                                  @focus="showBondDetail(diseaseList,symptom.diseases,0)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive"
                                                         :bondType="0"
                                                         :currentBondType="bondType"
                                                         :valueList="diseaseList"
                                                         :selectedList="symptom.diseases"
                                                         v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    Otázky
                                </v-col>
                                <v-col cols="12" md="1">
                                    <v-btn @click="showQuestions" icon elevation="2">
                                        <v-icon v-if="!isQuestionDetailActive" color="red">mdi-arrow-right-drop-circle</v-icon>
                                        <v-icon v-if="isQuestionDetailActive" color="red">mdi-arrow-left-drop-circle</v-icon>
                                    </v-btn>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive && !isQuestionDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive && !isQuestionDetailActive">
                        <description-detail v-model="symptom.description" v-on:changed="onDescriptionChanged"/>
                    </v-col>
                    <v-col v-if="!isDetailActive && !isDescriptionDetailActive && isQuestionDetailActive">
                        <disease-question-detail :items="selectedDiseases"/>
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
                           @click="createSymptom">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateSymptom">
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
    import SymptomApi from "@backend/api/symptom";
    import DiseaseApi from "@backend/api/disease";
    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import SymptomDiseaseDto from "../../../../models/professionInfo/bonds/SymptomDiseaseDto";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";

    import DiseaseQuestionDetail from "../../../Shared/diseaseQuestionDetail.vue";
    import SymptomQuestionsSymptomDto from "../../../../models/professionInfo/bonds/SymptomQuestionsSymptomDto";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    @Component({
        name: "SymptomDetailDialog",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons, DiseaseQuestionDetail, DescriptionHtmlOutput
        },
    })
    export default class SymptomDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        diseaseList: PagedStickyList = new PagedStickyList("disease");
        isSymptomLoaded: boolean = false;
        @Prop({ default: "" })
        showDialog: boolean = false;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: "" })
        symptomId: number = -1;

        @Prop({ default: "" })
        linkedDisease: number = -1;

        onDescriptionChanged() {
            this.modified = true;
        }

        saving: boolean = false;
        symptom: SymptomDto = new SymptomDto();

        dis: SymptomDiseaseDto = new SymptomDiseaseDto;

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

        //questions variables
        isQuestionDetailActive: boolean = false;

        get diseaseString() {
            var disease = "";
            for (var key in this.symptom.diseases) {
                if (!this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId))
                    continue;
                disease = disease + this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId).name
                disease = disease + " - " + this.symptom.diseases[key].bondStrength.toString() + ", ";
            }
            return disease.substring(0, disease.length - 2);
        }

        get selectedDiseases() {
            var selected = [];

            for (var key in this.symptom.diseases) {
                if (!this.diseaseList.selectedNative.find(x => x.id == this.symptom.diseases[key].diseaseId))
                    continue;
                selected.push(this.diseaseList.selectedNative.find(x => x.id === this.symptom.diseases[key].diseaseId));
            }

            return selected;
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
                this.isQuestionDetailActive = false;
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
                this.isQuestionDetailActive = false;
            }
            else {
                this.detailWidth = 600;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
        }

        showQuestions() {
            if (!this.isQuestionDetailActive) {
                this.detailWidth = 1300;
                this.isQuestionDetailActive = !this.isQuestionDetailActive;
                this.isDetailActive = false;
                this.isDescriptionDetailActive = false;
            }
            else {
                this.detailWidth = 600;
                this.isQuestionDetailActive = !this.isQuestionDetailActive;
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

                }
                this.bondDetailDto.push(this.singleBondDetail);

                this.singleBondDetail = new BondDetailDto;

            }

        }

        updateBonds(item: BondDetailDto, bondType: number) {
            this.isTaken = false;
            switch (bondType) {
                case 0: {
                    this.symptom.diseases = this.symptom.diseases.filter(x => x.diseaseId != item.id);
                    this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.symptom.diseases.push({ diseaseId: item.id, bondStrength: item.bondStr, symptomsId: this.symptomId });
                        this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));
                        this.diseaseList.selected.push(item);
                    }
                    //this.diseaseList.sort();
                    break;
                }
            }

            let temp = Object.assign({}, this.symptom);
            this.symptom = temp;
            this.modified = true;
        }


        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.isSymptomLoaded = false;

            this.diseaseList = new PagedStickyList("disease", true);

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadSymptom();
                }
            }
            else {
                this.symptom = new SymptomDto;
                this.symptom.diseases = [];
                this.symptom.symptomQuestions = [];
                this.isSymptomLoaded = true;
                this.fillLinkedItems();
            }
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.isQuestionDetailActive = false;
            this.detailWidth = 600;
            this.modified = false;
            this.showDialogSave = false;
        }

        fillLinkedItems() {
            if (this.linkedDisease != -1) {
                this.symptom.diseases.push({ bondStrength: 0, diseaseId: this.linkedDisease, symptomsId: this.symptomId });
                this.getDisease();
            }
        }

        async loadSymptom() {
            this.loading = true;

            var result = await SymptomApi.getSymptomDetail(this.symptomId);

            if (result.success) {
                this.symptom = result.data;
                this.getDisease();

                this.isSymptomLoaded = true;
            }

            this.loading = false;
        }

        async getDisease() {
            if (this.symptom.diseases != null && this.symptom.diseases.length > 0) {
                const diseaseIds = this.symptom.diseases.map(x => x.diseaseId);
                var result = await DiseaseApi.getByIds(diseaseIds);

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.symptom.diseases;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        if (disease != null && disease != undefined) {
                            this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                        }
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }

        async saveByKey() {
            if (this.isNew)
                await this.createSymptom();
            else
                await this.updateSymptom();
        }

        async createSymptom() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = await SymptomApi.addSymptom(this.symptom);
            if (result.success) {
                this.symptom = result.data;
                this.modified = false;
                this.hideDetailDialog();
                this.$emit("addSymptom");
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

        async updateSymptom() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            var result = await SymptomApi.updateSymptom(this.symptom);
            if (result.success) {
                this.symptom = result.data;
                this.modified = false;
                this.$emit("updateSymptom");
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
