<template>
    <v-card class="mb-12"
            color="white"
            height="auto"
            elevation="4">
        <v-card-title>Máte bolesti hrudníku ?</v-card-title>
        <v-card-text>
            <v-row>
                <v-radio-group v-model="optionValue" column class="card-options">
                    <v-radio v-for="option in options"
                             :label="option.label"
                             color="red darken-3"
                             :value="option.value"
                             :key="option.id"/>
                </v-radio-group>
            </v-row>
            <v-row>
                hodnota boxu: {{optionValue}}
            </v-row>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
import { Component, Vue, Prop} from "vue-property-decorator";


    @Component({
        name: "QuestionCard",
        components: {
        },
    })
    export default class QuestionCard extends Vue {

        optionValue: number = 0;

        options = [
            { id: 1, label: "Velmi velké", value: 10 },
            { id: 2, label: "Velké", value: 8 },
            { id: 3, label: "Střední", value: 5 },
            { id: 4, label: "Malé", value: 2 }
        ];


    //@Prop({ default: null })
    //objekt který bude mít v sobě odpověď na příznak aby vyhodnotil další dotaz


    //otázka
    //možnosti


    //emit možnosti a pak poslat
}
</script>

<style scoped>
    .card-options{
        padding-left:2rem;
    }
</style>
