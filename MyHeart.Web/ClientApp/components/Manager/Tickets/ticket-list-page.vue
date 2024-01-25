<template>
    <div>
        <top-bar :tabNames="['Dotazy']" />
        <v-container>
            <localized-data-table :headers="headers"
                                  :items="questions"
                                  :loading="loading"
                                  :page.sync="request.page"
                                  :options.sync="options"
                                  :items-per-page="request.pageSize"
                                  hide-default-footer
                                  :server-items-length="totalCount"
                                  @update:options="getQuestionList"
                                  @click:row="editDialogFromRow"
                                  @page-count="totalCount = $event">

                <template #top>
                    <v-text-field v-model="request.filter"
                                    placeholder="Vyhledejte dotaz"
                                    @keyup.enter="search"
                                    class="search">
                        <template v-slot:prepend-inner>
                            <v-icon class="mt-1">mdi-magnify</v-icon>
                        </template>
                        <template v-slot:append>
                            <v-btn @click="search">Vyhledat</v-btn>
                        </template>
                    </v-text-field>
                    <v-row class="checkboxRow">
                        <v-checkbox v-model="request.groups" label="Otevřená" value="0" @change="search" />
                        <v-checkbox v-model="request.groups" label="Čekání pacient" value="1" @change="search" />
                        <v-checkbox v-model="request.groups" label="Čekání lékař" value="2" @change="search" />
                        <v-checkbox v-model="request.groups" label="Archivovaná" value="3" @change="search" />
                    </v-row>
                    <!--hnus<v-divider></v-divider>-->
                </template>

                        <template v-slot:item.status="{ item }">
                            <span>{{  getQuestionStatus(item.status)  }}</span>
                        </template>

            </localized-data-table>

                    <v-pagination 
                                  v-model="request.page"
                                  :length.sync="pageCount"
                                  :total-visible="9"
                                  color="error" />
        </v-container>
        <v-snackbar right top v-model="showSnackbar" :color="snackBarColor">{{ snackBarMessage }}</v-snackbar>
    </div>

</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import TopBar from "@components/top-bar.vue";
    import QuestionApi from "@backend/api/question";

    import GroupedDataTableRequest from "../../../models/GroupedDataTableRequest";
    import QuestionDto from "../../../models/question/QuestionDto";    
    import EventBus from "@models/EventBus";
    import ColumnOrder from "@models/ColumnOrder";

@Component({
    name: "TicketListPage",
    components: {
        TopBar
    },
})

export default class TicketListPage extends Vue {
    loading: boolean = false;
    showDialogDelete: boolean = false;
    showDialogAdd: boolean = false;
    showSnackbar: boolean = false;
    snackBarColor: string = "";
    snackBarMessage: string = "";
    options: any = {
      page: 1,
      itemsPerPage: 10,
      sortBy: [],
      sortDesc: [],
    };
    headers = [
        { text: "Id", value: "id" },
        { text: "Předmět", value: "subject" },
        { text: "Jméno zadavatele", value: "name" },
        { text: "Datum vytvoření", value: "creationDate" },
        { text: "Stav", value: "status" },
        { text: "Akce", value: "action", sortable: false }
    ];

    questions: QuestionDto[] = [];

    request: GroupedDataTableRequest = { page: 1, pageSize: 10, filter: "", groups: ["0","1","2","3"], columnOrders: []};
    totalCount: number = 0;
    pageCount: number = 0;

    search() {
        this.request.page = 1;
        this.getQuestionList();
        console.log(this.request);
    }

    mounted() {
        // this.getQuestionList();

        if (this.$route.query.result) {
            this.questionArchived(this.$route.query.snackbarColor, this.$route.query.snackbarMessage)
        }
    }

    questionArchived(color: string, text: string){
        this.snackBarMessage = text;
        this.snackBarColor = color;
        this.showSnackbar = true;
        
        this.$router.replace({ 
            path: this.$route.path, 
            query: {} 
        });
    }

    editDialogFromRow(event, row) {
        this.goto(row.item.id);
    }

    goto(id) {
        this.$router.push("/client/question/" + id);
    }

    getQuestionStatus(num): string {
        switch (num) {
            case 0:
                return "Otevřená otázka";
            case 1:
                return "Čeká na odpověď pacienta";
            case 2:
                return "Čeká na odpověď lékaře";
            case 3:
                return "Archivovaná otázka";
        }
    }

    async getQuestionList() {
        this.loading = true;

        console.log(this.options)

        for (let index = 0; index < this.options.sortBy.length; index++) {
            
            
            let sortBy = this.options.sortBy[index];
            if(sortBy == "name"){
                sortBy = "user.firstName"
            }
            const sortDirection = this.options.sortDesc[index];            

            this.request.columnOrders =[{
                propertyPath: sortBy,
                direction: sortDirection
            }]
        }

        console.log(this.request)

        var result = await QuestionApi.getDataTable(this.request);

        if (result.success) {
            this.questions = result.data.data;
            console.log(this.questions)
            this.totalCount = result.data.totalCount;
            this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
        }

        this.loading = false;
    }

    ticketFromRow(event, row) {
        this.$router.push("/question/" + row.item.id);
    }

    noPropagation(event: Event) {
        event.stopPropagation();
    }
}
</script>

<style scoped>
    #app-root .checkboxRow .v-input {
        margin: 0px 15px !important;
    }
    .v-input__prepend-inner{
        align-self: center!important;
    }

</style>
