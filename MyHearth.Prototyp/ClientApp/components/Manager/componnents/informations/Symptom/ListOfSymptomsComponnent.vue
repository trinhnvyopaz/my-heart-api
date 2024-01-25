<template>
    <div>
        <div class="margin-15" v-if="loading">
            <v-progress-linear indeterminate
                               color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="openSymptom(null)">{{ $locale({i: 'common.addNew'}) }}</v-btn>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="search"
                                      style="float: right; margin-right: 15px"
                                      class="col-8"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.name'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.name'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="symptoms"
                          :search="search"
                          :page.sync="page"
                          :items-per-page="itemsPerPage"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCount = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openSymptom(item)">
                            <td>{{ item.name }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>
        </v-card>

        <v-dialog v-model="openSymptomControl" width="1000" persistent>
            <symptom></symptom>
        </v-dialog>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import { SymptomModel } from '../../../../../backend/entities/symptomModel'
    import { translations } from '../../../../../utils/translations';
    import userStore from '@backend/store/userStore';

    import symptomComponnent from './symptomComponnentInfoDetail';

    @Component({
        components: {
            "symptom": symptomComponnent
        }
    })

    export default class ListOfMedicinesnComponnent extends Vue {
        loading: boolean = true;
        search: string = "";
        headers: any[] = [];
        symptoms: SymptomModel[] = [];
        newSymptom: SymptomModel = {} as any;
        openSymptomControl: boolean = false;

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;

        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        openSymptom(item) {
            //let fromObject = { text: document.title, to: this.$route.path };
            //this.breadcrumbStore.Add(fromObject);

            if (item)
                this.$router.push({ path: '/symptom/' + item.id });
            else
                this.$router.push({ path: '/symptom/' + 0 });
        }

        init() {
            this.headers = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
                { id: 1, binding: null, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, binding: null, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, binding: null, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
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