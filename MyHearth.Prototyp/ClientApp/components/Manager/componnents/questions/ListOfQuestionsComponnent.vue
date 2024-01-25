<template>
    <div class="margin-15">
        <div v-if="loading">
            <v-progress-linear indeterminate color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-md-2 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="openQuestion(null)">{{ $locale({i: 'common.addNew'}) }}</v-btn>
            </div>

            <div class="col-md-10 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      style="float: right; margin-right: 15px"
                                      class="col-3"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.patient'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.patient'}) }}</span>
                </v-tooltip>

                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      style="float: right; margin-right: 15px"
                                      class="col-3"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.time'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.time'}) }}</span>
                </v-tooltip>

                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      style="float: right; margin-right: 15px"
                                      class="col-3"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.timeToEnd'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.timeToEnd'}) }}</span>
                </v-tooltip>

                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      style="float: right; margin-right: 15px"
                                      class="col-3"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.dosctor'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.dosctor'}) }}</span>
                </v-tooltip>

                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-switch v-on="on"
                                  style="float: right; margin-right: 15px"
                                  class="col-3"
                                  :label="$locale({i: 'filter.video'})"></v-switch>
                    </template>
                    <span>{{ $locale({i: 'filter.video'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="items"
                          hide-default-footer
                          class="elevation-1">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openQuestion(item)">
                            <td class="short-question">{{ item.subject }}</td>
                            <td>{{ item.client.firstName }} {{ item.client.lastName }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right"></v-pagination>
        </v-card>
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

    export default class ListOfQuestionsComponnent extends Vue {
        loading: boolean = true;
        search: string = "";
        headers: any[] = [];
        items: any[] = [];

        openQuestion(item) {
            alert(translations[userStore.getCulture()].common.notInDemo)
        }

        created() {
            this.headers = [
                { text: translations[userStore.getCulture()].common.subject, value: 'subject' },
                { text: translations[userStore.getCulture()].common.client, sortable: false, value: 'client' }
            ];

            this.items = [
                {
                    id: 2,
                    subject: "Test - active",
                    messages: [],
                    doctor: {
                        id: 2,
                        firstName: 'Roman',
                        lastName: 'Musil',
                        email: 'r.musil@test.com',
                        role: 1,
                        weight: 90,
                        height: 190,
                        healthProblems: '',
                        allergy: ''
                    },
                    client: {
                        id: 1,
                        firstName: 'Josef',
                        lastName: 'Jindra',
                        email: 'j.jindra@test.com',
                        role: 3,
                        weight: 90,
                        height: 190,
                        healthProblems: '',
                        allergy: ''
                    }
                }
            ];

            this.loading = false;
        }
    }
</script>

<style>
</style>