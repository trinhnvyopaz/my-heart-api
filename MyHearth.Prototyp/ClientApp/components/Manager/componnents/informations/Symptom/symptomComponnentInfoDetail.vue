<template>
    <div>
        <v-breadcrumbs :items="breadcrumbItems" large></v-breadcrumbs>

        <v-card class="margin-15">
            <v-card-title primary-title class="title" style="background-color: ghostwhite">
                <v-btn color="primary" @click="goBack()" class="back-button">
                    {{ $locale({i: 'common.back'}) }}
                </v-btn>
                {{ $locale({i: 'common.symptom'}) }} {{symptom.name}}

                <v-spacer></v-spacer>
                <v-btn color="error"
                       class="delete-button"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.delete'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-title>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                    <v-text-field v-model="symptom.name"></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <v-textarea v-model="symptom.description"></v-textarea>
                                </v-flex>
                            </v-layout>

                        </v-container>
                    </v-form>
                </div>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'managerPage.disease'}) }}
                    </v-tab>
                    <v-tab-item>
                        <div class="row">
                            <v-btn color="success" class="margin-15" style="float: left" v-on:click="addDiseases = true">{{ $locale({i: 'common.addNew'}) }}</v-btn>
                        </div>

                        <v-data-table :headers="diseaseHeaders"
                                      :items="diseases"
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      class="elevation-1"
                                      @page-count="newDiseasePageCount = $event">

                            <template v-slot:body="{ items }">
                                <tbody>
                                    <tr v-for="item in items" :key="item.id" v-on:click="openDisease(item)">
                                        <td>{{ item.name }}</td>
                                        <td>{{ item.binding }}</td>
                                    </tr>
                                </tbody>
                            </template>
                        </v-data-table>
                    </v-tab-item>
                </v-tabs>
            </v-card-text>
        </v-card>

        <v-dialog v-model="addDiseases" width="800" persistent>
            <div>
                <v-card>
                    <v-card-text>
                        <v-data-table id="newSymptomTable"
                                      v-model="selectedDiseases"
                                      :headers="newDiseaseHeaders"
                                      :items="diseases"
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newDiseasePageCount = $event">
                        </v-data-table>

                        <div class="row" v-for="item in selectedDiseases" :key="item">
                            <div class="col-5" style="text-align: center; margin: auto">
                                {{item.name}}
                            </div>
                            <div class="col-6">
                                <v-select :items="bindings"
                                          :label="$locale({i: 'common.binding'})"></v-select>
                            </div>
                        </div>
                    </v-card-text>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="primary"
                               class="delete-button"
                               v-on:click="addDiseases = false">
                            {{ $locale({i: 'common.back'}) }}
                        </v-btn>
                        <v-btn color="success"
                               v-on:click="addDiseases = false">
                            {{ $locale({i: 'common.save'}) }}
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </div>
        </v-dialog>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import { translations } from "@utils/translations";
    import userStore from '@backend/store/userStore'
    import { SymptomModel } from '../../../../../backend/entities/symptomModel';

    @Component({
        components: {
        }
    })

    export default class SymptomComponnent extends Vue {
        symptom: SymptomModel = {} as any;
        selectedDiseases: any[] = [];
        diseaseHeaders: any[] = [];
        newDiseaseHeaders: any[] = [];
        diseases: any[] = [];
        addDiseases: boolean = false;

        newDiseasesPage: number = 1;
        newDiseasePageCount: number = 0;
        newDiseaseItemsPerPage: number = 50;
        bindings: any[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        breadcrumbItems: any[] = [];
        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        goBack() {
            window.history.back();
        }

        openDisease(item) {
            this.$router.push({ path: '/information/' + item.id });
        }

        created() {
            if (this.breadcrumbStore.breadcrumbs.length > 0) {
                let toObject = { text: document.title };
                this.breadcrumbStore.Add(toObject);

                this.breadcrumbItems = this.breadcrumbStore.breadcrumbs;
            }

            this.diseases = [
                {
                    id: 1,
                    name: "Etiam",
                    description: 'Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
                    binding: 3,
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
                    description: 'Maecenas aliquet accumsan leo.',
                    binding: 2,
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
                    binding: 6,
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

            this.diseaseHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' },
            ];

            this.newDiseaseHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
            ];

            let symptoms = [
                { id: 0, name: "", binding: null, description: '' },

                { id: 1, name: "Morbi", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, name: "Mauris", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, name: "Curabitur", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 4, name: "Morbi", binding: 2, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 5, name: "Mauris", binding: 6, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 6, name: "Curabitur", binding: 7, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 7, name: "Morbi", binding: 1, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 8, name: "Mauris", binding: 8, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 9, name: "Curabitur", binding: 10, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];

            this.symptom = symptoms.find(x => x.id == this.$attrs.symptomId);
        }

    }
</script>

<style>
    .bold-span {
        font-weight: bold;
    }
</style>