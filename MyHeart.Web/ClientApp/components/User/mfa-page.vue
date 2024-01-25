<template>
    <div class="varContent">
        <h3>Nastavení 2 faktorového ověření</h3>
        <div v-if="mfaError" class="red--text">{{mfaError}}</div>
            <p id="mfaContainer">
                <div v-if="mfaValidating">{{recoveryCode}}</div>
            </p>
            <v-row>
                <v-col cols="2">
                    <v-text-field v-if="mfaSet || mfaValidating"
                                  id="mfaCode"
                                  v-model="code"
                                  color="error"
                                  label="MFA kód"
                                  v-on:keyup.enter="send" />
                </v-col>
            </v-row>

        <v-card-actions>
            <v-btn v-if="!mfaSet && !mfaValidating" class="custom" @click="requestMfa()" :disabled="loadingUser">Nastavit 2FA</v-btn>
            <v-btn v-if="mfaValidating" class="custom" @click="validateMfa()" :disabled="loadingUser">Ověřit 2FA</v-btn>
            <v-btn v-if="mfaSet" class="custom delete" @click="removeMfa()" :disabled="loadingUser">Zrušit 2FA</v-btn>
        </v-card-actions>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
import LoginDto from "@models/user/LoginDto";
import UserApi from "@backend/api/user";
import AuthStore from "@backend/store/authStore.ts";
import Events from "@models/shared/Events";
    import EventBus from "@models/EventBus";
    import MfaLoginDto from "../../models/user/MfaLoginDto";

@Component({
  name: "MfaPage"
})
export default class MfaPage extends Vue {
    loadingUser: boolean | string = false;

    mfaSet: boolean = false;
    mfaValidating: boolean = false;
    mfaError: string = "";
    code: string = "";
    recoveryCode: string = "";

    @Prop({ default: -1 })
    id: number;

    mounted() {
        if (this.id == -1) {
            this.loadUserMfaStatus();
        }
    }

    async requestMfa() {
        this.loadingUser = true;

        if (this.id != -1) {
            var result = await UserApi.generateFirstMFA(this.id);
        } else {
            var result = await UserApi.generateMfa();
        }

        if (result.success) {
            const element = document.createElement("img");
            element.src = result.data.link;

            const container = document.getElementById("mfaContainer");
            container.appendChild(element);

            this.recoveryCode = result.data.singleUse;

            this.mfaValidating = true;
        } else {
            if (result.data == "BADUSER") {
                this.$router.push("/login");
            }
        }

        this.loadingUser = false;
    }

    async validateMfa() {
        this.mfaError = "";
        if (this.code == "") {
            this.mfaError = "Kód nesmí být prázdný";
            return;
        }

        this.loadingUser = true;

        if (this.id != -1) {
            var result = await UserApi.validateFirstMfa(this.id, this.code);
        } else {
            var result = await UserApi.validateMfa(this.code);
        }

        if (result.success) {
            const element = document.getElementById("mfaContainer");
            const child = element.children[0];
            child.remove();

            this.mfaValidating = false;
            this.mfaSet = true;
            this.recoveryCode = "";
            this.code = "";

            if (this.id != -1) {
                this.$router.push("/login");
            }
        } else {
            this.mfaError = result.data;
        }

        this.loadingUser = false;
    }

    async removeMfa() {
        this.mfaError = "";
        if (this.code == "") {
            this.mfaError = "Kód nesmí být prázdný";
            return;
        }

        this.loadingUser = true;

        var result = await UserApi.removeMfa(this.code);

        if (result.success) {
            this.mfaValidating = false;
            this.mfaSet = false;
            AuthStore.clearMfa();
            this.code = "";
        } else {
            this.mfaError = result.data;
        }

        this.loadingUser = false;
    }

    async loadUserMfaStatus() {
        this.loadingUser = true;

        var result = await UserApi.doesUserHaveMfa();

        if (result.success) {
            if (result.data == "active") {
                this.mfaSet = true;
                this.mfaValidating = false;
            }
        }

        this.loadingUser = false;
    }

    async send() {
        if (!this.mfaSet && !this.mfaValidating) {
            this.requestMfa();
        } else if (this.mfaValidating) {
            this.validateMfa();
        } else if (this.mfaSet) {
            this.removeMfa();
        }
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

    .card-header-text {
        color: #fdfdfd !important;
    }

    a {
        text-decoration: none;
        color: black !important;
    }
</style>
