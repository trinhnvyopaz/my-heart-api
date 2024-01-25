<template>
    <div class="margin-15">
        <div v-if="loading">
            <v-progress-linear indeterminate color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="openFarmak(null)">{{ $locale({i: 'common.addNew'}) }}</v-btn>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
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
                          :items="items"
                          hide-default-footer
                          class="elevation-1">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openFarmak(item)">
                            <td>{{ item.character }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right"></v-pagination>
        </v-card>
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

    export default class ListOfPreventiveMeasuresComponnent extends Vue {
        loading: boolean = true;
        search: string = "";
        headers: any[] = [];
        items: any[] = [];

        openFarmak(item) {
            if (item)
                this.$router.push({ path: '/preventive-measure/' + item.id });
            else
                this.$router.push({ path: '/preventive-measure/' + 0 });
        }

        created() {
            this.headers = [
                { text: translations[userStore.getCulture()].medicalGroup.character, value: 'character' }
            ];

            this.items = [
              {
                    id: 1,
                    character: "Testing"
                }
            ];

            this.loading = false;
        }
    }
</script>

<style></style>
