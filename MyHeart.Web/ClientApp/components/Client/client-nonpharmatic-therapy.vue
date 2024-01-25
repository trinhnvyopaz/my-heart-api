<template>
    <div>
        <top-bar :tabNames="['Provedené výkony']" v-model="selectedTab" />
        <v-tabs-items v-model="selectedTab">
            <v-tab-item><nonpharmacy-therapy /></v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import TopBar from "@components/top-bar.vue";
    import AnamnesisPharmacy from './therapy/anamnesis-pharmacy'
    import NonpharmacyTherapy from './therapy/nonpharmacy-therapy'

    @Component({
        name: "ClientDiseases",
        components: {
            AnamnesisPharmacy, NonpharmacyTherapy, TopBar
        },
    })

    export default class ClientDiseases extends Vue {
        loading: boolean = false;

        selectedTab: number = null;

        mounted() {

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

    a {
        text-decoration: none;
        color: black !important;
    }
</style>
