﻿<template>
    <div>
        <top-bar :tabNames="['Zdravotní Zprávy']" v-model="selectedTab" />
        <v-tabs-items v-model="selectedTab">
            <v-tab-item><reports-overview /></v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import TopBar from "@components/top-bar.vue";

    import ReportsOverview from "./reports/reports-overview.vue"

    @Component({
        name: "ClientReports",
        components: {
            ReportsOverview, TopBar
        },
    })

    export default class ClientReports extends Vue {
        loading: boolean = false;

        selectedTab: number = null;

        mounted() {

        }
    }
</script>