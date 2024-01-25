<template>
    <div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="searchActive"
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
            <v-data-table :headers="headersActive"
                          :items="questionsActive"
                          :search="searchActive"
                          :page.sync="pageActive"
                          :items-per-page="itemsPerPageActive"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCountActive = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openActiveQuestion(item)">
                            <td class="short-question">{{ item.subject }}</td>
                            <td>{{ item.client.firstName }} {{ item.client.lastName }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="pageActive" :length="pageCountActive"></v-pagination>
        </v-card>

        <v-dialog v-model="selectedQuestionActive" v-if="selectedQuestionActive.doctor" persistent :width="windowWidth">
            <v-card style="overflow: hidden">

                <v-card-title style="background-color: ghostwhite">
                    <h1 class="question-title">{{ $locale({i: 'question.doctor'}) }}</h1>
                    <h1>{{selectedQuestionActive.doctor.firstName}} {{selectedQuestionActive.doctor.lastName}}</h1>
                </v-card-title>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <v-btn class="margin-15" style="float: right" color="info" v-on:click="clientDetail(selectedQuestionActive.client)">Informace o pacientovi</v-btn>
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

                        <v-textarea class="col-12" solo v-model="textAreaTest" :placeholder="$locale({i: 'question.text'})"></v-textarea>
                        <v-btn style="float: right" color="success" v-on:click="sendMessage()">{{ $locale({i: 'common.send'}) }}</v-btn>

                    </div>
                </div>

                <v-divider></v-divider>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary"
                           v-on:click="selectedQuestionActive = false">
                        {{ $locale({i: 'common.back'}) }}
                    </v-btn>
                </v-card-actions>

            </v-card>
        </v-dialog>

        <v-dialog v-model="openClientDetailDialog" v-if="selectedQuestionActive.client" persistent width="1000px">
            <v-card>
                <v-form>
                    <v-container>
                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.firstName'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.firstName}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.lastName'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.lastName}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.email'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.email}}</span><br />
                            </v-flex>

                        </v-layout>

                        <v-layout>
                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.weight'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.weight}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.height'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.height}}</span><br />
                            </v-flex>

                            <v-flex xs12 md1></v-flex>

                            <v-flex xs12
                                    md3>
                                <span class="bold-span">{{ $locale({i: 'clientPage.healthProblems'}) }}</span><br />
                                <span>{{selectedQuestionActive.client.healthProblems}}</span><br />
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
                                <span>{{selectedQuestionActive.client.allergy}}</span><br />
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


    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    import { translations } from '@utils/translations';
    import userStore from '@backend/store/userStore';
    import { QuestionModel } from '../../backend/entities/questionModel';

    @Component({
        components: {
        }
    })

    export default class ActiveQuestionList extends Vue {
        questionsActive: QuestionModel[] = [];
        headersActive: any[] = [];
        searchActive: string = "";
        pageActive: number = 1;
        pageCountActive: number = 0;
        itemsPerPageActive: number = 50;
        openQuestionDetailActive: boolean = false;
        selectedQuestionActive: QuestionModel = {} as any;
        openClientDetailDialog: boolean = false;

        date: string = "12.5.1950";
        textAreaTest: string = "";
        windowWidth: number = 0;

        closeClientDetail() {
            this.openClientDetailDialog = false;
        }

        clientDetail(client) {
            this.openClientDetailDialog = true;
        }

        openActiveQuestion(item) {
            this.windowWidth = screen.width / 1.50;
            this.selectedQuestionActive = item;
            this.openQuestionDetailActive = true;
        }

        sendMessage() {
            this.textAreaTest = "";
        }

        created() {
            this.headersActive = [
                { text: translations[userStore.getCulture()].common.subject, value: 'subject' },
                { text: translations[userStore.getCulture()].common.client, sortable: false, value: 'client' },
            ];

            this.questionsActive = [
                {
                    id: 2,
                    subject: "Test - active",
                    messages: [],
                    doctor: {
                        id: 2,
                        firstName: 'Roman',
                        lastName: 'Musil',
                        email: 'r.musil@test.com',
                        role: 1,
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
        }

    }
</script>

<style>
</style>