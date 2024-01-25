<template>
    <div>
        <div class="margin-15" v-if="loading">
            <v-progress-linear indeterminate
                               color="error"></v-progress-linear>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-12 col-sm-12">
                <v-btn color="success" class="margin-15" style="float: left" v-on:click="openUserControl(newUser)">{{ $locale({i: 'userAdministration.addNew'}) }}</v-btn>
            </div>

            <div class="col-lg-6 col-md-12 col-sm-12">
                <v-tooltip top>
                    <template v-slot:activator="{ on }">
                        <v-text-field v-on="on"
                                      v-model="search"
                                      style="float: right; margin-right: 15px"
                                      class="col-8"
                                      append-icon="search"
                                      :label="$locale({i: 'filter.byFullnameAndDate'})"
                                      single-line
                                      hide-details></v-text-field>
                    </template>
                    <span>{{ $locale({i: 'filter.byFullnameAndDate'}) }}</span>
                </v-tooltip>
            </div>
        </div>

        <v-card class="manager-table shadow">
            <v-data-table :headers="headers"
                          :items="users"
                          :search="search"
                          :page.sync="page"
                          :items-per-page="itemsPerPage"
                          hide-default-footer
                          class="elevation-1"
                          @page-count="pageCount = $event">

                <template v-slot:body="{ items }">
                    <tbody>
                        <tr v-for="item in items" :key="item.id" v-on:click="openUserDetail(item)">
                            <td>{{ item.firstName }}</td>
                            <td>{{ item.lastName }}</td>
                            <td>{{ item.email }}</td>
                        </tr>
                    </tbody>
                </template>

            </v-data-table>

            <v-pagination prev-icon="keyboard_arrow_left" next-icon="keyboard_arrow_right" v-model="page" :length="pageCount"></v-pagination>

        </v-card>

        <v-dialog v-model="openUserComponnent" width="1000" persistent>
            <user :user="selectedUser" v-on:dialog-closed="closeUserComponnent"></user>
        </v-dialog>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';
    import { useModule } from "vuex-simple";

    import { UserModel } from '../../../../backend/entities/userModel';
    import { translations } from '../../../../utils/translations';
    import userStore from '@backend/store/userStore'

    import UserComponnent from './UserComponnent';

    @Component({
        components: {
            "user": UserComponnent
        }
    })

    export default class UserAdministrationComponnent extends Vue {
        loading: boolean = true;
        search: string = "";
        headers: any[] = [];
        users: any[] = [];
        newUser: UserModel = {} as any;
        selectedUser: any = {} as any;
        openUserComponnent: boolean = false;

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 50;

        breadcrumbStore: any = useModule(this.$store, ["Breadcrumb"]);

        openUserDetail(item) {
            let fromObject = { text: document.title, to: this.$route.path };
            this.breadcrumbStore.Add(fromObject);

            this.$router.push({ path: '/user/' + item.id });
        }

        openUserControl(item) {
            this.selectedUser = item;
            this.openUserComponnent = true;
        }

        closeUserComponnent() {
            this.openUserComponnent = false;
            this.selectedUser = {} as any;
            this.newUser = {} as any;
            this.init();
        }

        init() {
            this.headers = [
                { text: translations[userStore.getCulture()].userAdministration.dataTable.firstName, value: 'firstName' },
                { text: translations[userStore.getCulture()].userAdministration.dataTable.lastName, value: 'lastName' },
                { text: translations[userStore.getCulture()].userAdministration.dataTable.email, value: 'email' },
            ];

            this.users = [
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

            this.loading = false;
        }

        created() {
            this.breadcrumbStore.Clear();
            this.init();
        }

    }
</script>

<style>
    .sort-icon {
        cursor: pointer;
        font-size: 30px;
        float: right;
    }

    .v-data-table table > tbody :hover {
        cursor: pointer !important;
        /*background-color: #fb8a00 !important;*/
    }

    .v-data-table table > thead > tr > th {
        font-size: 20px;
        font-weight: bold;
    }

    .v-data-table table > tbody > tr > td {
        font-size: 15px;
    }
</style>