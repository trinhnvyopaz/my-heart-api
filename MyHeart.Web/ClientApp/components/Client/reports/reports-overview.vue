<template>
    <div>
        <v-container>
            <custom-data-table :headers="headers"
                               :getData="getData"
                               @click:row="rowClicked"
                               @click:add="addDialog"
                               ref="dataTable"
                               :serverPaginated="false">
                <template #item.creationDate="{item}">
                    {{new Date(item.creationDate).toLocaleDateString()}}
                </template>
                <template #item.reportType="{item}">
                    {{reportTypes[item.reportType].text}}
                </template>
            </custom-data-table>
        </v-container>
        <simple-dialog v-model="dialogShown"
                       :title="isNew ? 'Nahrát zprávu' : 'Upravit zprávu'"
                       positive="Uložit"
                       negative="Odstranit"
                       @positiveClick="postReport"
                       @negativeClick="deleteReport">
            <v-text-field v-model="report.title" label="Název" />
            <v-text-field v-model="report.description" label="Popis" />
            <v-select label="Typ" v-model="report.reportType" :items="reportTypes" />
            <v-file-input label="Soubor" v-if="report.files == null || report.files.length == 0" v-model="file" prepend-icon=""/>
            <v-btn v-else @click="downloadReport" class="custom">Stáhnout zprávu</v-btn>
        </simple-dialog>

        <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{ snackbarText }}</v-snackbar>
    </div>

</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import UserReportsApi from '@backend/api/userReport';
    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";
    import CustomDataTable from "@components/Shared/customDataTable.vue";

    @Component({
        name: "ReportsOverview",
        components: {
            CustomDataTable, SimpleDialog
        },
    })
    export default class ReportsOverview extends Vue {
        isNew = true;
        dialogShown: boolean = false;
        snackbarShown = false;
        snackbarText = "";
        snackBarColor = "";
        headers: any[] = [
            { text: 'Název', value: 'title' },
            { text: 'Popis', value: 'description' },
            { text: 'Typ', value: 'reportType' },
            { text: 'Datum nahrání', value: 'creationDate' },
        ];
        async getData() {
            if (AuthStore.isImpersonating()) {
                var impersonated = AuthStore.getImpersonatedUser() as UserDto;
                return await UserReportsApi.getUserReportsForUser(impersonated.id);
            } else {
                return await UserReportsApi.getUserReports();
            }
        }

        report = {
            userId: 0,
            title: '',
            description: '',
            files: [],
            reportType: 0
        };
        file: any = null;

        reportTypes: any[] = [
            { text: 'Ambulance Report', value: 0 },
            { text: 'Hospitalizace', value: 1 },
            { text: 'Laboratorní výsledky', value: 2 }
        ];

        rowClicked(item) {
            this.report = item;
            this.isNew = false;
            this.file = null
            this.dialogShown = true;

            console.log(item)
        }

        addDialog() {
            this.isNew = true;
            this.report = {
                userId: 0,
                title: '',
                description: '',
                files: [],
                reportType: 0
            };

            this.file = null

            this.dialogShown = true;
        }

        postReport()
        {
            if(this.isNew){
                this.uploadReport();
            }else{
                this.updateReport();
            }

        }

        async updateReport(){
            console.warn('triggered', 'updateReport')

            await this.processFile()

            console.log(this.report, this.file)

            var result = await UserReportsApi.updateUserReport(this.report);

            if(result.success){
                this.snackbarText = "Uloženo"
                this.snackBarColor = "success"
                this.snackbarShown = true
            }

            console.warn('result', result)


            this.$refs.dataTable.refresh();
        }

        processFile(){
            return new Promise((resolve, reject) => {
                if(this.file != null && this.report.files == null || this.report.files.length == 0){
                    var userReportFile = { 
                        name: this.file.name, 
                        extension: this.file.name.split('.').pop(), 
                        content: null, 
                        mimeType: this.file.type 
                    };

                    var reader = new FileReader();
                    reader.readAsArrayBuffer(this.file);
                    reader.onload = () => {
                        var arrayBuffer = reader.result;
                        var bytes = new Uint8Array(arrayBuffer as ArrayBuffer);
                        var content = Array.from(bytes);

                        userReportFile.content = content;

                        console.warn('userReportFile', userReportFile);

                        this.report.files.push(userReportFile);

                        // Resolve the Promise when onload is complete
                        resolve();
                    }

                    reader.onerror = (error) => {
                        // Reject the Promise with an error
                        reject(error);
                    }
                } else {
                    // Resolve the Promise immediately if no file processing is necessary
                    resolve();
                }
            });
        }

        async uploadReport() {
            console.warn('FILE', this.file);

            var user: UserDto = AuthStore.getImpersonatedOrCurrentUser();

            await this.processFile();
            this.report.userId = user.id;

            console.warn('model', this.report);
            var result = await UserReportsApi.saveUserReport(this.report);

            console.warn('result', result)

            if(result.success){
                this.snackbarText = "Uloženo"
                this.snackBarColor = "success"
                this.snackbarShown = true
            }
            else{
                this.snackbarText = "Nepodařilo se smazat"
                this.snackBarColor = "error"
                this.snackbarShown = true
            }

            this.$refs.dataTable.refresh();
        }

        async downloadReport(item) {
            let file = this.report.files[0];
            if (!file) {
                console.log('no file');
                return;
            }
            await UserReportsApi.downloadReport(file.id, file.name);
        }

        async deleteReport() {
            var result = await UserReportsApi.deleteUserReport(this.report.id);
            console.log(result)

            if(result.success){
                this.snackbarText = "Smazáno"
                this.snackBarColor = "success"
                this.snackbarShown = true

                console.log(this.snackbarShown, this.snackbarText)
            }
            else{
                this.snackbarText = "Nepodařilo se smazat"
                this.snackBarColor = "error"
                this.snackbarShown = true
            }
            this.$refs.dataTable.refresh();
        }

    }
</script>


<style scoped>
</style>
