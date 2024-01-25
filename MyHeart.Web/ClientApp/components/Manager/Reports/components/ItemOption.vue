<template>
    <div class="d-flex item-option">
        <template v-if="data.type == 7">
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected"
                        class="my-auto option-item-c" style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <!-- <v-select placeholder="Vyberte typ dat" :items="tables" item-text="label" item-value="value"
                        variant="outlined" v-model="data.key"></v-select> -->
                    <v-autocomplete placeholder="Vyberte typ dat" :items="otherData" item-text="name" item-value="id"
                        variant="outlined" v-model="data.diseasePharmacyId" @input="handleSelectChange">
                        <template v-slot:append-item>
                            <div v-intersect="loadMore" />
                        </template>
                    </v-autocomplete>
                    <v-text-field v-if="data.key == 16 || data.key == 18" placeholder="datum" variant="outlined"
                        v-model="data.dateMined"></v-text-field>
                    <v-text-field v-if="data.key == 17" placeholder="dávkování" variant="outlined"
                        v-model="data.doseMined"></v-text-field>
                    <v-textarea v-if="data.key != 17" placeholder="popis" variant="outlined" v-model="data.noteMined"
                        rows="2"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.key == 4">
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected" class="my-auto"
                        style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <v-text-field label="" type="text" v-model="data.itemMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>
                </v-col>
                <v-col cols="12" class="pb-0">
                    <div
                         :style="`color: ${data.diseasePharmacyId == 3 ? 'green' : 'red'};`">
                        {{ data.noteMined }}
                    </div>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.noteMined">
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected" class="my-auto"
                        style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <v-text-field label="" type="text" v-model="data.itemMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>
                    <v-text-field label="" type="text" v-model="data.dateMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>

                </v-col>
                <v-col cols="9" offset="3" style="padding-top: 0;">
                    <v-textarea label="" v-model="data.noteMined" single-line placeholder="Popis" rows="2"
                        style="margin-top: 0 !important;padding-top: 0;"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.key == 14 && data.type == 5">
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected" class="my-auto"
                        style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <v-text-field label="" v-model="data.itemMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>
                    <v-text-field label="" v-model="data.doseMined" single-line
                        style="margin-top: 0 !important;  margin-left: 8px;"></v-text-field>
                    <v-text-field label="" v-model="data.doseUnitMined" single-line
                        style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                    <v-text-field label="" v-model="data.frequencyMined" single-line
                        style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.type == 6 && (data.key == 16 || data.key == 18)">
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected" single-line
                        class="my-auto" style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <v-text-field label="" type="text" v-model="data.itemMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>
                    <v-text-field label="" type="text" v-model="data.dateMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>

                </v-col>
                <v-col cols="9" offset="3" style="padding-top: 0;">
                    <v-textarea label="" v-model="data.noteMined" single-line placeholder="Popis" rows="2"
                        style="margin-top: 0 !important;padding-top: 0;"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else>
            <v-row>
                <v-col cols="3">
                    <v-checkbox :label="headers[keyNames[data.key]]" color="green" v-model="data.isSelected" class="my-auto"
                        style="width: 160px;">
                    </v-checkbox>
                </v-col>
                <v-col cols="9">
                    <template v-if="data.key == 17 && data.type == 6">
                        <v-text-field v-model="data.itemMined"
                            style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                        <v-text-field v-model="data.doseMined"
                            style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                        <v-text-field v-model="data.doseUnitMined"
                            style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                        <v-text-field v-model="data.frequencyMined"
                            style="margin-top: 0 !important; margin-left: 8px;"></v-text-field>
                    </template>
                    <v-text-field v-else label="" :type="checkDataType(data).typeInput" v-model="data.itemMined" single-line
                        style="margin-top: 0 !important;"></v-text-field>
                </v-col>
            </v-row>

        </template>
        <v-icon v-if="data.index" class="mt-5 ml-3 cursor-pointer me-2" size="small" @click="editForm(key, index)">
            mdi-pencil </v-icon>
        <v-icon v-if="data.index" class="mt-5 ml-3 cursor-pointer" @click="deleteForm(key, index)"
            color="red">mdi-delete</v-icon>
    </div>
</template>
<script lang="ts">
import SelectionApi from "@backend/api/selection";
import { headers, keyNames } from '@constants/type.ts';
import { Component, Prop, Vue, Watch } from "vue-property-decorator";

const defaultParams = {
    filter: "",
    page: 1,
    pageSize: 10,
};
export type optionType = {
    key: number,
    keyName: string,
    value: string,
    dateMined: string,
    noteMined: string,
    isSelected: boolean,
    type: number,
    index: number,
    itemId: number,
    table: string,
    diseasePharmacyId: number,
    itemMined: string,
    doseMined: string,
    doseUnitMined: string,
    frequencyMined: string,
}
@Component({
    name: "ItemOption",
})
export default class ItemOption extends Vue {
    @Prop({ default: () => ({}) }) data!: optionType;
    @Prop({ default: () => ({}) }) deleteForm: (key: string, index: number) => void;
    @Prop({ default: () => ({}) }) editForm: (key: string, index: number) => void;
    @Prop({
        default: () => ([
            { key: 3, name: 'Pracoviště' },
            { key: 4, name: 'Jméno' },
            { key: 5, name: 'Příjmení' },
            { key: 6, name: 'Rodné číslo' },
            { key: 7, name: 'Adresa' }
        ])
    }) personalDatasType: { key: number, name: string }[];
    @Prop({
        default: () => ([
            { key: 8, name: 'Výška' },
            { key: 9, name: 'Váha' },
            { key: 10, name: 'Tlak krve' },
            { key: 11, name: 'Tepová frekvence' },
            { key: 12, name: 'LDL' }
        ])
    }) healthConditionsType: { key: number, name: string }[];
    headers = headers;
    tables = [{ label: "Diseases", value: "16" },
    { label: "Pharmaceuticals", value: "17" },
    { label: "Non-pharmacological treatment", value: "18" },
    { label: "Preventive measures", value: "20" }]
    otherData: any[] = [];
    rulesRequired = [(v: any) => !!v || "Povinné"];
    params: any = { ...defaultParams };
    keyNames = keyNames;
    isLoading: boolean = false;
    selectedItem: null;
    @Watch("data.key", { immediate: true })
    async handleChangeTable(value) {
        if (this.data.type == 7) {
            this.params = { ...defaultParams };
            await this.fetchDataTable(value);
        }
    }

    // @Watch("data", { deep: true, immediate: true })
    // async handleChangeData(value) {
    //     if (value.type == 7) {
    //         await this.fetchDataTable(value.key, false, true);
    //     }
    // }

    async handleSearch(value: string) {
        this.params.filter = value;
        this.params.page = 1;
        await this.fetchDataTable(this.data.table);
    }
    async fetchDataTable(value, isLoadMore = false, isShow = false) {
        this.isLoading = true;
        try {
            let result = { data: { data: [] } };
            if (isShow) {
                this.params.page = 1;
                this.params.ids = this.data.diseasePharmacyId;
            }
            if (value == "16") {
                result = await SelectionApi.getDiseases(this.params);
            }
            if (value == "17") {
                result = await SelectionApi.getPharmaceuticals(this.params);
            }
            if (value == "18") {
                result = await SelectionApi.getNonpharmaticTherapies(this.params);
            }

            if (value == "20") {
                result = await SelectionApi.getPreventiveMeasures(this.params);
            }

            this.otherData = isLoadMore ? [...this.otherData, ...result.data.data] : result.data.data;
        } catch (error) {
            console.log(error);
        } finally {
            this.isLoading = false
        }

    }
    async loadMore(entries, observer, isIntersecting) {
        if (isIntersecting) {
            this.params.page++;
            await this.fetchDataTable(this.data.key, true);
        }
    }
    checkDataType(input) {
        if (!input.typeInput) {
            if (/\d{4}\-\d{2}\-\d{2}/.test(input.value))
                input.typeInput = "date";
            else input.typeInput = "text";
        }
        return input;
    }
    async handleSelectChange() {
        this.data.itemMined = this.otherData.find(x => x.id == this.data.diseasePharmacyId)?.name
    }
}
</script>
<style scoped>
.item-option {
    padding: 0 20px;
    max-width: 100%;
    overflow-x: hidden;
}

.item-option .col-9 {
    padding-top: 0;
}

/* .item-option .v-input--checkbox .v-input__slot {
    @media (max-width: 1023px) {
        flex-wrap: wrap;
    }
} */
</style>
