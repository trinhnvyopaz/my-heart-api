<template>
    <div>
        <v-container>
            <localized-data-table :headers="headers"
                          :items="therapyNews"
                          :loading="loading"
                          @click:row="editDialogFromRow"
                          :page.sync="request.page"
                          :items-per-page="request.pageSize"
                          hide-default-footer
                          @page-count="totalCount = $event">

                <template #top>
                    <v-text-field v-model="request.filter"
                                  placeholder="Vyhledejte novinky"
                                  prepend-inner-icon="mdi-magnify"
                                  @keyup.enter="search"
                                  class="search">
                        <template v-slot:append>
                            <v-btn @click="search">Vyhledat</v-btn>
                        </template>
                    </v-text-field>
                </template>

                <template v-slot:no-data>
                    Nenalezeny žádné záznamy
                </template>

                <template v-slot:item.action="{ item }">
                    <v-tooltip bottom>
                        <template v-slot:activator="{ on, attrs }">
                            <v-btn icon elevation="2"
                                   @click="showDeleteDialog(item, $event)"
                                   v-bind="attrs"
                                   v-on="on">
                                <v-icon color="red">mdi-delete</v-icon>
                            </v-btn>
                        </template>
                        <span>Smazání</span>
                    </v-tooltip>
                </template>
            </localized-data-table>

            <v-row>
                <v-col></v-col>
                <v-col cols="8">
                    <v-pagination @input="getTherapyNews" v-model="request.page" :length.sync="pageCount" :total-visible="9" />
                </v-col>
                <v-col align="right">
                    <v-btn class="custom" @click="addDialog">
                        Přidat
                    </v-btn>
                </v-col>
            </v-row>
        </v-container>

        <template>
            <v-dialog v-model="showDialogDelete"
                      width="500">
                <v-card>
                    <v-card-title primary-title>
                        Smazat Novinku o léčbě
                    </v-card-title>

                    <v-card-text>
                        Opravdu chcete smazat Novinku o léčbě : {{therapy.text}} ?
                    </v-card-text>

                    <v-divider></v-divider>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn class="error"
                               text
                               @click="showDialogDelete = false">
                            Storno
                        </v-btn>
                        <v-btn class="error"
                               text
                               @click="deleteTherapyNews(therapy.id)">
                            Smazat
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </template>

        <therapy-news-detail-dialog-deep :isNew="createDialog" :showDialog="showDialogAdd" :therapyNewsId="therapyNewsId" v-on:hideDetailDialog="hideDetailDialog" v-on:createTherapyNews="createTherapyNews" v-on:updateTherapyNews="updateTherapyNews"></therapy-news-detail-dialog-deep>
        <v-snackbar right top v-model="showDeleteSnackbar" color="error"  >Novinka byla smazaná</v-snackbar>
        <v-snackbar right top v-model="showUpdateSnackbar" color="success">Novinka byla upravena</v-snackbar>
        <v-snackbar right top v-model="showCreateSnackbar" color="success">Novinka byla vytvořena</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import TherapyNewsDto from "@models/professionInfo/TherapyNewsDto";
    import ProfessionInfoApi from "@backend/api/therapyNews";

    import TherapyNewsDetailDialogDeep from "@components/Manager/ProfessionInformation/TherapyNews/therapy-news-detail-dialog-deep.vue";
    import DataTableRequest from "../../../../models/DataTableRequest";

@Component({
    name: "TherapyNewsListPage",
    components: {
        TherapyNewsDetailDialogDeep
    },
})
export default class TherapyNewsListPage extends Vue {
    loading: boolean = false;
    showDialogAdd: boolean = false;
    createDialog: boolean = false;
    showDialogDelete: boolean = false;
    showDeleteSnackbar: boolean = false;
    showUpdateSnackbar: boolean = false;
    showCreateSnackbar: boolean = false;
  headers = [
    { text: "Text", value: "text" },
      { text: "Web Link", value: "webLink" },
      { text: "Akce", value: "action", sortable: false }
  ];

    therapyNewsId: number = -1;
    therapy: TherapyNewsDto = new TherapyNewsDto;
    therapyNews: TherapyNewsDto[] = [];

    request: DataTableRequest = { page: 1, pageSize: 10, filter: "" };
    totalCount: number = 0;
    pageCount: number = 0;

    mounted() {
        this.getTherapyNews();
    }

    search() {
        this.request.page = 1;
        this.getTherapyNews();
    }

    async getTherapyNews() {
        this.loading = true;

        var result = await ProfessionInfoApi.getDataTable(this.request);

        if (result.success) {
            this.therapyNews = result.data.data;
            this.totalCount = result.data.totalCount;
            this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
        }

        this.loading = false;
    }

    addDialog() {
        this.createDialog = true;
        this.showDialogAdd = true;
    }

    editDialogFromRow(event, row) {
        this.editDialog(row.item.id, null);
    }

    editDialog(therapyNewsId: number, event: Event) {
        this.createDialog = false;
        this.therapyNewsId = therapyNewsId;
        this.showDialogAdd = true;

        if (event != null) {
            event.stopPropagation();
        }
    }

    hideDetailDialog() {
        this.showDialogAdd = false;
    }

    async deleteTherapyNews(therapyNewsId: number) {

        var result = await ProfessionInfoApi.deleteTherapyNews(therapyNewsId);

        if (result.success) {
            this.therapyNews = this.therapyNews.filter(x => x.id != therapyNewsId);
            this.showDialogDelete = false;
            this.showDeleteSnackbar = true;
        }
    }

    showDeleteDialog(Therapy: TherapyNewsDto, event: Event) {
        this.therapy = Therapy;
        this.showDialogDelete = true;

        event.stopPropagation();
    }

    createTherapyNews() {
        this.getTherapyNews();
        this.showCreateSnackbar = true;
    }

    updateTherapyNews() {
        this.getTherapyNews();
        this.showUpdateSnackbar = true;
    }

    @Watch("showCreateSnackbar", { deep: true })
    onValueChanged() {
        if (this.showCreateSnackbar)
            this.getTherapyNews();
    }     

}
</script>

<style lang="scss" scoped></style>
