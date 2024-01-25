<template>
    <v-card>
        <v-card-title>Sociálně - pracovní anamnéza</v-card-title>

        <v-card-text>
            <v-container grid-list-lg>

                <v-layout row wrap>

                    <v-flex xs12 v-if="anamnesis">

                        <v-checkbox color="error" label="Žije s partnerem" v-model="anamnesis.isSocial_LivingWithPartner" />
                        <v-checkbox color="error" label="Pracující" v-model="anamnesis.isSocial_Working" />
                        <v-checkbox color="error" label="Starobní důchod" v-model="anamnesis.isSocial_Pension" />
                        <v-checkbox color="error" label="Částečně invalidní důchod" v-model="anamnesis.isSocial_PartialDisabilityPension" />
                        <v-checkbox color="error" label="Invalidní důchod" v-model="anamnesis.isSocial_DisabilityPension" />

                        <v-btn @click="save()" color="error">Uložit</v-btn>

                    </v-flex>

                </v-layout>

            </v-container>
        </v-card-text>

    </v-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis'

    @Component({
        name: "AnamnesisSocial"
    })
    export default class AnamnesisSocial extends Vue {

        loadingAnamnesis: boolean = false;
        anamnesis: any = null;


        mounted() {
            this.loadAnamnesis();
        }

        async loadAnamnesis() {
            this.loadingAnamnesis = true;

            var result = await anamnesisApi.getSocialAnamneses();
            console.warn('result', result);
            this.anamnesis = result.data;

            console.warn('anamnesis', this.anamnesis);

            this.loadingAnamnesis = false;
        }

        async save() {
            await anamnesisApi.saveSocialAnamnesis(this.anamnesis);
            this.loadAnamnesis();
        }

    }
</script>


<style scoped>
</style>
