<template>
    <div>
        <top-bar :tabNames="['Novinky v léčbě']" />
        <v-container>
            <v-row>
                <v-col>
                    <client-therapy-news-subscription-settings  />
                </v-col>
            </v-row>

            <v-list-item-group>
                <v-row  v-for="(item, index) in therapyNews" :key="index" @click="goToNewsDetail(item)">
                    <v-col>
                        <v-list class="rounded-xl news-row" xs12 two-line>
                            <v-list-item-content>
                                <v-list-item-title>
                                    <div class="d-flex">
                                        <span class="news-title mr-auto">{{item.text}}</span>
                                        <span class="news-date">{{datetimeToDateString(item.lastUpdateDate)}}</span>
                                    </div>
                                </v-list-item-title>
                                <v-list-item-subtitle class="news-description news-description-small" v-html="item.description"/>
                            </v-list-item-content>
                        </v-list>
                    </v-col>
                </v-row>
            </v-list-item-group>
        </v-container>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import TherapyNewsApi from "@backend/api/therapyNews";
    import TopBar from "@components/top-bar.vue";
    import ClientTherapyNewsSubscriptionSettings from "./client-therapy-news-subscription-settings";
    import DataTableRequest from "../../models/DataTableRequest";
    import ColumnOrderParser from "@helpers/ColumnOrderParser"

    @Component({
        name: "ClientTherapyNews",
        components: {
            ClientTherapyNewsSubscriptionSettings, TopBar
        },
    })

    export default class ClientTherapyNews extends Vue {
        loading: boolean = false;
        therapyNews: any[] = [];
        showSettings: boolean = false;
        maxPageSize: number = 2147483647;
        request: DataTableRequest = { page: 1, pageSize: this.maxPageSize, filter: "", secondFilter: null, columnOrders: ColumnOrderParser.parseOptions({
            sortBy: ["createDate"],
            sortDesc: [true]
        })};

        mounted() {
            this.loadNews();
        }

        goToNewsDetail(item){
            console.log(item)
            this.$router.push("/client/news/detail/" + item.id);
        }

        async loadNews() {
            this.loading = true;

            var result = await TherapyNewsApi.getDataTable(this.request);
            console.warn(result);
            if(result.success){
                this.therapyNews = result.data.data;

            }
            this.loading = false;
        }


        datetimeToDateString(datetime) {
            var date = new Date(datetime);
            var d = date.getDate();
            var dd = d < 10 ? '0' + d : '' + d;
            var m = date.getMonth() + 1;
            var mm = m < 10 ? '0' + m : '' + m;
            var yyyy = date.getFullYear();

            return dd + '. ' + mm + '. ' + yyyy;
        }
    }
</script>


<style scoped>
    .card-body {
        margin-top: 2rem;
    }

    .card-header-color-gray {
        background-color: rgb(225, 225, 225) !important;
        color: black !important;
    }

    .card-header-color-red {
        background-color: rgb(199, 0, 0) !important;
        color: #fdfdfd !important;
    }

    .card-header {
        margin-left: 1.5rem;
        margin-right: 1.5rem;
        bottom: 1.5rem;
    }

    .card-footer {
        margin-left: 1rem;
        margin-right: 1rem;
    }

    .card-body-text {
        margin-left: 1rem;
        margin-right: 1rem;
    }

    .card-header-text {
        color: #fdfdfd !important;
    }

    a{
        text-decoration: none;
        color: black !important;
    }
    .news-row{
        padding: 4px 16px;
    }
    .news-description-small{
        max-height: 47px;
    }
</style>
