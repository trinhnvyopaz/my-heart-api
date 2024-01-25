<template>
    <div>
        <v-tabs color="error" v-model="selectedTab" centered icons-and-text background-color="rgb(245,245,245)">
            <v-tabs-slider></v-tabs-slider>
            <v-tab v-for="item in items" :key="item.id">
                {{ item.title }}
                <v-icon color="item.color">{{ item.icon }}</v-icon>
            </v-tab>

        </v-tabs>
        <v-tabs-items v-model="selectedTab">
            <v-tab-item >
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <users-list></users-list>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <doctors></doctors>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <profession-info></profession-info>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <statistics-list></statistics-list>
                    </v-card-text>
                </v-card>
            </v-tab-item>
            <v-tab-item>
                <v-card flat class="lightgrey-color">
                    <v-card-text>
                        <ticket-list></ticket-list>
                    </v-card-text>
                </v-card>
            </v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop } from "vue-property-decorator";
    import UsersList from "@components/Manager/Users/users-list-page.vue";
    import Doctors from "@components/Manager/Doctors/doctors-page.vue";
    import ProfessionInfo from "@components/Manager/ProfessionInformation/professionInfo-page.vue";
    import TicketList from "@components/Manager/Tickets/ticket-list-page.vue";

    @Component({
        name: "ManagerPage",
        components: {
            UsersList, Doctors, ProfessionInfo, TicketList
        },
    })
    export default class ManagerPage extends Vue {

    selectedTab: number = 0;
  items = [
    {
      icon: "mdi-account-group",
      title: "Pacienti",
      id: 0,
          url: "#Users",
          color: "dimgrey"
    },
    {
      icon: "mdi-doctor",
      title: "Lékaři",
      id: 1,
        url: "#Users",
        color: "dimgrey"
    },
    {
      icon: "mdi-information",
      title: "Odborné informace",
      id: 2,
        url: "#",
        color: "dimgrey"
    },
    {
      icon: "mdi-chart-bar",
      title: "Statistika",
      id: 3,
        url: "#",
        color: "dimgrey"
    },
    {
      icon: "mdi-account-question",
      title: "Veškeré dotazy",
      id: 4,
        url: "#",
        color: "dimgrey"
    }
  ];

  mounted() {}
}
</script>

<style scoped>
    .lightgrey-color {
        background-color: rgb(245,245,245) !important;
    }
</style>
