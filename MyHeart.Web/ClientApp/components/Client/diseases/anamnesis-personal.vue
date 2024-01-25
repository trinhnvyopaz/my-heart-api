<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder=""
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable"
                               :serverPaginated="false">
                <template #item.isPersonal_Type="{item}">
                    {{getPeronalTypeText(item.isPersonal_Type)}}
                </template>
                <template #item.isPersonal_Date="{item}">
                    {{item.isPersonal_Date.toLocaleString()}}
                </template>
                <template #item.isPersonal_Value="{item}">
                    {{item.isPersonal_Value + " " + suffix(item)}}
                </template>
            </custom-data-table>
        </v-container>
        <simple-edit-dialog v-model="dialogShown"
                            :isNew="isNew"
                            :title="isNew ? 'Přidat měření' : 'Upravit měření'"
                            @save="saveAnamnesis"
                            @delete="deleteAnamnesis"
                            @discard="discard">
            <v-select :items="anamnesisTypes" label="Typ" v-model="editableAnamnesis.isPersonal_Type" />
            <v-text-field label="Hodnota" v-model="editableAnamnesis.isPersonal_Value" :suffix="suffix(editableAnamnesis)" :rules="value_rules(editableAnamnesis)"></v-text-field>
            <date-text-field v-model="editableAnamnesis.isPersonal_Date"/>
            <time-text-field v-model="editableAnamnesis.isPersonal_Date"/>
        </simple-edit-dialog>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis';
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";
    import SimpleEditDialog from "@components/Shared/simpleEditDialog.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";
    import DateTextField from "@components/Shared/dateTextField.vue";
    import TimeTextField from "@components/Shared/timeTextField.vue";

    @Component({
        name: "AnamnesisPersonal",
        components: {
            CustomDataTable, SimpleEditDialog, DateTextField, TimeTextField
        },
    })
    export default class AnamnesisPersonal extends Vue {

        loadingAnamnesis: boolean = false;

        anamnesisTypes: any[] = [
            { value: 0, text: 'Tlak' },
            { value: 1, text: 'Cholesterol' },
            { value: 2, text: 'Laboratorní výsledky' },
            { value: 3, text: 'Výška' },
            { value: 4, text: 'Hmotnost' },
            { value: 5, text: 'Tep' },
            { value: 6, text: 'Odběr krve' },
        ];

        headers: any[] = [
            { text: 'Typ', value: 'isPersonal_Type' },
            { text: 'Datum & čas', value: 'isPersonal_Date' },
            { text: 'Hodnota', value: 'isPersonal_Value' },
        ];
        searchString: string = '';

        editableAnamnesis = {
            id: 0,
            isPersonal_Type: 0,
            isPersonal_Date: new Date(),
            isPersonal_Value: ""
        }

        dialogShown = false;
        isNew = true;
        error = "";


        date: Date = new Date();


        value_rules(item) {
            let isNumeric = v => v && !isNaN(v);
            let numeric_rule = v => isNumeric(v) || "Hodnota musí být číslo.";
            let range_rule = (lo, hi) => {
                return (v => (lo <= v && v <= hi) || "Hodnota musí být mezi " + lo + " a " + hi + ".");
            };
            let isPressureFormat = v => {
                let parts = v.split("/");
                return parts.length == 2 && isNumeric(parts[0]) && isNumeric(parts[1])
            }
            let isPressureRange = (v, i, lo, hi) => {
                let parts = v.split("/");
                return lo <= parts[i] && parts[i] <= hi;
            }
            return [
                [
                    v => isPressureFormat(v) || "Hodnota musí být dvě čísla oddělená lomítkem.",
                    v => isPressureRange(v, 0, 30, 300) || "Systolický tlak musí být mezi 30 a 300.",
                    v => isPressureRange(v, 1, 30, 300) || "Diastolický tlak musí být mezi 30 a 300.",
                ],
                [numeric_rule, range_rule(0, 1000)],
                [],
                [numeric_rule, range_rule(50, 250)],
                [numeric_rule, range_rule(30, 250)],
                [numeric_rule, range_rule(50, 200)],
                []
            ][item.isPersonal_Type];
        }

        suffix(item) {
            return ["mm Hg", "mmol/l", "", "cm", "kg", "/min", ""][item.isPersonal_Type];
        }

        async getData() {
            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                return await anamnesisApi.getPersonalAnamnesesForPerson(impersonated.id);
            } else {
                return await anamnesisApi.getPersonalAnamneses();
            }
        }

        async saveAnamnesis() {
            var user: UserDto = AuthStore.getImpersonatedOrCurrentUser();
            this.editableAnamnesis.userId = user.id;
            var result;
            if(this.isNew){
                result = await anamnesisApi.savePersonalAnamnesis(this.editableAnamnesis);
            }else{
                result = await anamnesisApi.updatePersonalAnamnesis(this.editableAnamnesis.id, this.editableAnamnesis);
            }
             
            if (result.success) {
                this.dialogShown = false;
                this.$refs.dataTable.refresh();
            }
        }

        async deleteAnamnesis() {
            await anamnesisApi.deletePersonalAnamnesis(this.editableAnamnesis.id);
            this.$refs.dataTable.refresh();
        }


        editDialogFromRow(event, row) {            
            this.editableAnamnesis = Object.assign({}, row.item);
            this.isNew = false;
            this.dialogShown = true;
        }

        addDialog() {
            this.editableAnamnesis = Object.assign({}, {
                id: 0,
                isPersonal_Type: 0,
                isPersonal_Date: new Date(),
                isPersonal_Value: ""
            });
            
            this.isNew = true;
            this.dialogShown = true;
        }
        getPeronalTypeText(type){
            var anamnesisType =this.anamnesisTypes[type];
            if(anamnesisType == null)
                return ''

            return this.anamnesisTypes[type]?.text
        }
    }
</script>