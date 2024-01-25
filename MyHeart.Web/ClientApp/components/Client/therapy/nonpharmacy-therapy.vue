<template>
    <div>
        <v-container grid-list-lg>

            <v-layout row wrap>

                <v-flex xs12>
                    <localized-data-table hide-default-footer :items="paginatedNonpharmacies" :headers="headers" @click:row="rowClicked">

                        <!-- <template v-slot:top>
                            <v-btn  @click="displayNewItemModal()">Přidat</v-btn>
                        </template> -->

                    </localized-data-table>
                    <v-layout justify-end>
                        <v-row>
                            <v-col></v-col>
                            <v-col cols="8">
                                <v-pagination @input="loadData"
                                            v-model="page"
                                            :length.sync="pageCount"
                                            :total-visible="itemsPerPage" />
                            </v-col>
                            <v-col align="right">
                                <v-btn @click="displayNewItemModal">
                                    Přidat
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-layout>
                </v-flex>

            </v-layout>

        </v-container>

        <v-dialog width="800" v-model="showNewItemModal" >
            <v-container>
                <div>
                    <v-row align="center">
                        <v-col>
                            <h3>Nový provedený výkon</h3>
                        </v-col>
                        <v-col class="d-flex justify-end">
                            <svg class="closeButton" @click="showNewItemModal=false"><use href="#icon_close"></use></svg>
                        </v-col>
                    </v-row>                    
                </div>
                <localized-data-table hide-default-footer :items="nonpharmacies" :headers="nonpharmaciesHeaders" :loading="loadingNonpharmacies" >
                        <template v-slot:top>
                            <v-text-field ref="searchField" 
                                            v-model="searchString" 
                                            label="Vyhledat" 
                                            @click:append="loadNonpharmacies" 
                                            @keyup.enter="loadNonpharmacies()">
                                    <template v-slot:append>
                                            <v-icon color="primary">mdi-magnify</v-icon>
                                    </template>   
                                </v-text-field>
                        </template>

                        <template v-slot:item.facilityName="{ item }">
                            <v-text-field dense v-model="item.facilityName" color="error" />
                        </template>

                        <template v-slot:item.note="{ item }">
                            <v-text-field dense v-model="item.note" color="error" />
                        </template>

                        <template v-slot:item.actions="{ item }">
                            <v-btn icon small @click="assignNonpharmacy(item)">
                                <v-icon small>mdi-arrow-right-bold</v-icon>
                            </v-btn>
                        </template>

                    </localized-data-table>
            </v-container>       
        </v-dialog>

        <simple-edit-dialog v-model="showEditModal"
                                :isNew="false"
                                title="Upravit"
                                @save="assignNonpharmacy(selectedItem)"
                                @delete="removeNonpharmacy">
                    <v-container grid-list-xl>
                        <v-layout row wrap>

                            <v-flex xs12 md6>
                                <v-text-field readonly v-model="selectedItem.nonpharmaticTherapy.name" label="Název" color="error"/>
                            </v-flex>

                            <v-flex xs12 md6>
                                <v-text-field v-model="selectedItem.facilityName" label="Pracoviště" color="error"/>
                            </v-flex>

                            <v-flex xs12>
                                <v-text-field v-model="selectedItem.note" label="Poznámka" color="error"/>
                            </v-flex>

                        </v-layout>
                    </v-container>   
        </simple-edit-dialog>
    </div>    
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import userNonpharmacyApi from '@backend/api/userNonpharmacy'
    import userNonpharmacy from "../../../backend/api/userNonpharmacy";
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";
    import SimpleEditDialog from "@components/Shared/simpleEditDialog.vue";

    @Component({
        name: "NonpharmacyTherapy",
        components:{
            SimpleEditDialog
        }
    })
    export default class NonpharmacyTherapy extends Vue {

        loadingData: boolean = false;
        loadingNonpharmacies: boolean = false;

        showNewItemModal: boolean = false;

        items: any[] = [];
        headers: any[] = [
            { text: 'Název', value: 'nonpharmaticTherapy.name' },
            { text: 'Pracoviště', value: 'facilityName' },
            { text: 'Poznámka', value: 'note' },
            { text: 'Smazat', value: 'actions' }
        ];

        nonpharmacies: any[] = [];
        nonpharmaciesHeaders: any[] = [
            { text: 'Název', value: 'nonpharmaticTherapy.name' },
            { text: 'Pracoviště', value: 'facilityName' },
            { text: 'Poznámka', value: 'note' },
            { text: 'Přidat', value: 'actions' }
        ];

        searchString: string = '';

        showEditModal: boolean = false;
        selectedItem: any = {
            nonpharmaticTherapy: {
                name: ''
            },
            facilityName: '',
            note: ''
        };
  
        page: number = 1;
        pageCount: number = 1;

        itemsPerPage = 10

        mounted() {
            this.loadData();
        }

        async loadData() {
            this.loadingData = true;

            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                var result = await userNonpharmacy.getNonpharmacyForPerson(impersonated.id);
            } else {
                var result = await userNonpharmacy.getNonpharmacy();
            }

            this.items = result.data;

            console.warn('items', this.items);

            this.loadingData = false;
        }

        get paginatedNonpharmacies() {
            const start = (this.page - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.items.slice(start, end);
        }

        async loadNonpharmacies() {
            if (!this.searchString || this.searchString.length < 3)
                return;

            this.loadingNonpharmacies = true;
            var result = await userNonpharmacyApi.getNonpharmaticTherapiesByName(this.searchString);
            this.nonpharmacies = result.data.map(obj => { return { nonpharmaticTherapyId: obj.id, nonpharmaticTherapy: obj, facilityName: '', note: '' } });
            this.loadingNonpharmacies = false;
        }

        async assignNonpharmacy(item) {
            this.showNewItemModal = false;
            this.showEditModal = false;
            this.loadingData = true;

            var user: UserDto = null;
            if (AuthStore.isImpersonating()) {
                user = AuthStore.getImpersonatedUser() as UserDto;
            } else {
                user = AuthStore.getCurrentUser() as UserDto;
            }

            item.userId = user.id;

            await userNonpharmacyApi.saveNonPharmacy(item);

            this.loadData();
        }

        async removeNonpharmacy() {
            this.loadingData = true;

            await userNonpharmacyApi.deleteNonpharmacy(this.selectedItem.id);

            this.loadData();
        }

        rowClicked(item, args) {
            this.displayEditModal(item);
        }

        displayEditModal(item) {
            this.selectedItem = Object.assign({}, item);
            console.warn(this.selectedItem);
            this.showEditModal = true;
        }

        displayNewItemModal() {
            this.searchString = null;
            this.nonpharmacies = [];
            this.showNewItemModal = true;

            setTimeout(() => {
                this.$refs.searchField.focus();
            }, 100);
        }

    }
</script>


<style scoped>
</style>
