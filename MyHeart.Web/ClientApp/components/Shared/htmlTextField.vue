<template>
    <v-text-field :label="label" :value="extractText(value)" readonly @focus="$emit('focus')"/>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";

    @Component({
        name: "HtmlTextField"
    })

    export default class HtmlTextField extends Vue {

        @Prop({ default: null })
        value: string;

        @Prop({ default: "Popis" })
        label: string;

        extractText(html: string) {
            var span = document.createElement('span');
            span.innerHTML = html;
            return span.innerText;
        }
    }
</script>
