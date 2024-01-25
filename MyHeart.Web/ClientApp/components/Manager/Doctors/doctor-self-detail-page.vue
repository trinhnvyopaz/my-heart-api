<template>
    <div class="main-layout">
        <top-bar :tabNames="['Profil', 'Notifikace', '2FA', 'Přidat mobil']" v-model="selectedTab" />
        <v-tabs-items class="tabs" v-model="selectedTab">
            <v-tab-item>

                <v-card>

                    <v-card-text>

                        <v-container>

                            <v-row>
                                <v-col >
                                    <v-text-field v-model="doctorDetail.firstName"
                                                  justify="stretch"
                                                  color="error"
                                                  label="Jméno"
                                                  readonly="" />
                                    <v-text-field v-model="doctorDetail.lastName"
                                                  color="error"
                                                  label="Příjmení"
                                                  readonly="" />
                                </v-col>
                                <v-col>
                                    <span class="editor-label">Popis</span>
                                    <vue-editor :editorOptions="editorOptions" class="resize-target" v-model="doctorDetail.description" />
                                </v-col>

                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-text-field v-model="doctorDetail.email"
                                                  color="error"
                                                  label="Email"
                                                  readonly="" />
                                    <v-text-field v-model="doctorDetail.phone"
                                                  color="error"
                                                  label="Telefon" />
                                </v-col>
                                <v-col>
                                    <span class="editor-label">Pracoviště</span>
                                    <vue-editor :editorOptions="editorOptions" class="resize-target" v-model="doctorDetail.workPlace" />
                                </v-col>

                            </v-row>

                        </v-container>

                    </v-card-text>

                    <v-divider></v-divider>

                    <v-card-actions class="justify-end">
                        <v-btn @click="save()">Uložit</v-btn>
                    </v-card-actions>
                </v-card>


            </v-tab-item>

            <v-tab-item>
                <v-card>
                    <v-card-title>Notifikace</v-card-title>
                </v-card>
            </v-tab-item>

            <v-tab-item>
                <MfaPage></MfaPage>
            </v-tab-item>

            <v-tab-item>
                <v-card flat class="pepe-color">
                    <v-card-text>
                        <v-card>
                            <v-card-title>Přihlásit telefon</v-card-title>

                            <v-card-text>
                                <p id="qrContainer">
                                </p>
                            </v-card-text>

                            <v-card-actions>
                                <v-btn @click="getMobileLogin()" :disabled="loadingUser || qrGenerated">Vygenerovat QR kód</v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-card-text>
                </v-card>
            </v-tab-item>
        </v-tabs-items>

        <v-snackbar v-model="showSnack" timeout="1500" color="success" top right>
            Změny uloženy
        </v-snackbar>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import { Component, Prop } from "vue-property-decorator";

    import DoctorDetailDto from "../../../models/doctor/DoctorDetailDto";

    import DoctorAPI from "@backend/api/doctor";

    import TopBar from "@components/top-bar.vue";
    import MfaPage from "../../User/mfa-page.vue";
    import UserApi from "@backend/api/user";

    @Component({
        name: "UserDetailPage",
        components: {
            MfaPage, TopBar
        },
    })
    export default class DoctorSelfDetailPage extends Vue {
        selectedTab: number = null;
        loadingUser: boolean | string = false;
        id: number = 0;
        doctorDetail: DoctorDetailDto = new DoctorDetailDto();
        qrGenerated = false;

        showSnack: boolean = false;
        
        editorOptions = {
            modules: {
                toolbar: false,
            },
        };

        mounted() {
            this.loadUser();
        }

        // resize() {
        //     const height = $('.ql-toolbar')[0].scrollHeight - 50;

        //     if (!this.$vuetify.breakpoint.lgAndUp) {
        //         height += 30
        //     }

        //     $(".resize-target").css("margin-bottom", height + "px");
        // }

        async loadUser() {
            this.loadingUser = "error";
            var result

            console.log(this.$route.params.id)

            if (this.$route.params.id) {
                var id = parseInt(this.$route.params.id);
                result = await DoctorAPI.getDoctorDetailByUserId(id)
            }else{
                result = await DoctorAPI.getCurrentDoctorDetail();
            }

            console.log(result)

            if (result.success) {
                    this.doctorDetail = result.data;
                    console.log(result.data);
                }
            
            this.loadingUser = false;
        }

        async save() {
            this.loadingUser = true;

            var result = await DoctorAPI.updateDoctor(this.doctorDetail);
            console.warn('update result', result);

            this.showSnack = true;

            this.loadingUser = false;
        }

        async getMobileLogin() {
            this.loadingUser = true;

            var result = await UserApi.mobileLogin();
            if (result.success) {
                const element = document.createElement("img");
                element.src = result.data;

                const container = document.getElementById("qrContainer");
                container.appendChild(element);

                this.qrGenerated = true;
            }

            this.loadingUser = false;
        }

    }
</script>

<style scoped>
    .card-body {
        margin-top: 2rem;
    }

    .card-header-color-gray {
        background-color: rgb(225, 225, 225) !important;
        color: black !important;
    }

    .card-header-color-red {
        background-color: rgb(199, 0, 0) !important;
        color: #fdfdfd !important;
    }

    .main-layout{
        margin: 0 24px;
    }

    .card-header {
        margin-left: 1.5rem;
        margin-right: 1.5rem;
        bottom: 1.5rem;
    }

    .card-footer {
        margin-left: 1rem;
        margin-right: 1rem;
    }

    .card-body-text {
        margin-left: 1rem;
        margin-right: 1rem;
    }
    .tabs{   
        margin-top: 24px;

    }

    .v-card{
        border-radius: 20px!important;
        background-color: #F2F2F2!important;
    }

    .card-header-text {
        color: #fdfdfd !important;
    }

    a {
        text-decoration: none;
        color: black !important;
    }
    .editor-label{
        color: #707070;
        font-size: 14px;
    }
    .v-divider{
        margin: 0 10px;
    }
</style>
