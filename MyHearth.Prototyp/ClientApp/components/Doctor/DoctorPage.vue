<template>
    <div class="margin-15 shadow">
        <v-tabs fixed-tabs v-model="selectedTab">
            <v-tab href="#newQuestions" @click="setTabName('newQuestions')">
                {{ $locale({i: 'doctorPage.newQuestions'}) }}
            </v-tab>
            <v-tab href="#activeQuestions" @click="setTabName('activeQuestions')">
                {{ $locale({i: 'doctorPage.activeQuestions'}) }}
                <font-awesome-icon class="notification-star" icon="exclamation-triangle" />
            </v-tab>
            <v-tab href="#archivedQuestions" @click="setTabName('archivedQuestions')">
                {{ $locale({i: 'doctorPage.archivedQuestions'}) }}
            </v-tab>
            <v-tab href="#patientList" @click="setTabName('patientList')">
                {{ $locale({i: 'doctorPage.patientList'}) }}
            </v-tab>

            <v-tab-item id="newQuestions">
                <new-question-list></new-question-list>
            </v-tab-item>

            <v-tab-item id="activeQuestions">
                <active-question-list></active-question-list>
            </v-tab-item>

            <v-tab-item id="archivedQuestions">
                <archived-question-list></archived-question-list>
            </v-tab-item>

            <v-tab-item id="patientList">
                <patient-list></patient-list>
            </v-tab-item>

        </v-tabs>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    import NewQuestionList from './components/questions/NewQuestionList';
    import ActiveQuestionList from './components/questions/ActiveQuestionList';
    import ArchivedQuestionList from './components/questions/ArchivedQuestionList';
    import PatientList from './components/patient/PatientList';

    @Component({
        components: {
            "new-question-list": NewQuestionList,
            "active-question-list": ActiveQuestionList,
            "archived-question-list": ArchivedQuestionList,
            "patient-list": PatientList
        }
    })

    export default class DoctorPage extends Vue {
        selectedTab: any = "newQuestions";

        setTabName(tabName) {
            this.$router.push({ path: '/doctor_page/' + tabName });
        }

        created() {
            this.selectedTab = this.$attrs.selectedTab;
        }

    }
</script>

<style>
    .notification-star {
        color: red;
        margin-left: 10px !important;
    }
</style>