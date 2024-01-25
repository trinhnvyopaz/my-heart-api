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
                {{ $locale({i: 'medicalPlace.title'}) }}
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
                                <v-text-field v-model="medicalPlace.name"></v-text-field>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'medicalPlace.character'}) }}</span>
                                <v-select :items="characters" v-model="medicalPlace.character"></v-select>
                            </v-flex>
                            <v-flex xs12 md1></v-flex>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'medicalPlace.contact'}) }}</span>
                                <v-text-field v-model="medicalPlace.contact"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'preventiveMeasures.title'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'nonPharma.title'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addPreventiveMeasures">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedPreventiveMeasures.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedPreventiveMeasures"
                                      item-key="id"
                                      :headers="preventiveMeasuresHeaders"
                                      :items="medicalPlace.preventiveMeasures"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12">
                        </v-data-table>
                    </v-tab-item>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addNonPharmas">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedNonPharmas.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedNonPharmas"
                                      item-key="id"
                                      :headers="nonPharmasHeaders"
                                      :items="medicalPlace.nonPharmas"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12">
                        </v-data-table>
                    </v-tab-item>
                </v-tabs>

            </v-card-text>
        </v-card>

        <v-dialog v-model="showPreventiveMeasures" width="800" persistent>
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
                                      v-model="selectedPreventiveMeasures"
                                      :headers="preventiveMeasuresHeaders"
                                      :items="preventiveMeasures"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>

                    <div class="row" v-for="item in selectedPreventiveMeasures" :key="item.id">
                        <div class="col-5" style="text-align: center; margin: auto">
                            {{item.character}}
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

        <v-dialog v-model="showNonPharmas" width="800" persistent>
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
                                      v-model="selectedNonPharmas"
                                      :headers="nonPharmasHeaders"
                                      :items="nonPharmas"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>

                    <div class="row" v-for="item in selectedNonPharmas" :key="item.id">
                        <div class="col-5" style="text-align: center; margin: auto">
                            {{item.description}}
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
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import userStore from '@backend/store/userStore'
    import { translations } from "@utils/translations";

    @Component({
        components: {
        }
    })

    export default class DetailOfMedicalPlaceComponnent extends Vue {
        loading: boolean = true;
        medicalPlace: any = null;
        characters: any[] = [translations[userStore.getCulture()].medicalPlace.outpatient, translations[userStore.getCulture()].medicalPlace.beds];
        preventiveMeasures: any[] = [];
        preventiveMeasuresHeaders: any[] = [];
        nonPharmasHeaders: any[] = [];
        nonPharmas: any[] = [];
        selectedNonPharmas: any[] = [];
        selectedPreventiveMeasures: any[] = [];
        showPreventiveMeasures: boolean = false;
        showNonPharmas: boolean = false;
        bindings: any[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        goBack() {
            window.history.back();
        }

        addNonPharmas() {
            this.selectedNonPharmas = this.medicalPlace.nonPharmas;

            if (this.medicalPlace.nonPharmas.length > 0) {
                this.nonPharmas.forEach(x => {
                    let data = this.medicalPlace.nonPharmas.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showNonPharmas = true;
        }

        addPreventiveMeasures() {
            this.selectedPreventiveMeasures = this.medicalPlace.preventiveMeasures;

            if (this.medicalPlace.preventiveMeasures.length > 0) {
                this.preventiveMeasures.forEach(x => {
                    let data = this.medicalPlace.preventiveMeasures.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showPreventiveMeasures = true;
        }

        closeNew() {
            this.selectedPreventiveMeasures = [];
            this.selectedNonPharmas = [];

            this.showPreventiveMeasures = false;
            this.showNonPharmas = false;
        }

        addNew() {
            if (this.selectedPreventiveMeasures.length > 0)
                this.medicalPlace.preventiveMeasures = JSON.parse(JSON.stringify(this.selectedPreventiveMeasures));
            if (this.selectedNonPharmas.length > 0)
                this.medicalPlace.nonPharmas = JSON.parse(JSON.stringify(this.selectedNonPharmas));

            this.closeNew();
        }

        created() {
            this.preventiveMeasuresHeaders = [
                { text: translations[userStore.getCulture()].medicalGroup.character, value: 'character' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' }
            ];

            this.preventiveMeasures = [
                {
                    id: 1,
                    character: "Testing",
                    binding: null
                }
            ];

            this.nonPharmasHeaders = [
                { text: translations[userStore.getCulture()].common.description, value: 'description' },
                { text: translations[userStore.getCulture()].nonPharma.efficiency, value: 'efficiency' },
                { text: translations[userStore.getCulture()].nonPharma.lengthOfHospitalization, value: 'lengthOfHospitalization' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' },
            ];

            this.nonPharmas = [
                {
                    id: 1,
                    description: "Testing",
                    efficiency: 25,
                    binding: null,
                    lengthOfHospitalization: translations[userStore.getCulture()].lengthOfHospitalization.outpatient
                }
            ];

            let preventiveMeasuresCopy = JSON.parse(JSON.stringify(this.preventiveMeasures));
            preventiveMeasuresCopy[0].binding = 3;

            let nonPharmasCopy = JSON.parse(JSON.stringify(this.nonPharmas));
            nonPharmasCopy[0].binding = 3;

            let medicalPlaces = [
                {
                    id: 0,
                    name: "",
                    character: "",
                    contact: "",
                    preventiveMeasures: [],
                    nonPharmas: []
                },
                {
                    id: 1,
                    name: "testing - medical place",
                    character: this.characters[0],
                    contact: "+420 221 221 221",
                    preventiveMeasures: preventiveMeasuresCopy,
                    nonPharmas: nonPharmasCopy
                }
            ];

            this.medicalPlace = medicalPlaces.find(x => x.id == this.$attrs.medicalPlaceId);

            this.loading = false;
        }
    }
</script>

<style>
</style>