<!-- src/components/MyDataTable.vue -->
<template>
  <v-data-table no-data-text="Nejsou k dispozici žádné data" v-bind="$attrs" v-on="$listeners"
    :class="{ 'cursor-pointer': hasEvenRowOnClicked }">
    <template v-for="(_, slot) in $scopedSlots" v-slot:[slot]="props">
      <slot :name="slot" v-bind="props" />
    </template>
  </v-data-table>
</template>

<script>
export default {
  inheritAttrs: false,
  computed: {
    hasEvenRowOnClicked() {
      return this.$listeners["click:row"];
    },
  }
};
</script>
<style scoped>
.cursor-pointer {
  cursor: pointer;
}
</style>
