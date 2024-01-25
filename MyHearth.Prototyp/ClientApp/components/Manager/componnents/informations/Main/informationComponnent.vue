<template>
    <div>
        <v-breadcrumbs v-if="breadcrumbItems.length > 0" :items="breadcrumbItems" large></v-breadcrumbs>

        <v-card class="margin-15">
            <v-card-title primary-title class="title" style="background-color: ghostwhite">
                <v-btn color="primary" @click="goBack()" class="back-button"> {{ $locale({i: 'common.back'}) }}</v-btn>
                {{ $locale({i: 'common.information'}) }} {{information.name}}
            </v-card-title>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                    <v-text-field v-model="information.name"></v-text-field>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.createdDate'}) }}</span><br />
                                    <div style="margin-top: 20px">
                                        <span class="readonly-information">{{information.createdDate}}</span><br />
                                    </div>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.author'}) }}</span><br />
                                    <div style="margin-top: 20px">
                                        <span class="readonly-information">{{information.author.firstName}} {{information.author.lastName}}</span><br />
                                    </div>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <v-textarea solo v-model="information.description"></v-textarea>
                                </v-flex>
                            </v-layout>

                        </v-container>
                    </v-form>
                </div>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'information.symptoms'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'information.medicine'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="row">
                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <v-btn color="success" class="margin-15" style="float: left" v-on:click="addSymptom()">{{ $locale({i: 'common.addNew'}) }}</v-btn>
                            </div>

                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <v-tooltip top>
                                    <template v-slot:activator="{ on }">
                                        <v-text-field v-on="on"
                                                      v-model="searchSymptom"
                                                      append-icon="search"
                                                      :label="$locale({i: 'filter.name'})"
                                                      single-line
                                                      hide-details></v-text-field>
                                    </template>
                                    <span>{{ $locale({i: 'filter.name'}) }}</span>
                                </v-tooltip>
                            </div>
                        </div>

                        <v-data-table id="infomrationDetailTable"
                                      v-model="selectedDeleteSymptom"
                                      :headers="symptomsHeaders"
                                      :items="symptoms"
                                      :search="searchSymptom"
                                      :page.sync="symptomsPage"
                                      :items-per-page="symptomsItemsPerPage"
                                      hide-default-footer
                                      class="elevation-1"
                                      @page-count="symptomsPageCount = $event">

                            <template v-slot:body="{ items }">
                                <tbody>
                                    <tr v-for="item in items" :key="item.id">
                                        <td v-on:click="openSymptom(item)">{{ item.name }}</td>
                                        <td v-on:click="openSymptom(item)">{{ item.binding }}</td>
                                        <td style=" width: 1%" v-on:click="deleteSymptom(item)">
                                            <font-awesome-icon class="table-icon-trash" icon="trash" />
                                        </td>
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
            </v-card-text>

            <v-divider></v-divider>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.delete'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-actions>
        </v-card>

        <v-dialog v-model="addNewSymptom" width="800" persistent>
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
                                      v-model="selectedNewSymptom"
                                      :headers="newHeaders"
                                      :items="newSymptoms"
                                      :search="searchNewSymptom"
                                      :page.sync="newSymptomPage"
                                      :items-per-page="newSymptomItemsPerPage"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1"
                                      @page-count="newSymptomPageCount = $event">
                        </v-data-table>

                        <div class="row" v-for="item in selectedNewSymptom" :key="item">
                            <div class="col-5" style="text-align: center; margin: auto">
                                {{item.name}}
                            </div>
                            <div class="col-6">
                                <v-select :items="bindings"
                                          :label="$locale({i: 'common.binding'})"></v-select>
                            </div>
                        </div>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="newSymptomPage" :length="newSymptomPageCount"></v-pagination>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeNew()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="error"
                           v-on:click="closeNew()">
                        {{ $locale({i: 'common.delete'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="closeNew()">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import { InformationModel } from '../../../../../backend/entities/informationModel';
    import { SymptomModel } from '../../../../../backend/entities/symptomModel';
    import { MedicineModel } from '../../../../../backend/entities/medicineModel';
    import { translations } from '../../../../../utils/translations';
    import userStore from '@backend/store/userStore'

    @Component({
        components: {
        }
    })

    export default class InformationComponnent extends Vue {
        //@Prop() information;

        information: InformationModel = {} as any;
        checkbox: boolean = false;

        searchSymptom: string = "";
        medicineSymptom: string = "";

        symptomsHeaders: any[] = [];
        medicinesHeaders: any[] = [];
        symptoms: SymptomModel[] = [];
        medicines: MedicineModel[] = [];

        symptomsPage: number = 1;
        symptomsPageCount: number = 0;
        symptomsItemsPerPage: number = 5;
        medicinesPage: number = 1;
        medicinesPageCount: number = 0;
        medicinesItemsPerPage: number = 5;

        addNewSymptom: boolean = false;
        newHeaders: any[] = [];
        newSymptoms: SymptomModel[] = [];
        searchNewSymptom: string = '';
        newSymptomPage: number = 1;
        newSymptomPageCount: number = 0;
        newSymptomItemsPerPage: number = 50;
        selectedNewSymptom: any[] = [];
        selectedDeleteSymptom: any[] = [];
        bindings: any[] = [1,2,3,4,5,6,7,8,9,10];

        breadcrumbItems: any[] = [];
        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        goBack() {
            window.history.back();
        }

        addSymptom() {
            this.addNewSymptom = true;
        }

        deleteSymptom(item) {
            alert(translations[userStore.getCulture()].common.notInDemo);
        }

        openSymptom(item) {
            this.breadcrumbStore.Remove(this.breadcrumbStore.breadcrumbs[this.breadcrumbStore.breadcrumbs.length - 1]);

            let fromObject = { text: document.title, to: this.$route.path };
            this.breadcrumbStore.Add(fromObject);

            this.$router.push({ path: '/symptom/' + item.id });
        }

        addMedicine() {

        }

        close() {
            this.$emit("dialog-closed");
        }

        closeNew() {
            this.addNewSymptom = false;
            this.selectedNewSymptom = [];
            this.selectedDeleteSymptom = [];
        }

        init() {
            this.symptomsHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
                { text: translations[userStore.getCulture()].common.binding, value: 'binding' },
            ];

            this.medicinesHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
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

            this.medicines = [
                { id: 1, name: "Lorem ipsum", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, name: "Maecenas aliquet", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, name: "Morbi", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];

            this.newHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.newSymptoms = [
                { id: 1, name: "Morbi", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, name: "Mauris", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, name: "Curabitur", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 4, name: "Morbi2", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 5, name: "Mauris2", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 6, name: "Curabitur2", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 7, name: "Morbi3", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 8, name: "Mauris3", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 9, name: "Curabitur3", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 10, name: "Morbi4", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 11, name: "Mauris4", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 12, name: "Curabitur4", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 13, name: "Morbi5", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 14, name: "Mauris5", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 15, name: "Curabitur5", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 16, name: "Morbi6", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 17, name: "Mauris6", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 19, name: "Curabitur6", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },
                { id: 20, name: "Morbi7", binding: 3, description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 21, name: "Mauris7", binding: 3, description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 22, name: "Curabitur7", binding: 3, description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];

            let informations = [
                {
                    id: 1,
                    name: "Etiam",
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

            this.information = informations.find(x => x.id == this.$attrs.informationId);
        }

        created() {
            if (this.breadcrumbStore.breadcrumbs.length > 2 && this.breadcrumbStore.breadcrumbs[this.breadcrumbStore.breadcrumbs.length - 1].text == translations["cs-CZ"].breadcrumbNames.find(x => x.name == "symptomDetail").translation) {
                this.breadcrumbStore.Remove(this.breadcrumbStore.breadcrumbs[this.breadcrumbStore.breadcrumbs.length - 1]);
                this.breadcrumbStore.Remove(this.breadcrumbStore.breadcrumbs[this.breadcrumbStore.breadcrumbs.length - 1]);
            }

            if (this.breadcrumbStore.breadcrumbs.length > 0) {
                let toObject = { text: document.title };
                this.breadcrumbStore.Add(toObject);

                this.breadcrumbItems = this.breadcrumbStore.breadcrumbs;
            }

            this.init();
        }

    }
</script>

<style>
    .table-icon-trash {
        color: #FF5252;
        font-size: 25px;
        cursor: pointer;
        float: right;
    }

    #newSymptomTable table > tbody :hover {
        cursor: default !important;
        background-color: transparent !important;
    }

    .bold-span {
        font-weight: bold;
    }

    .readonly-information {
        font-weight: bold;
        color: black;
    }

    .back-button {
        margin-right: 15px;
    }
</style>