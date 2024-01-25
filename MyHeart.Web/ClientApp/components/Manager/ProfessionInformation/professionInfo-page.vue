<template>
    <div>
        <top-bar :tabNames="tabNames" v-model="selectedTab"/>
        <v-tabs-items v-model="selectedTab">
            <v-tab-item><disease-list-page /></v-tab-item>
            <v-tab-item><symptom-list-page /></v-tab-item>
            <v-tab-item><symptom-questions-list-page /></v-tab-item>
            <v-tab-item><medicament-group-list-page /></v-tab-item>
            <v-tab-item><pharmacy-list-page /></v-tab-item>
            <!--<v-tab-item><commercial-name-list-page/></v-tab-item>-->
            <v-tab-item><nonpharmatic-list-page /></v-tab-item>
            <v-tab-item><preventiv-measures-list-page /></v-tab-item>
            <v-tab-item><medical-facilities-list-page /></v-tab-item>
            <!--<v-tab-item><therapy-news-list-page /></v-tab-item>-->
            <v-tab-item><therapy-news-list-page-refactor /></v-tab-item>
        </v-tabs-items>
        </div>
    </template>

    <script lang="ts">
        import Vue from "vue";
        import { Component, Prop } from "vue-property-decorator";
        import DiseaseListPage from "@components/Manager/ProfessionInformation/Disease/disease-list-page.vue";
        import SymptomListPage from "@components/Manager/ProfessionInformation/Symptoms/symptom-list-page.vue";
        import TherapyNewsListPage from "@components/Manager/ProfessionInformation/TherapyNews/therapy-news-list-page.vue";
        import TherapyNewsListPageRefactor from "@components/Manager/ProfessionInformation/TherapyNews/therapy-news-list-page-refactor.vue";
        import MedicamentGroupListPage from "@components/Manager/ProfessionInformation/MedicamentGroup/medicament-list-page.vue";
        import NonpharmaticListPage from "@components/Manager/ProfessionInformation/NonpharmaticTherapy/nonpharmatic-list-page.vue";
        import PharmacyListPage from "@components/Manager/ProfessionInformation/Pharmacy/pharmacy-list-page.vue";
        import PreventivMeasuresListPage from "@components/Manager/ProfessionInformation/PreventiveMeasures/preventive-list-page.vue";
        import MedicalFacilitiesListPage from "@components/Manager/ProfessionInformation/MedicalFacilities/medical-facilities-list-page.vue";
        import SymptomQuestionsListPage from "@components/Manager/ProfessionInformation/SymptomQuestions/symptom-questions-list-page.vue";
        import CommercialNameListPage from "@components/Manager/ProfessionInformation/CommercialNames/commercial-name-list-page.vue";
        import TopBar from "@components/top-bar.vue";

        @Component({
            name: "ProfessionInfoPage",
            components: {
                TopBar, DiseaseListPage, SymptomListPage, TherapyNewsListPage, MedicamentGroupListPage, NonpharmaticListPage, PharmacyListPage, PreventivMeasuresListPage, MedicalFacilitiesListPage, SymptomQuestionsListPage, CommercialNameListPage, TherapyNewsListPageRefactor
            },
        })
        export default class ProfessionInfoPage extends Vue {

            selectedTab: number = 0;

            tabNames = [
                "Onemocnění",
                "Seznam Příznaků",
                "Otázky k příznakům",
                "Lékové skupiny",
                "Farmaka",
                "Nefarmakologická léčba",
                "Preventivní opatření",
                "Zdravotnická zařízení",
                //"Novinky v léčbě",
                "Novinky v léčbě"
            ];

            mounted() { }
        }
    </script>
    <style scoped>
        .lightgrey-color {
            background-color: rgb(245,245,245) !important;
        }
    </style>
