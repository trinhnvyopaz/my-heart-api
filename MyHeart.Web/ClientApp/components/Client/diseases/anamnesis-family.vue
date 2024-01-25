<template>
    <v-card>
        <v-card-title>Rodinná anamnéza</v-card-title>

        <v-card-text>
            <v-container grid-list-lg>

                <v-layout row wrap>

                    <v-flex xs12 v-if="anamnesis">

                        <v-checkbox color="error" label="ICHS <50 let věku" v-model="anamnesis.isFamily_ICHS" />
                        <v-checkbox color="error" label="Chlopenní vada" v-model="anamnesis.isFamily_ValveDefect" />
                        <v-checkbox color="error" label="Fibrilace síní" v-model="anamnesis.isFamily_AtrialFibrillation" />
                        <v-checkbox color="error" label="Náhlá smrt" v-model="anamnesis.isFamily_SuddenDeath" />
                        <v-checkbox color="error" label="Kardiostimulátor" v-model="anamnesis.isFamily_Pacemaker" />

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

            var result = await anamnesisApi.getFamilyAnamneses();
            this.anamnesis = result.data;

            this.loadingAnamnesis = false;
        }

        async save() {
            await anamnesisApi.saveFamilyAnamnesis(this.anamnesis);
            this.loadAnamnesis();
        }

    }
</script>


<style scoped>
</style>
