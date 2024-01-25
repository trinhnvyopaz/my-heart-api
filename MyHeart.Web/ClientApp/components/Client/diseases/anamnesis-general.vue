<template>

    <div class="varContent">

        <v-row>
            <v-col cols="12" sm="6" md="4">
                <v-label>Osobní anamnéza</v-label>
                <v-checkbox label="ICHS <50 let věku" v-model="familyAnamnesis.isFamily_ICHS" />
                <v-checkbox label="Chlopenní vada" v-model="familyAnamnesis.isFamily_ValveDefect" />
                <v-checkbox label="Fibrilace síní" v-model="familyAnamnesis.isFamily_AtrialFibrillation" />
                <v-checkbox label="Náhlá smrt" v-model="familyAnamnesis.isFamily_SuddenDeath" />
                <v-checkbox label="Kardiostimulátor" v-model="familyAnamnesis.isFamily_Pacemaker" />
            </v-col>
            <v-col cols="12" sm="6" md="4">
                <v-label>Abusus</v-label>
                <v-checkbox label="Alkohol" v-model="abususAnamnesis.isAbusus_Alcohol" />
                <v-checkbox label="Kuřák" v-model="abususAnamnesis.isAbusus_Smoker" />
                <v-checkbox label="Bývalý kuřák" v-model="abususAnamnesis.isAbusus_Exsmoker" />
            </v-col>
            <v-col cols="12" sm="6" md="4">
                <v-label>Sociální anamnéza</v-label>
                <v-checkbox label="Žije s partnerem" v-model="socialAnamnesis.isSocial_LivingWithPartner" />
                <v-checkbox label="Pracující" v-model="socialAnamnesis.isSocial_Working" />
                <v-checkbox label="Starobní důchod" v-model="socialAnamnesis.isSocial_Pension" />
                <v-checkbox label="Částečně invalidní důchod" v-model="socialAnamnesis.isSocial_PartialDisabilityPension" />
                <v-checkbox label="Invalidní důchod" v-model="socialAnamnesis.isSocial_DisabilityPension" />
            </v-col>
        </v-row>
        <v-divider></v-divider>
        <v-row justify="end">
            <v-btn @click="save" class="mr-4">Uložit</v-btn>
        </v-row>
        <v-snackbar right top v-model="showSnackbar" :color="snackBarColor">{{snackBarMessage}}</v-snackbar>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";

    import anamnesisApi from '@backend/api/anamnesis'
    import UserDto from "../../../models/user/UserDto";
    import AuthStore from "@backend/store/authStore";

    @Component({
        name: "AnamnesisAbusus"
    })
    export default class AnamnesisAbusus extends Vue {

        loadingAnamnesis: boolean = false;

        familyAnamnesis: any = {
            userId: 0,
            isFamilyAnamnesis: true,
            isFamily_ICHS: false,
            isFamily_ValveDefect: false,
            isFamily_AtrialFibrillation: false,
            isFamily_SuddenDeath: false,
            isFamily_Pacemaker: false
        };
        abususAnamnesis: any = { 
            userId: 0, 
            IsSocial_LivingWithPartner: true, 
            IsSocial_Working: false, 
            IsSocial_Pension: false, 
            IsSocial_PartialDisabilityPension: false, 
            IsSocial_DisabilityPension: false 
        };
        socialAnamnesis: any = {
            userId: 0,
            isAbususAnamnesis: true, 
            isAbusus_Alcohol: false, 
            isAbusus_Smoker: false, 
            isAbusus_Exsmoker: false
        };

        isImpersonating: boolean = false;
        user: UserDto = null;

        showSnackbar = false;
        snackBarColor = "";
        snackBarMessage = "";

        mounted() {
            this.isImpersonating = AuthStore.isImpersonating();
            if (this.isImpersonating) {
                this.user = AuthStore.getImpersonatedUser() as UserDto;
            } else {
                this.user = AuthStore.getCurrentUser() as UserDto;
            }

            this.loadAnamnesis();
        }

        async loadAnamnesis() {
            this.loadingAnamnesis = true;

            await Promise.all([
                this.loadAbsus(),
                this.loadFamily(),
                this.loadSocial()
            ]);

            this.loadingAnamnesis = false;
        }

        async loadSocial() {
            if (this.isImpersonating) {
                var sAnamnesis = await anamnesisApi.getSocialAnamnesesFromPerson(this.user.id);
            } else {
                var sAnamnesis = await anamnesisApi.getSocialAnamneses();
            }

            this.socialAnamnesis = sAnamnesis.data;
            if (this.socialAnamnesis == null) {
                this.socialAnamnesis = { userId: this.user.id, IsSocial_LivingWithPartner: true, IsSocial_Working: false, IsSocial_Pension: false, IsSocial_PartialDisabilityPension: false, IsSocial_DisabilityPension: false };
            }
        }

        async loadFamily() {
            if (this.isImpersonating) {
                var fAnamnesis = await anamnesisApi.getFamilyAnamnesesFromPerson(this.user.id);
            } else {
                var fAnamnesis = await anamnesisApi.getFamilyAnamneses();
            }

            this.familyAnamnesis = fAnamnesis.data;
            if (this.familyAnamnesis == null) {
                this.familyAnamnesis = { userId: this.user.id, isFamilyAnamnesis: true, isFamily_ICHS: false, isFamily_ValveDefect: false, isFamily_AtrialFibrillation: false, isFamily_SuddenDeath: false, isFamily_Pacemaker: false };
            }
        }

        async loadAbsus() {
            if (this.isImpersonating) {
                var aAnamnesis = await anamnesisApi.getAbususAnamnesesFromPerson(this.user.id);
            } else {
                var aAnamnesis = await anamnesisApi.getAbususAnamneses();
            }
            
            this.abususAnamnesis = aAnamnesis.data;
            if (this.abususAnamnesis == null) {
                this.abususAnamnesis = { userId: this.user.id ,isAbususAnamnesis: true, isAbusus_Alcohol: false, isAbusus_Smoker: false, isAbusus_Exsmoker: false };
            }
        }

        async save(){
            var socialResponse = await anamnesisApi.saveSocialAnamnesis(this.socialAnamnesis);
            var abususResponse = await anamnesisApi.saveAbususAnamnesis(this.abususAnamnesis);
            var familyResponse = await anamnesisApi.saveFamilyAnamnesis(this.familyAnamnesis);

            if(socialResponse.success && abususResponse.success && familyResponse.success){
                this.showSnackBar("Uloženo", "success")
            }else{
                this.showSnackBar("Nepodařilo se uložit změny", "error")
            }
        }

        showSnackBar(message: string, color: string){
            this.showSnackbar = true;
            this.snackBarColor = color;
            this.snackBarMessage = message;
        }
    }
</script>


<style scoped>
</style>
