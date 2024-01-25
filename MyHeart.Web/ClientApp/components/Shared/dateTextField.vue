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
        <v-date-picker class="custom-datepicker" :value="innerFormatted" @input="dateInnerChanged" />
    </v-menu>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";

    @Component({
        name: "DateTextField"
    })

    export default class DateTextField extends Vue {

        dateInnerChanged(value) {
            this.menuShown = false;
            console.log("received", value);
            let [Y, M, d] = value.split('-');
            let H = this.date ? this.date.getHours() : 0;
            let m = this.date ? this.date.getMinutes() : 0;
            this.date = new Date(Number(Y), Number(M)-1, Number(d), H, m);
        }

        get innerFormatted() {
            if(this.date)
                return `${this.date.getFullYear()}-${(this.date.getMonth() + 1 + "").padStart(2, '0')}-${(this.date.getDate()+"").padStart(2, '0')}`
            return "";
        }

        get userFormatted() {
            if (this.date)
                return this.date.getDate() + ". " + (this.date.getMonth() + 1) + ". " + this.date.getFullYear();
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

        @Prop({ default: "Datum" })
        label: string;
    }
</script>
