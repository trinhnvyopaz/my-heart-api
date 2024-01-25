<template>
    <div class="d-flex">
        <template v-if="data.type == 7">
            <v-row>
                <v-col cols="12" class="p-0">
                    <v-select :disabled="loadingData" :loading="loadingData" label="Vyberte typ dat" :items="tables" item-text="label" item-value="value"
                        variant="outlined" v-model="data.key"></v-select>
                </v-col>
                <v-col cols="12" class="p-0">
                    <v-autocomplete label="Vyberte typ dat" :items.sync="otherData" item-text="name" item-value="id"
                        variant="outlined" v-model="data.diseasePharmacyId" @input="handleSelectChange">
                        <template v-slot:append-item>
                            <div v-intersect="loadMore" />
                        </template>
                    </v-autocomplete>
                </v-col>
                <v-col v-if="data.key == 16 || data.key == 18" cols="12" class="p-0">
                    <v-text-field label="datum" variant="outlined" v-model="data.dateMined"></v-text-field>
                </v-col>
                <v-col v-if="data.key == 17" cols="12" class="p-0">
                    <v-text-field label="dávkování" variant="outlined" v-model="data.doseMined"></v-text-field>
                </v-col>
                <v-col v-if="data.key != 17" cols="12" class="p-0">
                    <v-textarea label="popis" variant="outlined" v-model="data.noteMined" rows="3"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.key == 4">
            <v-row>
                <v-col cols="12">
                    <v-text-field label="hodnota" type="text" v-model="data.itemMined" placeholder="hodnota"></v-text-field>
                </v-col>
                <v-col v-if="data.noteMined" cols="12" class="pb-0">
                    <div v-if="data.diseasePharmacyId == 3"
                        :style="`color: ${data.diseasePharmacyId == 3 ? 'green' : 'red'};`">
                        {{ data.noteMined }}
                    </div>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.noteMined">
            <v-row>
                <v-col cols="12">
                    <v-text-field label="hodnota" type="text" v-model="data.itemMined" placeholder="hodnota"></v-text-field>
                    <v-text-field label="datum" type="text" v-model="data.dateMined" placeholder="datum"></v-text-field>
                </v-col>
                <v-col cols="12" style="padding-top: 0;">
                    <v-textarea label="" v-model="data.noteMined" placeholder="Popis" rows="2"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else-if="data.key == 14 && data.type == 5">
            <v-text-field label="název" v-model="data.itemMined" placeholder="název"></v-text-field>
            <v-text-field label="síla" v-model="data.doseMined" placeholder="síla" style="margin-left: 4px;"></v-text-field>
            <v-text-field label="jednotka" v-model="data.doseUnitMined" placeholder="jednotka"
                style="margin-left: 4px;"></v-text-field>
            <v-text-field label="dávkování" v-model="data.frequencyMined" placeholder="dávkování"
                style="margin-left: 4px;"></v-text-field>
        </template>
        <template
            v-else-if="(data.type == 6 && (data.key == 16 || data.key == 18)) || (data.type == 5 && (data.key == 15 || data.key == 13))">
            <v-row>
                <v-col cols="12">
                    <v-text-field label="hodnota" type="text" v-model="data.itemMined" placeholder="hodnota"></v-text-field>
                    <v-text-field label="datum" type="text" v-model="data.dateMined" placeholder="datum"></v-text-field>
                </v-col>
                <v-col cols="12" style="padding-top: 0;">
                    <v-textarea label="Popis" v-model="data.noteMined" placeholder="Popis" rows="2"
                        style="margin-top: 0 !important;padding-top: 0;"></v-textarea>
                </v-col>
            </v-row>
        </template>
        <template v-else>
            <template v-if="data.key == 17 && data.type == 6">
                <v-text-field label="název" v-model="data.itemMined" placeholder="název"></v-text-field>
                <v-text-field label="síla" v-model="data.doseMined" placeholder="síla"
                    style="margin-left: 4px;"></v-text-field>
                <v-text-field label="jednotka" v-model="data.doseUnitMined" placeholder="jednotka"
                    style="margin-left: 4px;"></v-text-field>
                <v-text-field label="dávkování" v-model="data.frequencyMined" placeholder="dávkování"
                    style="margin-left: 4px;"></v-text-field>
            </template>
            <v-text-field v-else label="" :type="checkDataType(data).typeInput" v-model="data.itemMined" single-line
                style="margin-top: 0 !important;"></v-text-field>
        </template>
        <v-icon v-if="data.index" class="mt-5 ml-3 cursor-pointer me-2" size="small" @click="editForm(key, index)">
            mdi-pencil </v-icon>
        <v-icon v-if="data.index" class="mt-5 ml-3 cursor-pointer" @click="deleteForm(key, index)"
            color="red">mdi-delete</v-icon>
    </div>
</template>
<script lang="ts">
import SelectionApi from "@backend/api/selection";
import { headers } from '@constants/type.ts';
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
const defaultParams = {
    filter: "",
    page: 1,
    pageSize: 10,
};
@Component({
    name: "ItemOptionDialog",
})
export default class ItemOption extends Vue {
    @Prop({ default: () => ({}) }) data: {
        key: number,
        keyName: string,
        value: string,
        date: string,
        description: string,
        isSelected: boolean,
        type: number,
        name: string,
        strength: string,
        unit: string,
        dosing: string,
        index: number,
        diseasePharmacyId: number,
        itemId: number,
        table: string,
        dateMined: string,
        noteMined: string,
        itemMined: string,
        doseMined: string,
        doseUnitMined: string,
        frequencyMined: string,
    };
    @Prop() editForm: (key: number, index: number) => void
    @Prop() deleteForm: (key: number, index: number) => void
    headers = headers;
    rulesRequired = [(v: any) => !!v || "Povinné"];
    tables = [{ label: "Diseases", value: "16" },
    { label: "Pharmaceuticals", value: "17" },
    { label: "Non-pharmacological treatment", value: "18" },
    // { label: "Preventive measures", value: "20" }
]
    otherData: any[] = [];
    params: any = { ...defaultParams };
    loadingData = false;
    @Watch("data.key")
    async handleChangeTable(value) {
        if (this.data.type == 7) {
            this.otherData = [];
            this.params = { ...defaultParams };
            await this.fetchDataTable(value);

        }
    }

    async fetchDataTable(value, isLoadMore = false, isAdd = false) {
        try {
            this.loadingData = true;
            let result = { data: { data: [] } };
            if (isAdd) {
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
            this.otherData = isLoadMore ? [...this.otherData, ...result.data.data] : result.data.data;

        } catch (error) {

        } finally {
            this.loadingData = false;
        }

        // if (value == "20") {
        //     result = await SelectionApi.getPreventiveMeasures(this.params);
        // }

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
.p-0 {
    padding: 0 !important;
}
</style>
