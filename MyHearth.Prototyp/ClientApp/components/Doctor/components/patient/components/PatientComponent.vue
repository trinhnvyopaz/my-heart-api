<template>
    <div v-if="!loading" class="margin-15">
        <div style="margin-bottom: 15px">
            <v-card>
                <v-card-title class="title">
                    <v-btn color="primary" @click="goBack()" class="back-button">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                    {{ $locale({i: 'patient.newPatient'}) }}
                    <v-spacer></v-spacer>
                    <v-btn color="success" @click="goBack()" class="back-button">
                        {{ $locale({i: 'common.save'}) }}
                    </v-btn>
                </v-card-title>

                <v-card-text>
                    <div>
                        <v-form>
                            <v-container>
                                <v-layout>
                                    <v-flex xs12
                                            md3>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.firstName'}) }}</span><br />
                                        <v-text-field v-model="patient.firstName"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 md1 />
                                    <v-flex xs12
                                            md3>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.lastName'}) }}</span><br />
                                        <v-text-field v-model="patient.lastName"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 md1 />
                                    <v-flex xs12
                                            md3>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.pin'}) }}</span><br />
                                        <v-text-field v-model="patient.pin"></v-text-field>
                                    </v-flex>
                                </v-layout>

                                <v-layout>
                                    <v-flex xs12
                                            md3>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.subscription'}) }}</span><br />
                                        <v-text-field v-model="patient.subscription"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 md1 />
                                </v-layout>

                                <v-layout>
                                    <v-flex xs12
                                            md2>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.anamnesis'}) }}</span><br />
                                        <v-switch v-model="patient.anamnesis"
                                                  @change="anamnesisHasChanged"></v-switch>
                                    </v-flex>
                                    <v-flex v-if="patient.anamnesis"
                                            xs12
                                            md2>
                                        <span class="bold-span">{{ $locale({i: 'clientPage.notFound'}) }}</span><br />
                                        <v-switch v-model="patient.anamnesisNotFound"
                                                  @change="anamnesisHasChanged"></v-switch>
                                    </v-flex>
                                </v-layout>
                                <v-layout class="margin-15" v-if="patient.anamnesis">
                                    <v-date-picker first-day-of-week="1" v-model="patient.anamnesisDate" locale="cs-CZ"></v-date-picker>
                                    <v-flex xs12 md1 />
                                    <v-flex xs12
                                            md6>
                                        <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                        <v-textarea v-model="patient.anamnesisDescription"></v-textarea>
                                    </v-flex>
                                </v-layout>
                            </v-container>
                        </v-form>
                    </div>
                </v-card-text>
            </v-card>
        </div>

        <v-tabs fixed-tabs v-model="selectedTab">
            <v-tab v-show="patient.anamnesis && !patient.anamnesisNotFound" href="#anamnesis">
                {{ $locale({i: 'clientPage.anamnesis'}) }}
            </v-tab>
            <v-tab v-show="patient.id > 0" href="#activeQuestions">
                {{ $locale({i: 'doctorPage.activeQuestions'}) }}
            </v-tab>
            <v-tab v-show="patient.id > 0">
                {{ $locale({i: 'doctorPage.archivedQuestions'}) }}
            </v-tab>

            <v-tab-item id="anamnesis">
                <v-form>
                    <v-container>
                        <v-layout class="row" v-if="patient.anamnesis">
                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.familyAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-checkbox v-model="patient.anamnesisDetail.ichs"
                                                :label="$locale({i: 'anamnesis.ichs'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.flapDefect"
                                                :label="$locale({i: 'anamnesis.flapDefect'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.atrialFibrillation"
                                                :label="$locale({i: 'anamnesis.atrialFibrillation'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.suddenDeath"
                                                :label="$locale({i: 'anamnesis.suddenDeath'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.pacemaker"
                                                :label="$locale({i: 'anamnesis.pacemaker'})"></v-checkbox>
                                </v-card-text>
                            </v-card>

                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.personalAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-select multiple
                                              :items="diseases"
                                              :label="$locale({i: 'anamnesis.disease'})"
                                              v-model="patient.anamnesisDetail.disease"></v-select>
                                    <v-select multiple
                                              :items="nonPharmacologicalTreatments"
                                              :label="$locale({i: 'anamnesis.nonPharmacologicalTreatment'})"
                                              v-model="patient.anamnesisDetail.nonPharmacologicalTreatment"></v-select>
                                </v-card-text>
                            </v-card>

                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.farmaAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-select multiple
                                              :items="pharmacologicalTreatments"
                                              :label="$locale({i: 'clientPage.farmaAnamnesis'})"
                                              v-model="patient.anamnesisDetail.pharmacologicalTreatmentInPharmaca"></v-select>
                                    <v-text-field v-model="patient.anamnesisDetail.pharmacologicalTreatmentPower"
                                                  :placeholder="$locale({i: 'anamnesis.pharmacologicalTreatmentPower'})"></v-text-field>
                                    <v-select multiple
                                              :items="dosages"
                                              :label="$locale({i: 'anamnesis.nonPharmacologicalTreatment'})"
                                              v-model="patient.anamnesisDetail.dosage"></v-select>
                                    <v-textarea v-if="patient.anamnesisDetail.dosage.includes('vlastní')" v-model="patient.anamnesisDetail.pharmacologicalTreatmentPowerDesc" :placeholder="$locale({i: 'common.description'})"></v-textarea>
                                </v-card-text>
                            </v-card>

                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.allergiesAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-select multiple
                                              :items="pharmacologicalTreatments"
                                              :label="$locale({i: 'clientPage.farmaAnamnesis'})"
                                              v-model="patient.anamnesisDetail.pharmacologicalTreatmentInAllergy"></v-select>
                                    <v-textarea v-model="patient.anamnesisDetail.pharmacologicalTreatmentInAllergyDesc"
                                                :placeholder="$locale({i: 'common.description'})"></v-textarea>
                                </v-card-text>
                            </v-card>

                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.abususAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-checkbox v-model="patient.anamnesisDetail.alcohol"
                                                :label="$locale({i: 'anamnesis.alcohol'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.exSmoker"
                                                :label="$locale({i: 'anamnesis.exSmoker'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.activeSmoker"
                                                :label="$locale({i: 'anamnesis.activeSmoker'})"></v-checkbox>
                                </v-card-text>
                            </v-card>

                            <v-card class="margin-15 col-sm-12 col-md-2">
                                <v-card-title>
                                    {{ $locale({i: 'clientPage.socialAnamnesis'}) }}
                                </v-card-title>
                                <v-card-text>
                                    <v-checkbox v-model="patient.anamnesisDetail.livesWithPartner"
                                                :label="$locale({i: 'anamnesis.livesWithPartner'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.working"
                                                :label="$locale({i: 'anamnesis.working'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.retirementPension"
                                                :label="$locale({i: 'anamnesis.retirementPension'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.partialDisabilityPension"
                                                :label="$locale({i: 'anamnesis.partialDisabilityPension'})"></v-checkbox>
                                    <v-checkbox v-model="patient.anamnesisDetail.fullDisabilityPension"
                                                :label="$locale({i: 'anamnesis.fullDisabilityPension'})"></v-checkbox>
                                </v-card-text>
                            </v-card>
                        </v-layout>
                    </v-container>
                </v-form>
            </v-tab-item>

            <v-tab-item id="activeQuestions">
                <v-card>
                    <active-questions class="col-12"></active-questions>
                </v-card>
            </v-tab-item>

            <v-tab-item>
                <v-card>
                    <archived-questions class="col-12"></archived-questions>
                </v-card>
            </v-tab-item>
        </v-tabs>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    import ActiveQuestions from "@components/Doctor/components/questions/ActiveQuestionList";
    import ArchivedQuestions from "@components/Doctor/components/questions/ArchivedQuestionList";

    @Component({
        components: {
            "active-questions": ActiveQuestions,
            "archived-questions": ArchivedQuestions
        }
    })

    export default class PatientComponent extends Vue {
        loading: boolean = true;
        patients: any[] = [];
        patient: any = null;
        diseases: any[] = ["Etiam", "Curabitur"];
        nonPharmacologicalTreatments: any[] = ["Lorem ipsum", "Maecenas aliquet"];
        pharmacologicalTreatments: any[] = ["Lorem"];
        dosages: any[] = ["1-0-0", "1-0-1", "0-0-1", "½-0-0", "½-0-½", "0-0-½", "vlastní"];
        selectedTab: any = "anamnesis";

        anamnesisHasChanged() {
            if (this.patient.anamnesis && !this.patient.anamnesisNotFound)
                this.selectedTab = "anamnesis";
            else
                this.selectedTab = "activeQuestions";
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.patients = [
                {
                    id: 0,
                    firstName: "",
                    lastName: "",
                    pin: "",
                    subscription: "",
                    anamnesisDate: "",
                    anamnesisDescription: "",
                    anamnesis: false,
                    anamnesisNotFound: false,
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
                },
                {
                    id: 1,
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

            this.patient = this.patients.find(x => x.id == this.$attrs.patientId);

            this.loading = false;
        }
    }
</script>

<style>
</style>