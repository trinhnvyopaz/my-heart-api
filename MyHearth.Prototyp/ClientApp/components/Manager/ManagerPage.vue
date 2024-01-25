<template>
    <div class="margin-15 shadow">
        <v-tabs centered grow v-model="selectedTab">
            <v-tab href="#users" @click="setTabName('users')">
                {{ $locale({i: 'managerPage.userAdministration'}) }}
            </v-tab>
            <v-tab href="#stats" @click="setTabName('stats')">
                {{ $locale({i: 'managerPage.statistics'}) }}
            </v-tab>
            <v-tab href="#informations" @click="setTabName('informations')">
                {{ $locale({i: 'managerPage.mainInformation'}) }}
            </v-tab>
            <v-tab href="#questions" @click="setTabName('questions')">
                {{ $locale({i: 'question.title'}) }}
            </v-tab>

            <v-tab-item id="users">
                <user-administration></user-administration>
            </v-tab-item>

            <v-tab-item id="stats">
                <statistics></statistics>
            </v-tab-item>

            <v-tab-item id="informations">
                <v-tabs centered>
                    <v-tab>
                        {{ $locale({i: 'managerPage.disease'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'managerPage.listOfSymptoms'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.title'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'pharma.title'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'nonPharma.title'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'preventiveMeasures.title'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'managerPage.madicalPlaces'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'news.title'}) }}
                    </v-tab>

                    <v-tab-item>
                        <main-information></main-information>
                    </v-tab-item>
                    <v-tab-item>
                        <list-of-symptoms></list-of-symptoms>
                    </v-tab-item>
                    <v-tab-item>
                        <medical-group></medical-group>
                    </v-tab-item>
                    <v-tab-item>
                        <pharmas></pharmas>
                    </v-tab-item>
                    <v-tab-item>
                        <non-pharmas></non-pharmas>
                    </v-tab-item>
                    <v-tab-item>
                        <preventive-measures></preventive-measures>
                    </v-tab-item>
                    <v-tab-item>
                        <medical-places></medical-places>
                    </v-tab-item>
                    <v-tab-item>
                        <news></news>
                    </v-tab-item>
                </v-tabs>
            </v-tab-item>

            <v-tab-item id="questions">
                <questions></questions>
            </v-tab-item>

        </v-tabs>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import userStore from '@backend/store/userStore'
    import { translations } from "../../utils/translations";

    import UserAdministrationComponnent from './componnents/users/UserAdministrationComponnent';
    import StatisticsComponnent from './componnents/stats/StatisticsComponnent';
    import MainInformationComponnent from './componnents/informations/Main/MainInformationComponnent';
    import ListOfSymptomsComponnent from './componnents/informations/Symptom/ListOfSymptomsComponnent';
    import ListOfMedicalPlacesComponnent from './componnents/informations/MedicalPlaces/ListOfMedicalPlacesComponnent';
    import ListOfMedicalGroupsComponnent from './componnents/informations/MedicalGroup/ListOfMedicalGroupsComponnent';
    import ListOfPharmasComponnent from "./componnents/informations/Pharmas/ListOfPharmasComponnent";
    import ListOfNonPharmasComponnent from "./componnents/informations/NonPharmas/ListOfNonPharmasComponnent";
    import ListOfPreventiveMeasuresComponnent from "./componnents/informations/PreventiveMeasures/ListOfPreventiveMeasuresComponnent";
    import ListOfNewsComponnent from "./componnents/informations/News/ListOfNewsComponnent";
    import ListOfQuestionsComponnent from "./componnents/questions/ListOfQuestionsComponnent";

    @Component({
        components: {
            "user-administration": UserAdministrationComponnent,
            "statistics": StatisticsComponnent,
            "main-information": MainInformationComponnent,
            "list-of-symptoms": ListOfSymptomsComponnent,
            "medical-places": ListOfMedicalPlacesComponnent,
            "medical-group": ListOfMedicalGroupsComponnent,
            "pharmas": ListOfPharmasComponnent,
            "non-pharmas": ListOfNonPharmasComponnent,
            "preventive-measures": ListOfPreventiveMeasuresComponnent,
            "news": ListOfNewsComponnent,
            "questions": ListOfQuestionsComponnent
        }
    })

    export default class ManagerPage extends Vue {
        selectedTab: string = "";
        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        setTabName(tabName) {
            this.$router.push({ path: '/manager_page/' + tabName });
            this.switchPageTitle();
        }

        switchPageTitle() {
            this.$router.beforeEach((to, from, next) => {

                switch (to.params.tabName) {
                    case "users":
                        document.title = translations[userStore.getCulture()].managerPage.userAdministration;
                        break;
                    case "stats":
                        document.title = translations[userStore.getCulture()].managerPage.statistics;
                        break;
                    case "symptoms":
                        document.title = translations[userStore.getCulture()].managerPage.listOfSymptoms;
                        break;
                    case "medicines":
                        document.title = translations[userStore.getCulture()].managerPage.listOfMedicines;
                        break;
                    case "madicalPlaces":
                        document.title = translations[userStore.getCulture()].managerPage.madicalPlaces;
                        break;
                    case "questions":
                        document.title = translations[userStore.getCulture()].question.title;

                    default:
                        break;
                }

                next();
            });
        }

        created() {
            //TODO
            this.breadcrumbStore.Clear();
            this.selectedTab = this.$attrs.tabName;
            this.switchPageTitle();
            document.title = translations[userStore.getCulture()].managerPage.mainInformation;

            let test = "password";
            test = btoa(test);
            console.log(atob(test));
        }
    }
</script>

<style>
</style>