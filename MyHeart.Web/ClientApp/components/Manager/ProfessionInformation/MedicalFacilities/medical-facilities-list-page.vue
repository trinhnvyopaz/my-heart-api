<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte zařízení"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <medical-facilities-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :medicalFacilitiesId="medicalFacilityId"
                                    @updateMedicalFacility="updateMedicalFacilities" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import MedicalFacilitiesDto from "../../../../models/professionInfo/MedicalFacilitiesDto";
    import ProfessionInfoApi from "@backend/api/medicalFacilitie";

    import MedicalFacilitiesDetailDialogDeep from "@components/Manager/ProfessionInformation/MedicalFacilities/medical-facilities-detail-dialog-deep.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";

    @Component({
        name: "MedicalFacilitiesListPage",
        components: {
            MedicalFacilitiesDetailDialogDeep, CustomDataTable
        },
    })
    export default class MedicalFacilitiesListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";

        headers = [
            { text: "Název", value: "name" },
            { text: "Charakter", value: "character" },
            { text: "Email", value: "email" },
            { text: "Telefon", value: "telephone" },
        ];
        getData = ProfessionInfoApi.getDataTable;

        medicalFacilities: MedicalFacilitiesDto[] = [];
        medicalFacilityId: number = -1;
        medicalFacility: MedicalFacilitiesDto = new MedicalFacilitiesDto;

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(MedicalFacilitieId: number, event: Event) {
            this.isNew = false;
            this.medicalFacilityId = MedicalFacilitieId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
        }

        updateMedicalFacilities(message: string) {
            this.$refs.dataTable.refresh();
            this.showSnackbar(message);
        }

    }
</script>