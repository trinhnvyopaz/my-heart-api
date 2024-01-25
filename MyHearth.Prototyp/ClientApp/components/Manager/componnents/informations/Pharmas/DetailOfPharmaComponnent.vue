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
                {{ $locale({i: 'pharma.title'}) }}
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
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span>
                                <v-text-field v-model="pharma.name"></v-text-field>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12
                                    md2>
                                <span class="bold-span">{{ $locale({i: 'pharma.dosage'}) }}</span>
                                <v-select :items="dosages"
                                          v-model="pharma.dosage"></v-select>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12 md2>
                                <span class="bold-span">{{ $locale({i: 'pharma.minDosage'}) }}</span>
                                <v-text-field v-model="pharma.minDosage" type="number"></v-text-field>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12 md2>
                                <span class="bold-span">{{ $locale({i: 'pharma.maxDosage'}) }}</span>
                                <v-text-field v-model="pharma.maxDosage" type="number"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'pharma.commercialTitle'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'medicalGroup.title'}) }}
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
                                <v-text-field class="col-4" v-model="newCommercialName" :placeholder="$locale({i: 'pharma.commercialTitle'})"></v-text-field>
                                <v-btn color="primary" @click="addNewCommercialName" class="back-button">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="pharma.commercialNames.length > 0" color="error" @click="deleteCommercialName">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedCommercialTitle"
                                      item-key="id"
                                      :items="pharma.commercialNames"
                                      :headers="commercialNameHeaders"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"></v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addMedicalGroups">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedMedicalGroups.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedMedicalGroups"
                                      item-key="id"
                                      :headers="medicalGroupsHeaders"
                                      :items="pharma.medicalGroups"
                                      :page.sync="newMedicalGroupsPage"
                                      :items-per-page="newMedicalGroupItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12"
                                      @page-count="newMedicalGroupPageCount = $event"></v-data-table>
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
                                      :items="pharma.indications"
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
                                      :items="pharma.sideEffects"
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
                                      :items="pharma.contraIndications"
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

        <v-dialog v-model="showMedicalGroups" width="800" persistent>
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
                    <v-data-table v-model="selectedMedicalGroups"
                                  item-key="id"
                                  :headers="medicalGroupsHeaders"
                                  :items="medicalGroups"
                                  :page.sync="newMedicalGroupsPage"
                                  :items-per-page="newMedicalGroupItemsPerPage"
                                  hide-default-footer
                                  show-select
                                  class="elevation-1 col-12"
                                  @page-count="newMedicalGroupPageCount = $event"></v-data-table>
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

        <v-dialog v-model="showSymptoms" width="800" persistent>
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

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSideEffectsPage" :length="newSideEffectPageCount"></v-pagination>
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
                                      :page.sync="newDiseasesPage"
                                      :items-per-page="newDiseaseItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newDiseasePageCount = $event">
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newDiseasesPage" :length="newDiseasePageCount"></v-pagination>
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

        <v-dialog v-model="showContra" width="800" persistent>
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

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newDiseasesPage" :length="newDiseasePageCount"></v-pagination>
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

    export default class DetailOfPharmaComponnent extends Vue {
        loading: boolean = true;
        pharma: any = null;
        newCommercialName: any = "";
        showMedicalGroups: boolean = false;
        showIndication: boolean = false;
        showContra: boolean = false;
        showSymptoms: boolean = false;
        dosages: any[] = [translations[userStore.getCulture()].dosage.once, translations[userStore.getCulture()].dosage.twice, translations[userStore.getCulture()].dosage.other];

        selectedCommercialTitle: any[] = [];
        commercialNameHeaders: any[] = [];
        selectedMedicalGroups: any[] = [];
        medicalGroupsHeaders: any[] = [];
        medicalGroups: any[] = [];
        newMedicalGroupsPage: number = 1;
        newMedicalGroupPageCount: number = 0;
        newMedicalGroupItemsPerPage: number = 50;
        selectedIndication: any[] = [];
        diseaseHeaders: any[] = [];
        diseases: any[] = [];
        newDiseasesPage: number = 1;
        newDiseasePageCount: number = 0;
        newDiseaseItemsPerPage: number = 50;
        selectedContraindication: any[] = [];
        selectedSideEffects: any[] = [];
        sideEffectHeaders: any[] = [];
        symptoms: any[] = [];
        newSideEffectsPage: number = 1;
        newSideEffectPageCount: number = 0;
        newSideEffectItemsPerPage: number = 50;

        addNewCommercialName() {
            if (this.newCommercialName) {
                let newName = { id: this.pharma.commercialNames.length + 1, name: this.newCommercialName };
                this.pharma.commercialNames.push(newName);
                this.newCommercialName = "";
            }
        }

        deleteCommercialName() {
            this.selectedCommercialTitle.forEach(x => {
                this.pharma.commercialNames.splice(this.pharma.commercialNames.indexOf(x), 1);
            });

            this.selectedCommercialTitle = [];
        }

        closeNew() {
            this.selectedMedicalGroups = [];
            this.selectedIndication = [];
            this.selectedContraindication = [];
            this.selectedSideEffects = [];

            this.showMedicalGroups = false;
            this.showIndication = false;
            this.showContra = false;
            this.showSymptoms = false;
        }

        addNew() {
            if (this.selectedMedicalGroups.length > 0)
                this.pharma.medicalGroups = this.selectedMedicalGroups;
            if (this.selectedIndication.length > 0)
                this.pharma.indications = this.selectedIndication;
            if (this.selectedContraindication.length > 0)
                this.pharma.contraIndications = this.selectedContraindication;
            if (this.selectedSideEffects.length > 0)
                this.pharma.sideEffects = this.selectedSideEffects;

            this.closeNew();
        }

        addSymptoms() {
            this.selectedSideEffects = this.pharma.sideEffects;

            this.showSymptoms = true;
        }

        addMedicalGroups() {
            this.selectedMedicalGroups = this.pharma.medicalGroups;

            this.showMedicalGroups = true;
        }

        addIndicationDiseases() {
            this.selectedIndication = this.pharma.indications;

            this.showIndication = true;
        }

        addContraindicationDiseases() {
            this.selectedContraindication = this.pharma.contraIndications;

            this.showContra = true;
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.commercialNameHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.sideEffectHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
                { id: 1, binding: null, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, binding: null, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, binding: null, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];

            this.medicalGroupsHeaders = [
                { text: translations[userStore.getCulture()].medicalGroup.character, value: 'character' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' }
            ];

            this.medicalGroups = [
                {
                    id: 1,
                    binding: 10,
                    character: "Test"
                }
            ];

            this.diseaseHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

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

            let pharmas = [
                {
                    id: 0,
                    name: "",
                    commercialNames: [],
                    medicalGroups: [],
                    indications: [],
                    contraIndications: [],
                    sideEffects: [],
                    dosage: "",
                    minDosage: null,
                    maxDosage: null
                },
                {
                    id: 1,
                    name: "Fusce",
                    commercialNames: [{ id: 1, name: "Fusce - test" }],
                    medicalGroups: [this.medicalGroups[0]],
                    indications: [this.diseases[0], this.diseases[1]],
                    contraIndications: [this.diseases[2]],
                    sideEffects: [this.symptoms[0], this.symptoms[1]],
                    dosage: translations[userStore.getCulture()].dosage.twice,
                    minDosage: 2,
                    maxDosage: 8
                },
            ];

            this.pharma = pharmas.find(x => x.id == this.$attrs.pharmaId);

            this.loading = false;
        }
    }
</script>

<style>
</style>