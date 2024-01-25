<template>
    <div>
        <top-bar />
        <div class="medical-report-detail">
            <v-container fluid>
                <v-card class="pt-2 elevation-3 px-3 card">
                    <v-card-title class="medical-report-detail-title">Základní informace</v-card-title>
                    <v-card class="card-wrapper">
                        <v-row class="j-between">
                            <v-col cols="12" md="6" style="padding: 0;">
                                <v-skeleton-loader v-if="loading" class="skeleton-loader" type="image"></v-skeleton-loader>
                                <perfect-scrollbar v-else class="pdf-preview">
                                     <pre v-html="reportData.ocrText" style="height: 100%; padding-top: 32px ; font-size: 10px">
                                     </pre>
                                </perfect-scrollbar>
                            </v-col>
                            <v-col cols="12" md="6" class="right-custom">
                                <div v-if="loading" class="skeleton-loader skeleton-overlay">
                                    <v-progress-circular indeterminate color="primary"></v-progress-circular>
                                </div>
                                <v-form v-model="isValidForm" ref="formOption" lazy-validation>
                                    <div v-for="(value, key) in reportData.data" :class="`box box-${key}`">
                                        <v-card-title>{{ title[key] }}</v-card-title>
                                        <v-card>
                                            <div class="card-custom" v-if="key != keyMessageType">
                                                <ItemOption v-for="(dataDetail, index) in value" :data="dataDetail"
                                                    :key="index" />
                                                <v-card-text>
                                                    <span class="text-custom cursor-pointer"
                                                        @click="showDialog(value, key)">
                                                        <font-awesome-icon :icon="['fas', 'plus-circle']" size="lg"
                                                            style="color: #1459d2; font-size: 24px;" />
                                                        <b>Add more options</b>
                                                    </span>
                                                </v-card-text>
                                            </div>
                                            <div class="card-custom" v-if="key == keyMessageType">
                                                <v-card-text class="my-auto">
                                                    <v-radio-group v-model="value[0].itemMined">
                                                        <v-radio label="Ambulantní zpráva" value="Ambulantní zpráva"
                                                            color="green"></v-radio>
                                                        <v-radio label="Hospitalizace" value="Hospitalizace" color="green"
                                                            class="mt-3"></v-radio>
                                                    </v-radio-group>
                                                </v-card-text>
                                            </div>
                                        </v-card>
                                    </div>
                                </v-form>
                            </v-col>
                        </v-row>
                    </v-card>
                    <v-card-actions class="text-center">
                        <v-btn :loading="submitLoading" elevation="2" text @click="saveReport">
                            Uložit
                        </v-btn>
                        <v-btn :loading="submitLoading" elevation="2" text class="delete" @click="cancelReport">
                            Zahodit
                        </v-btn>
                        <v-btn v-if="reportData.isDataManager" :loading="submitLoading" @click="downloadExcel"
                            class="btn-spacing">Export</v-btn>
                    </v-card-actions>
                </v-card>
            </v-container>
            <v-snackbar right top color="success">Lékař byl aktualizován</v-snackbar>
            <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{ snackbarText }}</v-snackbar>
            <add-more-option-dialog :option="option" ref="addMoreDialog" :editForm="editForm" :deleteForm="deleteForm"
                @submit="addSection"></add-more-option-dialog>
        </div>
    </div>
</template>

<script>
import MedicalReportsApi from '@backend/api/medicalReport';
import { title, headers, keyMessageType, keyOther, keyNames, typeNames, typeKeys } from '../../../constants/type.ts'
import ItemOption from './components/ItemOption.vue';
import AddMoreOptionDialog from './components/AddMoreOptionDialog.vue';
import TopBar from "@components/top-bar.vue";

export default {
    components: {
        ItemOption,
        AddMoreOptionDialog,
        TopBar
    },
    data() {
        const id = parseInt(this.$route.params.id);
        return {
            isValidForm: true,
            reportData: {
                url: "",
                data: {
                    personalData: [
                        {
                            key: 3,
                            keyName: "Pracoviště",
                            value: "",
                            isSelected: false,
                            isEdit: true,
                            index: 0,
                        },

                    ],
                    healthCondition: [
                        {
                            key: 8,
                            keyName: "Výška",
                            value: "",
                            isSelected: false,
                            isEdit: true,
                            index: 0,
                        },

                    ],
                    knownData: [
                        {
                            key: 13,
                            keyName: "Známá onemocnění",
                            value: "",
                            isSelected: false,
                            isEdit: true,
                            index: 0,
                        },
                    ],
                },
            },
            id: id,
            snackbarText: "",
            snackbarShown: false,
            snackBarColor: "",
            title: title,
            headers: headers,
            keyMessageType: keyMessageType,
            keyOther: keyOther,
            keyNames: keyNames,
            typeNames: typeNames,
            isEditForm: false,
            option: {
                type: 0,
                key: 0,
                keyName: '',
                itemMined: '',
                isSelected: false,
                isEdit: true,
                index: 0,
            },
            ruleRequired: [(v) => !!v || 'Required.'],
            loading: false,
            submitLoading: false,
        };
    },

    created() {
        this.loading = true;
        this.getData(this.id);
    },

    methods: {
        async getData(id) {
            this.loading = true;
            try {
                const result = await MedicalReportsApi.getUserReport(id);
                this.reportData = result.data;
            } catch (error) {
                console.log(error);
                this.$router.push("/admin/reports");
            } finally {
                this.loading = false;
            }
        },

        async saveReport() {
            this.submitLoading = true;
            try {
                if (!this.$refs.formOption.validate())
                    return;
                const form = this.reportData;
                for (const property in form.data) {
                    if (form.data[property] && form.data[property].length > 0) {
                        form.data[property] = form.data[property].filter((item) => item.isSelected);
                    }
                }
                //form.data.knownData = form.data.knownData.map((item) => {
                //    if (item.type == 5 && (item.key == 15 || item.key == 13)) {
                //        //item.value = `${item.value}|${item.date}`;
                //        item.itemMined = item.itemMined;
                //        item.dateMined = item.dateMined;
                //    }
                //    if (item.type == 5 && item.key == 14) {
                //        //item.value = `${item.name}|${item.strength}|${item.unit}|${item.dosing}`;
                //        item.itemMined = item.itemMined;
                //        item.doseMined = item.doseMined;
                //        item.doseUnitMined = item.doseUnitMined;
                //        item.frequencyMined = item.frequencyMined;
                //    }
                //    return item;
                //});
                //form.data.newlyDiscoveredData = form.data.newlyDiscoveredData.map((item) => {
                //    if (item.type == 6 && (item.key == 16 || item.key == 18)) {
                //        //item.value = `${item.value}|${item.date}`;
                //        item.itemMined = item.itemMined;
                //        item.dateMined = item.dateMined;
                //    }
                //    if (item.type == 6 && item.key == 17) {
                //        //item.value = `${item.name}|${item.strength}|${item.unit}|${item.dosing}`;
                //        item.itemMined = item.itemMined;
                //        item.doseMined = item.doseMined;
                //        item.doseUnitMined = item.doseUnitMined;
                //        item.frequencyMined = item.frequencyMined;
                //    }
                //    return item;
                //});
                //form.data.other = form.data.other.map((item) => {
                //    if (item.type == 7) {
                //        //item.value = `${item.table}|${item.itemId}|${item.date}|${item.description}`;
                //        item.itemMined = item.itemMined;
                //        item.diseasePharmacyId = item.diseasePharmacyId;
                //        item.dateMined = item.dateMined;
                //        item.noteMined = item.noteMined;
                //    }
                //    return item;
                //});
                const result = await MedicalReportsApi.UpdateMedicalReport(this.id, form);
                if (result.success) {
                    this.snackbarText = "Uloženo"
                    this.snackBarColor = "success"
                    this.snackbarShown = true
                }
                this.reportData = this.getData(this.id)

            } catch (error) {

            } finally {
                this.submitLoading = false;
            }
        },
        cancelReport() {
            this.$router.push("/admin/reports");
        },
        editForm(key, idx) {
            const data = this.reportData.data[key][idx];
            this.option = data;
            this.option.index = idx;
            this.isEditForm = true;
            this.$refs.AddMoreOptionDialog.open(this.option);
        },
        deleteForm(key, index) {
            this.reportData.data[key].splice(index, 1);
        },
        showDialog(obj, type) {
            this.option.type = typeKeys[type];
            if (typeKeys[type] == 7) {
                this.option.key = 16;
            }

            this.$refs.addMoreDialog.open(this.option)
        },
        addSection(option) {
            const typeName = typeNames[option.type];
            this.reportData.data[typeName].push(option);
            this.option = {}
            this.$refs.addMoreDialog.close();
        },
        async downloadExcel() {
            try {
                this.submitLoading = true;
                await MedicalReportsApi.getMedicalReportExport(this.reportData.id);
            } finally {
                this.submitLoading = false;
            }
        }

    }
};
</script>

<style scoped>
.medical-report-detail-title {
    padding-bottom: 10px;
}

.card-wrapper {
    height: calc(100vh - 210px);
    overflow: hidden;

    @media (max-width: 1023px) {
        overflow-y: auto;
    }
}

.card-wrapper div {
    height: 100%;
}

.card-wrapper>div {
    width: 100%;
}



.medical-report-detail {
    height: calc(100vh - 64px);
    overflow: hidden;
}

.j-between {
    display: flex;
    justify-content: space-between;
}

.card {
    background-color: whitesmoke !important;
}

.v-text-field {
    margin-top: 0px !important;
}

.color-red .v-input__control .v-input__slot {
    border-color: rgb(219, 67, 67) !important;
}

.color-green .v-input__control .v-input__slot {
    border-color: rgb(44, 150, 44) !important;
}

.v-input {
    margin-top: 0 !important;
}

.pdf-preview {
    margin-top: 0;
    margin-left: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #f0f0f0;
    height: 100%;
    border-bottom-left-radius: 8px;
    overflow-y: auto;
    max-height: 100%;
}

.card-custom {
    min-height: 100px;
    height: fit-content;
    overflow-y: auto;
}

.pdf-preview img {
    width: 100%;
    height: auto;
    object-fit: cover;
}

.text-center {
    justify-content: center;
    align-items: center;
}

.right-custom {
    height: calc(100vh - 64px) !important;
    overflow-y: auto;
    position: relative;

    @media (max-width: 1023px) {
        height: auto !important;
    }
}

.cursor-pointer {
    cursor: pointer;
}



.card-wrapper .right-custom {
    height: 100% !important;
}

.skeleton-overlay {
    background-color: rgba(255, 255, 255, 0.5);
}

.right-custom .skeleton-loader {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    z-index: 100;
}

.box-messageDate .card-custom {
    border-top: 5px solid #F44336;
}

.box-personalData .card-custom {
    border-top: 5px solid #00C853;
}

.box-healthStatus .card-custom {
    border-top: 5px solid #2196F3;
}

.box-knownData .card-custom {
    border-top: 5px solid #FFFF8D;
}

.box-newlyDiscoveredData .card-custom {
    border-top: 5px solid #FFA726;
}

.box-other .card-custom {
    border-top: 5px solid #009688;
}
</style>
<style>
.skeleton-loader {
    height: calc(100vh - 64px);
    width: 100%;
    border-radius: 8px;
}

.v-skeleton-loader,
.skeleton-loader .v-skeleton-loader__image {
    height: 100%;
    width: 100%;
}

.card-custom col {
    padding: 0 !important;
}

.card-custom .v-input {
    margin-top: 0 !important;
}
</style>
