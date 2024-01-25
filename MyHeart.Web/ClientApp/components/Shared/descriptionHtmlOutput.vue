<template>
    <div style="max-height: 160px; overflow-x: auto; " class="mt-2">
        <label>{{title}}</label><br />
        <span v-html="description" />
        <hr />
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";

    @Component({
        name: "DescriptionHtmlOutput"
    })

    export default class DescriptionHtmlOutput extends Vue {

        @Prop({ default: null })
        description: string;

        @Prop({ default: "Popis" })
        title: string;
    }
</script>
