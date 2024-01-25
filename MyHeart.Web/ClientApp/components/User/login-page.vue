<template>
    <div class>
        <v-container>
            <v-row align="center" justify="center">
                <v-col cols="12" sm="8" md="5">

                    <logo />

                    <v-row align="center" justify="center">
                        <v-col cols="10">
                            <v-tabs v-model="selectedTab" centered show-arrows center-active>
                                <v-tabs-slider></v-tabs-slider>
                                <v-tab>
                                    Přihlášení přes účet
                                </v-tab>
                                <v-tab>
                                    Přihlášení přes mobil
                                </v-tab>
                            </v-tabs>
                            <v-tabs-items v-model="selectedTab" align="center" justify="center">
                                <v-tab-item align="center" justify="center" style="padding-top: 10px">

                                    <strong class="red--text" v-if="error">{{ error }}</strong>

                                    <v-text-field v-model="login.email" id="email" label="Email"
                                        v-on:keyup.enter="tryLogin" />

                                    <v-text-field id="password" v-model="login.password" type="password" label="Heslo"
                                        v-on:keyup.enter="tryLogin" />

                                    <v-text-field v-if="mfa" id="mfaCode" v-model="code" label="MFA kód"
                                        v-on:keyup.enter="tryLogin" />

                                    <v-row>
                                        <v-col>
                                            <v-checkbox v-model="login.rememberMe" label="Pamatovat si přihlášení ?"
                                                class="mb-0 pb-0 checkbox" />
                                        </v-col>
                                        <v-col align="end">
                                            <v-btn class="forgotten-password-btn" text :to="'/recover'">Zapomněl jsem heslo
                                            </v-btn>
                                        </v-col>
                                    </v-row>
                                    <div>
                                        <v-progress-circular indeterminate v-if="loading" color="#2474e3" />
                                        <v-btn class="custom wide" v-if="!loading" @click="tryLogin">Přihlásit se</v-btn>
                                    </div>
                                    <div style="margin-top: 140px">
                                        <v-btn class="custom empty" @click="registerUser">Registrace</v-btn>
                                    </div>
                                </v-tab-item>

                                <v-tab-item>
                                    <strong class="red--text" v-if="phoneError">{{ phoneError }}</strong>

                                    <v-card-text align="center" justify="center">
                                        <p id="qrContainer">
                                        </p>
                                    </v-card-text>

                                    <v-card-actions class="justify-center">
                                        <v-progress-circular indeterminate v-if="loading" color="#2474e3" />
                                    </v-card-actions>
                                </v-tab-item>
                            </v-tabs-items>
                        </v-col>
                    </v-row>
                </v-col>
            </v-row>
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import LoginDto from "@models/user/LoginDto";
import UserApi from "@backend/api/user";
import AuthStore from "@backend/store/authStore.ts";
import Events from "@models/shared/Events";
import EventBus from "@models/EventBus";
import MfaLoginDto from "../../models/user/MfaLoginDto";
import PhoneAuthDto from "../../models/user/PhoneAuthDto";
import UserDto, { UserType } from "../../models/user/UserDto";
import Logo from "../Shared/Logo.vue"

@Component({
    name: "LoginPage",
    components: {
        Logo
    }
})
export default class LoginPage extends Vue {
    login: LoginDto = new LoginDto();
    loading: boolean = false;
    mfa: boolean = false;
    code: string = "";
    selectedTab: number = 0;
    phoneLoginData: PhoneAuthDto = new PhoneAuthDto();
    phoneWaiting: boolean = false;
    timer;

    error: string | null = null;
    phoneError: string | null = null;

    @Watch("selectedTab")
    onSelectedTabChanged() {
        if (!this.phoneWaiting) {
            this.GetPhoneLoginCode()
        }
    }

    mounted() {
        if (AuthStore.isUserLoggedIn()) {
            this.navigateWhenLoggedIn();
        }
    }

    navigateWhenLoggedIn() {
        const user = AuthStore.getCurrentUser() as UserDto;

        if (user.userType == UserType.Admin || user.userType == UserType.SuperAdmin) {
            this.$router.push("/admin/doctors/");
        } else if (user.userType == UserType.Doctor) {
            this.$router.push("/admin/users");
        } else if (user.userType == UserType.DataManager) {
            this.$router.push("/admin/reports");
        } else {
            this.$router.push("/client/dash/");
        }
    }

    async GetPhoneLoginCode() {
        this.removeQrCode();
        //this.loading = true;
        this.error = "";
        this.phoneWaiting = true;
        this.loading = true;

        var result = await UserApi.loginByPhone();

        if (result.success) {
            this.phoneLoginData = result.data;

            const element = document.createElement("img");
            element.src = this.phoneLoginData.image;

            const container = document.getElementById("qrContainer");
            container.appendChild(element);
            this.loading = false;

            this.timer = setInterval(this.CheckLogin, 10000);
        } else {
            this.phoneWaiting = false;
            this.loading = false;
        }
    }

    removeQrCode() {
        const container = document.getElementById("qrContainer");
        while (container?.firstChild) {
            container.removeChild(container.firstChild);
        }
    }

    async CheckLogin() {
        var result = await UserApi.checkLoginByPhone(this.phoneLoginData.id, this.phoneLoginData.secret);

        if (result.success) {
            clearInterval(this.timer);
            var token = result.data.token;
            if (result.data.mfaSecret != null && result.data.mfaSecret != "") {
                AuthStore.saveMfa(result.data.mfaSecret);
            } else {
                AuthStore.clearMfa();
            }

            AuthStore.saveToken(token);

            if (await this.getCurrentUser()) {
                EventBus.$emit(Events.UserLoggedIn);
                this.navigateWhenLoggedIn()
            } else {
                AuthStore.clearToken();
                AuthStore.clearCurrentUser();
                EventBus.$emit(Events.UserLoggedOut);
                this.error = "Chyba pri prihlaseni";
            }
        } else {
            if (result.data == "INVALID") {
                this.phoneError = "Naskenovaný kód není platný. Vygenerovali jsme pro vás nový. Zkuste to prosím znovu.";
                this.phoneWaiting = false;
                clearInterval(this.timer);
                this.GetPhoneLoginCode()
                return;
            } else if (result.data == "STALE") {
                this.phoneWaiting = false;
                clearInterval(this.timer);
                this.GetPhoneLoginCode()
                return;
            } else if (result.data == "WAIT") {

            } else if (result.data == "BADUSER") {
                this.phoneError = "Nesprávný uživatel, zkuste to prosím znovu";
                this.phoneWaiting = false;
                clearInterval(this.timer);
                this.GetPhoneLoginCode()
                return;
            }


        }
    }

    async tryLogin() {
        this.loading = true;
        this.error = "";
        let result;
        if (this.mfa) {
            result = await UserApi.loginMfa({ email: this.login.email, password: this.login.password, mfaCode: this.code, rememberMe: this.login.rememberMe });
            console.log(1);

        } else if (AuthStore.hasSavedMFA()) {
            result = await UserApi.loginMfa({ email: this.login.email, password: this.login.password, mfaCode: AuthStore.getMfa(), rememberMe: this.login.rememberMe });
        } else {
            result = await UserApi.login(this.login);
        }

        if (result.success) {
            if (result.data == "MFANEEDED") {
                this.mfa = true;
            } else if (result.data.error == "MFACONFIRM") {
                console.log(`REGNEEDED - going to - ${`register/verify/${result.data.id}`}`)
                this.$router.push(`register/verify/${result.data.id}`).catch(err => { });
            } else {
                var token = result.data.token;
                if (result.data.mfaSecret != null && result.data.mfaSecret != "") {
                    AuthStore.saveMfa(result.data.mfaSecret);
                } else {
                    AuthStore.clearMfa();
                }

                AuthStore.saveToken(token);

                if (await this.getCurrentUser()) {
                    EventBus.$emit(Events.UserLoggedIn);
                    this.navigateWhenLoggedIn()
                } else {
                    AuthStore.clearToken();
                    AuthStore.clearCurrentUser();
                    EventBus.$emit(Events.UserLoggedOut);
                    this.error = "Chyba pri prihlaseni";
                }
            }
        } else {
            if (result.data == null) {
                switch (result.status) {
                    case 401:
                        this.error = "Špatné heslo nebo jméno";
                        break;
                    case 400:
                        this.error = "Špatný request";
                        break;
                    case 429:
                        this.error = "Překročn počet pokusů, počkejte prosím 5 minut";
                        break;
                }
            } else {
                if (this.mfa == false && result.data.title == "MFA bylo zadáno špatně") {
                    AuthStore.clearMfa();
                    this.tryLogin();
                } else {
                    this.error = result.data.title;
                }
            }
        }

        this.loading = false;
    }

    async registerUser() {
        this.$router.push("/register").catch(err => { });
    }

    async getCurrentUser(): Promise<boolean> {
        var result = await UserApi.getCurrentUser();

        if (result.success) {
            AuthStore.saveCurrentUser(result.data);

            return true;
        }

        return false;
    }
}
</script>

<style lang="css" scoped>
.forgotten-password-btn {
    background-color: transparent !important;
    color: #28282B !important;
    text-decoration: underline;
    letter-spacing: 0px;
    color: #28282B;
    opacity: 0.7;
    padding: 0 !important;
}
</style>

