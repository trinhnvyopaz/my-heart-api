<template>
    <div>
        <top-bar :tabNames="['Automatické rozhovory']" />
        <v-container>

            <localized-data-table :headers="headers"
                          :options.sync="options"
                          :items="questionaires"
                          :loading="loading"
                          @click:row="rowClick"
                          :page.sync="request.page"
                          :items-per-page="request.pageSize"
                          hide-default-footer
                          :server-items-length="totalCount"
                          @page-count="totalCount = $event">
                <template #top>
                    <v-text-field v-model="request.filter"
                                  placeholder="Vyhledejte rozhovory"
                                  @keyup.enter="search"
                                  class="search">
                        <template v-slot:prepend-inner>
                            <v-icon class="mt-1">mdi-magnify</v-icon>
                        </template>
                        <template v-slot:append>
                            <v-btn @click="search">Vyhledat</v-btn>
                        </template>
                    </v-text-field>
                </template>
                <template v-slot:item.user.firstName="{ item }">
                    <span>{{ item.user.firstName + ' ' + item.user.lastName }}</span>
                </template>
                <template v-slot:item.createdDate="{ item }">
                    <span>{{ new Date(item.createdDate).toLocaleString() }}</span>
                </template>

            </localized-data-table>

                    <v-pagination @input="getQuestionaire" v-model="request.page" :length.sync="pageCount" :total-visible="9"/>

        </v-container>

        <!-- <disease-detail-dialog-deep :isNew="createDialog"
                           :showDialog="showDialogAdd"
                           :diseaseId="diseaseId"
                           v-on:hideDetailDialog="hideDetailDialog"
                           v-on:addDisease="addDisease"
                           v-on:updateDisease="updateDisease">
    </disease-detail-dialog-deep> -->
        <questionaire-detail-dialog @closeDialog="closeDialog" :showDialog="showDialog" :questionaireId="questionaire.id" />
        <!-- <v-snackbar right top v-model="showDeleteSnackbar" color="error">Onemocnění bylo smazáno</v-snackbar>
    <v-snackbar right top v-model="showUpdateSnackbar" color="success">Onemocnění bylo upraveno</v-snackbar>
    <v-snackbar right top v-model="showCreateSnackbar" color="success">Onemocnění bylo vytvořeno</v-snackbar> -->
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import TopBar from "@components/top-bar.vue";
import QuestionaireApi from "@backend/api/questionaire";

import ClientQuestionaireDTO from "@models/questionaire/ClientQuestionaireDTO";
import DataTableRequest from "@models/DataTableRequest";
import QuestionaireDetailDialog from "@components/Manager/Questionaire/questionaire-detail-dialog.vue"
import ColumnOrderParser from "@helpers/ColumnOrderParser"

@Component({
    name: "QuestionaireListPage",
    components: {
        QuestionaireDetailDialog, TopBar
    },
})
export default class QuestionaireListPage extends Vue {
    loading: boolean = false;
    showDialog: boolean = false;

    headers = [
        { text: "Uživatel", value: "user.firstName" },
        { text: "Datum", value: "createdDate" },
    ];

    options: any = {};

    questionaires: ClientQuestionaireDTO[] = [];

    questionaire: ClientQuestionaireDTO = new ClientQuestionaireDTO;

    request: DataTableRequest = { page: 1, pageSize: 10, filter: "", secondFilter: "", columnOrders: [] };
    totalCount: number = 0;
    pageCount: number = 0;

    @Watch("options", {deep: true})
    onPropertyChanged(value) {
        
        this.request.columnOrders = ColumnOrderParser.parseOptions(value);
        this.getQuestionaire();
    }

    
    search() {
        this.request.page = 1;
        this.getQuestionaire();
    }

    async getQuestionaire() {
        this.loading = true;

        var result = await QuestionaireApi.getDataTable(this.request);
        console.log(result)
        if (result.success) {
            this.questionaires = result.data.data;
            this.totalCount = result.data.totalCount;
            this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
        }

        this.loading = false;
    }

    rowClick(event, row) {
        console.log(row.item)
        this.questionaire = row.item
        this.showDialog = true;
    }
    closeDialog(){
        console.log("Closing dialog...")
        this.showDialog = false;
    }
}
</script>

<style lang="scss" scoped></style>
