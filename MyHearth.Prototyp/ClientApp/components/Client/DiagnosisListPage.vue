<template>
    <div class="margin-15 shadow" style="background-color: white">

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="search"
                                      style="float: right; margin-right: 15px"
                                      class="col-8"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.questionSubject'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.questionSubject'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <div>
            <v-card class="manager-table shadow">
                <v-data-table :headers="headers"
                              :items="diagnosis"
                              :search="search"
                              :page.sync="page"
                              :items-per-page="itemsPerPage"
                              hide-default-footer
                              class="elevation-1"
                              @page-count="pageCount = $event">

                    <template v-slot:body="{ items }">
                        <tbody>
                            <tr v-for="item in items" :key="item.id" v-on:click="openDiagnosis(item)">
                                <td>{{ item.questionSubject }}</td>
                                <td>{{ item.createdDate }}</td>
                            </tr>
                        </tbody>
                    </template>

                </v-data-table>

                <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>
            </v-card>
        </div>

        <v-dialog v-model="openDiagnosisDetail" v-if="selectedDiagnosis.author" fullscreen hide-overlay transition="dialog-bottom-transition">
            <v-card>
                <v-toolbar dark color="primary">
                    <v-btn icon dark @click="openDiagnosisDetail = false">
                        <v-icon>close</v-icon>
                    </v-btn>
                    <v-toolbar-title>
                        {{ $locale({i: 'common.information'}) }} {{selectedDiagnosis.name}}
                    </v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-toolbar-items>
                        <!--<v-btn dark text @click="openConversationDialog = false">Save</v-btn>-->
                    </v-toolbar-items>
                </v-toolbar>

                <div>
                    <v-form>
                        <v-container>
                            <v-card>
                                <div class="margin-15">
                                    <v-layout>
                                        <v-flex xs12
                                                md3>
                                            <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                            <span>{{selectedDiagnosis.name}}</span>
                                        </v-flex>

                                        <v-flex xs12
                                                md3>
                                        </v-flex>
                                    </v-layout>

                                    <v-layout>
                                        <v-flex xs12
                                                md12>
                                            <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                            <span>{{selectedDiagnosis.description}}</span>
                                        </v-flex>
                                    </v-layout>
                                </div>
                            </v-card>
                        </v-container>
                    </v-form>
                </div>

                <v-tabs style="overflow: hidden">
                    <v-tab>
                        {{ $locale({i: 'information.symptoms'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'information.medicine'}) }}
                    </v-tab>

                    <v-tab-item class="margin-15">
                        <div class="row">
                            <div class="col-lg-6 col-md-12 col-sm-12"></div>

                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <v-tooltip top>
                                    <template v-slot:activator="{ on }">
                                        <v-text-field v-on="on"
                                                      v-model="symptomSearch"
                                                      append-icon="search"
                                                      :label="$locale({i: 'filter.name'})"
                                                      single-line
                                                      hide-details></v-text-field>
                                    </template>
                                    <span>{{ $locale({i: 'filter.name'}) }}</span>
                                </v-tooltip>
                            </div>
                        </div>

                        <v-data-table id="clientInfomrationDetailTable"
                                      :headers="symptomsHeaders"
                                      :items="symptoms"
                                      :search="symptomSearch"
                                      :page.sync="symptomsPage"
                                      :items-per-page="symptomsItemsPerPage"
                                      hide-default-footer
                                      class="elevation-1"
                                      @page-count="symptomsPageCount = $event">

                            <template v-slot:body="{ items }">
                                <tbody>
                                    <tr v-for="item in items" :key="item.id">
                                        <td>{{ item.name }}</td>
                                    </tr>
                                </tbody>
                            </template>
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="symptomsPage" :length="symptomsPageCount"></v-pagination>
                    </v-tab-item>

                    <v-tab-item>
                        Testing medicine
                    </v-tab-item>
                </v-tabs>

            </v-card>
        </v-dialog>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import { translations } from '../../utils/translations';
    import userStore from '@backend/store/userStore';
    import { InformationModel } from '../../backend/entities/informationModel';
    import { SymptomModel } from '../../backend/entities/symptomModel';

    @Component({
        components: {
        }
    })

    export default class DiagnosisListPage extends Vue {
        search: string = "";
        headers: any[] = [];
        diagnosis: any[] = [];
        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;
        selectedDiagnosis: any = {} as any;
        openDiagnosisDetail: boolean = false;

        symptomSearch: string = "";
        symptomsHeaders: any[] = [];
        symptoms: SymptomModel[] = [];

        symptomsPage: number = 1;
        symptomsPageCount: number = 0;
        symptomsItemsPerPage: number = 5;

        openDiagnosis(item) {
            this.selectedDiagnosis = item;
            this.openDiagnosisDetail = true;
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.headers = [
                { text: translations[userStore.getCulture()].common.questionSubject, value: 'questionSubject' },
                { text: translations[userStore.getCulture()].common.createdDate, value: 'createdDate' },
            ];

            this.diagnosis = [
                {
                    id: 1,
                    name: "Etiam",
                    questionSubject: 'Test',
                    description: 'Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
                    createdDate: "05.2.2017",
                    author: {
                        id: 1,
                        firstName: 'Josef',
                        lastName: 'Petr',
                        email: 'test@test.com',
                        role: 2,
                        weight: 90,
                        height: 190,
                        healthProblems: '',
                        allergy: ''
                    },
                },
                {
                    id: 2,
                    name: "Curabitur",
                    questionSubject: 'Proin',
                    description: 'Maecenas aliquet accumsan leo.',
                    createdDate: "12.2.2018",
                    author: {
                        id: 2,
                        firstName: 'Roman',
                        lastName: 'Musil',
                        email: 'test@test.com',
                        role: 1,
                        weight: 90,
                        height: 190,
                        healthProblems: '',
                        allergy: ''
                    },
                },
                {
                    id: 3,
                    name: "Nunc",
                    questionSubject: 'Lorem ipsum',
                    description: 'Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem. Proin mattis lacinia justo. Fusce suscipit libero eget elit.',
                    createdDate: "20.3.2019",
                    author: {
                        id: 2,
                        firstName: 'Roman',
                        lastName: 'Musil',
                        email: 'test@test.com',
                        role: 1,
                        weight: 90,
                        height: 190,
                        healthProblems: '',
                        allergy: ''
                    },
                }
            ];

            this.symptomsHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
                { id: 1, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 4, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 5, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 6, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 7, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 8, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 9, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];
        }

    }
</script>

<style>
</style>