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
                {{ $locale({i: 'nonPharma.title'}) }}
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
                                <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span>
                                <v-textarea v-model="nonPharma.description"></v-textarea>
                            </v-flex>
                        </v-layout>

                        <v-layout>
                            <v-flex xs12
                                    md1>
                                <span class="bold-span">{{ $locale({i: 'nonPharma.efficiency'}) }}</span>
                                <v-text-field prefix="%" v-model="nonPharma.efficiency" type="number"></v-text-field>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12
                                    md2>
                                <span class="bold-span">{{ $locale({i: 'nonPharma.lengthOfHospitalization'}) }}</span>
                                <v-select :items="lengthOfHospitalizations" v-model="nonPharma.lengthOfHospitalization"></v-select>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.indication'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'nonPharma.complication'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'nonPharma.workplacePerformingExercise'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addIndicationDiseases">
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
                        <v-data-table v-model="selectedIndication"
                                      item-key="id"
                                      :headers="diseaseHeaders"
                                      :items="nonPharma.indications"
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"
                                      @page-count="newDiseasePageCount = $event">

                            <template v-slot:item.name="{ item }">
                                <div @click="openDisease(item)">
                                    <span>{{ item.name }}</span>
                                </div>
                            </template>
                        </v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addComplications">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedComplications.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedComplications"
                                      item-key="id"
                                      :headers="diseaseHeaders"
                                      :items="nonPharma.complications"
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"
                                      @page-count="newDiseasePageCount = $event">

                            <template v-slot:item.name="{ item }">
                                <div @click="openDisease(item)">
                                    <span>{{ item.name }}</span>
                                </div>
                            </template>
                        </v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addWorkplacePerformingExercise">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedWorkplacePerformingExercise.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedWorkplacePerformingExercise"
                                      item-key="id"
                                      :headers="medicalPlacesesHeaders"
                                      :items="nonPharma.medicalPlaces"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12">

                            <template v-slot:item.name="{ item }">
                                <div @click="openDisease(item)">
                                    <span>{{ item.name }}</span>
                                </div>
                            </template>
                        </v-data-table>
                    </v-tab-item>
                </v-tabs>

            </v-card-text>
        </v-card>

        <v-dialog v-model="showIndication" width="800" persistent>
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

        <v-dialog v-model="showComplication" width="800" persistent>
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
                                      v-model="selectedComplications"
                                      :headers="diseaseHeaders"
                                      :items="diseases"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>

                    <div class="row" v-for="item in selectedComplications" :key="item.id">
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

        <v-dialog v-model="showWorkplacePerformingExercise" width="800" persistent>
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
                                      v-model="selectedWorkplacePerformingExercise"
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

    export default class DetailOfNonPharmaComponnent extends Vue {
        loading: boolean = true;
        nonPharma: any = null;
        showIndication: boolean = false;
        showComplication: boolean = false;
        showWorkplacePerformingExercise: boolean = false;
        dosages: any[] = [translations[userStore.getCulture()].dosage.once, translations[userStore.getCulture()].dosage.twice, translations[userStore.getCulture()].dosage.other];
        lengthOfHospitalizations: any[] = [translations[userStore.getCulture()].lengthOfHospitalization.outpatient, translations[userStore.getCulture()].lengthOfHospitalization.days, translations[userStore.getCulture()].lengthOfHospitalization.more];
        bindings: any[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        medicalPlaceses: any[] = [];
        medicalPlacesesHeaders: any[] = [];
        selectedComplications: any[] = [];
        selectedWorkplacePerformingExercise: any[] = [];
        selectedIndication: any[] = [];
        diseaseHeaders: any[] = [];
        diseases: any[] = [];
        newDiseasesPage: number = 1;
        newDiseasePageCount: number = 0;
        newDiseaseItemsPerPage: number = 50;

        closeNew() {
            this.selectedIndication = [];
            this.selectedComplications = [];
            this.medicalPlaceses = [];

            this.showIndication = false;
            this.showComplication = false;
            this.showWorkplacePerformingExercise = false;

            this.diseases.forEach(x => x.binding = null);
        }

        addNew() {
            if (this.selectedIndication.length > 0)
                this.nonPharma.indications = JSON.parse(JSON.stringify(this.selectedIndication));
            if (this.selectedComplications.length > 0)
                this.nonPharma.complications = JSON.parse(JSON.stringify(this.selectedComplications));
            if (this.selectedWorkplacePerformingExercise.length > 0)
                this.nonPharma.medicalPlaces = JSON.parse(JSON.stringify(this.selectedWorkplacePerformingExercise));

            this.closeNew();
        }

        addWorkplacePerformingExercise() {
            this.selectedWorkplacePerformingExercise = this.nonPharma.medicalPlaces;

            this.showWorkplacePerformingExercise = true;
        }

        addIndicationDiseases() {
            this.selectedIndication = this.nonPharma.indications;

            if (this.nonPharma.indications.length > 0) {
                this.diseases.forEach(x => {
                    let data = this.nonPharma.indications.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showIndication = true;
        }

        addComplications() {
            this.selectedComplications = this.nonPharma.complications;

            if (this.nonPharma.complications.length > 0) {
                this.diseases.forEach(x => {
                    let data = this.nonPharma.complications.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showComplication = true;
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.diseaseHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' }
            ];

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

            let diseasesCopy = JSON.parse(JSON.stringify(this.diseases));
            diseasesCopy[0].binding = 3;
            diseasesCopy[1].binding = 8;
            diseasesCopy[2].binding = 1;

            let nonPharmas = [
                {
                    id: 0,
                    description: "",
                    indications: [],
                    complications: [],
                    efficiency: null,
                    lengthOfHospitalization: "",
                    medicalPlaces: []
                },
                {
                    id: 1,
                    description: "Testing",
                    indications: [diseasesCopy[0], diseasesCopy[1]],
                    complications: [diseasesCopy[2]],
                    efficiency: 25,
                    lengthOfHospitalization: translations[userStore.getCulture()].lengthOfHospitalization.outpatient,
                    medicalPlaces: []
                },
            ];

            this.nonPharma = nonPharmas.find(x => x.id == this.$attrs.nonPharmaId);

            this.loading = false;
        }
    }
</script>

<style>
</style>