<template>
    <div class="margin-15">

        <v-card>
            <v-card-title primary-title class="title" style="background-color: ghostwhite">
                <v-btn color="primary" @click="goBack()" class="back-button"> {{ $locale({i: 'common.back'}) }}</v-btn>
                {{ $locale({i: 'clientPage.myProfile'}) }}
            </v-card-title>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.firstName'}) }}</span><br />
                                    <v-text-field v-model="user.firstName"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.lastName'}) }}</span><br />
                                    <v-text-field v-model="user.lastName"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.email'}) }}</span><br />
                                    <v-text-field v-model="user.email"></v-text-field>
                                </v-flex>

                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.weight'}) }}</span><br />
                                    <v-text-field v-model="user.weight"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.height'}) }}</span><br />
                                    <v-text-field v-model="user.height"></v-text-field>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.healthProblems'}) }}</span><br />
                                    <v-text-field v-model="user.healthProblems"></v-text-field>
                                </v-flex>

                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <v-menu ref="menu"
                                            v-model="menu"
                                            :close-on-content-click="false"
                                            transition="scale-transition"
                                            offset-y
                                            full-width
                                            min-width="290px">
                                        <template v-slot:activator="{ on }">
                                            <v-text-field v-model="date"
                                                          :label="$locale({i: 'clientPage.birthDay'})"
                                                          prepend-icon="event"
                                                          readonly
                                                          v-on="on"></v-text-field>
                                        </template>
                                        <v-date-picker ref="picker"
                                                       locale="cs-CZ"
                                                       v-model="date"
                                                       :max="new Date().toISOString().substr(0, 10)"
                                                       min="1950-01-01"
                                                       @change="save"></v-date-picker>
                                    </v-menu>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                </v-flex>

                                <v-flex xs12 md1></v-flex>

                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'clientPage.allergy'}) }}</span><br />
                                    <v-text-field v-model="user.allergy"></v-text-field>
                                </v-flex>

                            </v-layout>

                        </v-container>
                    </v-form>
                </div>

                <v-tabs>
                    <v-tab>
                        {{ $locale({i: 'clientPage.uploads'}) }}
                    </v-tab>

                    <v-tab-item>
                        <!--<v-btn class="margin-15" color="success" v-on:click="uploadAsset()">{{ $locale({i: 'common.chooseFile'}) }}</v-btn>-->
                        <v-file-input class="col-2" :label="$locale({i: 'common.chooseFile'})"></v-file-input>

                        <div v-for="item in assets">
                            <v-btn class="margin-15" color="primary" v-on:click="downloadAsset(item.url)">{{item.name}}</v-btn>
                            <font-awesome-icon class="icon-trash" icon="trash" v-on:click="deleteAsset(item)" />
                        </div>
                    </v-tab-item>
                </v-tabs>

            </v-card-text>

            <v-divider></v-divider>

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

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import { translations } from '../../utils/translations';
    import userStore from '@backend/store/userStore';
    import { UserModel } from '../../backend/entities/userModel';

    @Component({
        components: {
        }
    })

    export default class ClientProfilePage extends Vue {
        user: UserModel = {} as any;
        assets: any[] = [];

        date: any = null;
        menu: boolean = false;

        @Watch("menu")
        menu(val) {
            val && setTimeout(() => (this.$refs.picker.activePicker = 'YEAR'))
        }

        save(date) {
            this.$refs.menu.save(date)
        }

        downloadAsset(url) {
            alert(translations[userStore.getCulture()].common.notInDemo);
        }

        deleteAsset(asset) {
            alert(translations[userStore.getCulture()].common.notInDemo);
        }

        goBack() {
            window.history.back();
        }

        created() {
            this.user = {
                id: 1,
                firstName: 'Josef',
                lastName: 'Jindra',
                email: 'j.jindra@test.com',
                role: 3,
                weight: 90,
                height: 190,
                healthProblems: 'Astma',
                allergy: ''
            };

            this.assets = [
                { name: "soubor1.pdf", url: '' },
                { name: "soubor2.pdf", url: '' },
                { name: "soubor3.pdf", url: '' }
            ];
        }

    }
</script>

<style>
    .icon-trash {
        color: #FF5252;
        font-size: 25px;
        cursor: pointer;
        margin-top: 15px;
    }
</style>