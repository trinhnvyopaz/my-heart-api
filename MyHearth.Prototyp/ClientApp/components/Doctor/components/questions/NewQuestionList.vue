<template>
    <div>

        <h3 style="text-align: center" class="margin-15">{{ $locale({i: 'demo.zeroNew'}) }}</h3>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';

    @Component({
        components: {
        }
    })

    export default class NewQuestionList extends Vue {

        created() {

        }

    }
</script>

<style>
</style>