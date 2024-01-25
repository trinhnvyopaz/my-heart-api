<template>  

    <div>
        <top-bar :tabNames="['Pacienti']" />
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               :allowedGroups="[3]"
                               searchPlaceholder="Vyhledejte pacienta"
                               @click:row="rowClicked"
                               ref="dataTable" />
        </v-container>

        <simple-dialog v-model="dialogShown"
                       :title="'Pacient ' + userName"
                       positive="Impersonovat"
                       positive2="Profil"
                       negative="Odstranit"
                       @positiveClick="startImpersonating"
                       @positive2Click="gotoProfile"
                       width="570">
            <!-- <v-text-field label="ID" v-model="user.id"/>
            <v-text-field label="Jméno" v-model="user.firstName"/>
            <v-text-field label="Přijmení" v-model="user.lastName"/>
            <v-text-field label="Email" v-model="user.email"/> -->
        </simple-dialog>

        <v-snackbar right top v-model="showSnackbar" color="success">{{snackbarText}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import TopBar from "@components/top-bar.vue";
    import UserDto from "../../../models/user/UserDto";
    import UserApi from "@backend/api/user";
    import AuthStore from "@backend/store/authStore";

    import Events from "@models/shared/Events";
    import EventBus from "@models/EventBus";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";
    import GroupedDataTableRequest from "@models/GroupedDataTableRequest";

@Component({
    name: "UsersListPage",
    components: {
        TopBar, SimpleDialog, CustomDataTable
    }
})
export default class UsersListPage extends Vue {
    loading: boolean = false;
    totalCount: number = 0;
    pageCount: number = 0;
    headers = [
        { text: "Id", value: "id" },
        { text: "Jméno", value: "firstName" },
        { text: "Příjmení", value: "lastName" },
        { text: "Email", value: "email" },
    ];
    doctorRequest: GroupedDataTableRequest = {
            page: 1,
            pageSize: 10,
            filter: "",
            groups: [3]
    }

    getData = UserApi.getDataTable;

    dialogShown: boolean = false;
    userName: string = "";
    userId: number = -1;
    user: UserDto = new UserDto;
    showSnackbar: boolean = false;
    snackbarText: string = "";

    rowClicked(event, row) {
        this.userId = row.item.id;
        this.userName = row.item.firstName + " " + row.item.lastName;
        this.user = row.item;
        this.dialogShown = true;
    }
    gotoProfile() {
        this.$router.push("/user/detail/" + this.userId);
    }

    startImpersonating() {
        AuthStore.setImpersonating(this.user);
        EventBus.$emit(Events.StartedImpersonating);
    }
}
</script>

<style scoped>

</style>
