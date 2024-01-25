<template>
    <div>

        <v-card>
            <v-card-title primary-title class="title">
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
                <v-btn color="primary"
                       @click="close()">
                    {{ $locale({i: 'common.back'}) }}
                </v-btn>
                <v-btn color="error"
                       v-on:click="close()">
                    {{ $locale({i: 'common.delete'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="close()">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-actions>
        </v-card>

        <v-dialog v-model="openConversationDialog" fullscreen hide-overlay transition="dialog-bottom-transition">
            <v-card>
                <v-toolbar dark color="primary">
                    <v-btn icon dark @click="openConversationDialog = false">
                        <v-icon>close</v-icon>
                    </v-btn>
                    <v-toolbar-title>Dotaz</v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-toolbar-items>
                        <!--<v-btn dark text @click="openConversationDialog = false">Save</v-btn>-->
                    </v-toolbar-items>
                </v-toolbar>

                <v-card-text>
                    <h1>
                        {{ $locale({i: 'common.notInDemo'}) }}
                    </h1>
                </v-card-text>

            </v-card>
        </v-dialog>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';

    import { translations } from '../../../../utils/translations';
    import userStore from '@backend/store/userStore'

    @Component({
        components: {
        }
    })

    export default class UserComponnent extends Vue {
        roles: any[] = [];
        headers: any[] = [];
        //clients: any[] = [];
        search: string = "";
        openConversationDialog: boolean = false;

        page: number = 1;
        pageCount: number = 0;
        itemsPerPage: number = 5;

        @Prop() user;

        openConversation(item) {
            this.openConversationDialog = true;
        }

        close() {
            this.$emit("dialog-closed");
        }

        created() {
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

            //this.clients = [
            //    {
            //        firstName: "Jan",
            //        lastName: "Petra",
            //        createdDate: "24.1.2019"
            //    },
            //    {
            //        firstName: "Roman",
            //        lastName: "Petr",
            //        createdDate: "30.2.2019"
            //    }
            //];
        }

    }
</script>

<style>
    .bold-span {
        font-weight: bold;
    }
</style>