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
                {{ $locale({i: 'news.title'}) }}
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
                                <span class="bold-span">{{ $locale({i: 'news.text'}) }}</span>
                                <v-textarea v-model="news.text"></v-textarea>
                            </v-flex>
                        </v-layout>

                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'news.href'}) }}</span>
                                <v-text-field v-model="news.href"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-form>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'news.diseases'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="margin-15">
                            <div class="row">
                                <v-btn color="success" class="back-button" @click="addDiseases">
                                    {{ $locale({i: 'common.addNew'}) }}
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn v-if="selectedDiseases.length > 0" color="error">
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                                <v-btn v-else disabled>
                                    {{ $locale({i: 'common.delete'}) }}
                                </v-btn>
                            </div>
                        </div>
                        <v-data-table v-model="selectedDiseases"
                                      item-key="id"
                                      :headers="diseasesHeaders"
                                      :items="news.diseases"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1 col-12">
                        </v-data-table>
                    </v-tab-item>
                </v-tabs>

            </v-card-text>
        </v-card>

        <v-dialog v-model="showDiseases" width="800" persistent>
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
                                      v-model="selectedDiseases"
                                      :headers="diseasesHeaders"
                                      :items="diseases"
                                      hide-default-footer
                                      show-select
                                      class="elevation-1">
                        </v-data-table>
                    </div>

                    <div class="row" v-for="item in selectedDiseases" :key="item.id">
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

    export default class DetailOfNewsComponnent extends Vue {
        loading: boolean = true;
        news: any = [];
        diseases: any[] = [];
        diseasesHeaders: any[] = [];
        selectedDiseases: any[] = [];
        showDiseases: boolean = false;
        bindings: any[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        goBack() {
            window.history.back();
        }

        closeNew() {
            this.selectedDiseases = [];

            this.showDiseases = false;

            this.diseases.forEach(x => x.binding = null);
        }

        addNew() {
            if (this.selectedDiseases.length > 0)
                this.news.diseases = JSON.parse(JSON.stringify(this.selectedDiseases));

            this.closeNew();
        }

        addDiseases() {
            this.selectedDiseases = this.news.diseases;

            if (this.news.diseases.length > 0) {
                this.diseases.forEach(x => {
                    let data = this.news.diseases.find(c => c.id == x.id);
                    if (data)
                        x.binding = data.binding;
                });
            }

            this.showDiseases = true;
        }

        created() {
            this.diseasesHeaders = [
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

            let news = [
                {
                    id: 0,
                    text: "",
                    href: "",
                    diseases: []
                },
                {
                    id: 1,
                    text: "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aliquam erat volutpat. In convallis. Nam sed tellus id magna elementum tincidunt. Nulla pulvinar eleifend sem. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Fusce tellus. Mauris elementum mauris vitae tortor. Etiam dui sem, fermentum vitae, sagittis id, malesuada in, quam. Quisque porta. Fusce dui leo, imperdiet in, aliquam sit amet, feugiat eu, orci. In dapibus augue non sapien. Mauris elementum mauris vitae tortor. Maecenas fermentum, sem in pharetra pellentesque, velit turpis volutpat ante, in pharetra metus odio a lectus.",
                    href: "http://www.lorem-ipsum.cz/?action=generate&lang=cz&par=5&start_w_lipsum=on",
                    diseases: diseasesCopy
                }
            ];

            this.news = news.find(x => x.id == this.$attrs.newsId);

            this.loading = false;
        }
    }
</script>

<style>
</style>