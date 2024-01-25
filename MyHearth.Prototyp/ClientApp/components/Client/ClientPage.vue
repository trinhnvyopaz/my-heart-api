<template>
    <div>
        <div class="row">
            <div class="col-3"></div>
            <div class="col-6">
                <v-card class="shadow">
                    <v-container class="pa-2"
                                 fluid>
                        <v-row>
                            <v-col v-for="card in cards"
                                   :key="card.title"
                                   class="col-lg-6 col-md-6 col-sm-12">
                                <v-card v-if="card.title" class="card-board" v-bind:style="{ background: card.color }" v-on:click="openWindow(card.href)">
                                    <h2 class="card-board-title">{{card.title}}</h2>
                                    <h5 class="card-board-subTitle">{{card.subTitle}}</h5>
                                </v-card>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-card>
            </div>
            <div class="col-3"></div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import { translations } from '../../utils/translations';
    import userStore from '@backend/store/userStore';

    @Component({
        components: {
        }
    })

    export default class ClientPage extends Vue {
        cards: any[] = [];

        openWindow(href) {
            this.$router.push({ path: '/' + href });
        }

        created() {
            this.cards = [
                { title: translations[userStore.getCulture()].clientPage.newQuestion, subTitle: "", href: "newQuestion", color: "#4caf50" },
                { title: translations[userStore.getCulture()].clientPage.questionArchive, subTitle: translations[userStore.getCulture()].demo.oneActive, href: "questionArchive", color: "#ff5252" },
                { title: translations[userStore.getCulture()].clientPage.myDiagnoses, subTitle: "", href: "myDiagnoses", color: "#fb8c00" },
                { title: translations[userStore.getCulture()].clientPage.myProfile, subTitle: "", href: "clientProfile", color: "#5cbbf6" },
            ];
        }

    }
</script>

<style>
    .card-board {
        height: 200px;
    }

        .card-board:hover {
            cursor: pointer;
            box-shadow: 10px 10px 15px 10px lightgrey !important;
        }

    .card-board-title {
        color: white;
        transform: translateY(100%);
        text-align: center;
    }

    .card-board-subTitle {
        color: white;
        margin-right: 10px;
        bottom: 0;
        right: 0;
        position: absolute;
    }
</style>