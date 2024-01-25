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
                                    Přidat otázku
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
                                    Upravit otázku
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="symptomQuestion.text" :title="htmlOutputTitle" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">

                                    <v-text-field :value="symptomsString"
                                                  color="error"
                                                  label="Příznaky"
                                                  @focus="showBondDetail(symptomsList,symptomsSelectedList,0)" />

                                    <!--<v-autocomplete v-model="symptomQuestion.symptoms"
                                                :items="symptomsCombo"
                                                item-text="symptoms.name"
                                                item-value="symptomsId"
                                                return-object
                                                color="error"
                                                label="Příznak"
                                                multiple>
                                    <template v-slot:item="{ item, on }">
                                        <v-row align="center" justify="center">
                                            <v-col align="center">
                                                <v-list-item-title v-html="item.symptoms.name" v-on="on" @click.stop.prevent></v-list-item-title>
                                            </v-col>
                                            <v-col>
                                                <v-text-field align="center"
                                                              v-model="item.bondStrength"
                                                              color="error"
                                                              label="Sila vazby"
                                                              @click.stop.prevent
                                                              type="number" />
                                            </v-col>
                                        </v-row>
                                    </template>
                                </v-autocomplete>-->
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="symptomsList" :selectedList="symptomsSelectedList" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">

                                    <v-text-field :value="diseaseString"
                                                  color="error"
                                                  label="Nemoci"
                                                  @focus="showBondDetail(diseaseList, diseaseSelectedList, 1)" />

                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="1" :currentBondType="bondType" :valueList="diseaseList" :selectedList="diseaseSelectedList" v-on:showBondDetail="showBondDetail" />
                                </v-col>

                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive && !isDescriptionDetailActive">
                        <bond-detail :items="valueList" :bondType="bondType" :strength="strength" :multi="multi" v-on:updateBonds="updateBonds"></bond-detail>
                    </v-col>
                    <v-col v-if="!isDetailActive && isDescriptionDetailActive">
                        <description-detail v-model="symptomQuestion.text" v-on:changed="onDescriptionChanged"/>
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
                           @click="createSymptomQuestions">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateSymptomQuestions">
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
    import SymptomQuestionsApi from "@backend/api/symptomQuestions";
    import SymptomQuestionsDto from "../../../../models/professionInfo/SymptomQuestionsDto";
    import SymptomQuestionsSymptomDto from "../../../../models/professionInfo/bonds/SymptomQuestionsSymptomDto";
    import SymptomQuestionsDiseaseDto from "../../../../models/professionInfo/bonds/SymptomQuestionsDiseaseDto";

    import SymptomDto from "../../../../models/professionInfo/SymptomDto";
    import SymptomApi from "@backend/api/symptom";

    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto"
    import DiseaseApi from "@backend/api/disease"

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import MathTools from '../../../../backend/tools/MathTools';

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    //import autocompleteBondStrength from "../../../Shared/AutocompleteBondStrength";

    @Component({
        name: "SymptomQuestionsDetailDialog",
        components: {
            BondDetail, BondDetailButtons, DescriptionHtmlOutput, DescriptionDetail, DescriptionDetailButtons
        },
    })
    export default class SymptomQuestionsDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        showDialog: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: 0 })
        symptomQuestionId: number = -1;

        @Prop({ default: "" })
        linkedSymptom: number = -1;

        onDescriptionChanged() {
            this.modified = true;
        }

        checkbox: boolean = false;
        symptomQuestion: SymptomQuestionsDto = new SymptomQuestionsDto();
        symptoms: SymptomDto[] = [];
        combo: SymptomQuestionsSymptomDto[] = [];
        diseases: DiseaseDto[] = [];

        diseaseSelectedList: SymptomQuestionsDiseaseDto[] = [];
        symptomsSelectedList: SymptomQuestionsSymptomDto[] = [];
        symptomsList: PagedStickyList = new PagedStickyList("symptom");
        diseaseList: PagedStickyList = new PagedStickyList("disease");

        bondStrength: number = 0;
        detailWidth: number = 600;
        bondDetailDto: BondDetailDto[] = [];
        singleBond: BondDetailDto = new BondDetailDto;
        singleBondDetail: BondDetailDto = new BondDetailDto;
        isDetailActive: boolean = false;
        isDiseaseDetailActive: boolean = false;
        bondType: number = -1;
        valueList: PagedStickyList = null;
        selectedList: object[] = null;
        strength: boolean = false;
        multi: boolean = false;
        htmlOutputTitle: string = "text";
        isDescriptionDetailActive: boolean = false;

        get symptomsString() {
            var symptoms = "";
            for (var key in this.symptomsList.selectedNative) {
                symptoms = symptoms += this.symptomsList.selectedNative[key].name
            }

            return symptoms;
        }

        get diseaseString() {
            var diseases = "";
            for (var key in this.diseaseList.selectedNative) {
                diseases = diseases += this.diseaseList.selectedNative[key].name;
                diseases = diseases + " - " + this.diseaseList.selected.find(x => x.id == this.diseaseList.selectedNative[key].id).bondStr.toString() + ", ";
            }

            return diseases.substring(0, diseases.length - 2);
        }

        get symptomsCombo() {
            if (!this.saving) {
                if (this.combo.length == 0 || this.isNew) {
                    this.combo = this.symptoms.map(x => new SymptomQuestionsSymptomDto(this.symptomQuestionId, 0, x));
                }

                if (this.symptomQuestion.id > 0 && this.symptomsSelectedList.length > 0 && this.combo.length > 0) {
                    for (var key in this.symptomsSelectedList) {
                        this.combo.find(x => x.symptomId == this.symptomsSelectedList[key].symptomId).bondStrength = this.symptomsSelectedList[key].bondStrength;
                    }
                }
                return this.combo
            }
        }

        showDescriptionDetail() {
            this.isDetailActive = false;
            this.isDiseaseDetailActive = false;

            if (!this.isDescriptionDetailActive) {
                this.detailWidth = 1300;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
            else {
                this.detailWidth = 600;
                this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
            }
        }

        changeDefaultBondStrength() {
            this.bondStrength = MathTools.clamp(this.bondStrength, 0, 10);
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
                        if (selectedList.length > 0 && selectedList.find(x => x.symptomId === valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.symptomId === valueList.nativeList[key].id).bondStrength;
                            this.singleBondDetail.isSelected = true;
                        }
                        else {
                            this.singleBondDetail.bondStr = 0;
                            this.singleBondDetail.isSelected = false;
                        }
                        break;
                    }
                    case 1: {
                        if (selectedList.length > 0 && selectedList.find(x => x.diseaseId === valueList.nativeList[key].id) != null) {
                            this.singleBondDetail.bondStr = selectedList.find(x => x.diseaseId === valueList.nativeList[key].id).bondStrength;
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

        updateBonds(item: BondDetailDto, bondType: number) {
            switch (bondType) {
                case 0: {

                    this.symptomsSelectedList = this.symptomsSelectedList.filter(x => x.symptomId != item.id);
                    this.symptomsList.selected = this.symptomsList.selected.filter(x => x.id != item.id);
                    this.symptomsList.selectedNative = this.symptomsList.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.symptomsList.selectedNative.push(this.symptomsList.nativeList.find(x => x.id === item.id));

                        this.symptomsSelectedList.push({ symptomId: item.id, bondStrength: item.bondStr, symptomQuestionsId: this.symptomQuestionId, symptom: this.symptomsList.selectedNative.find(x => x.id === item.id) });

                        this.symptomsList.selected.push(item);
                    }
                    //this.symptomsList.sort();
                    break;
                }
                case 1: {
                    this.diseaseSelectedList = this.diseaseSelectedList.filter(x => x.diseaseId != item.id);
                    this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                    if (item.isSelected) {
                        this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));

                        this.diseaseSelectedList.push({ diseaseId: item.id, bondStrength: item.bondStr, symptomQuestionsId: this.symptomQuestionId, disease: this.diseaseList.selectedNative.find(x => x.id === item.id) });

                        this.diseaseList.selected.push(item);
                    }
                    //this.diseaseList.sort();
                    break;
                }
            }

            

            this.symptomQuestion = Object.assign({}, this.symptomQuestion);
            this.modified = true;
        }

        isSymptomSelected(item: any) {

            if (this.isNew) {
                return false;
            }
            else {
                if (this.symptomQuestion.id > 0) {
                    return this.symptomsSelectedList.some(x => x.symptomId == item.symptomsId);
                }
                else {
                    return false;
                }

            }
        }

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.isDescriptionDetailActive = false;
            this.isDiseaseDetailActive = false;

            if (bondType === 1) {
                this.strength = true;
                this.multi = true;
            } else {
                this.strength = false;
                this.multi = false;
            }

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

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            this.diseaseSelectedList = [];
            this.symptomsSelectedList = [];
            this.symptomsList = new PagedStickyList("symptom");
            this.diseaseList = new PagedStickyList("disease", true);

            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadSymptomQuestions();
                }
            }
            else {
                this.symptomQuestion = new SymptomQuestionsDto;
                this.symptomsSelectedList = [];
                this.diseaseSelectedList = [];
                this.fillLinkedItems();
            }

            this.combo = [];
            this.isDetailActive = false;
            this.isDescriptionDetailActive = false;
            this.detailWidth = 600;
            this.modified = false;
            this.showDialogSave = false;
        }

        fillLinkedItems() {
            if (this.linkedSymptom != -1) {
                this.symptomQuestion.symptom = { symptoms: null, bondStrength: 0, symptomsId: this.linkedSymptom, symptomQuestionsId: this.symptomQuestionId };
                this.getSymptoms();
            }
        }

        async loadSymptomQuestions() {
            this.loading = true;

            var result = await SymptomQuestionsApi.getSymptomQuestionDetail(this.symptomQuestionId);

            if (result.success) {
                this.symptomQuestion = result.data;
                if (this.symptomQuestion.diseases != null) {
                    this.diseaseSelectedList = this.symptomQuestion.diseases;
                }
                if (this.symptomQuestion.symptom != null) {
                    var s = this.symptomQuestion.symptom;
                    this.symptomsSelectedList.push({ bondStrength: s.bondStrength, symptomQuestionsId: s.symptomQuestionsId, symptoms: s.symptoms, symptomsId: s.symptomsId });
                }
            }

            this.getSymptoms();
            this.getDiseases();
            this.loading = false;
        }

        async getSymptoms() {
            if (this.symptomQuestion.symptom != null) {
                var s = this.symptomQuestion.symptom;

                var result = await SymptomApi.getSymptomDetail(s.symptomsId);

                if (result.success) {
                    var sDetail = result.data;

                    this.symptomsList.selected.push({ bondStr: s.bondStrength, id: s.symptomsId, name: sDetail.name, isSelected: true });

                    this.symptomsList.selectedNative.push(sDetail);
                }
            }
        }

        async getDiseases() {
            if (this.symptomQuestion.diseases != null && this.symptomQuestion.diseases.length > 0) {
                var result = await DiseaseApi.getByIds(this.symptomQuestion.diseases.map(x => x.diseaseId));

                if (result.success) {
                    var diseases = result.data;

                    var diseaseBonds = this.symptomQuestion.diseases;
                    for (var key in diseaseBonds) {
                        var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                        this.diseaseList.selected.push({ bondStr: diseaseBonds[key].bondStrength, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                    }

                    this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
                }
            }
        }

        async saveByKey() {
            if (this.isNew)
                await this.createSymptomQuestions();
            else
                await this.updateSymptomQuestions();
        }


        async createSymptomQuestions() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            // create
            if (this.symptomQuestion == null) this.symptomQuestion = new SymptomQuestionsDto();

            if (this.diseaseSelectedList.length > 0) {
                //this.symptomQuestion.diseases = this.diseaseSelectedList;
                var selected = [];
                for (var sel in this.diseaseSelectedList) {
                    var select = this.diseaseSelectedList[sel];
                    selected.push(new SymptomQuestionsDiseaseDto(select.symptomQuestionsId, select.bondStrength, select.disease));
                }

                this.symptomQuestion.diseases = selected;
            }
            
            if (this.symptomsSelectedList.length > 0) {
                var s = this.symptomsSelectedList[0];

                if (this.symptomQuestion.symptom == null) {
                    this.symptomQuestion.symptom = new SymptomQuestionsSymptomDto(s.symptomQuestionsId, s.bondStrength, s.symptoms);
                } else {
                    this.symptomQuestion.symptom.bondStrength = s.bondStrength;
                    this.symptomQuestion.symptom.symptomQuestionsId = s.symptomQuestionsId;
                    this.symptomQuestion.symptom.symptoms = s.symptoms;
                    this.symptomQuestion.symptom.symptomsId = s.symptomsId;
                }
            }
            


            var result = await SymptomQuestionsApi.createSymptomQuestions(this.symptomQuestion);
            if (result.success) {
                this.symptomQuestion = result.data;
                this.modified = false;
                this.$emit("createSymptomQuestion");
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

        async updateSymptomQuestions() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            // update
            if (this.diseaseSelectedList.length > 0) {
                //this.symptomQuestion.diseases = this.diseaseSelectedList;
                var selected = [];
                for (var sel in this.diseaseSelectedList) {
                    var select = this.diseaseSelectedList[sel];
                    selected.push(new SymptomQuestionsDiseaseDto(select.symptomQuestionsId, select.bondStrength, select.disease));
                }

                this.symptomQuestion.diseases = selected;
            }

            if (this.symptomsSelectedList.length > 0) {
                var s = this.symptomsSelectedList[0];

                if (this.symptomQuestion.symptom == null) {
                    this.symptomQuestion.symptom = new SymptomQuestionsSymptomDto(s.symptomQuestionsId, s.bondStrength, s.symptoms);
                } else {
                    this.symptomQuestion.symptom.bondStrength = s.bondStrength;
                    this.symptomQuestion.symptom.symptomQuestionsId = s.symptomQuestionsId;
                    this.symptomQuestion.symptom.symptoms = s.symptoms;
                }
            }
            var result = await SymptomQuestionsApi.updateSymptomQuestions(this.symptomQuestion);
            if (result.success) {
                this.symptomQuestion = result.data;
                this.modified = false;
                this.$emit("updateSymptomQuestion");
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
