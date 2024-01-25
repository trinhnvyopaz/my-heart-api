<template>
    <div >
        <v-container>
            <v-row align="center" justify="center">
                <v-col cols="12" sm="8" md="4">
                    <logo/>
                    <v-container>
                        <v-row v-for="(string, index) in error" :key="index">
                            <strong class="red--text">{{string}}</strong>
                        </v-row>
                        <v-row align="center" justify="center">
                            <v-col cols="12">
                                <v-text-field v-model="email"
                                                color="error"
                                                label="Email" />
                            </v-col>
                        </v-row>
                        <v-row >
                            <v-col class="d-flex justify-center">
                                <v-btn elevation="2"
                                        justify="center"
                                        align="center"
                                        text
                                        :disabled="saving"
                                        @click="updatePassword">
                                    Resetovat heslo
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-col>
            </v-row>           
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import UserApi from "@backend/api/user";
import Logo from "../Shared/Logo.vue"

@Component({
    name: "PasswordRecoveryRequestPage",
  components: {
    Logo
  }
})
export default class PasswordRecoveryRequestPage extends Vue {
    email: string = "";
    error: string[] = [];
    saving: boolean = false;

    get hasError() {
        return this.error.length > 0;
    }
    
    mounted() {
    }

   async updatePassword() {
        this.saving = true;

       var result = await UserApi.requestRecoverPassword('"' + this.email + '"');
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
           this.$router.push("/");

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
