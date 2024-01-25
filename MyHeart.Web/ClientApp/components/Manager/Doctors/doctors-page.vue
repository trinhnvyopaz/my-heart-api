<template>
    <div>
        <top-bar :tabNames="['Lékaři']" />
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               searchPlaceholder="Vyhledejte lékaře"
                               @click:row="rowClicked"
                               :allowedGroups="[2]"
                               @click:add="showDialogAdd=true"
                               ref="dataTable" />
        </v-container>
        <simple-dialog v-model="showDialogDelete"
                       title="Odstranit lékaře"
                       positive="Storno"
                       negative="Odstranit"
                       @negativeClick="deleteUser">
            Opravdu chcete odstranit lékaře {{doctorName}}?
        </simple-dialog>

        <add-doctor-dialog v-model="showDialogAdd" @addNewDoctor="addNewDoctor"></add-doctor-dialog>

        <v-snackbar right top v-model="showSnackbar" color="success">Lékař byl založen</v-snackbar>
        <v-snackbar right top v-model="showSnackbarMfa" color="success">{{mfaText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
    import { Component, Prop } from "vue-property-decorator";
    import TopBar from "@components/top-bar.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";
    import UserDto from "../../../models/user/UserDto";
    import UserApi from "@backend/api/user";
    import AddDoctorDialog from "@components/Manager/Doctors/doctor-add-dialog.vue";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";
    import GroupedDataTableRequest from "@models/GroupedDataTableRequest";

    @Component({
        name: "DoctorsPage",
        components: {
            AddDoctorDialog, TopBar, CustomDataTable, SimpleDialog
        },
    })
    export default class DoctorsPage extends Vue {

        users: UserDto[] = [];
        showDialogDelete: boolean = false;
        showDialogMfaDelete: boolean = false;
        showDialogAdd: boolean = false;
        doctorName: string = "";
        mfaName: string = "";
        userId: number = -1;
        mfaId: number = -1;
        showSnackbar: boolean = false;
        showSnackbarMfa: boolean = false;
        mfaText: string = "";


        getData = UserApi.getDataTable;

        selectedTab: number = 0;
        headers = [
            { text: "Id", value: "id" },
            { text: "Jméno", value: "firstName" },
            { text: "Příjmení", value: "lastName" },
            { text: "Email", value: "email" }
        ];


        showDeleteDialog(UserId: number, doctorName: string, event: Event) {
            this.userId = UserId;
            this.doctorName = doctorName;
            this.showDialogDelete = true;

            event.stopPropagation();
        }

        showMfaDeleteDialog(UserId: number, mfaName: string, event: Event) {
            this.mfaId = UserId;
            this.mfaName = mfaName;
            this.showDialogMfaDelete = true;

            event.stopPropagation();
        }

        async deleteUser() {
            var result = await UserApi.deleteUser(this.userId);

            if (result.success) {
                this.users = this.users.filter(x => x.id != this.userId);
            }

            this.showDialogDelete = false;
        }

        hideAddDialog() {
            this.showDialogAdd = false;
        }

        addNewDoctor() {
            this.showSnackbar = true;
            this.$refs.dataTable.refresh();
        }

        doctorFromRow(id, event) {
            this.$router.push("/doctor/detail/" + id);
        }

        rowClicked(item, args) {
            console.warn(item);
            console.warn(args);
            this.doctorFromRow(item.id, args);
        }

        async removeMfa() {
            var result = await UserApi.adminRemoveMfa(this.mfaId);

            if (result.success) {
                this.mfaText = "MFA bylo odstraněno";
            } else {
                this.mfaText = result.data;
            }

            this.showSnackbarMfa = true;
            this.showDialogMfaDelete = false;
        }
}
</script>

<style scoped>
    .pepe-color {
        background-color: rgb(245,245,245) !important;
    }
</style>
