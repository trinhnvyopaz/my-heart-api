<template>
    <div>
        <simple-dialog v-model="dialogShown"
                       :title="title"
                       positive="Uložit"
                       :positiveEnabled="isFormValid"
                       :negative="isNew ? '' : 'Odstranit'"
                       @positiveClick="save"
                       @negativeClick="attemptDelete"
                       @click:outside="attemptClose"
                       @keydown.esc.prevent="attemptClose">
            <v-form ref="form" v-model="isFormValid">
                <slot />
            </v-form>
        </simple-dialog>
        <simple-dialog v-model="closeDialogShown"
                       title="Máte neuložené změny"
                       positive="Uložit"
                       negative="Zahodit"
                       @positiveClick="save"
                       @negativeClick="discard" />
        <simple-dialog v-model="deleteDialogShown"
                       title="Opravdu ostranit?"
                       positive="Neodstraňovat"
                       negative="Odstranit"
                       @negativeClick="doDelete" />
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";

    @Component({
        name: "SimpleEditDialog",
        components: {
            SimpleDialog
        },
    })
    export default class SimpleEditDialog extends Vue {
        @Prop({ default: "Dialog title" }) title: string;
        @Prop({ default: 500 }) width: number;
        @Prop({ default: true }) isNew: boolean;
        @Prop({ default: false }) isModified: boolean;

        @Prop({ default: false }) value: boolean = false; // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        closeDialogShown = false;
        deleteDialogShown = false;
        isFormValid = false;

        attemptClose() {
            console.log("attemptClose", this.isModified)
            
            if (this.isModified) {
                this.closeDialogShown = true;
            } else {
                this.dialogShown = false;
            }
        }

        save() {
            this.$emit("save");
        }

        discard() {
            this.dialogShown = false;
            this.$emit("discard");
        }

        attemptDelete() {
            this.deleteDialogShown = true;
        }

        doDelete() {
            this.dialogShown = false;
            this.$emit("delete");
        }
    }
</script>

<style scoped>
    .v-form {
        width: 100%;
    }
</style>