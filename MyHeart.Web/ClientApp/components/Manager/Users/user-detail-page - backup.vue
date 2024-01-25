<template>
    <div>
        <v-tabs color="error" v-model="selectedTab" centered icons-and-text background-color="rgb(245,245,245)">
            <v-tabs-slider></v-tabs-slider>
            <v-tab v-for="item in items" :key="item.id">
                {{ item.title }}
                <v-icon color="red">{{ item.icon }}</v-icon>
            </v-tab>

        </v-tabs>
        <v-tabs-items v-model="selectedTab">
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <base-info></base-info>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <questions-list></questions-list>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>

                    </v-card-text>
                </v-card>
            </v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import { Component, Prop } from "vue-property-decorator";
    import BaseInfo from "@components/Manager/Users/user-detail-baseInfo-page.vue";
    import Breadcrumbs from "@components/Shared/breadcrumbs.vue";
    import QuestionsList from "@components/Manager/Users/user-questions-list-page.vue";

    @Component({
        name: "UserDetailPage",
        components: {
            BaseInfo, Breadcrumbs, QuestionsList
        },
    })
    export default class UserDetailPage extends Vue {

        selectedTab: number = 0;
        items = [
            {
                icon: "mdi-account-group",
                title: "Základní Informace a Anamnézy",
                id: 0
            },
            {
                icon: "mdi-frequently-asked-questions",
                title: "Dotazy pacienta",
                id: 1
            },
            {
                icon: "mdi-file-document",
                title: "Dokumenty",
                id: 2
            }
        ];

        mounted() { }
    }
</script>

<style scoped>
    .lightgrey-color {
        background-color: rgb(245,245,245) !important;
    }
</style>
