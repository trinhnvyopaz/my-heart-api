<template>
    <div>
        <v-dialog v-model="showDialog"
                  :width="detailWidth"
                  @click:outside="clickOutside"
                  persistent
                  @keydown.escape="clickOutside">
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
                                    Přidat komerční název
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
                                    Upravit komerční název
                                </v-col>
                            </v-row>
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col>
                                    <v-text-field v-model="commercialName.name"
                                                  color="error"
                                                  label="Komerční název" />
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="11">
                                    <v-text-field :value="pharmacyString"
                                                  color="error"
                                                  label="Farmaka"
                                                  @focus="showListDetail(activeSubstances, commercialName.pharmacy, 0)" />
                                </v-col>
                                <v-col cols="12" md="1">
                                    <list-detail-button :isDetailActive="isListDetailActive"
                                                        :listType="0"
                                                        :currentListType="listType"
                                                        :valueList="activeSubstances"
                                                        :selectedList="commercialName.pharmacy"
                                                        v-on:showListDetail="showListDetail" />
                                </v-col>
                                <!--<v-col>
                                <v-autocomplete v-model="commercialName.pharmacy"
                                                :items="pharmacyCombo"
                                                item-text="pharmacy.activeSubstanceName"
                                                item-value="pharmacyId"
                                                return-object
                                                color="error"
                                                label="Farmaka"
                                                multiple/>
                            </v-col>-->
                            </v-row>

                        </v-card-text>
                    </v-col>

                    <v-col v-if="isListDetailActive">
                        <list-detail :items="valueList" :listType="listType" v-on:updateModelList="updateModelList" />
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
                           @click="createCommercialName"
                           :disabled="!modified">
                        Založit
                    </v-btn>
                    <v-btn v-if="!isNew"
                           text
                           @click="updateCommercialName"
                           :disabled="!modified">
                        Upravit
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <template>
            <v-dialog v-model="showDialogSave">
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
                </v-card-actions>
            </v-dialog>
        </template>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import CommercialNameApi from "@backend/api/commercialName";
    import CommercialNameDto from "../../../../models/professionInfo/CommercialNameDto";
    import CommercialNamePharmacyDto from "../../../../models/professionInfo/bonds/CommercialNamePharmacyDto";

    import PharmacyDto from "../../../../models/professionInfo/PharmacyDto";
    import PharmacyApi from "@backend/api/pharmacy";

    import ListDetailDto from "../../../../models/professionInfo/helpClass/listDetailDto";
    import ListDetail from "../../../Shared/listDetail.vue";
    import ListDetailButton from "../../../Shared/listDetailButton.vue";

    @Component({
        name: "SymptomQuestionsDetailDialog",
        components: {
            ListDetail, ListDetailButton
        },
    })
    export default class SymptomQuestionsDetailDialog extends Vue {
        loading: boolean = false;
        error: string | null = null;
        detailWidth: number = 600;
        modified: boolean = false;
        showDialogSave: boolean = false;

        @Prop({ default: "" })
        showDialog: boolean = false;

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: 0 })
        commercialNameId: number = -1;

        commercialName: CommercialNameDto = new CommercialNameDto();
        pharmacyList: PharmacyDto[] = [];

        activeSubstances: any[] = [];
        listDetailDto: ListDetailDto[] = [];
        singleItem: ListDetailDto = new ListDetailDto;
        valueList: object[] = null;
        listType: number = -1;
        isListDetailActive: boolean = false;

        get pharmacyString() {
            var pharmacy = "";
            for (var key in this.commercialName.pharmacy) {
                if (!this.pharmacyList.find(x => x.id == this.commercialName.pharmacy[key].pharmacyId))
                    continue;
                pharmacy = pharmacy + this.pharmacyList.find(x => x.id == this.commercialName.pharmacy[key].pharmacyId).activeSubstanceName + ", "
            }
            return pharmacy;
        }

        get pharmacyCombo() {
            return this.pharmacyList.map(x => new CommercialNamePharmacyDto(this.commercialNameId, 0, x))
        }

        @Watch("showDialog", { deep: true })
        onValueChanged() {
            if (!this.isNew) {
                if (this.showDialog) {
                    this.loadCommercialName();
                }

            }
            else {
                this.commercialName = new CommercialNameDto;
                this.commercialName.pharmacy = [];
            }

            this.isListDetailActive = false;
            this.modified = false;
            this.showDialogSave = false;
            this.getPharmacy();
        }

        async loadCommercialName() {
            this.loading = true;

            var result = await CommercialNameApi.getCommercialNameDetail(this.commercialNameId);

            if (result.success) {
                this.commercialName = result.data;
            }

            this.loading = false;
        }

        async getPharmacy() {
            this.loading = true;

            var result = await PharmacyApi.getPharmacy();

            if (result.success) {
                this.pharmacyList = result.data;
                this.activeSubstances = result.data.map(obj => { return { id: obj.id, name: obj.activeSubstanceName } })
            }

            this.loading = false;
        }


        async createCommercialName() {
            this.loading = true;
            this.error = "";

            if (this.modified) {
                // create
                var result = await CommercialNameApi.createCommercialName(this.commercialName);
                if (result.success) {
                    this.commercialName = result.data;
                    this.modified = false;
                    this.$emit("createCommercialName");
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
            } else {
                this.hideDetailDialog();
            }

            this.loading = false;
        }

        async updateCommercialName() {
            this.loading = true;
            this.error = "";

            if (this.modified) {
                // update
                var result = await CommercialNameApi.updateCommercialName(this.commercialName);
                if (result.success) {
                    this.commercialName = result.data;
                    this.modified = false;
                    this.$emit("updateCommercialName");
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
            } else {
                this.hideDetailDialog();
            }

            this.loading = false;
        }

        showListDetail(valueList: object[], selectedList: object[], listType: number) {
            if (!this.isListDetailActive) {
                this.getListDetailList(valueList, selectedList, listType);
                this.valueList = this.listDetailDto;
                this.listType = listType;
                this.detailWidth = 1300;
                this.isListDetailActive = !this.isListDetailActive;
            }
            else {
                this.detailWidth = 600;
                this.isListDetailActive = !this.isListDetailActive;
            }
        }

        getListDetailList(valueList: object[], selectedList: object[], listType: number) {
            this.listDetailDto = [];
            for (var key in valueList) {
                this.singleItem.id = valueList[key].id;
                this.singleItem.name = valueList[key].name;
                switch (listType) {
                    case 0:
                        if (selectedList.length > 0 && selectedList.find(x => x.pharmacyId == valueList[key].id) != null) {
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
        }

        updateModelList(item: ListDetailDto, listType: number) {
            switch (listType) {
                case 0:
                    this.commercialName.pharmacy = this.commercialName.pharmacy.filter(x => x.pharmacyId != item.id);
                    if (item.isSelected) {
                        this.commercialName.pharmacy.push({ commercialNamesId: this.commercialNameId, pharmacyId: item.id, bondStrength: 10, pharmacy: null });
                    }
                    break;
            }
            let temp = Object.assign({}, this.commercialName);
            this.commercialName = temp;
            this.modified = true;
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
            if (this.isNew) {
                this.createCommercialName();
            } else {
                this.updateCommercialName();
            }
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
