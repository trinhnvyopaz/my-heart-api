<template>
    <div>
        <v-container>
            <localized-data-table :headers="headers" :getData="getData" @click:row="rowClicked" :options.sync="options"
                :items="questionaires" :loading="loading" :page.sync="request.page" :items-per-page="request.pageSize"
                hide-default-footer :server-items-length="totalCount" @page-count="totalCount = $event">
                <template #top>
                    <v-text-field v-model="request.filter" placeholder="Vyhledejte rozhovory" @keyup.enter="search"
                        class="search">
                        <template v-slot:prepend-inner>
                            <v-icon class="mt-1">mdi-magnify</v-icon>
                        </template>
                        <template v-slot:append>
                            <v-btn @click="search">Vyhledat</v-btn>
                        </template>
                    </v-text-field>
                </template>
                <template #item.title="{ item }">
                    {{ item.userReport.title }}
                </template>
                <template #item.fileName="{ item }">
                    {{ item.userReportFile.name }}
                </template>
                <template #item.description="{ item }">
                    {{ item.userReport.description }}
                </template>
                <template #item.lastUpdateDate="{ item }">
                    {{ formatting.formatDate(item.lastUpdateDate, "DD/MM/YYYY") }}
                </template>
                <template #item.reportType="{ item }">
                    {{ reportTypes[item.userReport.reportType].text }}
                </template>
            </localized-data-table>
            <v-pagination @input="getData" v-model="request.page" :length.sync="pageCount" :total-visible="9" />
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import MedicalReportsApi from '@backend/api/medicalReport';
import SimpleDialog from "@components/Shared/simpleDialog.vue";
import ClientQuestionaireDTO from "@models/questionaire/ClientQuestionaireDTO";
import ColumnOrderParser from "@helpers/ColumnOrderParser"
import DataTableRequest from "../../../models/DataTableRequest";
import Formatting from '@utils/formatting';

@Component({
    name: "ReportsOverview",
    components: {
        SimpleDialog
    },
})
export default class ReportsOverview extends Vue {
    isNew = true;
    dialogShown: boolean = false;
    loading: boolean = false;
    options: any = {};
    snackbarShown = false;
    snackbarText = "";
    snackBarColor = "";
    request: DataTableRequest = { page: 1, pageSize: 10, filter: "", secondFilter: "", columnOrders: [] };
    questionaires: ClientQuestionaireDTO[] = [];
    totalCount: number = 0;
    pageCount: number = 0;
    formatting = Formatting;
    headers: any[] = [
        { text: 'Název', value: 'title', sortable: false, width: '20%' },
        { text: 'název souboru', value: 'fileName', sortable: false, width: '20%' },
        { text: 'Popis', value: 'description', sortable: false, width: '30%' },
        { text: 'Typ', value: 'reportType', sortable: false, width: '20%' },
        { text: 'Datum nahrání', value: 'lastUpdateDate', sortable: false, width: '10%' },
    ];

    mounted() {
        this.getData();
    }

    onPropertyChanged(value) {
        this.request.columnOrders = ColumnOrderParser.parseOptions(value);
        this.getData();
    }

    search() {
        this.request.page = 1;
        this.getData();
    }

    async getData() {
        this.loading = true;
        if (this.request.filter) {
            this.request.filter = this.request.filter.trim();
        }
        var result = await MedicalReportsApi.getAllMedicalReports(this.request);
        if (result.success) {
            this.questionaires = result.data.data;
            this.totalCount = result.data.totalCount;
            this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
        }
        this.loading = false;
    }

    report = {
        userId: 0,
        title: '',
        description: '',
        files: [],
        reportType: 0
    };
    file: any = null;

    reportTypes: any[] = [
        { text: 'Ambulance Report', value: 0 },
        { text: 'Hospitalizace', value: 1 },
        { text: 'Laboratorní výsledky', value: 2 }
    ];

    reportFromRow(id, event) {
        this.$router.push("/admin/report/detail/" + id);
    }

    rowClicked(item, args) {
        console.warn(item);
        console.warn(args);
        this.reportFromRow(item.id, args);
    }
}
</script>


<style scoped>
</style>
