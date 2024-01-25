<template>
    <div>
        <div class="margin-15" v-if="loading">
            <v-progress-linear indeterminate
                               color="error"></v-progress-linear>
        </div>

        <h1 class="margin-15">{{ $locale({i: 'common.notInDemo'}) }}</h1>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    @Component({
        components: {
        }
    })

    export default class StatisticsComponnent extends Vue {
        loading: boolean = true;

        created() {
            this.loading = false;
        }
    }
</script>

<style>

</style>