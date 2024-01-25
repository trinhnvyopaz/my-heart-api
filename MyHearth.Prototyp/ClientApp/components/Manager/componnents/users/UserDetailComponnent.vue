<template>
    <div>
        <v-breadcrumbs :items="breadcrumbItems" large></v-breadcrumbs>

        <v-card class="margin-15">
            <v-card-title primary-title class="title">
                <v-btn color="primary" @click="goBack()" class="back-button"> {{ $locale({i: 'common.back'}) }}</v-btn>
                {{ $locale({i: 'common.user'}) }} {{user.firstName}} {{user.lastName}}
            </v-card-title>

            <v-divider></v-divider>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'userAdministration.dataTable.firstName'}) }}</span><br />
                                    <v-text-field v-model="user.firstName"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md2></v-flex>

                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'userAdministration.dataTable.lastName'}) }}</span><br />
                                    <v-text-field v-model="user.lastName"></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'userAdministration.dataTable.email'}) }}</span><br />
                                    <v-text-field v-model="user.email"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md2></v-flex>

                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'userAdministration.dataTable.role'}) }}</span><br />

                                    <v-select :items="roles"
                                              item-text="name"
                                              item-value="value"
                                              v-model="user.role"></v-select>
                                </v-flex>
                            </v-layout>
                        </v-container>
                    </v-form>
                </div>
            </v-card-text>

            <v-card-text v-if="user.id > 0">
                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'userComponnent.history'}) }}
                    </v-tab>

                    <v-tab-item>
                        <div class="row">
                            <div class="col-lg-6 col-md-12 col-sm-12">

                            </div>

                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <v-tooltip top>
                                    <template v-slot:activator="{ on }">
                                        <v-text-field v-on="on"
                                                      v-model="search"
                                                      style="margin-right: 15px"
                                                      append-icon="search"
                                                      :label="$locale({i: 'filter.subject'})"
                                                      single-line
                                                      hide-details></v-text-field>
                                    </template>
                                    <span>{{ $locale({i: 'filter.subject'}) }}</span>
                                </v-tooltip>
                            </div>
                        </div>

                        <v-data-table :headers="headers"
                                      :items="user.questions"
                                      :page.sync="page"
                                      :items-per-page="itemsPerPage"
                                      :search="search"
                                      hide-default-footer
                                      class="elevation-1"
                                      @page-count="pageCount = $event">

                            <template v-slot:body="{ items }">
                                <tbody>
                                    <tr v-for="item in items" :key="item.id" v-on:click="openConversation(item)">
                                        <td>{{ item.subject }}</td>
                                        <td>{{ item.client.firstName }} {{ item.client.lastName }}</td>
                                        <td>{{ item.state }}</td>
                                    </tr>
                                </tbody>
                            </template>

                        </v-data-table>

                        <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>

                    </v-tab-item>
                </v-tabs>
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.delete'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-actions>
        </v-card>

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

        <v-dialog v-model="openClientDiagnosisDialog" v-if="selectedQuestion.diagnosis" persistent width="1000px">
            <v-card>
                <v-card-title primary-title class="title" style="background-color: ghostwhite">
                    {{ $locale({i: 'common.information'}) }} {{selectedQuestion.diagnosis.name}}
                </v-card-title>

                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                    <span>{{selectedQuestion.diagnosis.name}}</span>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                </v-flex>

                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <span>{{selectedQuestion.diagnosis.description}}</span>
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

        <v-dialog v-model="openQuestionDetailActive" v-if="selectedQuestion.client" persistent :width="windowWidth">
            <v-card style="overflow: hidden">

                <v-card-title style="background-color: ghostwhite">
                    <h1 class="question-title">{{ $locale({i: 'question.doctor'}) }}</h1>
                    <h1>{{user.firstName}} {{user.lastName}}</h1>
                </v-card-title>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <v-btn class="margin-15" style="float: right" color="info" v-on:click="clientDetail(selectedQuestion.client)">Informace o pacientovi</v-btn>
                    </div>

                    <div class="col-lg-3 col-md-12 col-sm-12"></div>
                    <div class="col-lg-6 col-md-12 col-sm-12">

                        <v-card class="col-8 message-left">
                            <p class="message-content">This is an awesome message!</p>
                            <div class="message-timestamp">13:37</div>
                        </v-card>

                        <v-card class="col-8 message-right" style="margin-bottom: 30px">
                            <p class="message-content">I agree that your message is awesome!</p>
                            <div class="message-timestamp">13:38</div>
                        </v-card>

                    </div>
                </div>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           v-on:click="openQuestionDetailActive = false">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                </v-card-actions>

            </v-card>
        </v-dialog>

        <v-dialog v-model="openQuestionDetail" v-if="selectedQuestion.client" persistent :width="windowWidth">
            <v-card style="overflow: hidden">
                <v-card-title style="background-color: ghostwhite">
                    <h1 class="question-title">{{ $locale({i: 'question.doctor'}) }}</h1>
                    <h1>{{user.firstName}} {{user.lastName}}</h1>
                </v-card-title>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <v-btn class="margin-15" style="float: right" color="warning" v-on:click="clientDiagnosis(selectedQuestion)">{{ $locale({i: 'clientPage.diagnosis'}) }}</v-btn>
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
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import { translations } from '../../../../utils/translations';
    import userStore from '@backend/store/userStore'
    import { QuestionModel } from '../../../../backend/entities/questionModel';
    import { SymptomModel } from '../../../../backend/entities/symptomModel';

    @Component({
        components: {
        }
    })

    export default class UserDetailComponnent extends Vue {
        breadcrumbItems: any[] = [];
        roles: any[] = [];
        headers: any[] = [];
        search: string = "";
        openConversationDialog: boolean = false;

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 5;
        user: any = {} as any;
        date: string = "12.5.1950";

        questionsActive: any[] = [];
        headersActive: any[] = [];
        searchActive: string = "";
        pageActive: number = 1;
        pageCountActive: number = 0;
        itemsPerPageActive: number = 50;
        openQuestionDetail: boolean = false;
        openClientDiagnosisDialog: boolean = false;
        openQuestionDetailActive: boolean = false;
        selectedQuestion: any = {} as any;
        openClientDetailDialog: boolean = false;

        searchSymptom: string = "";
        symptomsHeaders: any[] = [];
        symptoms: SymptomModel[] = [];
        symptomsPage: number = 1;
        symptomsPageCount: number = 0;
        symptomsItemsPerPage: number = 5;

        textAreaTest: string = "";
        windowWidth: number = 0;

        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        goBack() {
            window.history.back();
        }

        clientDiagnosis(item) {
            this.openClientDiagnosisDialog = true;
        }

        closeClientDetail() {
            this.openClientDetailDialog = false;
        }

        clientDetail(client) {
            this.openClientDetailDialog = true;
        }

        openConversation(item) {
            this.windowWidth = screen.width / 1.50;
            if (item.state == translations[userStore.getCulture()].demo.active) {
                this.selectedQuestion = item;
                this.openQuestionDetailActive = true;
            }

            else {
                this.selectedQuestion = item;
                this.openQuestionDetail = true;
            }
        }

        close() {
            this.$emit("dialog-closed");
        }

        beforeRouteEnter(to, from, next) {
            console.log(from);

            next();
        }

        created() {
            let toObject = { text: document.title };
            this.breadcrumbStore.Add(toObject);

            this.breadcrumbItems = this.breadcrumbStore.breadcrumbs;

            console.log(this.breadcrumbStore.breadcrumbs);

            this.roles = [
                {
                    value: 1,
                    name: "Admin"
                },
                {
                    value: 2,
                    name: "Doktor"
                }
            ];

            this.headers = [
                { text: translations[userStore.getCulture()].common.subject, value: 'subject' },
                { text: translations[userStore.getCulture()].common.client, sortable: false, value: 'client' },
                { text: translations[userStore.getCulture()].common.state, value: 'state' },
            ];

            let users = [
                {
                    id: 1,
                    firstName: 'Josef',
                    lastName: 'Petr',
                    email: 'j.petr@test.com',
                    role: 2,
                    weight: 90,
                    height: 190,
                    healthProblems: '',
                    allergy: '',
                    questions: [],
                },
                {
                    id: 2,
                    firstName: 'Roman',
                    lastName: 'Musil',
                    email: 'r.musil@test.com',
                    role: 1,
                    weight: 90,
                    height: 190,
                    healthProblems: '',
                    allergy: '',
                    questions: [
                        {
                            id: 1,
                            subject: "Test",
                            state: translations[userStore.getCulture()].demo.closed,
                            messages: [],
                            diagnosis: {
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
                                }
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
                            subject: "Test - active",
                            state: translations[userStore.getCulture()].demo.active,
                            messages: [],
                            diagnosis: {},
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
                    ]
                },
                {
                    id: 3,
                    firstName: 'Jan',
                    lastName: 'Marek',
                    email: 'j.marek@test.com',
                    role: 2,
                    weight: 90,
                    height: 190,
                    healthProblems: '',
                    allergy: '',
                    questions: [],
                }
            ];
            this.user = users.find(x => x.id == this.$attrs.userId);

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
    .bold-span {
        font-weight: bold;
    }
</style>