<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte příznak"
                               @click:row="editDialogFromRow"
                               @click:add="addDialog"
                               ref="dataTable" />
        </v-container>

        <symptom-detail-dialog-deep :isNew="isNew"
                                    v-model="dialogShown"
                                    :symptomId="symptomId"
                                    @updateSymptom="updateSymptom" />

        <v-snackbar right top v-model="snackbarShown" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";
    import SymptomDto from "@models/professionInfo/SymptomDto";
    import ProfessionInfoApi from "@backend/api/symptom";

    import SymptomDetailDialogDeep from "@components/Manager/ProfessionInformation/Symptoms/symptom-detail-dialog-deep.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";
    import EventBus from "@models/EventBus";
    import Events from "@models/shared/Events";

    @Component({
        name: "SymptomListPage",
        components: {
            SymptomDetailDialogDeep, CustomDataTable
        },
    })
    export default class SymptomListPage extends Vue {
        dialogShown: boolean = false;
        isNew: boolean = false;
        snackbarShown: boolean = false;
        snackbarText: string = "";

        headers = [
            { text: "Název", value: "name" },
        ];
        getData = ProfessionInfoApi.getDataTable;

        symptomId: number = -1;

        addDialog() {
            this.isNew = true;
            this.dialogShown = true;
        }

        editDialogFromRow(event, row) {
            this.editDialog(row.item.id, null);
        }

        editDialog(symptomId: number, event: Event) {
            this.isNew = false;
            this.symptomId = symptomId;
            this.dialogShown = true;

            if (event != null) {
                event.stopPropagation();
            }
        }

        showSnackbar(text: string) {
            this.snackbarShown = true;
            this.snackbarText = text;
        }

        updateSymptom(message: string) {
            EventBus.$emit(Events.SymptomUpdated)
            this.$refs.dataTable.refresh();
            this.showSnackbar(message);
        }

    }
</script>