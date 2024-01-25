<template>
    
    <v-container align-v="center" class="text-center">
        <div v-if="!fetching">
            <div v-if="!completed">
                <h3 slot="header" v-localize="{i: 'registrationComplete.registrationComplete'}" />
                <label v-localize="{i: 'registrationComplete.checkEmail'}" />
                <label><router-link to="/login">Přihlásit se</router-link></label>
            </div>
            <div v-if="success && completed">
                <h3 slot="header">Aktivace účtu proběhla úspěšně</h3>
                <label><router-link to="/login">Přihlásit se</router-link></label>
            </div>
            <div v-if="!success && completed">
                <h3 slot="header">Aktivace účtu se nezdařila</h3>
                <label>Mohlo dojít vypršení platnosti odkazu. Zkuste se znovu</label>
                <label><router-link to="/register">registrovat</router-link></label>
            </div>
        </div>
        <div v-if="fetching">
            <b-spinner variant="success" label="Spinning"></b-spinner>
        </div>
    </v-container>

</template>
<script lang="ts">
    import Vue from 'vue'
    import { Component, Watch } from "vue-property-decorator";
    import UserApi from "@backend/api/user";

    @Component({
        components: {
            name: "RegistrationCompletePage"}
    })
    export default class RegistrationCompletePage extends Vue {
        completed: boolean = false
        fetching: boolean = false;
        success: boolean = false;

        mounted() {
            this.registrationComplete();
        }

        async registrationComplete(){

            let token = this.$route.query.token;
            console.log(token)
            if (token != null) {
                this.fetching = true

                var result = await UserApi.activateEmail(token);

                if (result.success) {
                    this.success = result.success
                    this.completed = true
                    this.fetching = false;
                }
            }
        }
    }
</script>
<style scoped>
</style>
