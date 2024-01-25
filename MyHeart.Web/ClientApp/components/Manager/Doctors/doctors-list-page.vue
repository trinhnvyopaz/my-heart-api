<template>
    <div class="">
        <v-container fluid>
            <v-card class=" elevation-3">
                <v-card-title>Výpis Lékařů</v-card-title>
                <v-card-text>
                    <localized-data-table :headers="headers" :items="users" :loading="loading" @click:row="doctorFromRow">
                        <template v-slot:item.isActive="{ item }">
                            <v-switch color="error"
                                      disabled
                                      v-model="item.isActive"></v-switch>
                        </template>

                        <template v-slot:item.action="{ item }">
                            <v-btn icon :to="`/doctor/detail/${item.id}`" elevation="2">
                                <v-icon color="blue">mdi-account-edit</v-icon>
                            </v-btn>
                            <v-btn icon elevation="2" @click="showDeleteDialog(item.id, item.lastName, $event)">
                                <v-icon color="red">mdi-account-off</v-icon>
                            </v-btn>
                        </template>
                    </localized-data-table>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn @click="showDialogAdd = true">
                        <v-icon>mdi-account-plus</v-icon>
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
                        Smazat lékaře
                    </v-card-title>

                    <v-card-text>
                        Opravdu chcete smazat lékaře : {{doctorName}} ?
                    </v-card-text>

                    <v-divider></v-divider>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn text
                               @click="showDialogDelete = false">
                            Storno
                        </v-btn>
                        <v-btn text
                               @click="deleteUser">
                            Smazat
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </template>

        <add-doctor-dialog :showDialog="showDialogAdd" v-on:hideAddDialog="hideAddDialog" v-on:addNewDoctor="addNewDoctor"></add-doctor-dialog>

        <v-snackbar right top v-model="showSnackbar" color="success" >Lékař byl založen</v-snackbar>
    </div>

</template>

<script lang="ts">
import { Component, Vue, Prop} from "vue-property-decorator";
import UserDto from "@models/user/UserDto";
import UserApi from "@backend/api/user";
import AddDoctorDialog from "@components/Manager/Doctors/doctor-add-dialog.vue";

@Component({
    name: "DoctorsListPage",
    components: {
        AddDoctorDialog
    },
})
export default class DoctorsListPage extends Vue {
    loading: boolean = false;
    showDialogDelete: boolean = false;
    showDialogAdd: boolean = false;
    userId: number = -1;
    doctorName: string = "";
    showSnackbar: boolean = false;
  headers = [
    { text: "Id", value: "id" },
    { text: "Jméno", value: "firstName" },
    { text: "Příjmení", value: "lastName" },
    { text: "Email", value: "email" },
    { text: "Active", value: "isActive" },
    { text: "Akce", value: "action", sortable: false }
  ];
  @Prop() users: UserDto[];

    showDeleteDialog(UserId: number, doctorName: string, event: Event) {
        this.userId = UserId;
        this.doctorName = doctorName;
        this.showDialogDelete = true;

        event.stopPropagation();
    }

    async deleteUser() {
        this.$emit("deleteUser", this.userId);

        this.showDialogDelete = false;
    }

    hideAddDialog() {
        this.showDialogAdd = false;
    }

    addNewDoctor() {
        this.showSnackbar = true;
        this.$emit("refreshUsers");
    }

    doctorFromRow(event, row) {
        this.$router.push("/doctor/detail/" + row.item.id);
    }
}
</script>

<style lang="scss" scoped></style>
