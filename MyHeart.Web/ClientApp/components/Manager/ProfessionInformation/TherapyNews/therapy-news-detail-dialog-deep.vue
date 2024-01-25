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
                                    Přidat Novinku v léčbě
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
                                    Upravit Novinku v léčbě
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="therapyNews.text"
                                                  color="error"
                                                  label="Název"
                                                  @change="onDescriptionChanged" />
                                </v-col>
                            </v-row>

                            <!-- DESCRIPTION -->
                            <v-row>
                                <v-col cols="12" md="11" @click="showDescriptionDetail">
                                    <description-html-output :description="therapyNews.description" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <description-detail-buttons :isDetailActive="isDescriptionDetailActive" v-on:showDescriptionDetail="showDescriptionDetail" />
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="therapyNews.webLink"
                                                  color="error"
                                                  label="Odkaz na stránku"
                                                  @change="onDescriptionChanged" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="diseaseString"
                                                  color="error"
                                                  label="Onemocnění"
                                                  @focus="showBondDetail(diseaseList,therapyNews.diseases,0)" />
                                </v-col>
                                <v-col cols="6" md="1">
                                    <bond-detail-buttons :isDetailActive="isDetailActive" :bondType="0" :currentBondType="bondType" :valueList="diseaseList" :selectedList="therapyNews.diseases" v-on:showBondDetail="showBondDetail" />
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-col>
                    <v-col v-if="isDetailActive">
                        <v-btn v-if="bondType == 0"
                               class="error"
                               text
                               :disabled="loading"
                               @click="showDiseaseAdd">
                            Přidat onemocnění
                        </v-btn>
                        <bond-detail :items="valueList" :bondType="bondType" v-on:updateBonds="updateBonds" :strength="strength"></bond-detail>
                    </v-col>
                    <v-col v-if="isDescriptionDetailActive">
                        <description-detail v-model="therapyNews.description" v-on:changed="onDescriptionChanged" />
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
                           @click="createTherapyNews">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           class="error"
                           text
                           :disabled="loading || !modified"
                           @click="updateTherapyNews">
                        Upravit
                    </v-btn>
                </v-card-actions>
            </v-card>

            <disease-detail-dialog :isNew="deepNew"
                                   :showDialog="isDiseaseAddActive"
                                   :diseaseId="deepId"
                                   v-on:hideDetailDialog="hideDiseaseAddDialog"
                                   v-on:addDisease="addDeepAsset"
                                   v-on:updateDisease="updateDeepAsset">
            </disease-detail-dialog>
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
    import TherapyNewsApi from "@backend/api/therapyNews";
    import TherapyNewsDto from "../../../../models/professionInfo/TherapyNewsDto";
    import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
    import TherapyNewsDiseaseDto from "../../../../models/professionInfo/bonds/TherapyNewsDiseaseDto";
    import DiseaseApi from "@backend/api/disease";

    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "../../../Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "../../../Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "../../../Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";

    import DiseaseDetailDialog from "@components/Manager/ProfessionInformation/Disease/disease-detail-dialog.vue";
@Component({
    name: "TherapyNewsDetailDialogDeep",
    components: {
        BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
        DescriptionHtmlOutput, DiseaseDetailDialog
    },
})
export default class TherapyNewsDetailDialogDeep extends Vue {
    deepId: number = -1;
    deepNew: boolean = true;
    loading: boolean = false;
    error: string | null = null;
    modified: boolean = false;
    showDialogSave: boolean = false;
    isDiseaseAddActive: boolean = false;

    @Prop({ default: "" })
    showDialog: boolean = false;

    @Prop({ default: "" })
    isNew: boolean = false;

    @Prop({ default: 0 })
    therapyNewsId: number = -1;

    onDescriptionChanged() {
        this.modified = true;
    }

    saving: boolean = false;
    therapyNews: TherapyNewsDto = new TherapyNewsDto();
    diseaseList: PagedStickyList = new PagedStickyList("disease");
    disease: TherapyNewsDiseaseDto = new TherapyNewsDiseaseDto;

    detailWidth: number = 600;
    bondDetailDto: BondDetailDto[] = [];
    singleBondDetail: BondDetailDto = new BondDetailDto;
    isDetailActive: boolean = false;
    isDescriptionDetailActive: boolean = false;
    bondType: number = -1;
    valueList: PagedStickyList = null;
    selectedList: object[] = null;
    isTaken: boolean = false;
    strength: boolean = false;

    showDiseaseAdd() {
        this.deepNew = true;
        this.deepId = -1;
        this.isDiseaseAddActive = true;
    }

    hideDiseaseAddDialog() {
        this.isDiseaseAddActive = false;
        this.diseaseList.loadPage();
    }

    addDeepAsset() {
        console.log("deep asset added");
    }

    updateDeepAsset() {
        console.log("deep asset updated");
    }

    get diseaseString() {
        var disease = "";
        for (var key in this.therapyNews.diseases) {
            if (!this.diseaseList.selectedNative.find(x => x.id == this.therapyNews.diseases[key].diseaseId))
                continue;
            disease = disease + this.diseaseList.selectedNative.find(x => x.id == this.therapyNews.diseases[key].diseaseId).name + ", ";
        }

        return disease.substring(0, disease.length - 2);
    }

    getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
        this.bondDetailDto = [];
        for (var key in valueList.nativeList) {
            this.singleBondDetail.id = valueList.nativeList[key].id;
            this.singleBondDetail.name = valueList.nativeList[key].name;
            switch (bondType) {
                case 0: {
                    if (selectedList.length > 0 && selectedList.find(x => x.diseaseId == valueList.nativeList[key].id) != null) {
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
                this.therapyNews.diseases = this.therapyNews.diseases.filter(x => x.diseaseId != item.id);
                this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                if (item.isSelected) {
                    this.therapyNews.diseases.push({ diseaseId: item.id, therapyNewsId: this.therapyNewsId });
                    this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));
                    this.diseaseList.selected.push(item);
                }
                //this.diseaseList.sort();
                break;
            }
        }

        this.therapyNews = Object.assign({}, this.therapyNews);
        this.modified = true;
    }

    showDescriptionDetail() {
        this.isDetailActive = false;

        if (!this.isDescriptionDetailActive) {
            this.detailWidth = 1300;
            this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
        }
        else {
            this.detailWidth = 600;
            this.isDescriptionDetailActive = !this.isDescriptionDetailActive;
        }
    }


    showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number) {
        this.isDescriptionDetailActive = false;

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
        this.diseaseList = new PagedStickyList("disease", true);

        if (!this.isNew) {
            if (this.showDialog) {
                this.loadTherapyNews();
            }
                
        }
        else {
            this.therapyNews = new TherapyNewsDto;
            this.therapyNews.diseases = [];
        }
        this.isDetailActive = false;
        this.isDescriptionDetailActive = false;
        this.detailWidth = 600;
        this.modified = false;
        this.showDialogSave = false;
    }     

    async loadTherapyNews() {
        this.loading = true;

        var result = await TherapyNewsApi.getTherapyNewsDetail(this.therapyNewsId);

        if (result.success) {
            this.therapyNews = result.data;

            this.getDisease();
        }

        this.loading = false;
    }

    async getDisease() {
        if (this.therapyNews.diseases != null && this.therapyNews.diseases.length > 0) {
            const diseaseIds = this.therapyNews.diseases.map(x => x.diseaseId);
            var result = await DiseaseApi.getByIds(diseaseIds);

            if (result.success) {
                var diseases = result.data;

                var diseaseBonds = this.therapyNews.diseases;
                for (var key in diseaseBonds) {
                    var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                    if (disease != null && disease != undefined) {
                        this.diseaseList.selected.push({ bondStr: 0, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                    }
                }

                this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
            }
        }
    }

    async saveByKey() {
        if (this.isNew)
            await this.createTherapyNews();
        else
            await this.updateTherapyNews();
    }

    async createTherapyNews() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = await TherapyNewsApi.createTherapyNews(this.therapyNews);
        if (result.success) {
            this.therapyNews = result.data;
            this.modified = false;
            this.$emit("createTherapyNews");
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

    async updateTherapyNews() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = await TherapyNewsApi.updateTherapyNews(this.therapyNews);
        if (result.success) {
            this.therapyNews = result.data;
            this.modified = false;
            this.$emit("updateTherapyNews");
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
    .dialog-footer{
        font-size:13px;
        color:rgb(150,150,150);
    }

</style>
