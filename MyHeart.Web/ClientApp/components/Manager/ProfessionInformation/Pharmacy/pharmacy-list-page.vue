<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte farmaka"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <pharmacy-detail-dialog-deep :isNew="isNew"
                                     v-model="dialogShown"
                                     :pharmacyId="pharmacyId"
                                     @updatePharmacy="updatePharmacy" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import PharmacyDto from "@models/professionInfo/PharmacyDto";
    import PharmacyApi from "@backend/api/pharmacy";

    import PharmacyDetailDialogDeep from "@components/Manager/ProfessionInformation/Pharmacy/pharmacy-detail-dialog-deep.vue";

    import CustomDataTable from "@components/Shared/customDataTable.vue";

    @Component({
        name: "PharmacyListPage",
        components: {
            PharmacyDetailDialogDeep, CustomDataTable
        },
    })
    export default class PharmacyListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";
        headers = [
            { text: 'Název', value: 'nameReg' },
            { text: 'ATC', value: 'atcWho' },
            { text: 'Doplněk', value: 'supplement' }
        ];
        getData = PharmacyApi.getDataTable;

        pharmacys: PharmacyDto[] = [];
        pharmacyId: number = -1;
        pharmacy: PharmacyDto = new PharmacyDto;

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(PharmacyId: number, event: Event) {
            this.isNew = false;
            this.pharmacyId = PharmacyId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
        }

        updatePharmacy(message: string) {
            this.$refs.dataTable.refresh();
            this.showSnackbar(message);
        }

    }
</script>

<style lang="scss" scoped></style>
