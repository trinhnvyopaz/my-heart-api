<template>
    <div>
        <localized-data-table :headers="headers"
                      :items="data"
                      :loading="loading"
                      :page.sync="request.page"
                      :items-per-page="request.pageSize"
                      hide-default-footer
                      @page-count="totalCount = $event"
                      @click:row="rowClicked"
                      :class="$listeners && $listeners['click:row'] ? 'clickable' : ''">

            <template #top>
                <v-text-field v-model="request.filter"
                              v-if="searchPlaceholder"
                              :placeholder="searchPlaceholder"
                              @keyup.enter="search"
                              class="search">
                    <template v-slot:prepend-inner>
                        <v-icon class="mt-1">mdi-magnify</v-icon>
                    </template>
                    <template v-slot:append>
                        <v-btn @click="search">Vyhledat</v-btn>
                    </template>
                </v-text-field>
            </template>
            <template v-for="(_, slot) in $scopedSlots" v-slot:[slot]="props">
                <slot :name="slot" v-bind="props" />
            </template>
        </localized-data-table>

        <v-row>
            <v-col></v-col>
            <v-col cols="8">
                <v-pagination @input="refresh"
                              v-model="request.page"
                              :length.sync="pageCount"
                              :total-visible="9" />
            </v-col>
            <v-col align="right">
                <v-btn v-if="$listeners && $listeners['click:add']" class="custom" @click="$emit('click:add')">
                    Přidat
                </v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import { Component, Prop, Watch } from "vue-property-decorator";
    import GroupedDataTableRequest from "../../../models/GroupedDataTableRequest";

    @Component({
        name: "CustomDataTable"
    })
    export default class CustomDataTable extends Vue {
        request: GroupedDataTableRequest = { page: 1, pageSize: 10, filter: "", secondFilter: "" };
        loading: boolean = false;
        totalCount: number = 0;
        pageCount: number = 0;

        @Prop(Function) readonly getData;
        @Prop(Array) readonly headers;
        @Prop(String) readonly searchPlaceholder;
        @Prop({ default: true }) readonly serverPaginated;
        @Prop({ default: () => [0, 1, 2] }) readonly allowedGroups: number[];

        data: any = [];

        mounted() {
            this.refresh();
        }
        search() {
            this.request.page = 1;
            this.refresh();
        }
        async refresh() {
            this.loading = true;

            this.request.groups = this.allowedGroups
       
            var result = await this.getData(this.request);

            if (result.success) {
                if (this.serverPaginated) {
                    this.data = result.data.data;
                    this.totalCount = result.data.totalCount;
                    this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
                }
                else {
                    this.data = result.data;
                }
            }
            this.loading = false;
        }

        rowClicked(item, args) {
            this.$emit('click:row', item, args);
        }
    }
</script>
<style scoped>

</style>