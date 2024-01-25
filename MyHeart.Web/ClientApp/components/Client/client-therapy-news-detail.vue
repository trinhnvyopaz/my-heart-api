<template>
    <div class="main-layout">
        <v-card class="rounded-xl">
            <v-card-title class="news-title">
                <div style="width: 100%;" class="d-flex">
                    <span class="news-title mr-auto">{{therapyNews.text}}</span>
                    <span class="news-date">{{datetimeToDateString(therapyNews.lastUpdateDate)}}</span>
                </div>
            </v-card-title>
            <v-card-text class="news-description news-description-big" v-html="therapyNews.description" />
        </v-card>
        <v-snackbar right top v-model="snackbarShown" color="success" v-html="snackbarText" />
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import TherapyNewsApi from "@backend/api/therapyNews";

@Component({
        name: "ClientTherapyNews",
        components: {
        },
    })


export default class ClientTherapyNewsDetail extends Vue {
    newsId: number = -1
    therapyNews: any = null;
    snackbarText: string = ""
    snackbarShown: boolean = false

    mounted(){
        this.getNewsId()
        this.loadData()
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

    async loadData(){
        var result = await TherapyNewsApi.getTherapyNewsDetail(this.newsId);
        if(result.success){
            this.therapyNews = result.data
        }else{
            this.snackbarText = "Nepodařilo se najít"
            this.snackbarShown = true
        }
    }

    getNewsId() {
        var path = window.location.pathname;
        var chunks = path.split('/');

        if (chunks.length == 5) {
            this.newsId = parseInt(chunks[4]);
        }
    }
}
</script>
<style scoped>
.main-layout{
    margin: 20px;
}
.news-description-big{
    margin-top: 16px;
}
</style>
