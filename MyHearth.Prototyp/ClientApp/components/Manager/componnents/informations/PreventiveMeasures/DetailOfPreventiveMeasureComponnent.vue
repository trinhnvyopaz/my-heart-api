<template>
    <div class="margin-15">
        <div v-if="loading">
            <v-progress-linear indeterminate color="error"></v-progress-linear>
        </div>

        <v-card>
            <v-card-title class="title" style="background-color: ghostwhite">
                <v-btn color="primary" @click="goBack()" class="back-button">
                    {{ $locale({i: 'common.back'}) }}
                </v-btn>
                {{ $locale({i: 'preventiveMeasures.title'}) }}
                <v-spacer></v-spacer>
                <v-btn color="success" @click="goBack()" class="back-button">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-title>

            <v-card-text>
                <v-form>
                    <v-container>
                        <v-layout>
                            <v-flex xs12
                                    md12>
                                <span class="bold-span">{{ $locale({i: 'medicalGroup.character'}) }}</span>
                                <v-textarea v-model="preventiveMeasure.character"></v-textarea>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.indication'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'nonPharma.workplacePerformingExercise'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addIndications">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedIndication.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table id="newSymptomTable"
                                      v-model="selectedIndication"
                                      item-key="id"
                                      :headers="diseaseHeaders"
                                      :items="preventiveMeasure.indications"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addMedicalPlaces">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedMedicalPlaces.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table id="newSymptomTable"
                                      v-model="selectedMedicalPlaces"
                                      item-key="id"
                                      :headers="medicalPlacesesHeaders"
                                      :items="preventiveMeasure.medicalPlaces"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"></v-data-table>
                    </v-tab-item>
                </v-tabs>

            </v-card-text>
        </v-card>

        <v-dialog v-model="showIndications" width="800" persistent>
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          append-icon="search"
                                          :label="$locale({i: 'filter.name'})"
                                          single-line
                                          hide-details></v-text-field>
                        </template>
                        <span>{{ $locale({i: 'filter.name'}) }}</span>
                    </v-tooltip>
                </v-card-title>

                <v-card-text>
                    <div>
                        <v-data-table id="newSymptomTable"
                                      v-model="selectedIndication"
                                      :headers="diseaseHeaders"
                                      :items="diseases"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>

                    <div class="row" v-for="item in selectedIndication" :key="item.id">
                        <div class="col-5" style="text-align: center; margin: auto">
                            {{item.name}}
                        </div>
                        <div class="col-6">
                            <v-select :items="bindings"
                                      v-model="item.binding"
                                      :label="$locale({i: 'common.binding'})"></v-select>
                        </div>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeNew()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="addNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showWorkPlaces" width="800" persistent>
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          append-icon="search"
                                          :label="$locale({i: 'filter.name'})"
                                          single-line
                                          hide-details></v-text-field>
                        </template>
                        <span>{{ $locale({i: 'filter.name'}) }}</span>
                    </v-tooltip>
                </v-card-title>

                <v-card-text>
                    <div>
                        <v-data-table id="newSymptomTable"
                                      v-model="selectedMedicalPlaces"
                                      :headers="medicalPlacesesHeaders"
                                      :items="medicalPlaceses"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeNew()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="addNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import { Component } from "vue-property-decorator";

    import userStore from '@backend/store/userStore'
    import { translations } from "@utils/translations";

    @Component({
        components: {}
    })

    export default class DetailOfPreventiveMeasureComponnent extends Vue {
        loading: boolean = true;
        preventiveMeasure: any = null;
        selectedIndication: any[] = [];
        selectedMedicalPlaces: any[] = [];
        diseaseHeaders: any[] = [];
        diseases: any[] = [];
        medicalPlacesesHeaders: any[] = [];
        medicalPlaceses: any[] = [];
        showIndications: boolean = false;
        showWorkPlaces: boolean = false;
        bindings: any[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        addIndications() {
            this.selectedIndication = this.preventiveMeasure.indications;

            if (this.preventiveMeasure.indications.length > 0) {
                this.diseases.forEach(x => {
                    let data = this.preventiveMeasure.indications.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showIndications = true;
        }

        addMedicalPlaces() {
            this.selectedMedicalPlaces = this.preventiveMeasure.medicalPlaces;

            this.showWorkPlaces = true;
        }

        closeNew() {
            this.selectedIndication = [];
            this.medicalPlaceses = [];

            this.showIndications = false;
            this.showWorkPlaces = false;

            this.diseases.forEach(x => x.binding = null);
        }

        addNew() {
            if (this.selectedIndication.length > 0)
                this.preventiveMeasure.indications = JSON.parse(JSON.stringify(this.selectedIndication));
            if (this.selectedMedicalPlaces.length > 0)
                this.preventiveMeasure.medicalPlaces = JSON.parse(JSON.stringify(this.selectedMedicalPlaces));

            this.closeNew();
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.diseases = [
                {
                    id: 1,
                    name: "Etiam",
                    description: 'Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
                    binding: null,
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
                    binding: null,
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
                    binding: null,
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
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' }
            ];

            this.medicalPlaceses = [

            ];

            this.medicalPlacesesHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            let diseasesCopy = JSON.parse(JSON.stringify(this.diseases));
            diseasesCopy[0].binding = 3;
            diseasesCopy[1].binding = 8;
            diseasesCopy[2].binding = 1;

            let preventiveMeasures = [
                {
                    id: 0,
                    character: "",
                    indications: [],
                    medicalPlaces: []
                },
                {
                    id: 1,
                    character: "Testing",
                    indications: [diseasesCopy[0], diseasesCopy[1]],
                    medicalPlaces: []
                }
            ];

            this.preventiveMeasure = preventiveMeasures.find(x => x.id == this.$attrs.preventiveMeasureId);

            this.loading = false;
        }
    }
</script>

<style>
</style>