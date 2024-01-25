<template>
    <v-tooltip left>
        <template v-slot:activator="{ on, attrs }">
            <v-btn :disabled="valueList.length < 1" icon elevation="2" @click="showListDetail(valueList,selectedList,listType)" v-bind="attrs" v-on="on">
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

    @Prop({ default: -1 })
    listType: number;

    @Prop({ default: -1 })
    currentListType: number;

    @Prop({ default: null })
    valueList: object[];

    @Prop({ default: null })
    selectedList: object[];

    showListDetail(valueList: object[], selectedList: object[], listType: number) {
        this.$emit("showListDetail", valueList, selectedList, listType);
    }

}
</script>
