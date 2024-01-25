<template>

    <v-card>
        <v-card-title>Abusus</v-card-title>

        <v-card-text>
            <v-container grid-list-lg>

                <v-layout row wrap>

                    <v-flex xs12 v-if="anamnesis">

                        <v-checkbox color="error" label="Alkohol" v-model="anamnesis.isAbusus_Alcohol" />
                        <v-checkbox color="error" label="Kuřák" v-model="anamnesis.isAbusus_Smoker" />
                        <v-checkbox color="error" label="Bývalý kuřák" v-model="anamnesis.isAbusus_Exsmoker" />

                        <v-card-actions>
                            <v-btn @click="save()" color="error">Uložit</v-btn>
                        </v-card-actions>



                    </v-flex>

                </v-layout>

            </v-container>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis'
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";

    @Component({
        name: "AnamnesisAbusus"
    })
    export default class AnamnesisAbusus extends Vue {

        loadingAnamnesis: boolean = false;
        anamnesis: any = null;


        mounted() {
            this.loadAnamnesis();
        }

        async loadAnamnesis() {
            this.loadingAnamnesis = true;

            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                var result = await anamnesisApi.getAbususAnamnesesFromPerson(impersonated.id);
            } else {
                var result = await anamnesisApi.getAbususAnamneses();
            }

            console.warn('result', result);
            this.anamnesis = result.data;

            if (this.anamnesis == null) {
                var user: UserDto = null;
                if (AuthStore.isImpersonating()) {
                    user = AuthStore.getImpersonatedUser() as UserDto;
                } else {
                    user = AuthStore.getCurrentUser() as UserDto;
                }

                this.anamnesis = { userId: user.id, isAbususAnamnesis: true, isAbusus_Alcohol: false, isAbusus_Smoker: false, isAbusus_Exsmoker: false };
            }
            console.warn('anamnesis', this.anamnesis);

            this.loadingAnamnesis = false;
        }

        async save() {
            await anamnesisApi.saveAbususAnamnesis(this.anamnesis);
            this.loadAnamnesis();
        }

    }
</script>


<style scoped>
</style>
