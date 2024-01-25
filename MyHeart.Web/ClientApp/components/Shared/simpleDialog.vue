<template>
    <v-dialog v-model="dialogShown" :width="width">
        <v-container>
            <v-row>
                <h3 style="margin-bottom: 25px">{{title}}</h3>
                <v-spacer />
                <svg class="closeButton" @click="dialogShown=false"><use href="#icon_close"></use></svg>
            </v-row>
            <v-row style="max-height: calc(80vh - 150px); overflow-y: auto; overflow-x: hidden">
                <slot />
            </v-row>
            <v-row>
                <div align="center" style="margin-top: 25px">
                    <v-btn v-if="positive" v-html="positive" class="custom" :disabled="!positiveEnabled"
                           @click="$emit('positiveClick');dialogShown=!autoclose" />
                    <v-btn v-if="positive2" v-html="positive2" class="custom"
                           @click="$emit('positive2Click');dialogShown=!autoclose" />
                    <v-btn v-if="negative" v-html="negative" class="custom delete"
                           @click="$emit('negativeClick');dialogShown=!autoclose" />
                    <v-btn v-if="negative2" v-html="negative2" class="custom delete"
                           @click="$emit('negative2Click');dialogShown=!autoclose" />
                    <v-spacer></v-spacer>
                </div>
            </v-row>
        </v-container>
    </v-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";

    @Component
    export default class SimpleDialog extends Vue {
        @Prop({ default: "Dialog title" }) title: string;
        @Prop({ default: 500 }) width: number;
        @Prop(String) positive: string;
        @Prop(String) positive2: string;
        @Prop(String) negative: string;
        @Prop(String) negative2: string;
        @Prop({ default: true }) autoclose: boolean; // automatically close dialog when either button is clicked
        @Prop({ default: true }) positiveEnabled: boolean;

        @Prop({ default: false }) value: boolean // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }
    }
</script>
<style scoped>
    .row {
        margin-left: unset;
        margin-right: unset;
    }
    .v-btn {
        margin-right: 10px !important;
    }
</style>