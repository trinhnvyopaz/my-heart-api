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
                {{ $locale({i: 'medicalGroup.newTitle'}) }}
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
                                <h3 class="margin-15">{{ $locale({i: 'medicalGroup.character'}) }}</h3>
                                <v-textarea v-model="medicalGroup.character"></v-textarea>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.activeSubstance'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.indication'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.sideEffects'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.contraindication'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addPharmas">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedPharmas.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedPharmas"
                                      :headers="farmaksHeaders"
                                      :items="medicalGroup.farmaks"
                                      :page.sync="newPharmasPage"
                                      :items-per-page="newFarmakItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"
                                      @page-count="newFarmakPageCount = $event"></v-data-table>
                    </v-tab-item>

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
                                      :items="medicalGroup.indicationDiseases"
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
                                <v-btn color="success" class="back-button" @click="addSymptoms">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedSideEffects.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedSideEffects"
                                      :headers="sideEffectHeaders"
                                      :items="medicalGroup.symptoms"
                                      :page.sync="newSideEffectsPage"
                                      :items-per-page="newSideEffectItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"
                                      @page-count="newSideEffectPageCount = $event"></v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addContraindicationDiseases">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedContraindication.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedContraindication"
                                      :headers="diseaseHeaders"
                                      :items="medicalGroup.contraindicationDiseases"
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
                </v-tabs>

            </v-card-text>
        </v-card>

        <v-dialog v-model="showPharmas">
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          v-model="searchNewSymptom"
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
                                      v-model="selectedPharmas"
                                      :headers="farmaksHeaders"
                                      :items="farmaks"
                                      :page.sync="newPharmasPage"
                                      :items-per-page="newFarmakItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newFarmakPageCount = $event">
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSymptomPage" :length="newSymptomPageCount"></v-pagination>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closePharmas()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="addNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showSymptoms" width="800" persistent>
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          v-model="searchNewSymptom"
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
                                      v-model="selectedSideEffects"
                                      :headers="sideEffectHeaders"
                                      :items="symptoms"
                                      :page.sync="newSideEffectsPage"
                                      :items-per-page="newSideEffectItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newSideEffectPageCount = $event">
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSymptomPage" :length="newSymptomPageCount"></v-pagination>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeSymptoms()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="addNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showIndication" width="800" persistent>
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          v-model="searchNewSymptom"
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
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newDiseasePageCount = $event">
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSymptomPage" :length="newDiseasePageCount"></v-pagination>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeIndication()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="addNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="showContra" width="800" persistent>
            <v-card>
                <v-card-title>
                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-text-field v-on="on"
                                          v-model="searchNewSymptom"
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
                                      v-model="selectedContraindication"
                                      :headers="diseaseHeaders"
                                      :items="diseases"
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newDiseasePageCount = $event">
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSymptomPage" :length="newDiseasePageCount"></v-pagination>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeContra()">
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

    export default class DetailOfMedicalGroupComponnent extends Vue {
        loading: boolean = true;
        medicalGroup: any = null;

        selectedIndication: any[] = [];
        selectedContraindication: any[] = [];
        diseaseHeaders: any[] = [];
        diseases: any[] = [];
        newDiseasesPage: number = 1;
        newDiseasePageCount: number = 0;
        newDiseaseItemsPerPage: number = 50;
        selectedPharmas: any[] = [];
        farmaksHeaders: any[] = [];
        farmaks: any[] = [];
        newPharmasPage: number = 1;
        newFarmakPageCount: number = 0;
        newFarmakItemsPerPage: number = 50;
        selectedSideEffects: any[] = [];
        sideEffectHeaders: any[] = [];
        symptoms: any[] = [];
        newSideEffectsPage: number = 1;
        newSideEffectPageCount: number = 0;
        newSideEffectItemsPerPage: number = 50;

        showPharmas: boolean = false;
        showSymptoms: boolean = false;
        showIndication: boolean = false;
        showContra: boolean = false;

        addPharmas() {
            this.selectedPharmas = this.medicalGroup.farmaks;

            this.showPharmas = true;
        }

        addIndicationDiseases() {
            this.selectedIndication = this.medicalGroup.indicationDiseases;

            this.showIndication = true;
        }

        addSymptoms() {
            this.selectedSideEffects = this.medicalGroup.symptoms;

            this.showSymptoms = true;
        }

        addContraindicationDiseases() {
            this.selectedContraindication = this.medicalGroup.contraindicationDiseases;

            this.showContra = true;
        }

        addNew() {
            if (this.selectedPharmas.length > 0)
                this.medicalGroup.farmaks = this.selectedPharmas;
            if (this.selectedContraindication.length > 0)
                this.medicalGroup.contraindicationDiseases = this.selectedContraindication;
            if (this.selectedIndication.length > 0)
                this.medicalGroup.indicationDiseases = this.selectedIndication;
            if (this.selectedSideEffects.length > 0)
                this.medicalGroup.symptoms = this.selectedSideEffects;

            this.closeIndication();
            this.closeContra();
            this.closePharmas();
            this.closeSymptoms();
        }

        closeIndication() {
            this.showIndication = false;
            this.selectedIndication = [];
        }

        closeContra() {
            this.showContra = false;
            this.selectedContraindication = [];
        }

        closePharmas() {
            this.showPharmas = false;
            this.selectedPharmas = [];
        }

        closeSymptoms() {
            this.showSymptoms = false;
            this.selectedSideEffects = [];
        }

        goBack() {
            window.history.back();
        }

        openFarmak(item) {
            this.$router.push({ path: '/farmak/' + item.id });
        }

        openDisease(item) {
            this.$router.push({ path: '/information/' + item.id });
        }

        created() {
            this.diseases = [
                {
                    id: 1,
                    name: "Etiam",
                    description: 'Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
                    binding: 10,
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
                    binding: 10,
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
                    binding: 10,
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
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.farmaksHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' },
            ];

            this.sideEffectHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
                { id: 1, binding: null, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, binding: null, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, binding: null, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];

            this.farmaks = [
                { id: 1, name: "Fusce", binding: 10 },
                { id: 2, name: "Cras", binding: 10 },
                { id: 3, name: "Nunc", binding: 10 }
            ];

            let medicalGroups = [
                {
                    id: 0,
                    character: "",
                    farmaks: [],
                    indicationDiseases: [],
                    symptoms: [],
                    contraindicationDiseases: []

                },
                {
                    id: 1,
                    character: "Test",
                    farmaks: [this.farmaks[0], this.farmaks[2]],
                    indicationDiseases: [this.diseases[0], this.diseases[1]],
                    symptoms: [this.symptoms[0]],
                    contraindicationDiseases: [this.diseases[2]]
                }
            ];

            this.medicalGroup = medicalGroups.find(x => x.id == this.$attrs.medicalGroupId);
            console.log(this.medicalGroup.selectedIndication);

            this.loading = false;
        }
    }
</script>

<style>
    .divider {
        border-top: 1px solid #0069d9;
    }
</style>