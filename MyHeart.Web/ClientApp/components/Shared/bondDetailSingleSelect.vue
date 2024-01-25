<template>
    <div style="height: 100%">
        <v-row justify="center">
            <v-col cols="12" md="12">
                <v-text-field v-model="searchString"
                              label="Hledat"
                              ref="searchBar"
                              clearable
                              @click:clear='clearSearch()'/>
            </v-col>
        </v-row>
        <v-row justify="center" v-if="hasSelected && !multi" style="height: 56px">
            <v-col cols="8" md="8">
                <v-checkbox :label="selected.name"
                            v-model="selected"
                            dense
                            style="margin: 0" />
            </v-col>
            <v-col cols="4" md="4" v-if="strength">
                <v-text-field v-model="bondStrength"
                              align="center"
                              type="number"
                              single-line
                              @input="changeDefaultBondStrength()"
                              min="0"
                              max="10"
                              dense
                              style="margin: 0;"
                              :id="'textfield-' + index" 
                              @wheel="$event.target.blur()"
                              @keydown="handleKeys($event)"/>
            </v-col>
        </v-row>
        <v-row justify="center" v-for="(item, index) in displayList" :key="item.id">
            <v-col cols="8" md="8" style="height: 56px">
                <v-checkbox :label="item.name"
                            v-model="item.isSelected"
                            @change="toggleSelection(item)"
                            dense
                            style="margin: 0"
                            :id="'checkbox-' + index" />
            </v-col>
            <v-col v-if="multi && strength" cols="4" md="4" style="height: 56px; padding-top: 0">
                <v-text-field v-model="item.bondStr"
                              type="number"
                              single-line
                              @input="changeBondStrength(item)"
                              min="0"
                              max="10"
                              style="margin: 0!important;padding: 0!important;"
                              :id="'textfield-' + index"
                              @focus="$event.target.select()"
                              @wheel="$event.target.blur()"
                              @keydown="handleKeys($event)"/>
            </v-col>
        </v-row>
     
                
        <v-pagination v-model="itemList.page"
                        :length="pageCount" />
              
       
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import BondDetailDto from "../../models/professionInfo/helpClass/bondDetailDto";
    import MathTools from '../../backend/tools/MathTools';
    import PagedStickyList from '../../backend/tools/PagedStickyList';

    @Component({
        name: "BondDetailSingleSelect"
    })
    export default class BondDetailSingleSelect extends Vue {
        @Prop({ default: null })
        items: PagedStickyList;

        @Prop({ default: -1 })
        bondType: number;

        @Prop({ default: true })
        strength: boolean;

        @Prop({ default: true })
        multi: boolean;

        itemList: PagedStickyList;

        page: number = 1;
        selected: BondDetailDto = null;
        hasSelected: boolean = false;
        bondStrength: number = 0;
        pageLength: number = 10;



        @Watch("items", {deep: true, immediate: true})
        onItemsChange(newValue){
            this.itemList = newValue
        }

        get pageCount() {
            this.fillSelected();
            return this.itemList.pageCount;
        }
        get displayList() {
            return this.itemList.displayList.filter(d => d.name != "");
        }

        get searchString() {
            return this.itemList.filter;
        }

        set searchString(searchString: string) {
            console.log("searchstring is " + searchString)

            if (searchString != null) {
                this.itemList.filter = searchString;
            }
        }

        singleItem: BondDetailDto = new BondDetailDto;

        toggleSelection(item: BondDetailDto) {
            console.log("toggleSelection")
            console.log(!this.multi)
            if (!this.multi) {
                if (!item.isSelected) {
                    item.bondStr = 0;
                    this.selected = null;
                    this.hasSelected = false;
                } else {
                    if (this.hasSelected) {
                        this.selected.isSelected = false;
                        this.selected.bondStr = 0;
                        this.updateBonds(this.selected);
                    } else {
                        this.hasSelected = true;
                    }
                    item.bondStr = this.bondStrength;
                    this.selected = item;
                }
            } else {
                if (!item.isSelected)
                    item.bondStr = 0;
            }

            this.updateBonds(item);

            if (this.strength && item.isSelected) {
                console.warn('STR')
                let index = this.displayList.indexOf(item);
                let tf = document.getElementById('textfield-' + index);
                tf.focus();

                console.warn(index);
            }
            else {
                console.warn('NOT STR');
                this.focus();
            }

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

        clearSearch() {
            this.itemList.filter = "";
        }

        fillSelected() {
            var selected = this.itemList.selected.filter(x => x.isSelected);

            if (selected.length > 0) {
                this.selected = selected[0];
                this.hasSelected = true;
            } else {
                this.searchString = null;
                this.hasSelected = false;
            }
        }

        changeDefaultBondStrength() {
            this.bondStrength = MathTools.clamp(this.bondStrength, 0, 10);
            this.selected.bondStr = this.bondStrength;

            this.updateBonds(this.selected);
        }

        updateBonds(item: BondDetailDto) {
            item.bondStr = parseInt(item.bondStr.toString(), 10);
            this.itemList = Object.assign([], this.itemList);

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

        handleKeys(e) {
            console.log(e.keyCode)
            if (e.keyCode === 38 || e.keyCode === 40) {
                e.target.blur();
            }
        }
    }
</script>

