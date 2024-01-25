<template>
    <div>
        <top-bar :tabNames="['Osobní údaje', 'Notifikace', '2FA', 'Přidat mobil', 'Tarify']" v-model="selectedTab"/>
        <v-tabs-items v-model="selectedTab">
            <v-tab-item>
                <div class="varContent">
                        <v-row>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.firstName" label="Jméno" readonly />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.lastName" label="Příjmení" readonly />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.email" label="Email" readonly />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.phone" label="Telefon" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.pin" label="Rodné číslo" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.insuranceNumber" label="Číslo pojištěnce" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-select v-model="userDetail.insuranceCompanyCode" :items="insuranceCompanyCodes" label="Kód pojišťovny" />
                            </v-col>
                        </v-row>

                        <v-row><v-col><h3>Adresa</h3></v-col></v-row>

                        <v-row>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.street" label="Ulice" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.streetNumber" label="Číslo popisné" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.city" label="Město" />
                            </v-col>
                            <v-col cols="12" md="6">
                                <v-text-field v-model="userDetail.zip" label="PSČ" />
                            </v-col>
                        </v-row>
                        <v-card-actions>
                            <v-btn class="custom" @click="save()" :disabled="loadingUser">Uložit</v-btn>
                        </v-card-actions>

                </div>

            </v-tab-item>

            <v-tab-item>
                <div class="varContent">
                    <!-- <h3>Notifikace o nových odpovědích</h3>
                    <v-switch v-model="notificationSettings.replyEmailNotification" label="Emailové notifikace o nových odpovědích" />
                    <h3>Notifikace o novinkách v léčbě</h3>
                    <v-switch v-model="notificationSettings.therapyNewsEmailNotification" label="Emailové notifikace o novinkách v léčbě" /> -->

                    <v-radio-group v-model="notificationSettings.subscriptionPreferences">
                        <v-radio v-for="(subOption, index) in subOptions" :key="index" :label="subOption.text" :value="subOption.value"  />
                    </v-radio-group>

                    <v-card-actions>
                        <v-btn class="custom" @click="saveNotificationSettings()" :disabled="loadingUser">Uložit nastavení notifikací</v-btn>
                    </v-card-actions>
                </div>
            </v-tab-item>

            <v-tab-item>
                <MfaPage/>
            </v-tab-item>

            <v-tab-item>
                <div class="varContent">
                    <h3>Přihlásit telefon</h3>
                    <p id="qrContainer"/>

                        <v-card-actions>
                            <v-btn class="custom" @click="getMobileLogin()" :disabled="loadingUser || qrGenerated">Vygenerovat QR kód</v-btn>
                        </v-card-actions>
                </div>
            </v-tab-item>
            <v-tab-item>
                <subscription-page @subscription-cancelled="onSubscriptionCancelled" :userDetail="userDetail" class="mt-4"/>
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

    import UserDetailDto from "../../../models/user/UserDetailDto";
    import UserNotificationSettingsDTO from "../../../models/user/UserNotificationSettingsDTO";

    import UserApi from "@backend/api/user";
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";

    import TopBar from "@components/top-bar.vue";
    import MfaPage from "../../User/mfa-page.vue";
    import SubscriptionPage from "@components/Manager/Subscription/subscription-page.vue"

    @Component({
        name: "UserDetailPage",
        components: {
            MfaPage, TopBar, SubscriptionPage
        },
    })
    export default class UserDetailPage extends Vue {
        selectedTab: number = null;
        loadingUser: boolean | string = false;
        id: number = 0;
        userDetail: UserDetailDto = new UserDetailDto();
        doctors: UserDto[] = [];
        notificationSettings: UserNotificationSettingsDTO = new UserNotificationSettingsDTO();
        qrGenerated = false;

        impersonating: boolean = AuthStore.isImpersonating();

        showSnack: boolean = false;

        subOptions: any[] = [
            { text: 'Všechny novinky', value: 0 },
            { text: 'Pouze moje onemocnění', value: 1 },
            { text: 'Nepřeji si dostávat žádné informace', value: 2 },
        ];

        insuranceCompanyCodes: any[] = [
            { text: 'Česká průmyslová zdravotní pojišťovna', value: 205 },
            { text: 'Oborová zdravotní pojišťovna zaměstnanců bank, pojišťoven a stavebnictví', value: 207 },
            { text: 'RBP, zdravotní pojišťovna', value: 2013 },
            { text: 'Všeobecná zdravotní pojišťovna České republiky', value: 111 },
            { text: 'Vojenská zdravotní pojišťovna ČR', value: 201 },
            { text: 'Zaměstnanecká pojišťovna Škoda', value: 209 },
            { text: 'Zdravotní pojišťovna ministerstva vnitra', value: 211}
        ];

        mounted() {
            if (this.$route.params.id) {
                this.id = parseInt(this.$route.params.id);
                this.loadUser();
                this.getDoctors();
                this.loadNotificationSettings();
                
            }
           
            if(this.$route.query.result){
                this.selectedTab = 4
            }         
        }

        get doctorCombo() {
            return this.doctors.map(u => ({ value: u.id, text: u.firstName + " " + u.lastName }));
        }

        onSubscriptionCancelled(){
            this.loadUser()
        }

        async loadUser() {
            this.loadingUser = "error";
            var result = await UserApi.getUserDetail(this.id);
            if (result.success) {
                this.userDetail = result.data;
            }
            this.loadingUser = false;
        }

        async loadNotificationSettings() {
            this.loadingUser = true;
            if (AuthStore.isImpersonating()) {
                const impersonated: UserDto = AuthStore.getImpersonatedUser() as UserDto;
                var result = await UserApi.getNotificationSettingsById(impersonated.id);
            } else {
                var result = await UserApi.getNotificationSettings();
            }
            
            if (result.success) {
                this.notificationSettings = result.data;
            }
            this.loadingUser = false;
        }

        async getDoctors() {
            this.loadingUser = true;
            var result = await UserApi.getUsers(3);
            if (result.success) {
                this.doctors = result.data;
            }
            this.loadingUser = false;
        }

        async save() {
            this.loadingUser = true;

            var result = await UserApi.updateUser(this.userDetail);
            console.warn('update result', result);

            this.showSnack = true;

            this.loadingUser = false;
        }

        async saveNotificationSettings() {
            this.loadingUser = true;

            var result = await UserApi.updateNotificationSettings(this.notificationSettings);
            console.warn('update notificationSettings', result);

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
</style>
