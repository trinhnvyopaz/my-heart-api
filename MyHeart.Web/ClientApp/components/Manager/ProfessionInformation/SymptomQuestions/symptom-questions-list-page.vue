<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte otázku"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable">
                <template #item.text="{item}">
                    {{stripHtml(item.text)}}
                </template>
            </custom-data-table>
        </v-container>

        <symptom-questions-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :symptomQuestionId="symptomQuestionId"
                                    @updateSymptomQuestions="updateSymptomQuestions" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import SymptomQuestionsDto from "../../../../models/professionInfo/SymptomQuestionsDto";
    import ProfessionInfoApi from "@backend/api/symptomQuestions";

    import SymptomQuestionsDetailDialogDeep from "@components/Manager/ProfessionInformation/SymptomQuestions/symptom-questions-detail-dialog-deep.vue";
    import DataTableRequest from "../../../../models/DataTableRequest";
    import CustomDataTable from "@components/Shared/customDataTable.vue";
    import EventBus from "@models/EventBus";
    import Events from "../../../../models/shared/Events";

    @Component({
        name: "SymptomQuestionsListPage",
        components: {
            SymptomQuestionsDetailDialogDeep, CustomDataTable
        },
    })
    export default class SymptomQuestionsListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";

        headers = [
            { text: "Text", value: "text" },
            { text: "Symptom", value: "symptomName" },
            { text: "Nemoci", value: "diseaseString" },
        ];
        getData = ProfessionInfoApi.getDataTable;

        symptomQuestionId: number = -1;
        symptomQuestion: SymptomQuestionsDto = new SymptomQuestionsDto;
        symptomQuestions: SymptomQuestionsDto[] = [];

        stripHtml(text) {
            return text.replace(/(<([^>]+)>)/gi, "");
        }

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(symptomQuestionId: number, event: Event) {
            this.isNew = false;
            this.symptomQuestionId = symptomQuestionId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
        }

        updateSymptomQuestions(message: string) {
            this.$refs.dataTable.refresh();
            this.showSnackbar(message);
        }

        mounted(){
            EventBus.$on(Events.SymptomUpdated, () => {
                this.$refs.dataTable.refresh()
            })
        }
    }
</script>

<style lang="scss" scoped></style>
