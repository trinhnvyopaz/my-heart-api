<template>
    <simple-dialog v-model="dialogShown"
                   title="Přidat lékaře"
                   positive="Založit"
                   @positiveClick="debouncedSendRequest"
                   width="600"
                   :autoclose="false">
        <strong class="red--text" v-html="error" v-if="error"></strong>
        <v-card-text>
            <v-text-field v-model="register.firstName"
                          color="error"
                          label="Jméno" />
            <v-text-field v-model="register.lastName"
                          color="error"
                          label="Příjmení" />
            <v-text-field v-model="register.email"
                          color="error"
                          label="Email" />
            Lékaři bude emailem zaslán aktivační odkaz.
        </v-card-text>
    </simple-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import RegisterDoctorDto from "@models/doctor/RegisterDoctorDto";
import DoctorApi from "@backend/api/doctor";
import SimpleDialog from "@components/Shared/simpleDialog.vue";
import { forEach, debounce } from "lodash";

@Component({
    name: "AddDoctorDialog",
    components: {
        SimpleDialog
    },

})
export default class AddDoctorDialog extends Vue {
    register: RegisterDoctorDto = new RegisterDoctorDto();

    error: string | null = null;

    debouncedSendRequest = debounce(() => {
        this.createDoctor();
    }, 1000);

    @Prop({ default: false }) value: boolean = false; // v-model = show or not
    get dialogShown() {
        return this.value;
    }
    set dialogShown(val) {
        this.$emit("input", val);
    }

    async createDoctor() {
        this.error = "";

        var result = await DoctorApi.registerDoctor(this.register);
        console.log(result);
        if (result.success) {
            this.$emit("addNewDoctor");
            this.register = new RegisterDoctorDto();
            this.dialogShown = false;
        }
        else {
            if(result.errors == null){
                this.error = "Došlo k neočekávané chybě"
            }else{
                for (var key in result.errors) {
                this.error = result.errors[key];
            }
            }

         
        }
    }

}
</script>