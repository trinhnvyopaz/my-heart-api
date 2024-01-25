<template>
    <div class>
        <v-container fluid>
            <v-card class="pt-2 elevation-3 px-3 card" :loading="loadingUser">
                <v-card-title>Základní informace</v-card-title>
                <v-card-text>
                    <v-row>
                        <v-col>
                            <v-text-field v-model="doctorDetail.firstName"
                                          color="error"
                                          label="Jméno" />
                        </v-col>
                        <v-col>
                            <v-text-field v-model="doctorDetail.lastName"
                                          color="error"
                                          label="Příjmení" />
                        </v-col>
                        <v-col>
                            <v-text-field v-model="doctorDetail.email"
                                          color="error"
                                          label="Email"
                                          readonly="" />
                        </v-col>
                        <v-col>
                            <div class="text-center">
                                <v-rating v-model="rating"></v-rating>
                            </div>
                        </v-col>

                    </v-row>

                    <v-card-title>Dny aktivní služby</v-card-title>
                    <div class="next-title">

                    </div>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn elevation="2"
                               text
                               @click="updateDoctor">
                            Uložit změny
                        </v-btn>
                    </v-card-actions>
                </v-card-text>
            </v-card>
        </v-container>
        <v-snackbar right top v-model="showSnackbar" color="success" >Lékař byl aktualizován</v-snackbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";


import moment from "moment";

    import DoctorDetailDto from "@models/doctor/DoctorDetailDto";
    import DoctorApi from "@backend/api/doctor";

@Component({
    name: "UserDetailBaseInfoPage",
  components: {
  }
})
export default class UserDetailBaseInfoPage extends Vue {
  id: number = 0;
    loadingUser: boolean | string = false;
    showSnackbar: boolean = false;
    doctorDetail: DoctorDetailDto = new DoctorDetailDto();
    rating: number = 4.4;

  mounted() {
    if (this.$route.params.id) {
      this.id = parseInt(this.$route.params.id);
        this.loadDoctor();
    }
  }

  async loadDoctor() {
        this.loadingUser = "error";

      var result = await DoctorApi.getDoctorDetail(this.id);

        if (result.success) {
            this.doctorDetail = result.data;
        }


        this.loadingUser = false;
    }

    async updateDoctor() {
        this.loadingUser = "error";

        var result = await DoctorApi.updateDoctor(this.doctorDetail);

        if (result.success) {
            this.showSnackbar = true;
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
    .next-title {
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

    .card{
        background-color: whitesmoke !important;
    }
</style>
