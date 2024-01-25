<template>
    <div>

            <v-btn @click="getPharmacyReport" class="ml-3">Export</v-btn>

            <v-container grid-list-lg>
                <v-layout row wrap>

                    <v-flex xs12>
                        <v-progress-linear  v-if="loadingAnamnesis" color="error" indeterminate />
                        <localized-data-table :items="anamneses" :headers="anamnesisHeaders" v-if="!loadingAnamnesis" @click:row="rowClicked" hide-default-footer>
                        </localized-data-table>
                        <v-layout justify-end>
                            <v-row>
                                <v-col></v-col>
                                <v-col cols="8">
                                    <v-pagination @input="loadAnamnesis"
                                                v-model="anamnesesPage"
                                                :length.sync="anamnesisPageCount"
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

            <v-dialog width="900" scrollable v-model="showNewItemModal">
                <v-container>
                    <div>
                        <v-row align="center">
                            <v-col cols="5">
                                <h3>Nová medikace</h3>
                            </v-col>
                            <v-col cols="5" class="d-flex justify-end">
                                <v-btn @click="deepDialogShow = true">Přidat farmakum</v-btn>
                            </v-col>
                            <v-col cols="2" class="d-flex justify-end">
                                <svg class="closeButton" @click="showNewItemModal=false"><use href="#icon_close"></use></svg>
                            </v-col>
                        </v-row>                    
                    </div>
                    <localized-data-table :items="paginatedPharmacies" :headers="pharmacyHeaders" hide-default-footer>
                            <template v-slot:top>
                                <v-container>
                                    <v-layout row>
                                        <v-flex>
                                            <v-text-field ref="searchField" 
                                                            v-model="nameSearchString" 
                                                            @keyup.enter="loadPharmacies" 
                                                            label="Název léčiva (min 3 znaky)" 
                                                            @click:append="loadPharmacies" 
                                                            :loading="loadingPharmacies">
                                                <template v-slot:append>
                                                    <v-icon color="primary">mdi-magnify</v-icon>
                                                </template></v-text-field>
                                        </v-flex>
                                        <v-flex>
                                            <v-text-field class="ml2" 
                                                            ref="searchField" 
                                                            v-model="strengthSearchString" 
                                                            @keyup.enter="loadPharmacies" 
                                                            label="Síla léčiva" 
                                                            append-icon="mdi-magnify" 
                                                            @click:append="loadPharmacies" 
                                                            :loading="loadingPharmacies">
                                                <template v-slot:append>
                                                    <v-icon color="primary">mdi-magnify</v-icon>
                                                </template></v-text-field>            
                                            </v-text-field>   
                                        </v-flex>                             
                                    </v-layout>            
                                </v-container>
                                                        
                            </template>

                            <template v-slot:item.IsPharmacy_MorningDose="{item}">
                                <v-text-field v-model="item.IsPharmacy_MorningDose" dense color="error" @focus="$event.target.select()" />
                            </template>

                            <template v-slot:item.IsPharmacy_AfternoonDose="{item}">
                                <v-text-field v-model="item.IsPharmacy_AfternoonDose" dense color="error" @focus="$event.target.select()" />
                            </template>

                            <template v-slot:item.IsPharmacy_EveningDose="{item}">
                                <v-text-field v-model="item.IsPharmacy_EveningDose" dense color="error" @focus="$event.target.select()" />
                            </template>

                            <template v-slot:item.IsPharmacy_Note="{item}">
                                <v-text-field v-model="item.IsPharmacy_Note" dense color="error" />
                            </template>

                            <template v-slot:item.actions="{item}">
                                <v-btn icon small @click="assignPharmacy(item)">
                                    <v-icon small>mdi-arrow-right-bold</v-icon>
                                </v-btn>
                            </template>

                        </localized-data-table>
                        <v-pagination @input="loadPharmacies"
                                      v-model="pharmaciesPage"
                                      :length.sync="pharmaciesPageCount"
                                      :total-visible="itemsPerPage" />
<!-- 
                    <v-card-actions>
                        <v-btn @click="showNewItemModal = false" color="error">Zavřít</v-btn>
                    </v-card-actions> -->
    

                </v-container>                                                          
            </v-dialog>

            <simple-edit-dialog v-model="showEditDialog"
                                :isNew="false"
                                title="Upravit medikaci"
                                @save="update"
                                @delete="removePharmacy">
                        <v-container grid-list-xl>
                            <v-layout row wrap>

                                <v-flex xs12 md6>
                                    <v-text-field readonly v-model="selectedItem.isPharmacy_Name" label="Název" color="error"/>
                                </v-flex>

                                <v-flex xs12 md6>
                                    <v-text-field readonly v-model="selectedItem.isPharmacy_Dose" label="Dávkování" color="error" />
                                </v-flex>


                                <v-flex xs12 md4>
                                    <v-text-field v-model="selectedItem.isPharmacy_MorningDose" label="Ráno" color="error" />
                                </v-flex>

                                <v-flex xs12 md4>
                                    <v-text-field v-model="selectedItem.isPharmacy_AfternoonDose" label="Odpoledne" color="error" />
                                </v-flex>

                                <v-flex xs12 md4>
                                    <v-text-field v-model="selectedItem.isPharmacy_EveningDose" label="Večer" color="error" />
                                </v-flex>

                                <v-flex xs12>
                                    <v-text-field v-model="selectedItem.isPharmacy_Note" label="Poznámka" color="error" />
                                </v-flex>

                            </v-layout>
                        </v-container>
            </simple-edit-dialog>


        <pharmacy-detail-dialog-deep :isNew="true"
                                    v-model="deepDialogShow"
                                    :pharmacyId="-1"
                                    @updatePharmacy="updatePharmacy" />
    </div>
    

</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis'
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";
    import PharmacyDetailDialogDeep from "@components/Manager/ProfessionInformation/Pharmacy/pharmacy-detail-dialog-deep.vue";
    import PdfReportApi from '@backend/api/pdfReports'
    import SimpleEditDialog from "@components/Shared/simpleEditDialog.vue";

    @Component({
        name: "AnamnesisPharmacy",
        components: {
            PharmacyDetailDialogDeep, SimpleEditDialog
        },
    })
    export default class AnamnesisPharmacy extends Vue {
        loadingPharmacies: boolean = false;
        loadingAnamnesis: boolean = false;

        showNewItemModal: boolean = false;

        nameSearchString: string = null;
        strengthSearchString: string = null;

        deepDialogShow: boolean = false

        pharmacies: any[] = [];
        pharmacyHeaders: any[] = [
            { text: 'Název', value: 'IsPharmacy_Name' },
            { text: 'Dávkování', value: 'IsPharmacy_Dose', sortable: false },
            { text: 'Ráno', value: 'IsPharmacy_MorningDose', sortable: false },
            { text: 'Odpoledne', value: 'IsPharmacy_AfternoonDose', sortable: false },
            { text: 'Večer', value: 'IsPharmacy_EveningDose', sortable: false },
            { text: 'Poznámka', value: 'IsPharmacy_Note', sortable: false },
            { text: 'Přidat', value: 'actions', sortable: false },
        ];

        anamneses: any[] = [];
        anamnesisHeaders: any[] = [
            { text: 'Název', value: 'isPharmacy_Name' },
            { text: 'Dávkování', value: 'isPharmacy_Dose', sortable: false },
            { text: 'Ráno', value: 'isPharmacy_MorningDose', sortable: false },
            { text: 'Odpoledne', value: 'isPharmacy_AfternoonDose', sortable: false },
            { text: 'Večer', value: 'isPharmacy_EveningDose', sortable: false },
            { text: 'Poznámka', value: 'isPharmacy_Note', sortable: false },
            { text: 'Odebrat', value: 'actions', sortable: false },
        ];

        anamnesesPage: number = 1;
        anamnesisPageCount: number = 1;
   
        pharmaciesPage: number = 1;
        pharmaciesPageCount: number = 1;

        itemsPerPage = 10

        selectedItem: any = {
            IsPharmacy_Name: '',
            IsPharmacy_Dose: '',
            IsPharmacy_MorningDose: '0',
            IsPharmacy_AfternoonDose: '0',
            IsPharmacy_EveningDose: '0',
            IsPharmacy_Note: ''
        };
        showEditDialog: boolean = false;
        
        updatePharmacy(message: string, pharmacy){
            console.log(pharmacy)
            var anamnesisPharmacy = {
                    IsPharmacyAnamnesis: true,
                    IsPharmacy_PharmacyId: pharmacy.id,
                    IsPharmacy_Name: pharmacy.nameReg,
                    IsPharmacy_Dose: pharmacy.supplement,
                    IsPharmacy_MorningDose: '0',
                    IsPharmacy_AfternoonDose: '0',
                    IsPharmacy_EveningDose: '0',
                    IsPharmacy_Note: ''
                }
            this.pharmacies.unshift(anamnesisPharmacy)
        }

        mounted() {
            this.loadAnamnesis();
        }

        get paginatedPharmacies() {
            const start = (this.pharmaciesPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.pharmacies.slice(start, end);
        }

        async loadPharmacies() {

            this.loadingPharmacies = true;

            var result = await anamnesisApi.getPharmacyByNameRegAndStrength(this.nameSearchString, this.strengthSearchString);
            this.pharmacies = result.data.map(obj => {
                return {
                    IsPharmacyAnamnesis: true,
                    IsPharmacy_PharmacyId: obj.id,
                    IsPharmacy_Name: obj.nameReg,
                    IsPharmacy_Dose: obj.supplement,
                    IsPharmacy_MorningDose: '0',
                    IsPharmacy_AfternoonDose: '0',
                    IsPharmacy_EveningDose: '0',
                    IsPharmacy_Note: ''
                }
            });

            this.pharmaciesPageCount = Math.ceil(this.pharmacies.length / this.itemsPerPage); 

            this.loadingPharmacies = false;
        }

        async loadAnamnesis() {
            this.loadingAnamnesis = true;

            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                var result = await anamnesisApi.getPharmacyAnamnesisForPerson(impersonated.id);

            } else {
                var result = await anamnesisApi.getPharmacyAnamnesis();
            }

            this.anamneses = result.data;
             this.anamnesisPageCount = Math.ceil(this.anamneses.length / this.itemsPerPage); 
            this.loadingAnamnesis = false;
        }

        async assignPharmacy(item) {
            this.showNewItemModal = false;
            this.loadingAnamnesis = true;

            var user: UserDto = null;
            if (AuthStore.isImpersonating()) {
                user = AuthStore.getImpersonatedUser() as UserDto;
            } else {
                user = AuthStore.getCurrentUser() as UserDto;
            }

            item.userId = user.id;

            await anamnesisApi.savePharmacyAnamnesis(item);
            this.loadAnamnesis();
        }

        async removePharmacy() {
            this.loadingAnamnesis = true;

            await anamnesisApi.deletePharmacyAnamnesis(this.selectedItem.id);

            this.loadAnamnesis();
        }

        async update() {
            this.loadingAnamnesis = true;
            this.showEditDialog = false;
            await anamnesisApi.savePharmacyAnamnesis(this.selectedItem);
            this.loadAnamnesis();
        }

        async save() {
            this.loadingAnamnesis = true;
            await anamnesisApi.savePharmacyAnamnesis(this.anamneses);
            this.loadingAnamnesis = false;
        }

        rowClicked(item, args) {
            this.displayEditDialog(item);
        }

        displayEditDialog(item) {
            this.selectedItem = Object.assign({}, item);
            this.showEditDialog = true;
        }

        displayNewItemModal() {
            this.nameSearchString = null;
            this.strengthSearchString = null;
            this.pharmacies = [];
            this.showNewItemModal = true;

            setTimeout(() => {
                this.$refs.searchField.focus();
            }, 100);
        }

        async getPharmacyReport(){
            var user = AuthStore.getCurrentUser();
            await PdfReportApi.downloadPharmaciesReport(user.id)
        }

    }
</script>


<style scoped>
</style>
