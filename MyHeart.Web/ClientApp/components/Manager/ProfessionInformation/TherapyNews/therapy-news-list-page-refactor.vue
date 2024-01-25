<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte novinku"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <therapy-news-detail-dialog-refactor :isNew="isNew"
                                             v-model="dialogShown"
                                             :therapyNewsId="therapyNewsId"
                                             @updateTherapyNews="updateTherapyNews"/>
        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import TherapyNewsDto from "@models/professionInfo/TherapyNewsDto";
    import ProfessionInfoApi from "@backend/api/therapyNews";

    import TherapyNewsDetailDialogDeep from "@components/Manager/ProfessionInformation/TherapyNews/therapy-news-detail-dialog-deep.vue";
    import TherapyNewsDetailDialogRefactor from "@components/Manager/ProfessionInformation/TherapyNews/therapy-news-detail-dialog-refactor.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";

@Component({
    name: "TherapyNewsListPageRefactor",
    components: {
        TherapyNewsDetailDialogDeep, TherapyNewsDetailDialogRefactor, CustomDataTable
    },
})
export default class TherapyNewsListPageRefactor extends Vue {
    dialogShown: boolean = false;
    isNew: boolean = false;
    snackbarShown: boolean = false;
    snackbarText: string = "";
    headers = [
        { text: "Text", value: "text" },
        { text: "Web Link", value: "webLink" }
    ];
    getData = ProfessionInfoApi.getDataTable;
    therapyNewsId: number = -1;
    therapy: TherapyNewsDto = new TherapyNewsDto;
    therapyNews: TherapyNewsDto[] = [];

    addDialog() {
        this.isNew = true;
        this.dialogShown = true;
    }

    editDialogFromRow(event, row) {
        this.editDialog(row.item.id, null);
    }

    editDialog(therapyNewsId: number, event: Event) {
        this.isNew = false;
        this.therapyNewsId = therapyNewsId;
        this.dialogShown = true;

        if (event != null) {
            event.stopPropagation();
        }
    }

    showSnackbar(text: string) {
        this.snackbarShown = true;
        this.snackbarText = text;
    }

    updateTherapyNews(message: string) {
        this.$refs.dataTable.refresh();
        this.showSnackbar(message);
    } 

}
</script>