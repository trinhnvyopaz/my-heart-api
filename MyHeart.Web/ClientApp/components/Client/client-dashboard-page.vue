<template>
    <div>
        <top-bar :tabNames="['Zdravotní stav']" />
        <v-btn @click="exportHealtStatus">Export</v-btn>
        <v-container>
            <v-progress-circular indeterminate v-if="loading" color="#2474e3"></v-progress-circular>
            <v-row v-else>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Hmotnost"
                                    icon="icon_scale"
                                    :date="weightAnamnesis.isPersonal_Date"
                                    :values="[weightAnamnesis.isPersonal_Value]"
                                    :valuesDiff="[-4.6]"
                                    :minValues="[50]"
                                    :maxValues="[130]"
                                    unit="kg" />
                </v-col>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Tlak krve"
                                    icon="icon_blood_pressure"
                                    :date="bloodPressureAnamnesis.isPersonal_Date"
                                    :values="[bloodPressureAnamnesis.isPersonal_Value]"
                                    :valuesDiff="[10,5]"
                                    :minValues="[100,70]"
                                    :maxValues="[140,90]"
                                    unit="" />
                </v-col>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Tepová frekvence"
                                    icon="icon_heart_rate"
                                    :date="heartRateAnamnesis.isPersonal_Date"
                                    :values="[heartRateAnamnesis.isPersonal_Value]"
                                    :valuesDiff="[3]"
                                    :minValues="[60]"
                                    :maxValues="[110]"
                                    unit="/min" />
                </v-col>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Hladina sérového LDL"
                                    icon="icon_ldl_level"
                                    :date="cholesterolAnamnesis.isPersonal_Date"
                                    :values="[cholesterolAnamnesis.isPersonal_Value]"
                                    :valuesDiff="[-4.6]"
                                    :minValues="[50]"
                                    :maxValues="[130]"
                                    unit="mmol/l" />
                </v-col>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Kouření"
                                    icon="icon_smoking"
                                    :textValue="abususAnamnesis.isAbusus_Smoker ? 'Ano' : 'Ne'" />
                </v-col>
                <v-col cols="12" md="6" lg="4">
                    <dashboard-tile title="Fyzická aktivita"
                                    icon="icon_activity"
                                    :textValue="activityFrequencyToText" />
                </v-col>
            </v-row>
        </v-container>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
    import AuthStore from "@backend/store/authStore";
    import TopBar from "@components/top-bar.vue";
    import DashboardTile from "@components/Client/dashboardTile.vue";
    import PdfApi from "@backend/api/pdfReports";
    import AnamnesisAPi from "@backend/api/anamnesis";

    @Component({
        name: "ClientDashBoardPage",
        components: {
            TopBar, DashboardTile
        }
    })
export default class ClientDashBoardPage extends Vue {
    loading: boolean = false;
    abususAnamnesis: any = {isAbusus_Smoker: false};
    generalAnamnesis: any = {isGeneral_PhysicalActivityFrequencyType: 0};
    personalAnamnesis: any = {};;
    weightAnamnesis: any = { isPersonal_Date: '', isPersonal_Value: '' };
    bloodPressureAnamnesis: any = { isPersonal_Date: '', isPersonal_Value: '' };
    cholesterolAnamnesis: any = { isPersonal_Date: '', isPersonal_Value: '' };
    heartRateAnamnesis: any = { isPersonal_Date: '', isPersonal_Value: '' };

    mounted(){
        this.loadData()
    }

    get activityFrequencyToText()
    {
        var type =this.generalAnamnesis.isGeneral_PhysicalActivityFrequencyType
        if(type == 0 || type == 1){
            return "Nízká"
        }
        else if(type == 2 || type == 3){
            return "Střední"
        }
        else{
            return "Vysoká"
        }
    }

    async loadData(){
        this.loading = true;

        let abususAnamnesisTask = AnamnesisAPi.getAbususAnamneses();
        let generalAnamnesisTask = AnamnesisAPi.getGeneralAnamneses();
        let userAnamnesisTask = AnamnesisAPi.getPersonalAnamneses();

        let [abususAnamnesis, generalAnamnesis, personalAnamnesis] = await Promise.all([abususAnamnesisTask, generalAnamnesisTask, userAnamnesisTask]);

        this.abususAnamnesis = abususAnamnesis.data;
        this.generalAnamnesis = generalAnamnesis.data;

        console.log(this.abususAnamnesis)
        console.log(this.generalAnamnesis)
        console.log(personalAnamnesis)

        if (personalAnamnesis !== null) {
            let orderedAnamnesis = personalAnamnesis.data.sort((a, b) => b.isPersonal_Date - a.isPersonal_Date);

            this.weightAnamnesis = orderedAnamnesis.find(a => a.isPersonal_Type === 5) ?? { isPersonal_Date: '', isPersonal_Value: '' };;
            this.bloodPressureAnamnesis = orderedAnamnesis.find(a => a.isPersonal_Type === 1) ?? { isPersonal_Date: '', isPersonal_Value: '' };;
            this.cholesterolAnamnesis = orderedAnamnesis.find(a => a.isPersonal_Type === 2) ?? { isPersonal_Date: '', isPersonal_Value: '' };;
            this.heartRateAnamnesis = orderedAnamnesis.find(a => a.isPersonal_Type === 6) ?? { isPersonal_Date: '', isPersonal_Value: '' };;

            console.log(this.bloodPressureAnamnesis)
        }

        this.loading = false;
    }

    async exportHealtStatus(){
        var user = AuthStore.getCurrentUser();
        await PdfApi.downloadHealthStatusReport(user.id);
    }
}
</script>


<style scoped>

</style>
