<template>
    <div>
        <top-bar :tabNames="['Onemocnění', 'Fyzikální vyšetření', 'Alergie', 'Anamnéza']" v-model="tab"/>

        <v-tabs-items v-model="tab">

            <v-tab-item><anamnesis-disease /></v-tab-item>
            <v-tab-item><anamnesis-personal /></v-tab-item>
            <v-tab-item><anamnesis-allergy /></v-tab-item>
            <v-tab-item><anamnesis-general /></v-tab-item>

        </v-tabs-items>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import TopBar from "@components/top-bar.vue";
    import AnamnesisPersonal from './diseases/anamnesis-personal'
    import AnamnesisAbusus from './diseases/anamnesis-abusus'
    import AnamnesisSocial from './diseases/anamnesis-social'
    import AnamnesisFamily from './diseases/anamnesis-family'
    import AnamnesisAllergy from './diseases/anamnesis-allergy'
    import AnamnesisDisease from './diseases/anamnesis-disease'
    import AnamnesisGeneral from './diseases/anamnesis-general'

    @Component({
        name: "ClientDiseases",
        components: {
            AnamnesisPersonal, AnamnesisAbusus, AnamnesisSocial, AnamnesisFamily, AnamnesisAllergy, AnamnesisDisease, AnamnesisGeneral, TopBar
        },
    })

    export default class ClientDiseases extends Vue {
        loading: boolean = false;

        tab: number = null;

        mounted() {

        }
    }
</script>
