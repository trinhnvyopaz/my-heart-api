<template>
    <div class="margin-15 shadow">

        <h1>{{question.text}}</h1>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';

    import { translations } from '../../utils/translations';
    import userStore from '@backend/store/userStore';

    @Component({
        components: {
        }
    })

    export default class QuestionDetail extends Vue {
        @Prop() question;

        created() {
           
        }

    }
</script>

<style>
   
</style>