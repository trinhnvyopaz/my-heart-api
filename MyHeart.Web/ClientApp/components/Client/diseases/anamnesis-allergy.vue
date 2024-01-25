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
            </custom-data-table>
        </v-container>
        <simple-edit-dialog v-model="dialogShown"
                            :isNew="isNew"
                            :title="isNew ? 'Přidat alergii' : 'Upravit alergii'"
                            @save="saveAnamnesis"
                            @delete="deleteAnamnesis">
            <v-text-field label="Typ alergie" v-model="editableAnamnesis.isAllergy_Name" />
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
        headers: any[] = [
            { text: 'Alergie', value: 'isAllergy_Name' }
        ];

        editableAnamnesis = {
            id: 0,
            isPersonal_Type: 0,
            isPersonal_Date: new Date(),
            isPersonal_Value: ""
        }

        dialogShown = false;
        isNew = true;

        async getData() {
            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                return await anamnesisApi.getAllergyAnamnesesFromPerson(impersonated.id);
            } else {
                return await anamnesisApi.getAllergyAnamneses();
            }
        }

        async saveAnamnesis() {
            var user: UserDto = AuthStore.getImpersonatedOrCurrentUser();
            this.editableAnamnesis.userId = user.id;
            console.log(this.editableAnamnesis)
            var result = await anamnesisApi.saveAllergyAnamnesis(this.editableAnamnesis);
            if (result.success) {
                this.dialogShown = false;
                this.$refs.dataTable.refresh();
            }
        }

        async deleteAnamnesis() {
            await anamnesisApi.deleteAllergyAnamnesis(this.editableAnamnesis.id);
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

    }
</script>