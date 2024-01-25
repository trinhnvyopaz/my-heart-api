<template>
    <v-dialog v-model="isShowDialog" title="Více možností" width="600">
        <v-form ref="form" v-model="isValidate">
            <v-card-text>
                <v-select v-if="!isOther" label="Vyberte typ dat" :items="optionKeysByType" item-text="name"
                    item-value="key" variant="outlined" v-model="option.key"></v-select>
                <!-- <v-text-field label="Hodnota" variant="outlined" v-model="option.value"
-                        :rules="rulesRequired"></v-text-field> -->
                <item-option-dialog :data="option" :editForm="editForm" :deleteForm="deleteForm"></item-option-dialog>
                <v-btn elevation="2" text @click="submit">
                    Více
                </v-btn>
            </v-card-text>
        </v-form>
    </v-dialog>
</template>
<script lang="ts">
import { keyNames } from "@constants/type.ts";
import { Component, Prop, Vue } from "vue-property-decorator";
import ItemOptionDialog from "./ItemOptionDialog.vue";

@Component({
    name: "AddMoreOptionDialog",
    components: {
        ItemOptionDialog
    }
})
export default class AddMoreOptionDialog extends Vue {
    @Prop() deleteForm: (key: number, index: number) => void;
    @Prop() editForm: (key: number, index: number) => void;
    @Prop({
        default: () => ([
            { key: 3, name: 'Pracoviště' },
            { key: 4, name: 'Jméno' },
            { key: 5, name: 'Příjmení' },
            { key: 6, name: 'Rodné číslo' },
            { key: 7, name: 'Adresa' }
        ])
    }) personalDataKeys: { key: number, name: string }[];
    @Prop({
        default: () => ([
            { key: 8, name: 'Výška' },
            { key: 9, name: 'Váha' },
            { key: 10, name: 'Tlak krve' },
            { key: 11, name: 'Tepová frekvence' },
            { key: 12, name: 'LDL' }
        ])
    }) healthConditionKeys: { key: number, name: string }[];
    @Prop({
        default: () => ([
            { key: 13, name: 'Známá onemocnění' },
            { key: 14, name: 'Známá farmaka' },
            { key: 15, name: 'Známé výkony' },
        ])
    }) knownDataKeys: { key: number, name: string }[];
    @Prop({
        default: () => ([
            { key: 16, name: 'Nová onemocnění' },
            { key: 17, name: 'Nová farmaka' },
            { key: 18, name: 'Nové výkony' },
            { key: 19, name: 'Vysazená léčba' },
            // { key: 20, name: 'Ostatní' },
        ])
    }) newDataKeys: { key: number, name: string }[];
    @Prop({
        default: () => ([
            { key: 2, name: "Datum zprávy" }
        ])
    }) messageDateKey: any;
    isValidate: boolean = false;
    isShowDialog: boolean = false;
    option: any = { type: 3, key: 3, itemMined: "", diseasePharmacyId: "", dateMined: "", noteMined: "" };
    error: any = {};

    rulesRequired = [(v: any) => !!v || "Povinné"];
    get isPersonalData() {
        return this.option.type == 3;
    }
    get isOther() {
        return this.option.type == 7;
    }
    get isHealthConditions() {
        return this.option.type == 4;
    }


    get optionKeysByType() {
        if (this.option.type == 2) {
            return this.messageDateKey;
        }
        if (this.option.type == 3) {
            return this.personalDataKeys;
        }
        if (this.option.type == 4) {
            return this.healthConditionKeys;
        }
        if (this.option.type == 5) {
            return this.knownDataKeys;
        }
        if (this.option.type == 6) {
            return this.newDataKeys;
        }
        return []
    }

    open(option: any) {
        //optionValue.table = "Diseases";
        //optionValue.key = "16";
        this.option = { ...option };
        this.$refs.form?.resetValidation();
        this.isShowDialog = true;
    }
    close() {
        this.isShowDialog = false;
    }
    submit() {
        if (this.$refs.form.validate()) {
            //if (this.isOther) {
            //    //this.option.value = `${this.option.table}|${this.option.itemId}|${this.option.date}|${this.option.description}`;
            //    this.option.itemMined = this.option.itemMined;
            //    this.option.diseasePharmacyId = this.option.diseasePharmacyId;
            //    this.option.dateMined = this.option.dateMined;
            //    this.option.noteMined = this.option.noteMined;
            //}
            this.option.key = [1, 2].includes(this.option.type) ? this.option.type : this.option.key;

            this.option.isSelected = true;
            this.option.keyName = keyNames[this.option.key];
            this.$emit("submit", this.option);
            this.close();
        }
    }
}
</script>
