<template>
    <div >


        <v-container>
            <v-row align="center" justify="center">
                <v-col cols="12" sm="8" md="4">
                    <logo />

                    <v-container>
                        <v-row v-for="(string, index) in error" :key="index">
                            <strong class="red--text">{{string}}</strong>
                        </v-row>
                        <v-row align="center" justify="center">
                            <v-col cols="12">
                                <v-text-field v-model="passwordRecovery.password"
                                                label="Heslo"
                                                :type="'password'"
                                                @input="validate" />
                            </v-col>
                        </v-row>
                        <v-row align="center" justify="center">
                            <v-col cols="12">
                                <v-text-field v-model="passwordRecovery.passwordAgain"
                                                label="Heslo znovu"
                                                :type="'password'"
                                                @input="validate" />
                            </v-col>
                        </v-row>
                    </v-container>

                    <v-btn elevation="2"
                            text
                            :disabled="hasError || saving"
                            @click="updatePassword">
                        Změnit heslo
                    </v-btn>
                </v-col>
            </v-row>
            
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";

    import UserApi from "@backend/api/user";
    import PasswordRecoveyDto from "../../models/user/PasswordRecoveyDto";
    import Logo from "../Shared/Logo.vue"

@Component({
    name: "PasswordRecoveryPage",
  components: {
    Logo
  }
})
export default class PasswordRecoveryPage extends Vue {
    passwordRecovery: PasswordRecoveyDto = new PasswordRecoveyDto()
    error: string[] = [];
    saving: boolean = false;

    get hasError() {
        return this.error.length > 0;
    }
    
    mounted() {
        const queryString = window.location.search;
        if (queryString == "") {
            this.error.push("Prosím zavřete stránku a otevřete znovu link z emailu, pokud tento problém přetrvává odešlete prosím novou žádost o změnu hesla.");
            return;
        }

        const urlParams = new URLSearchParams(queryString);

        if (!urlParams.has("token")) {
            this.error.push("Prosím zavřete stránku a otevřete znovu link z emailu, pokud tento problém přetrvává odešlete prosím novou žádost o změnu hesla.");
            return;
        }

        this.passwordRecovery.confirmString = decodeURI(urlParams.get("token"));
    }

    validate(): boolean {
        console.log("validating");

        this.error = [];
        if (this.passwordRecovery.password.length < 6) {
            this.error.push("Minimální délka hesla je 6 znaků");
        }

        if (this.passwordRecovery.password !== this.passwordRecovery.passwordAgain) {
            this.error.push("Hesla se neshodují");
        }

        if (this.error.length > 0) {
            return false;
        }
        return true;
    }

    async updatePassword() {
        if (!this.validate()) {
            return;
        }

        this.saving = true;

        var result = await UserApi.recoverPassword(this.passwordRecovery);
        if (!result.success) {
            for (var key in result.errors) {
                var errors = result.errors[key];
                if (typeof errors === 'string') {
                    this.error.push(`${errors} `);
                } else {
                    for (var index in errors) {
                        this.error.push(`${errors[index]} `);
                    }
                }
            }
        } else {
            this.$router.push("/login");
        }

        this.saving = false;
    }
}
</script>

<style scoped>
    .check-box-group
    {
        margin-top: -3rem;
        margin-left: 3.5rem;
    }
    .anamnesis {
        margin-left: 1rem;
    }

    .check-box-list-enter,
    .check-box-list-leave-to {
        visibility: hidden;
        height: 0;
        margin: 0;
        padding: 0;
        opacity: 0;
    }

    .check-box-list-enter-active {
        transition: all 0.3s;
    }
    .check-box-list-leave-active {
        transition: all 0.1s;
    }

    .card{
        background-color: whitesmoke !important;
    }
</style>
