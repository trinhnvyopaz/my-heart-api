<template>
  <v-toolbar extended extension-height="50px">
    <span class="display-1 ml-3 font-weight-light">{{ title }}</span>
    <template v-slot:extension>
      <div class="d-flex">
        <v-btn to="/" icon>
          <v-icon>mdi-home</v-icon>
        </v-btn>
        <div class="d-flex text-center" v-for="route in routes" :key="route.to">
          <v-divider vertical class="mx-2 my-auto"></v-divider>
          <router-link class="my-auto" v-if="route.to" :to="route.to">{{ route.name }}</router-link>
          <span v-else class="my-auto">{{ route.name }}</span>
        </div>
      </div>
    </template>
  </v-toolbar>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import IBreadcrumbModel from "@models/shared/IBreadcrumbModel";

@Component({
  name: "Breadcrumbs"
})
export default class Breadcrumbs extends Vue {
  @Prop({ default: "" })
  title: string;

  @Prop({ default: [] })
  routes: IBreadcrumbModel[];
}
</script>
