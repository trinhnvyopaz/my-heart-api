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
                        </template>
                    </localized-data-table>
                </v-card-text>
            </v-card>
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import UserDto from "@models/user/UserDto";
import UserApi from "@backend/api/user";

import moment from "moment";

@Component({
    name: "DoctorsActiveListPage"
})
export default class DoctorsActiveListPage extends Vue {
  loading: boolean = false;
  headers = [
    { text: "Id", value: "id" },
    { text: "Jméno", value: "firstName" },
    { text: "Příjmení", value: "lastName" },
    { text: "Email", value: "email" },
    { text: "Aktivní", value: "isActive" },
    { text: "Akce", value: "action", sortable: false }
  ];
    @Prop() users: UserDto[];


    doctorFromRow(event, row) {
        this.$router.push("/doctor/detail/" + row.item.id);
    }
}
</script>

<style lang="scss" scoped></style>
