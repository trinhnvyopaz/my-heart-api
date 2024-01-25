<template>
    <div>
        <div class="margin-15" v-if="loading">
            <v-progress-linear indeterminate
                               color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="addInformation()">{{ $locale({i: 'common.addNew'}) }}</v-btn>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="search"
                                      style="float: right; margin-right: 15px"
                                      class="col-8"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.byNameAndDate'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.byNameAndDate'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="informations"
                          :search="search"
                          :page.sync="page"
                          :items-per-page="itemsPerPage"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCount = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openInformation(item)">
                            <td>{{ item.name }}</td>
                            <td>{{ item.author.firstName }} {{ item.author.lastName }}</td>
                            <td>{{ item.createdDate }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>
        </v-card>

        <v-dialog v-model="openInformationControl" width="1000" fixed-header persistent>
            <v-card>
                <v-card-title primary-title class="title" style="background-color: ghostwhite">
                    {{ $locale({i: 'common.information'}) }}
                </v-card-title>

                <v-card-text>
                    <div>
                        <v-form>
                            <v-container>
                                <v-layout>
                                    <v-flex xs12
                                            md3>
                                        <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                        <v-text-field v-model="newInformation.name"></v-text-field>
                                    </v-flex>

                                    <v-flex xs12
                                            md3>
                                    </v-flex>
                                </v-layout>

                                <v-layout>
                                    <v-flex xs12
                                            md12>
                                        <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                        <v-textarea solo v-model="newInformation.description"></v-textarea>
                                    </v-flex>
                                </v-layout>

                            </v-container>
                        </v-form>
                    </div>
                </v-card-text>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="close()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    <v-btn color="error"
                           v-on:click="close()">
                        {{ $locale({i: 'common.delete'}) }}
                    </v-btn>
                    <v-btn color="success"
                           v-on:click="close()">
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
    import { useModule } from "vuex-simple";

    import { InformationModel } from '../../../../../backend/entities/informationModel';
    import { translations } from '../../../../../utils/translations';
    import userStore from '@backend/store/userStore';

    import informationComponnent from './informationComponnent';

    @Component({
        components: {
            "information": informationComponnent
        }
    })

    export default class MainInformationComponnent extends Vue {
        loading: boolean = true;
        search: string = "";
        headers: any[] = [];
        informations: InformationModel[] = [];
        newInformation: InformationModel = {} as any;
        selectedInformation: InformationModel = {} as any;
        openInformationControl: boolean = false;

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;

        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        addInformation() {
            this.openInformationControl = true;
        }

        openInformation(item) {
            let fromObject = { text: document.title, to: this.$route.path };
            this.breadcrumbStore.Add(fromObject);

            this.$router.push({ path: '/information/' + item.id });
        }

        close() {
            this.openInformationControl = false;
            this.newInformation.author = {} as any;
            this.newInformation = {} as any;
            this.selectedInformation = {} as any;
            this.init();
        }

        init() {
            this.newInformation.author = {} as any;

            this.headers = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' },
                { text: translations[userStore.getCulture()].common.author, value: 'author', sortable: false },
                { text: translations[userStore.getCulture()].common.createdDate, value: 'createdDate' },
            ];

            this.informations = [
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
        }

        created() {
            this.init();

            this.loading = false;
        }
    }
</script>

<style>
</style>