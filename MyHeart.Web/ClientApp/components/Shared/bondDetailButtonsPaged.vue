<template>
    <v-tooltip left>
        <template v-slot:activator="{ on, attrs }">
            <v-btn :disabled="valueList.length < 1" icon elevation="2" @click="showBondDetail(valueList,selectedList,bondType)" v-bind="attrs" v-on="on">
                <v-icon v-if="!isDetailActive || currentBondType != bondType" color="red">mdi-arrow-right-drop-circle</v-icon>
                <v-icon v-if="isDetailActive && currentBondType === bondType" color="red">mdi-arrow-left-drop-circle</v-icon>
            </v-btn>
        </template>
        <span>Zobrazit detail</span>
    </v-tooltip>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
    import PagedStickyList from "../../backend/tools/PagedStickyList";

    @Component({
        name: "BondDetailButtons"
    })
    export default class BondDetailButtons extends Vue {
        @Prop({ default: false })
        isDetailActive: boolean;

        @Prop({ default: -1 })
        bondType: number;

        @Prop({ default: -1 })
        currentBondType: number;

        @Prop({ default: null })
        valueList: PagedStickyList;

        @Prop({ default: null })
        selectedList: object[];

        showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number) {
            this.$emit("showBondDetail", valueList, selectedList, bondType);
        }

    }
</script>
