<template>
    <simple-dialog v-model="dialogShown"
                    title=""
                   width="600"
                   :autoclose="false">
        <v-card-text>
                <div class="d-flex flex-column justify-space-between align-center" v-if="success">
                    <SuccessBigSVG />
                    <label class="mt-6 result-message">Platba proběhla v pořádku</label>
                </div>
                <div class="d-flex flex-column justify-space-between align-center" v-if="!success">
                    <v-icon size="112" class="cancelled-icon" color="#ea4444">mdi-close</v-icon>
                    <label class="mt-6 result-message">Platba byla zrušena</label>
                </div>    
        </v-card-text>
    </simple-dialog>
</template>
<script lang="ts">

import { Component, Vue, Prop } from "vue-property-decorator";
import SimpleDialog from "@components/Shared/simpleDialog.vue";
import SuccessBigSVG from "@resources/successbig.svg"

@Component({
    name: "SubscriptionResultDialog",
    components: {
        SubscriptionResultDialog, SimpleDialog, SuccessBigSVG
    },

})
export default class SubscriptionResultDialog extends Vue {
    @Prop()
    success: boolean


    @Prop({ default: false }) value: boolean
    get dialogShown() {
        return this.value;
    }
    set dialogShown(val) {
        this.$emit("input", val);
    }



}
</script>
<style scoped>
.result-message{
    color: #0666EB;
    font-size: 28px;
    font-weight: 800;
}

</style>