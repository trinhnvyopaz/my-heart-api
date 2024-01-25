<template>
    <v-menu :close-on-content-click="false"
            transition="scale-transition"
            offset-y
            min-width="auto"
            v-model="menuShown">
        <template v-slot:activator="{ on, attrs }">
            <v-text-field :value="userFormatted"
                          :label="label"
                          readonly
                          v-bind="attrs"
                          v-on="on" />
        </template>
        <v-time-picker :value="innerFormatted" @input="dateInnerChanged" format="24hr" />
    </v-menu>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";

    @Component({
        name: "TimeTextField"
    })

    export default class TimeTextField extends Vue {

        dateInnerChanged(value) {
            console.log("received", value);
            let [H, m] = value.split(':');
            let Y = this.date ? this.date.getFullYear() : 2022;
            let M = this.date ? this.date.getMonth() : 0;
            let d = this.date ? this.date.getDate() : 1;
            this.date = new Date(Y, M, d, Number(H), Number(m));
        }

        get innerFormatted() {
            if (this.date)
                return `${(this.date.getHours() + "").padStart(2, '0')}:${(this.date.getMinutes() + "").padStart(2, '0')}`
            return "";
        }

        get userFormatted() {
            if (this.date)
                return this.date.getHours() + ":" + (this.date.getMinutes() + "").padStart(2, '0');
            return "";
        }

        menuShown = false;


        @Prop(Date) value: Date; // v-model = date
        get date() {
            return this.value;
        }
        set date(val) {
            console.log("emmiting", val);
            this.$emit("input", val);
        }

        @Prop({ default: "Čas" })
        label: string;
    }
</script>
