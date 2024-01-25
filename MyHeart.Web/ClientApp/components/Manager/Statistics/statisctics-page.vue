<template>
  <div>
    <top-bar :tabNames="['Statistika']" />
    <v-container class="d-flex justify-center align-center">
      <v-btn @click="downloadPacients" class="btn-spacing">Pacienti</v-btn>
      <v-btn @click="downloadDoctors" class="btn-spacing">Lékaři</v-btn>
      <v-btn @click="downloadQuestions" class="btn-spacing">Dotazy</v-btn>
      <v-btn @click="downloadDiseases" class="btn-spacing">Onemocnění</v-btn>
      <v-btn @click="downloadPharmacy" class="btn-spacing">Léčba</v-btn>
      <v-btn @click="downloadNonpharmacy">Provedené výkony</v-btn>
    </v-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import TopBar from "../../top-bar.vue";
import StatisticsApi from "@backend/api/statistics"

@Component({
  name: "StatiscticsPage",
  components: {
    TopBar
  },

})
export default class StatiscticsPage extends Vue {
    async downloadPacients(){
      await StatisticsApi.downloadPacients();
    }
    async downloadQuestions(){
      await StatisticsApi.downloadQuestions();
    }
    async downloadDoctors(){
      await StatisticsApi.downloadDoctors();
    }
    async downloadDiseases(){
      await StatisticsApi.downloadDiseases();
    }
    async downloadPharmacy(){
      await StatisticsApi.downloadPharmacy();
    }
    async downloadNonpharmacy(){
      await StatisticsApi.downloadNonpharmacy();
    }
}
</script>
<style scoped>
.btn-spacing{
  margin-right: 5px;
}
</style>