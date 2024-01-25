<template>
    <v-tooltip left>
        <template v-slot:activator="{ on, attrs }">
            <v-btn icon elevation="2" @click="showListDetail" v-bind="attrs" v-on="on">
                <v-icon v-if="!isDetailActive" color="red">mdi-arrow-right-drop-circle</v-icon>
                <v-icon v-if="isDetailActive" color="red">mdi-arrow-left-drop-circle</v-icon>
            </v-btn>
        </template>
        <span>Zobrazit detail</span>
    </v-tooltip>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";

@Component({
    name: "DescriptionDetailButtons"
})
export default class ListDetailButton extends Vue {
    @Prop({ default: false })
    isDetailActive: boolean;

    showListDetail() {
        this.$emit("showPharmacyDiscardDetail");
    }

}
</script>
