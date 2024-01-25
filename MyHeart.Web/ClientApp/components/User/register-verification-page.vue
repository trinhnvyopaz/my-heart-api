<template>
    <v-container grid-list-lg>
        <v-layout row wrap>

            <v-flex xs12>

                <v-tabs color="error" v-model="selectedTab" centered icons-and-text background-color="rgb(245,245,245)" show-arrows center-active>
                    <v-tabs-slider></v-tabs-slider>
                    <v-tab>
                        Autentikator
                        <font-awesome-icon icon="shield-alt"></font-awesome-icon>
                    </v-tab>
                    <v-tab>
                        SMS
                        <v-icon>mdi-message-settings</v-icon>
                    </v-tab>
                </v-tabs>
                <v-tabs-items v-model="selectedTab">
                    <v-tab-item>
                        <MfaPage :id="id"></MfaPage>
                    </v-tab-item>

                    <v-tab-item>
                        <v-card>
                            <v-card-title>SMS</v-card-title>

                            <v-card-text>
                                <form id="phoneForm" v-on:submit.prevent>
                                    <p v-if="errors.length">
                                        <b>Please correct the following error(s):</b>
                                        <ul>
                                            <li v-for="error in errors">{{ error }}</li>
                                        </ul>
                                    </p>

                                    <v-text-field id="phone"
                                                  v-model="phone"
                                                  type="text"
                                                  pattern="(?:\d\s*){9}"
                                                  color="error"
                                                  label="Telefonní číslo"
                                                  :disabled="loading"
                                                  v-on:keyup.enter="getSms"
                                                  prefix="+420"></v-text-field>
                                </form>
                                <v-btn color="error" @click="getSms" :disabled="loading">Poslat SMS</v-btn>

                                <div id="codeDiv" v-if="smsRequested">
                                    <form id="codeForm" v-on:submit.prevent>
                                        <v-text-field id="code"
                                                      v-model="code"
                                                      type="text"
                                                      pattern="(?:\d\s*){6}"
                                                      color="error"
                                                      label="Kód z SMS"
                                                      :disabled="loading"
                                                      v-on:keyup.enter="validateSMS"></v-text-field>
                                    </form>
                                    <v-btn color="error" @click="validateSMS" :disabled="loading">Ověřit kód</v-btn>
                                </div>

                            </v-card-text>
                        </v-card>
                    </v-tab-item>

                </v-tabs-items>

            </v-flex>

        </v-layout>

    </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import RegisterDto from "@models/user/LoginDto";
    import UserApi from "@backend/api/user";
    import MfaPage from "./mfa-page.vue";

@Component({
    name: "RegisterVerification",
    components: {
        MfaPage
    }
})
export default class RegisterVerificationPage extends Vue {
    selectedTab: number = null;

    register: RegisterDto = new RegisterDto();
    loading: boolean = false;
    id = -1;
    phone = "";
    smsRequested = false;
    code = "";

    error: string | null = null;
    errors = [];

    mounted() {
        if (this.$route.params.id) {
            this.id = parseInt(this.$route.params.id);
        }
    }

    async getSms() {
        this.loading = true;

        if (!$("#phoneForm")[0].checkValidity()) {
            this.errors = ["Telefonní číslo musí být české mobilní číslo bez předpony (9 číslic)"];
            this.loading = false;
            return;
        } else {
            this.errors = [];
        }

        var result = await UserApi.generateFirstSMS(this.id, this.phone);

        if (result.success) {
            this.smsRequested = true;
        } else {
            if (result.data == "BADUSER") {
                this.$router.push("/login");
            }

            this.errors.push(result.data);
        }

        this.loading = false
    }

    async validateSMS() {
        this.loading = true;

        if (!$("#codeForm")[0].checkValidity()) {
            this.errors = ["Kód musí mít 6 číslic"];
            return;
        } else {
            this.errors = [];
        }

        var result = await UserApi.validateFirstSMS(this.id, this.code);

        if (result.success) {
            this.$router.push("/login");
        } else {
            if (result.data == "BADUSER") {
                this.$router.push("/login");
            }

            this.errors.push(result.data);
        }

        this.loading = false
    }
}
</script>

<style lang="scss" scoped></style>
