<template>
    <div class="margin-">

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="search"
                                      style="float: right; margin-right: 15px"
                                      class="col-8"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.subject'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.subject'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="questions"
                          :search="search"
                          :page.sync="page"
                          :items-per-page="itemsPerPage"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCount = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openQuestion(item)">
                            <td class="short-question">{{ item.subject }}</td>
                            <td>{{ item.client.firstName }} {{ item.client.lastName }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>
        </v-card>

        <v-dialog v-model="openQuestionDetail" v-if="selectedQuestion.client" persistent :width="windowWidth">
            <v-card style="overflow: hidden">
                <v-card-title style="background-color: ghostwhite">
                    <h1 class="question-title">{{ $locale({i: 'question.doctor'}) }}</h1>
                    <h1>{{selectedQuestion.doctor.firstName}} {{selectedQuestion.doctor.lastName}}</h1>
                </v-card-title>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <v-btn class="margin-15" style="float: right" color="warning" v-on:click="clientDiagnosis(selectedQuestion.client)">{{ $locale({i: 'clientPage.diagnosis'}) }}</v-btn>
                        <v-btn class="margin-15" style="float: right" color="info" v-on:click="clientDetail(selectedQuestion.client)">Informace o pacientovi</v-btn>
                    </div>

                    <div class="col-lg-3 col-md-12 col-sm-12"></div>
                    <div class="col-lg-6 col-md-12 col-sm-12">

                        <v-card class="col-8 message-left">
                            <p class="message-content">This is an awesome message!</p>
                            <div class="message-timestamp">13:37</div>
                        </v-card>

                        <v-card class="col-8 message-right">
                            <p class="message-content">I agree that your message is awesome!</p>
                            <div class="message-timestamp">13:38</div>
                        </v-card>

                        <v-card style="margin-bottom: 30px" class="col-8 message-left">
                            <p class="message-content">Thanks!</p>
                            <div class="message-timestamp">13:40</div>
                        </v-card>

                        <v-textarea disabled class="col-12" solo v-model="textAreaTest" :placeholder="$locale({i: 'question.text'})"></v-textarea>
                        <v-btn style="float: right" disabled color="success">{{ $locale({i: 'common.send'}) }}</v-btn>

                    </div>
                </div>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           v-on:click="openQuestionDetail = false">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                </v-card-actions>

            </v-card>

        </v-dialog>

        <v-dialog v-model="openClientDetailDialog" v-if="selectedQuestion.client" persistent width="1000px">
            <v-card>
                <v-form>
                    <v-container>
                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.firstName'}) }}</span><br />
                                <span>{{selectedQuestion.client.firstName}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.lastName'}) }}</span><br />
                                <span>{{selectedQuestion.client.lastName}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.email'}) }}</span><br />
                                <span>{{selectedQuestion.client.email}}</span><br />
                            </v-flex>

                        </v-layout>

                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.weight'}) }}</span><br />
                                <span>{{selectedQuestion.client.weight}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.height'}) }}</span><br />
                                <span>{{selectedQuestion.client.height}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.healthProblems'}) }}</span><br />
                                <span>{{selectedQuestion.client.healthProblems}}</span><br />
                            </v-flex>

                        </v-layout>

                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.birthDay'}) }}</span><br />
                                <span>{{date}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.allergy'}) }}</span><br />
                                <span>{{selectedQuestion.client.allergy}}</span><br />
                            </v-flex>

                        </v-layout>

                    </v-container>
                </v-form>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           @click="closeClientDetail()">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>

                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="openClientDiagnosisDialog" persistent width="1000px">
            <v-card>
                <v-card-title primary-title class="title" style="background-color: ghostwhite">
                    {{ $locale({i: 'common.information'}) }} {{information.name}}
                </v-card-title>

                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                    <span>{{information.name}}</span>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.createdDate'}) }}</span><br />
                                    <span>{{information.createdDate}}</span>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.author'}) }}</span><br />
                                    <span>{{information.author.firstName}} {{information.author.lastName}}</span>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <span>{{information.description}}</span>
                                </v-flex>
                            </v-layout>

                        </v-container>
                    </v-form>
                </div>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'information.symptoms'}) }}
                    </v-tab>
                    <v-tab>
                        {{ $locale({i: 'information.medicine'}) }}
                    </v-tab>

                    <v-tab-item class="margin-15">
                        <div class="row">
                            <div class="col-lg-6 col-md-12 col-sm-12"></div>

                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <v-tooltip top>
                                    <template v-slot:activator="{ on }">
                                        <v-text-field v-on="on"
                                                      v-model="searchSymptom"
                                                      append-icon="search"
                                                      :label="$locale({i: 'filter.name'})"
                                                      single-line
                                                      hide-details></v-text-field>
                                    </template>
                                    <span>{{ $locale({i: 'filter.name'}) }}</span>
                                </v-tooltip>
                            </div>
                        </div>

                        <v-data-table id="clientInfomrationDetailTable"
                                      :headers="symptomsHeaders"
                                      :items="symptoms"
                                      :search="searchSymptom"
                                      :page.sync="symptomsPage"
                                      :items-per-page="symptomsItemsPerPage"
                                      hide-default-footer
                                      class="elevation-1"
                                      @page-count="symptomsPageCount = $event">

                            <template v-slot:body="{ items }">
                                <tbody>
                                    <tr v-for="item in items" :key="item.id">
                                        <td>{{ item.name }}</td>
                                    </tr>
                                </tbody>
                            </template>
                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="symptomsPage" :length="symptomsPageCount"></v-pagination>
                    </v-tab-item>

                    <v-tab-item>
                        Testing medicine
                    </v-tab-item>
                </v-tabs>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           v-on:click="openClientDiagnosisDialog = false">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import { translations } from '@utils/translations';
    import userStore from '@backend/store/userStore';
    import { QuestionModel } from '../../backend/entities/questionModel';
    import { UserModel } from '../../backend/entities/userModel';
    import { InformationModel } from '../../backend/entities/informationModel';
    import { SymptomModel } from '../../backend/entities/symptomModel';

    @Component({
        components: {
        }
    })

    export default class ArchivedQuestionList extends Vue {
        questions: QuestionModel[] = [];
        headers: any[] = [];
        search: string = "";
        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;
        openQuestionDetail: boolean = false;
        selectedQuestion: QuestionModel = {} as any;
        openClientDetailDialog: boolean = false;
        selectedClient: UserModel = {} as any;
        openClientDiagnosisDialog: boolean = false;
        
        information: InformationModel = {} as any;
        searchSymptom: string = "";
        medicineSymptom: string = "";

        symptomsHeaders: any[] = [];
        symptoms: SymptomModel[] = [];

        symptomsPage: number = 1;
        symptomsPageCount: number = 0;
        symptomsItemsPerPage: number = 5;
        medicinesPage: number = 1;
        medicinesPageCount: number = 0;
        medicinesItemsPerPage: number = 5;

        textAreaTest: string = "";
        windowWidth: number = 0;

        date: string = "12.5.1950";

        sendMessage() {
            this.textAreaTest = "";
        }

        clientDiagnosis(client) {
            this.openClientDiagnosisDialog = true;
        }

        closeClientDetail() {
            this.selectedClient = {} as any;
            this.openClientDetailDialog = false;
        }

        clientDetail(client) {
            this.selectedClient = client;
            this.openClientDetailDialog = true;
        }

        openQuestion(item) {
            this.windowWidth = screen.width / 1.50;
            this.selectedQuestion = item;
            this.openQuestionDetail = true;
        }

        closeQuestionDetail() {
            this.selectedQuestion = {} as any;
            this.openQuestionDetail = false;
        }

        created() {
            this.headers = [
                { text: translations[userStore.getCulture()].common.subject, value: 'subject' },
                { text: translations[userStore.getCulture()].common.client, sortable: false, value: 'client' },
            ];
            
            this.questions = [
                {
                    id: 1,
                    subject: "Test",
                    messages: [],
                    doctor: {
                        id: 1,
                        firstName: 'Roman',
                        lastName: 'Musil',
                        email: 'test@test.com',
                        role: 2,
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
                },
                {
                    id: 2,
                    subject: "Proin",
                    messages: [],
                    doctor: {
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
                },
                {
                    id: 3,
                    subject: "Lorem ipsum",
                    messages: [],
                    doctor: {
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
            
            this.information = {
                id: 1,
                name: "Etiam",
                description: 'Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
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
            };

            this.symptomsHeaders = [
                { text: translations[userStore.getCulture()].common.name, value: 'name' }
            ];

            this.symptoms = [
                { id: 1, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 2, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 3, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 4, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 5, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 6, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' },

                { id: 7, name: "Morbi", description: 'Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Maecenas libero. Morbi leo mi, nonummy eget tristique non, rhoncus non leo. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede.' },
                { id: 8, name: "Mauris", description: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Fusce suscipit libero eget elit.' },
                { id: 9, name: "Curabitur", description: 'Maecenas aliquet accumsan leo. Curabitur bibendum justo non orci. Curabitur sagittis hendrerit ante. Aliquam erat volutpat.' }
            ];
        }

    }
</script>

<style>
    #clientInfomrationDetailTable table > tbody :hover {
        cursor: default !important;
        background-color: transparent !important;
    }

    .client-infomration-card {
        cursor: pointer;
        font-weight: bold;
        background-color: #5cbbf6 !important;
        text-align: center;
    }

        .client-infomration-card:hover {
            box-shadow: 10px 10px 15px 10px lightgrey !important;
        }

    .client-diagnosis-card {
        cursor: pointer;
        font-weight: bold;
        background-color: #FFB300 !important;
        text-align: center;
    }

        .client-diagnosis-card:hover {
            box-shadow: 10px 10px 15px 10px lightgrey !important;
        }

    .client-infomration-chat {
        font-size: 20px;
    }
</style>