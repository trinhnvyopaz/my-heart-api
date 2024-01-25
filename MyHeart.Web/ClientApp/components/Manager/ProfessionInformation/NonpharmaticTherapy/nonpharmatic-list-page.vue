<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte léčbu"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <nonpharmacy-detail-dialog-deep :isNew="isNew"
                                        v-model="dialogShown"
                                        :nonpharmacyId="nonpharmacyId"
                                        @updateNonpharmacy="updateNonpharmacy" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import NonpharmacyDto from "@models/professionInfo/NonpharmacyDto";
    import NonpharmacyApi from "@backend/api/nonpharmacy";

    import NonpharmacyDetailDialogDeep from "@components/Manager/ProfessionInformation/NonpharmaticTherapy/nonpharmatic-detail-dialog-deep.vue";

    import CustomDataTable from "@components/Shared/customDataTable.vue";

    @Component({
        name: "NonpharmaticListPage",
        components: {
            NonpharmacyDetailDialogDeep, CustomDataTable
        },
    })
    export default class NonpharmaticListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";
        headers = [
            { text: "Název", value: "name" },
            { text: "Účinost", value: "efficiency" },
            { text: "Délka hospitalizace", value: "hospitalizationLength" },
        ];
        getData = NonpharmacyApi.getDataTable;

        nonpharmacyId: number = -1;
        nonpharmacy: NonpharmacyDto = new NonpharmacyDto;
        nonpharmacys: NonpharmacyDto[] = [];

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(NonpharmacyId: number, event: Event) {
            this.isNew = false;
            this.nonpharmacyId = NonpharmacyId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
        }

        updateNonpharmacy(message: string) {
            this.$refs.dataTable.refresh();
            this.showSnackbar(message);
        }
    }
</script>