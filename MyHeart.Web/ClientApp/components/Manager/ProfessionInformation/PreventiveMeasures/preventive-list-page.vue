<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte opatření"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <preventive-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :preventiveMeasuresId="preventiveMeasuresId"
                                    @updatePreventiveMeasure="updatePreventiveMeasure"
                                    @deletePreventiveMeasure="deletePreventiveMeasure" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import PreventiveMeasuresDto from "@models/professionInfo/PreventiveMeasuresDto";
import PreventiveMeasureApi from "@backend/api/preventiveMeasures";

import PreventiveDetailDialogDeep from "@components/Manager/ProfessionInformation/PreventiveMeasures/preventive-detail-dialog-deep.vue";

import CustomDataTable from "@components/Shared/customDataTable.vue";

@Component({
    name: "PreventiveListPage",
    components: {
        PreventiveDetailDialogDeep, CustomDataTable
    },
})
export default class PreventiveListPage extends Vue {
    dialogShown: boolean = false;
    isNew: boolean = false;
    snackbarShown: boolean = false;
    snackbarText: string = "";

    headers = [
        { text: "Název", value: "name" }
    ];
    getData = PreventiveMeasureApi.getDataTable;


    preventiveMeasuresId: number = -1;
    preventiveMeasure: PreventiveMeasuresDto = new PreventiveMeasuresDto;
    preventiveMeasures: PreventiveMeasuresDto[] = [];

    addDialog() {
        this.isNew = true;
        this.dialogShown = true;
    }

    editDialogFromRow(event, row) {
        this.editDialog(row.item.id, null);
    }

    editDialog(PreventiveMeasuresId: number, event: Event) {
        this.isNew = false;
        this.preventiveMeasuresId = PreventiveMeasuresId;
        this.dialogShown = true;

        if (event != null) {
            event.stopPropagation();
        }
    }

    showSnackbar(text: string) {
        this.snackbarShown = true;
        this.snackbarText = text;
    }

    updatePreventiveMeasure(message: string) {
        this.$refs.dataTable.refresh();
        this.showSnackbar(message);
    }    
    deletePreventiveMeasure(message: string){
        this.$refs.dataTable.refresh();
        this.showSnackbar(message);
    }
}
</script>

<style lang="scss" scoped></style>
