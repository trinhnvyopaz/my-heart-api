<template>
    <div style="height: 100%">
        <v-row>
            <v-col cols="10" md="10">
                <v-text-field v-model="searchString"
                              label="Hledat"
                              clearable />
            </v-col>
        </v-row>

        <v-row>
            <v-col cols="10" md="10">
                <v-checkbox label="Pouze aktivni"
                            v-model="showOnlyActive" />
            </v-col>
        </v-row>

        <v-row v-for="(item, index) in displayList" :key="item.id" style="height: 48px">
            <v-col cols="6" md="6">
                <v-checkbox :label="item.name"
                            v-model="item.isSelected"
                            @change="toggleSelection(item)"
                            dense
                            style="margin: 0"
                            :id="'checkbox-' + index" />
            </v-col>
            <v-col cols="6" md="4" v-if="!hidden">
                <v-text-field v-model="item.bondStr"
                              align="center"
                              label="Sila vazby"
                              type="number"
                              single-line
                              @input="changeBondStrength(item)"
                              min="0"
                              max="10"
                              dense
                              style="margin: 0"
                              :id="'textfield-' + index"
                              @wheel="$event.target.blur()"
                              @keydown="handleKeys($event)"/>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12" md="10">
                <div class="text-center">
                    <v-pagination v-model="page"
                                  :length="pageCount" />
                </div>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import BondDetailDto from "../../models/professionInfo/helpClass/bondDetailDto";
    import MathTools from '../../backend/tools/MathTools'

    @Component({
        name: "BondDetail"
    })
    export default class BondDetail extends Vue {
        @Prop({ default: null })
        items: BondDetailDto[];

        @Prop({ default: -1 })
        bondType: number;

        @Watch("bondType", { deep: true })
        onValueChanged() {
            setTimeout(() => {
                let firstTextfield = document.getElementById("textfield-0");
                firstTextfield.focus();
            }, 500)
        }

        searchString: string = "";
        page: number = 1;
        pageLength: number = 10;
        showOnlyActive: boolean = false;

        get pageCount() {
            this.page = 1;
            let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
            if (this.showOnlyActive) {
                filteredItems = filteredItems.filter(obj => obj.isSelected);
            }
            return Math.ceil(filteredItems.length / this.pageLength);
        }

        get displayList() {
            let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
            if (this.showOnlyActive) {
                filteredItems = filteredItems.filter(obj => obj.isSelected);
            }
            let startIndex = (this.page - 1) * this.pageLength;
            let result = filteredItems.slice(startIndex, startIndex + this.pageLength);
            return result;
        }

        singleItem: BondDetailDto = new BondDetailDto;

        toggleSelection(item: BondDetailDto) {
            if (!item.isSelected)
                item.bondStr = 0;
            this.updateBonds(item);

        }

        hidden: boolean = false;


        changeBondStrength(item: BondDetailDto) {
            let currPage = this.page;
            item.bondStr = MathTools.clamp(item.bondStr, 0, 10);

            if (item.bondStr > 0)
                item.isSelected = true;
            if (item.bondStr < 1)
                item.isSelected = false;

            this.updateBonds(item);

            setTimeout(() => {
                this.page = currPage;
            }, 1);

            setTimeout(() => {
                let tf = document.getElementById("textfield-" + this.displayList.indexOf(item));
                tf.focus();
            }, 2);

            

        }

        updateBonds(item: BondDetailDto) {
            item.bondStr = parseInt(item.bondStr.toString(), 10);
            this.items = Object.assign([], this.items);


            this.$emit("updateBonds", item, this.bondType);
        }

        focus() {
            let checkbox = document.getElementById('checkbox-0');

            if (checkbox != null) {
                checkbox.focus();
            } else {
                setTimeout(this.focus, 1000);
            }
        }

        mounted() {
            this.focus();
        }

        handleKeys(e) {
            console.log(e.keyCode)
            if (e.keyCode === 38 || e.keyCode === 40) {
                e.target.blur();
            }
        }

    }
</script>
