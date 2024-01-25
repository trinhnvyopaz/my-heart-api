<template>
    <div class="main">

        <v-btn color="error"
               @click="showManagerPage()">
            Manager page
        </v-btn>

        <v-btn color="success"
               @click="showClientPage()">
            Client page
        </v-btn>

        <v-btn color="warning"
               @click="showDoctorPage()">
            Doctor page
        </v-btn>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    @Component({
        components: {
        }
    })

    export default class MainPage extends Vue {

        showManagerPage() {
            this.$router.push('manager_page/informations')
        }

        showClientPage() {
            this.$router.push('client_page')
        }

        showDoctorPage() {
            this.$router.push('doctor_page/newQuestions')
        }

    }
</script>

<style>
    .main {
        margin: 15px;
    }
</style>