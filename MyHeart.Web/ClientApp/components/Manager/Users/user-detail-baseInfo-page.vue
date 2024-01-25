<template>
    <div >
        <v-container fluid>
            <v-card class="pt-2 elevation-3 px-3" :loading="loadingUser">
                <strong class="red--text" v-html="error" v-if="error"></strong>
                <v-card-title>Základní informace</v-card-title>
                <v-card-text>
                    <v-row>
                        <v-col>
                            <v-text-field v-model="userDetail.firstName"
                                          color="error"
                                          label="Jméno"
                                          readonly="" />
                        </v-col>
                        <v-col>
                            <v-text-field v-model="userDetail.lastName"
                                          color="error"
                                          label="Příjmení"
                                          readonly="" />
                        </v-col>
                        <v-col>
                            <v-text-field v-model="userDetail.email"
                                          color="error"
                                          label="Email"
                                          readonly="" />
                        </v-col>
                        <v-col>
                            <v-text-field v-model="userDetail.pin"
                                          color="error"
                                          label="Rodné číslo" />
                        </v-col>
                        <v-col>
                            <v-select v-model="userDetail.doctorId"
                                      :items="doctorCombo"
                                      color="error"
                                      label="Zadávající lékař" />
                        </v-col>
                        <v-col>
                            <v-switch v-model="userDetail.isSubscription" color="error" label="Předplatné" />
                        </v-col>
                    </v-row>

                    <v-card-title>Anamnéza</v-card-title>
                    <div class="anamnesis">
                        <v-row>
                            <v-col>
                                <v-checkbox v-model="userDetail.isFamilyAnamnesis"
                                            color="error"
                                            label="Rodinná" />
                            </v-col>
                            <v-col>
                                <v-checkbox v-model="userDetail.isPersonalAnamnesis"
                                            color="error"
                                            label="Osobní" />
                            </v-col>
                        </v-row>

                        <v-row class="check-box-group">
                            <v-col>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isFamily_ICHS"
                                                v-show="userDetail.isFamilyAnamnesis"
                                                color="error"
                                                label="ICHS <50 let věku" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isFamily_ValveDefect"
                                                v-show="userDetail.isFamilyAnamnesis"
                                                color="error"
                                                label="Chlopenní vada" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isFamily_AtrialFibrillation"
                                                v-show="userDetail.isFamilyAnamnesis"
                                                color="error"
                                                label="Fibrilace síní" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isFamily_SuddenDeath"
                                                v-show="userDetail.isFamilyAnamnesis"
                                                color="error"
                                                label="Náhlá smrt" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isFamily_Pacemaker"
                                                v-show="userDetail.isFamilyAnamnesis"
                                                color="error"
                                                label="Kardiostimulátor" />
                                </transition>
                            </v-col>
                            <v-col>
                                <transition name="check-box-list">
                                    <v-select v-model="userDetail.isPersonal_DiseaseId"
                                              v-show="userDetail.isPersonalAnamnesis"
                                              :items="diseaseCombo"
                                              color="error"
                                              label="Onemocnění"
                                              multiple />
                                </transition>
                                <transition name="check-box-list">
                                    <v-select v-model="userDetail.isPersonal_NonpharmaticId"
                                              v-show="userDetail.isPersonalAnamnesis"
                                              :items="nonpharmacyCombo"
                                              label="Nefarmakologická léčba"
                                              color="error"
                                              multiple />
                                </transition>
                            </v-col>
                        </v-row>


                        <v-row>
                            <v-col>
                                <v-checkbox v-model="userDetail.isPharmacyAnamnesis"
                                            color="error"
                                            label="Farmakologická" />
                            </v-col>
                            <v-col>
                                <v-checkbox v-model="userDetail.isAllergyAnamnesis"
                                            color="error"
                                            label="Alergie" />
                            </v-col>
                        </v-row>

                        <v-row class="check-box-group">
                            <transition name="check-box-list">
                                <v-col>
                                    <transition name="check-box-list">
                                        <v-select v-model="userDetail.isPharmacy_PharmacyId"
                                                  v-show="userDetail.isPharmacyAnamnesis"
                                                  label="Farmaka - Komerční názvy"
                                                  color="error"
                                                  multiple />
                                    </transition>
                                    <transition name="check-box-list">
                                        <v-text-field v-model="userDetail.isPharmacy_BoldStr"
                                                      v-show="userDetail.isPharmacyAnamnesis"
                                                      label="Síla účinné látky"
                                                      color="error" />
                                    </transition>
                                    <transition name="check-box-list">
                                        <v-select v-model="userDetail.isPharmacy_Dose"
                                                  v-show="userDetail.isPharmacyAnamnesis"
                                                  :items="['1-0-0','1-0-1','0-0-1','½-0-0','½-0-½','0-0-½','Vlastní']"
                                                  label="Dávkování"
                                                  color="error" />
                                    </transition>
                                </v-col>
                            </transition>
                            <transition name="check-box-list">
                                <v-col>
                                    <transition name="check-box-list">
                                        <v-select v-model="userDetail.isAllergy_PharmacyId"
                                                  v-show="userDetail.isAllergyAnamnesis"
                                                  :items="['Komereční název']"
                                                  label="Farmaka - Komerční názvy"
                                                  color="error"
                                                  multiple />
                                    </transition>
                                    <transition name="check-box-list">
                                        <v-text-field v-model="userDetail.isAllergy_Other"
                                                      v-show="userDetail.isAllergyAnamnesis"
                                                      label="Ostatní"
                                                      color="error" />
                                    </transition>
                                </v-col>
                            </transition>
                        </v-row>



                        <v-row>
                            <v-col>
                                <v-checkbox v-model="userDetail.isAbususAnamnesis"
                                            color="error"
                                            label="Abusus" />
                            </v-col>
                            <v-col>
                                <v-checkbox v-model="userDetail.isSocialAnamnesis"
                                            color="error"
                                            label="Sociálně - Pracovní" />
                            </v-col>
                        </v-row>

                        <v-row class="check-box-group">
                            <transition name="check-box-list">
                                <v-col>
                                    <transition name="check-box-list">
                                        <v-checkbox v-model="userDetail.isAbusus_Alcohol"
                                                    v-show="userDetail.isAbususAnamnesis"
                                                    color="error"
                                                    label="Alkohol více než 2 jednotky denně" />
                                    </transition>
                                    <transition name="check-box-list">
                                        <v-checkbox v-model="userDetail.isAbusus_Exsmoker"
                                                    v-show="userDetail.isAbususAnamnesis"
                                                    color="error"
                                                    label="Exkuřák" />
                                    </transition>
                                    <transition name="check-box-list">
                                        <v-checkbox v-model="userDetail.isAbusus_Smoker"
                                                    v-show="userDetail.isAbususAnamnesis"
                                                    color="error"
                                                    label="Aktivní kuřák" />
                                    </transition>
                                </v-col>
                            </transition>

                            <v-col>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isSocial_LivingWithPartner"
                                                v-show="userDetail.isSocialAnamnesis"
                                                color="error"
                                                label="Žije s partnerem" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isSocial_Working"
                                                v-show="userDetail.isSocialAnamnesis"
                                                color="error"
                                                label="Pracující" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isSocial_Pension"
                                                v-show="userDetail.isSocialAnamnesis"
                                                color="error"
                                                label="Starobní důchod" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isSocial_PartialDisabilityPension"
                                                v-show="userDetail.isSocialAnamnesis"
                                                color="error"
                                                label="Částečný invalidní důchod" />
                                </transition>
                                <transition name="check-box-list">
                                    <v-checkbox v-model="userDetail.isSocial_DisabilityPension"
                                                v-show="userDetail.isSocialAnamnesis"
                                                color="error"
                                                label="Plný invalidní důchod" />
                                </transition>

                            </v-col>

                        </v-row>
                    </div>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn elevation="2"
                           text
                           @click="updateUser">
                        Uložit změny
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-container>
        <v-snackbar right top v-model="showSnackbar" color="success">Uživatel byl aktualizován</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";


import moment from "moment";

    import UserDetailDto from "../../../models/user/UserDetailDto";
    import UserDto from "@models/user/UserDto";
    import DiseaseDto from "@models/professionInfo/DiseaseDto";
    import NonpharmacyDto from "@models/ProfessionInformation/NonpharmacyDto";

    import UserApi from "@backend/api/user";
    import DiseaseApi from "@backend/api/disease";
    import NonpharmacyApi from "@backend/api/nonpharmacy";

@Component({
    name: "UserDetailBaseInfoPage",
  components: {
  }
})
export default class UserDetailBaseInfoPage extends Vue {
  id: number = 0;
    loadingUser: boolean | string = false;

    doctors: UserDto[] = [];
    userDetail: UserDetailDto = new UserDetailDto();
    disease: DiseaseDto[] = [];
    nonpharmacy: NonpharmacyDto[] = [];
    showSnackbar: boolean = false;
    error: string | null = null;

  mounted() {
    if (this.$route.params.id) {
      this.id = parseInt(this.$route.params.id);
        this.loadUser();
        this.getDisease();
        this.getNonpharmacy();
        this.getDoctors();
    }
  }

    get doctorCombo() {
        return this.doctors.map(u => ({ value: u.id, text: u.firstName + " "+ u.lastName})); 
    }

    get diseaseCombo() {
        return this.disease.map(u => ({ value : u.id, text: u.name})); 
    }

    get nonpharmacyCombo() {
        return this.nonpharmacy.map(u => ({ value: u.id, text: u.description })); 
    }

  async loadUser() {
    this.loadingUser = "error";

    var result = await UserApi.getUserDetail(this.id);

    if (result.success) {
        this.userDetail = result.data;
    }


    this.loadingUser = false;
  }

    async getDoctors() {
        this.loadingUser = true;

        var result = await UserApi.getUsers(3);

        if (result.success) {
            this.doctors = result.data;
        }

        this.loadingUser = false;
    }

    async getDisease() {
        this.loadingUser = true;

        var result = await DiseaseApi.getDisease();

        if (result.success) {
            this.disease = result.data;
        }

        this.loadingUser = false;
    }

    async getNonpharmacy() {
        this.loadingUser = true;

        var result = await NonpharmacyApi.getNonpharmacy();

        if (result.success) {
            this.nonpharmacy = result.data;
        }

        this.loadingUser = false;
    }


    async updateUser() {
        this.loadingUser = "error";

        if (this.userDetail.pin != null && this.userDetail.pin.length > 10 && this.userDetail.pin.length < 12) {
            var result = await UserApi.updateUser(this.userDetail);

            if (result.success) {
                this.showSnackbar = true;
            }
        }
        else {
            this.error = "Rodnné číslo musí obsahovat 11 znaků"
        }

        this.loadingUser = false;
    }
}
</script>

<style scoped>
    .check-box-group
    {
        margin-top: -3rem;
        margin-left: 3.5rem;
    }
    .anamnesis {
        margin-left: 1rem;
    }

    .check-box-list-enter,
    .check-box-list-leave-to {
        visibility: hidden;
        height: 0;
        margin: 0;
        padding: 0;
        opacity: 0;
    }

    .check-box-list-enter-active {
        transition: all 0.3s;
    }
    .check-box-list-leave-active {
        transition: all 0.1s;
    }

    .card{
        background-color: whitesmoke !important;
    }
</style>
