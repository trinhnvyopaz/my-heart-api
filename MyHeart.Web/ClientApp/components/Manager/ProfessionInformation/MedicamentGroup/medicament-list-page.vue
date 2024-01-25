<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte skupinu"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <medicament-group-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :medicamentGroupId="medicamentGroupId"
                                    @updateMedicamentGroup="updateMedicamentGroup" />

        <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import MedicamentGroupDto from "../../../../models/professionInfo/MedicamentGroupDto";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";

    import MedicamentGroupDetailDialogDeep from "@components/Manager/ProfessionInformation/MedicamentGroup/medicament-group-detail-dialog-deep.vue";

    import CustomDataTable from "@components/Shared/customDataTable.vue";

    @Component({
        name: "MedicamentListPage",
        components: {
            MedicamentGroupDetailDialogDeep, CustomDataTable
        },
    })
    export default class MedicamentListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";
        snackBarColor: string = "";

        headers = [
            { text: "Název", value: "name" },
        ];
        getData = MedicamentGroupApi.getDataTable;

        medicamentGroupId: number = -1;
        medicamentGroup: MedicamentGroupDto = new MedicamentGroupDto;
        medicamentGroups: MedicamentGroupDto[] = [];

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(MedicamentGroupId: number, event: Event) {
            this.isNew = false;
            this.medicamentGroupId = MedicamentGroupId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string, color: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
            this.snackBarColor = color
        }

        updateMedicamentGroup(status) {
        this.$refs.dataTable.refresh();
        this.showSnackbar(status.message, status.color);
    }

    }
</script>

<style lang="scss" scoped></style>
