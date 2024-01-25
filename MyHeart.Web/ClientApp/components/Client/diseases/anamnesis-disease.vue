<template>
    <v-container>
            <v-flex xs12>
                <v-progress-linear v-if="loadingAnamnesis" color="error" indeterminate />
                <localized-data-table   :items="anamneses"
                                        :headers="headers"
                                        v-if="!loadingAnamnesis"
                                        @click:row="rowClicked"
                                        hide-default-footer>

                    <template v-slot:item.description="{item}">
                        <span v-html="item.disease.description" />
                    </template>

                    <template v-slot:item.startDateString="{ item }">
                        {{ new Date(item.startDateString).toLocaleDateString() }}
                    </template>
                </localized-data-table>
                <v-layout justify-end>
                    <v-row>
                        <v-col></v-col>
                        <v-col cols="8">
                            <v-pagination @input="loadAnamnesis"
                                        v-model="requestAnamnesis.page"
                                        :length.sync="requestAnamnesis.pageCount"
                                        :total-visible="requestAnamnesis.pageSize" />
                        </v-col>
                        <v-col align="right">
                            <v-btn @click="displayNewItemModal()">
                                Přidat
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-layout>
            </v-flex>

            <simple-edit-dialog v-model="showEditItemModal"
                                :isNew="false"
                                title="Upravit onemocnění"
                                @save="editItem"
                                @delete="removeItem">
                <div v-if="selectedItem">

                    <v-menu ref="menu2"
                            v-model="menu2"
                            :close-on-content-click="false"
                            :return-value.sync="selectedItem.startDateString"
                            transition="scale-transition"
                            offset-y
                            min-width="auto">
                        <template v-slot:activator="{ on, attrs }">
                            <v-text-field v-model="new Date(selectedItem.startDateString).toLocaleDateString()"
                                            label="Datum vzniku"
                                            prepend-icon="mdi-calendar"
                                            readonly
                                            v-bind="attrs"
                                            v-on="on"></v-text-field>
                        </template>
                        <v-date-picker v-model="selectedItem.startDateString"
                                        class="custom-datepicker"
                                        no-title
                                        scrollable>
                            <v-spacer></v-spacer>
                            <v-btn text
                                    @click="menu2 = false">
                                Cancel
                            </v-btn>
                            <v-btn text
                                    @click="$refs.menu2.save(selectedItem.startDateString)">
                                OK
                            </v-btn>
                        </v-date-picker>
                    </v-menu>

                    <v-text-field v-model="selectedItem.note" label="Poznámka" />
                </div>
        </simple-edit-dialog>

        <v-dialog v-model="showNewItemModal" scrollable max-width="800">
                <v-container style="display: flex; flex-direction: column;">
                    <v-row style="flex: none;">
                        <v-col>
                            <h3 style="margin-bottom: 25px">Přidat onemocnění</h3>
                        </v-col>
                        <v-col class="d-flex justify-end">
                            <svg class="closeButton" @click="showNewItemModal=false"><use href="#icon_close"></use></svg>
                        </v-col>
                    </v-row>
                    <div style="flex-grow: 1; overflow-y: auto;">
                        <localized-data-table :items="diseases"
                                        :headers="headers"
                                        :calculate-widths="true"
                                        :page.sync="request.page"
                                        :items-per-page="request.pageSize"
                                        hide-default-footer
                                        @page-count="totalCount = $event"
                                        dense>
                            <template v-slot:top>
                                <v-text-field   ref="searchField"
                                                v-model="request.filter"
                                                @keyup.enter="searchDiseases()"
                                                label="Název onemocnění (min 3 znaky)"
                                                @click:append="searchDiseases()"
                                                :loading="loadingDiseases">
                                    <template v-slot:append>
                                        <v-icon color="primary">mdi-magnify</v-icon>
                                    </template>
                                </v-text-field>

                            </template>

                            <template v-slot:item.description="{item}" :width="256">
                                <span v-html="item.disease.description" />
                            </template>

                            <template v-slot:item.startDateString="{item}">

                                <v-menu ref="dateMenu"
                                        v-model="item.showDateMenu"
                                        :close-on-content-click="false"
                                        transition="scale-transition"
                                        offset-y
                                        min-width="auto">
                                    <template v-slot:activator="{ on, attrs }">
                                        <v-text-field v-model="new Date(item.startDateString).toLocaleDateString()"
                                                        prepend-icon="mdi-calendar"
                                                        readonly
                                                        v-bind="attrs"
                                                        v-on="on"
                                                        dense
                                                        color="error"></v-text-field>
                                    </template>
                                    <v-date-picker v-model="tempDate"
                                                    class="custom-datepicker"
                                                    no-title
                                                    scrollable>
                                        <v-spacer></v-spacer>
                                        <v-btn text
                                                color="primary"
                                                @click="item.showDateMenu = false">
                                            Cancel
                                        </v-btn>
                                        <v-btn text
                                                color="primary"
                                                @click="assignDate(item)">
                                            OK
                                        </v-btn>
                                    </v-date-picker>
                                </v-menu>

                            </template>

                            <template v-slot:item.note="{item}">
                                <v-text-field v-model="item.note" dense color="error" />
                            </template>

                            <template v-slot:item.actions="{ item }">
                                <v-btn icon small @click="save(item)">
                                    <v-icon small>mdi-arrow-right-bold</v-icon>
                                </v-btn>
                            </template>

                        </localized-data-table>
                    </div>

                    <v-pagination @input="loadDiseases()" v-model="request.page" :length="request.pageCount" :total-visible="request.pageSize" color="error" />
                </v-container>
        </v-dialog>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis';
    import UserDto from "../../../models/user/UserDto";
    import AuthStore from "@backend/store/authStore";
    import SimpleEditDialog from "@components/Shared/simpleEditDialog.vue";
    import DataTableRequest from "../../../../models/DataTableRequest";


    @Component({
        name: "AnamnesisAbusus",
        components: {
            SimpleEditDialog
        }
    })
    export default class AnamnesisDisease extends Vue {

        menu: boolean = false;
        tempDate: string = "";
        menu2: boolean = false;

        loadingAnamnesis: boolean = false;
        loadingDiseases: boolean = false;

        showNewItemModal: boolean = false;
        showEditItemModal: boolean = false;

        anamneses: any[] = [];
        diseases: any[] = [];

        searchString: string = null;

        isImpersonating: boolean = false;
        user: UserDto = null;

        headers: any[] = [
            { text: 'Název', value: 'disease.name' },
            //{ text: 'Popis', value: 'description' },
            { text: 'Datum vzniku', value: 'startDateString' },
            { text: 'Poznámka', value: 'note' },
            { text: '', value: 'actions', align: 'end' }
        ];

        selectedItem: any = null;

        request: DataTableRequest = { page: 1, pageSize: 10, filter: "", pageCount: 0 };
        totalCount: number = 0;
        pageCount: number = 0;
        requestAnamnesis: DataTableRequest = { page: 1, pageSize: 10, filter: "", pageCount: 0 };
        mounted() {
            this.isImpersonating = AuthStore.isImpersonating();
            if (this.isImpersonating) {
                this.user = AuthStore.getImpersonatedUser() as UserDto;
            } else {
                this.user = AuthStore.getCurrentUser() as UserDto;
            }

            this.loadAnamnesis();
        }


        searchDiseases() {
            this.request.page = 1;
            this.loadDiseases();
        }

        async loadDiseases() {
            this.loadingDiseases = true;

            var response = await anamnesisApi.getDiseases(this.request);
            console.warn('RESPONSE', response);
            this.request.totalCount = response.data.totalCount;
            this.request.pageSize = response.data.pageSize;
            this.request.pageCount = Math.ceil(response.data.totalCount / response.data.pageSize);
            this.diseases = response.data.data.map(obj => {
                return {
                    diseaseId: obj.id,
                    disease: obj,
                    startDateString: new Date().toISOString().split('T')[0],
                    note: ''
                };
            })
            this.loadingDiseases = false;


            //if (!this.searchString.length || this.searchString.length < 3)
            //    return;

            //this.loadingDiseases = true;

            //var result = await anamnesisApi.getDiseasesByName(this.searchString);

            //console.warn('search result', result);

            //this.diseases = result.data.map(obj => {
            //    return {
            //        diseaseId: obj.id,
            //        disease: obj,
            //        startDateString: new Date().toISOString().split('T')[0],
            //        note: ''
            //    };
            //})

            //this.loadingDiseases = false;
        }

        async loadAnamnesis() {
            this.loadingAnamnesis = true;

            if (this.isImpersonating) {
                var result = await anamnesisApi.getDiseaseAnamnesesFromPerson(this.user.id, this.requestAnamnesis);
            } else {
                var result = await anamnesisApi.getDiseaseAnamneses(this.requestAnamnesis);
            }

            this.anamneses = result.data.data;
            this.requestAnamnesis.totalCount = result.data.totalCount;
            this.requestAnamnesis.pageSize = result.data.pageSize;
            this.requestAnamnesis.pageCount = Math.ceil(result.data.totalCount / result.data.pageSize);

            this.loadingAnamnesis = false;
        }

        async save(item) {
            this.showNewItemModal = false;
            this.loadingAnamnesis = true;

            item.userId = this.user.id;
            await anamnesisApi.saveDiseaseAnamnesis(item);

            this.loadAnamnesis();
        }

        async removeItem() {
            this.loadingAnamnesis = true;
            await anamnesisApi.deleteDiseaseAnamnesis(this.selectedItem.id);
            this.loadAnamnesis();
        }

        rowClicked(item, args) {
            this.displayEditItemModal(item);
        }

        displayEditItemModal(item) {
            this.selectedItem = Object.assign({}, item);
            this.showEditItemModal = true;
        }

        async editItem() {
            this.loadingAnamnesis = true;

            this.selectedItem.userId = this.user.id;
            await anamnesisApi.saveDiseaseAnamnesis(this.selectedItem);
            this.showEditItemModal = false;
            this.loadAnamnesis();
        }

        async displayNewItemModal() {
            this.searchString = null;
            this.request.filter = '';

            this.loadDiseases();

            this.showNewItemModal = true;

            setTimeout(() => {
                this.$refs.searchField.focus();
            }, 100);

        }

        assignDate(item) {
            item.startDateString = this.tempDate;
            item.showDateMenu = false;
        }

    }
</script>


<style scoped>


</style>

