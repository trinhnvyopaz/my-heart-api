<template>
    <div>
        <div class="margin-15" v-if="loading">
            <v-progress-linear indeterminate
                               color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-12 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="openPatient(null)">{{ $locale({i: 'userAdministration.addNew'}) }}</v-btn>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="items"
                          :page.sync="page"
                          :items-per-page="itemsPerPage"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCount = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openPatient(item)">
                            <td>{{ item.firstName }}</td>
                            <td>{{ item.lastName }}</td>
                            <td>{{ item.pin }}</td>
                            <td>{{ item.createdDate }}</td>
                            <td>
                                <span v-for="item in diseases">{{ item }}<span v-if="diseases.indexOf(item) != diseases.indexOf(diseases[diseases.length - 1])">, </span></span>
                            </td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>

        </v-card>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    import { translations } from '@utils/translations';
    import userStore from '@backend/store/userStore';

    @Component({
        components: {
        }
    })

    export default class PatientList extends Vue {
        loading: boolean = true;
        headers: any[] = [];
        items: any[] = [];
        diseases: any[] = ["Etiam", "Curabitur"];

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;

        openPatient(patient) {
            if (patient)
                this.$router.push({ path: '/patient/' + patient.id });
            else
                this.$router.push({ path: '/patient/' + 0 });
        }

        created() {
            this.headers = [
                { text: translations[userStore.getCulture()].clientPage.firstName, value: 'firstName' },
                { text: translations[userStore.getCulture()].clientPage.lastName, value: 'lastName' },
                { text: translations[userStore.getCulture()].clientPage.pin, value: 'pin' },
                { text: translations[userStore.getCulture()].clientPage.createdDate, value: 'createdDate' },
                { text: translations[userStore.getCulture()].anamnesis.disease, value: 'diseases' }
            ];

            this.items = [
                {
                    id: 1,
                    createdDate: "2018-03-07",
                    firstName: "Petr",
                    lastName: "Test",
                    pin: "9805060850",
                    subscription: "Test",
                    anamnesisDate: "2018-03-07",
                    anamnesisDescription: "Testing",
                    anamnesis: true,
                    anamnesisNotFound: false,
                    anamnesisDetail: {
                        ichs: true,
                        flapDefect: false,
                        atrialFibrillation: false,
                        suddenDeath: false,
                        pacemaker: true,
                        pharmacologicalTreatmentPower: "Vysoká",
                        disease: "Etiam",
                        nonPharmacologicalTreatment: "Lorem ipsum",
                        pharmacologicalTreatmentInPharmaca: "Lorem",
                        pharmacologicalTreatmentInAllergy: "",
                        dosage: "1-0-1",
                        pharmacologicalTreatmentPowerDesc: "",
                        pharmacologicalTreatmentInAllergyDesc: "",
                        alcohol: true,
                        exSmoker: true,
                        activeSmoker: false,
                        livesWithPartner: false,
                        working: true,
                        retirementPension: false,
                        partialDisabilityPension: false,
                        fullDisabilityPension: false
                    }
                },
                {
                    id: 2,
                    createdDate: "2018-05-07",
                    firstName: "Jana",
                    lastName: "Ta",
                    pin: "8555060510",
                    subscription: "Test",
                    anamnesisDate: "2018-05-07",
                    anamnesisDescription: "Testing 2",
                    anamnesis: true,
                    anamnesisNotFound: true,
                    anamnesisDetail: {
                        ichs: false,
                        flapDefect: false,
                        atrialFibrillation: false,
                        suddenDeath: false,
                        pacemaker: false,
                        pharmacologicalTreatmentPower: "",
                        disease: "",
                        nonPharmacologicalTreatment: "",
                        pharmacologicalTreatmentInPharmaca: "",
                        pharmacologicalTreatmentInAllergy: "",
                        dosage: "",
                        pharmacologicalTreatmentPowerDesc: "",
                        pharmacologicalTreatmentInAllergyDesc: "",
                        alcohol: false,
                        exSmoker: false,
                        activeSmoker: false,
                        livesWithPartner: false,
                        working: false,
                        retirementPension: false,
                        partialDisabilityPension: false,
                        fullDisabilityPension: false
                    }
                }
            ];

            this.loading = false;
        }
    }
</script>

<style>
</style>