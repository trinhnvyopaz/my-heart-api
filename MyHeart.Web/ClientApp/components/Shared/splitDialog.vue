<template>
    <div>
        <v-dialog v-model="dialogShown"
                  @click:outside="attemptClose"
                  persistent
                  width="1300px"         
                  scrollable        
                  @keydown.esc.prevent="attemptClose"
                  @keydown.ctrl.s.prevent="save">

                  <v-card color="transparent" style="background-color: transpatent;">
                    <v-card-title class="d-flex card-title">
                        <h6 class="order-0" v-if="$vuetify.breakpoint.smAndDown">{{title}}</h6>   
                        <h3 class="order-0 " v-else>{{title}}</h3>
                        <v-spacer />                 
                        <v-btn class="order-sm-1 order-2 mr-2" :class='[{"small-input": $vuetify.breakpoint.smAndDown}]' @click="addNew" v-if="showAddButton">{{addButtonText}}</v-btn>              
                        <v-spacer v-if="$vuetify.breakpoint.smAndUp"/>         
                        <svg class="closeButton order-sm-2 order-1" @click="attemptClose"><use href="#icon_close"></use></svg>    

                    </v-card-title>
                    <v-card-text class="mid-row">            
                        <v-row>
                            <v-col cols="6">
                                <div class="leftPanel">
                                    <v-form v-model="isFormValid">
                                        <slot />
                                    </v-form>
                                </div>
                            </v-col>
                            <v-col cols="6">
                                <div :class='[{"small-row": $vuetify.breakpoint.smAndDown}]' class="rightPanel"  style="height: calc(100% - 50px)">                          
                                    <v-tabs-items v-model="focusedDetail" ref="tabs">
                                        <slot name="rightPanel" />
                                    </v-tabs-items>
                                </div>
                            </v-col>
                        </v-row>           
                       
                      
                    </v-card-text>
                    <v-card-actions class="d-flex justify-end card-actions">
                        <v-btn :class='[{"small-input": $vuetify.breakpoint.smAndDown}]' v-if="!isNew" class="custom delete" @click="attemptDelete" >Odstranit</v-btn>
                        <v-btn :class='[{"small-input": $vuetify.breakpoint.smAndDown}]' class="custom" @click="save" :disabled="!isFormValid" style="margin-left: 10px">Uložit</v-btn>                   
                    </v-card-actions>
                  </v-card>
        </v-dialog>

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
    import TherapyNewsApi from "@backend/api/therapyNews";
    import TherapyNewsDto from "@models/professionInfo/TherapyNewsDto";
    import DiseaseDto from "@models/professionInfo/DiseaseDto";
    import TherapyNewsDiseaseDto from "@models/professionInfo/bonds/TherapyNewsDiseaseDto";
    import DiseaseApi from "@backend/api/disease";

    import BondDetailDto from "@models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "@components/Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "@components/Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "@components/Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "@components/Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "@components/Shared/descriptionHtmlOutput.vue";
    import PagedStickyList from "@backend/tools/PagedStickyList";

    import DiseaseDetailDialog from "@components/Manager/ProfessionInformation/Disease/disease-detail-dialog.vue";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";

    @Component({
        name: "SplitDialog",
        components: {
            BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
            DescriptionHtmlOutput, DiseaseDetailDialog, SimpleDialog
        },
    })
    export default class SplitDialog extends Vue {
        @Prop({ default: "Dialog title" }) title: string;
        @Prop({default: "Přidat"}) addButtonText: string;
        @Prop({default: false}) showAddButton: boolean

        @Prop({ default: false }) value: boolean = false; // v-model = show or not

        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        @Prop({ default: false }) isNew: boolean;
        @Prop({ default: false }) isModified: boolean;
        @Prop({ default: -1 }) focusedDetail;
        /*@Watch("focusedDetail")
        onFocusedChanged(val: number, oldVal: number) {            
            if (this.$refs.tabs && val >= 0 && val < this.$refs.tabs.$children.length && this.$refs.tabs.$children[val].$children.length > 0){
                this.$nextTick(() => this.$refs.tabs.$children[val].$children[0].quill.focus());
                console.log("x", this.$refs.tabs.$children[val].$children[0].quill);
            }
        }*/

        closeDialogShown = false;
        deleteDialogShown = false;
        isFormValid = false;
        details = [];

        mounted(){
        }

        attemptClose() {
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
        }

        attemptDelete() {
            this.deleteDialogShown = true;
        }

        doDelete() {
            this.dialogShown = false;
            this.$emit("delete");
        }

        addNew(){
            this.$emit("addNew");
        }
    }
</script>
<style scoped>
.leftPanel{
    max-height: 600px;
    overflow-y: auto;
}
.rightPanel{
    max-height: 600px;
    overflow-x: hidden;
    overflow-y: auto;
}

.scrollable-y {
    overflow-y: auto;
}

@media (max-width: 959px){
    .closeButton{
        scale: .7;
    }
    .leftPanel >>> .v-input{
        scale: .7;
        transform-origin: 0 0;
        margin-bottom: -10px;
    }
    .leftPanel >>> h3{
        scale: .7;
        transform-origin: 0 0;
        margin-bottom: -10px;
    }
    .leftPanel >>> .v-btn{
        scale: .7;
        transform-origin: 0 0;
        margin-bottom: -10px;
    }
    .leftPanel{
        max-height: 400px;
        overflow-y: auto;
    }
}
.small-input{
    scale: .7;
    transform-origin: 0 0;
    margin-bottom: -10px;
    /* font-size: 10px!important; */
}
.small-row{
    scale: .7;
    transform-origin: 0 0;
    width: 160%;
}
.v-sheet{
    box-shadow: none!important;
    flex: 1 1 100%!important;
}
.mid-row{
    overflow-y: hidden!important;
}

</style>
