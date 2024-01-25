<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte onemocnění"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <disease-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :diseaseId="diseaseId"
                                    @updateDisease="updateDisease" />

        <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import ProfessionInfoApi from "@backend/api/disease";
import DiseaseDetailDialogDeep from "@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue";

import DiseaseDto from "../../../../models/professionInfo/DiseaseDto";
import CustomDataTable from "@components/Shared/customDataTable.vue";

@Component({
    name: "DiseaseListPage",
    components: {
        DiseaseDetailDialogDeep, CustomDataTable
    },
})
export default class DiseaseListPage extends Vue {
    dialogShown: boolean = false;
    isNew: boolean = false;
    snackbarShown: boolean = false;
    snackbarText: string = "";
    snackBarColor: string = "";

    headers = [
      { text: "Onemocnění", value: "name" },
      { text: "Prevalence", value: "frequency" },
    ];
    getData = ProfessionInfoApi.getDataTable;

    diseaseId: number = -1;
    disease: DiseaseDto = new DiseaseDto();
    diseases: DiseaseDto[] = [];
        
    addDialog() {
        this.isNew = true;
        this.dialogShown = true;
    }

    editDialogFromRow(event, row) {
        this.editDialog(row.item.id, null);
    }

    editDialog(DiseaseId: number, event: Event) {
        this.isNew = false;
        this.diseaseId = DiseaseId;
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

    updateDisease(status) {
        this.$refs.dataTable.refresh();
        this.showSnackbar(status.message, status.color);
    }
}
</script>