<template>
    <div>
        <breadcrumbs title="Detail Lékaře"
                     :routes="[
        { name: 'Administrace', to: '/admin' },
        { name: 'Detail Lékaře' }
      ]" />
        <v-card>
            <v-tabs color="error" v-model="selectedTab" centered icons-and-text>
                <v-tabs-slider></v-tabs-slider>
                <v-tab v-for="item in items" :key="item.id">
                    {{ item.title }}
                    <v-icon color="red">{{ item.icon }}</v-icon>
                </v-tab>

            </v-tabs>
            <v-tabs-items v-model="selectedTab">
                <v-tab-item>
                    <v-card flat>
                        <v-card-text>
                            <base-info></base-info>
                        </v-card-text>
                    </v-card>
                </v-tab-item>
                <v-tab-item>
                    <v-card flat>
                        <v-card-text>
                            <questions-list></questions-list>
                        </v-card-text>
                    </v-card>
                </v-tab-item>
                <v-tab-item>
                    <v-card flat>
                        <v-card-text>
                            <questions-list></questions-list>
                        </v-card-text>
                    </v-card>
                </v-tab-item>
            </v-tabs-items>
        </v-card>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop } from "vue-property-decorator";
    import BaseInfo from "@components/Manager/Doctors/doctor-detail-baseInfo-page.vue";
    import Breadcrumbs from "@components/Shared/breadcrumbs.vue";
    import QuestionsList from "@components/Manager/Users/user-questions-list-page.vue";

    @Component({
        name: "DoctorDetailPage",
        components: {
            BaseInfo, Breadcrumbs, QuestionsList
        },
    })
    export default class DoctorDetailPage extends Vue {

        selectedTab: number = 0;
        items = [
            {
                icon: "mdi-account-group",
                title: "Osobní údaje",
                id: 0
            },
            {
                icon: "mdi-frequently-asked-questions",
                title: "Seznam dotazů",
                id: 1
            },
            {
                icon: "mdi-star-half",
                title: "Recenze od uživatelů",
                id: 1
            }
        ];

        mounted() {}
}
</script>

<style scoped>
</style>
