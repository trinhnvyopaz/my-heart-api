<template>
    <div class="">
        <v-container fluid>
            <v-card class=" elevation-3">
                <v-card-title>Komerční názvy</v-card-title>
                <v-card-text>
                    <localized-data-table :headers="headers" :items="commercialNames" :loading="loading" @click:item="editDialogFromRow">
                        <template v-slot:item.action="{ item }">
                            <v-tooltip bottom>
                                <template v-slot:activator="{ on, attrs }">
                                    <v-btn icon elevation="2"
                                           @click="showDeleteDialog(item, $event)"
                                           v-bind="attrs"
                                           v-on="on">
                                        <v-icon color="red">mdi-delete</v-icon>
                                    </v-btn>
                                </template>
                                <span>Smazání</span>
                            </v-tooltip>
                        </template>
                    </localized-data-table>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn  @click="addDialog">
                        <v-icon>mdi-closed-caption</v-icon>
                         Přidat
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-container>

        <template>
            <v-dialog v-model="showDialogDelete"
                      width="500">
                <v-card>
                    <v-card-title primary-title>
                        Smazat Otázku k příznaku
                    </v-card-title>

                    <v-card-text>
                        Opravdu chcete smazat komerční název : {{commercialName.Name}} ?
                    </v-card-text>

                    <v-divider></v-divider>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn 
                               text
                               @click="showDialogDelete = false">
                            Storno
                        </v-btn>
                        <v-btn text
                               @click="deleteCommercialName(commercialName.id)">
                            Smazat
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </template>

        <commercial-name-detail-dialog :isNew="createDialog" :showDialog="showDialogAdd" :commercialNameId="commercialNameId" v-on:hideDetailDialog="hideDetailDialog" v-on:createCommercialName="createCommercialName" v-on:updateCommercialName="updateCommercialName"></commercial-name-detail-dialog>
        <v-snackbar right top v-model="showDeleteSnackbar" color="error"  >Komerční název byl smazan</v-snackbar>
        <v-snackbar right top v-model="showUpdateSnackbar" color="success"   >Komerční název byl upraven</v-snackbar>
        <v-snackbar right top v-model="showCreateSnackbar" color="success">Komerční název byl vytvořen</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import CommercialNameDto from "../../../../models/professionInfo/CommercialNameDto";
import ProfessionInfoApi from "@backend/api/commercialName";

    import CommercialNameDetailDialog from "@components/Manager/ProfessionInformation/CommercialNames/commercial-name-detail-dialog.vue";

@Component({
    name: "CommercialNameListPage",
    components: {
        CommercialNameDetailDialog
    },
})
export default class CommercialNameListPage extends Vue {
    loading: boolean = false;
    showDialogAdd: boolean = false;
    createDialog: boolean = false;
    showDialogDelete: boolean = false;
    showDeleteSnackbar: boolean = false;
    showUpdateSnackbar: boolean = false;
    showCreateSnackbar: boolean = false;
  headers = [
    { text: "Name", value: "name" },
    { text: "Akce", value: "action", sortable: false }
  ];

    commercialNameId: number = -1;
    commercialName: CommercialNameDto = new CommercialNameDto;
    commercialNames: CommercialNameDto[] = [];

    mounted() {
        this.getCommercialNames();
    }

    async getCommercialNames() {
        this.loading = true;

        var result = await ProfessionInfoApi.getCommercialNames();

        if (result.success) {
            this.commercialNames = result.data;
        }

        this.loading = false;
    }

    addDialog() {
        this.createDialog = true;
        this.showDialogAdd = true;
    }

    editDialogFromRow(event, row) {
        this.editDialog(row.item.id, null);
    }

    editDialog(commercialNameId: number, event: Event) {
        this.createDialog = false;
        this.commercialNameId = commercialNameId;
        this.showDialogAdd = true;

        if (event != null) {
            event.stopPropagation();
        }
    }

    hideDetailDialog() {
        this.showDialogAdd = false;
    }

    async deleteCommercialName(commercialNameId: number) {

        var result = await ProfessionInfoApi.deleteCommercialName(commercialNameId);

        if (result.success) {
            this.commercialNames = this.commercialNames.filter(x => x.id != commercialNameId);
            this.showDialogDelete = false;
            this.showDeleteSnackbar = true;
        }
    }

    showDeleteDialog(CommercialName: CommercialNameDto, event: Event) {
        this.commercialName = CommercialName;
        this.showDialogDelete = true;

        event.stopPropagation();
    }

    createCommercialName() {
        this.getCommercialNames();
        this.showCreateSnackbar = true;
    }

    updateCommercialName() {
        this.getCommercialNames();
        this.showUpdateSnackbar = true;
    }

    @Watch("showCreateSnackbar", { deep: true })
    onValueChanged() {
        if (this.showCreateSnackbar)
            this.getCommercialNames();
    }     

}
</script>

<style lang="scss" scoped></style>
