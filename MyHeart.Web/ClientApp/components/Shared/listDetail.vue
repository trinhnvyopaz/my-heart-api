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
                            color="error"
                            v-model="showOnlyActive" />
            </v-col>
        </v-row>

        <v-row v-for="item in displayList" :key="item.id" dense style="height: 48px">
            <v-col cols="10" md="10">
                <v-checkbox :label="item.name"
                            color="error"
                            v-model="item.isSelected"
                            :input-value="item.isSelected"
                            @change="updateModelList(item)"
                            dense
                            style="margin: 0" />
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="10" md="10">
                <div class="text-center">
                    <v-pagination v-model="page"
                                  :length="pageCount"
                                  total-visible="7"
                                  color="error" />
                </div>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
    import ListDetailDto from "../../models/professionInfo/helpClass/listDetailDto";

    @Component({
        name: "ListDetail"
    })
    export default class ListDetail extends Vue {
        @Prop({ default: null })
        items: ListDetailDto[];

        @Prop({ default: -1 })
        listType: number;

        searchString: string = "";
        page: number = 1;
        pageLength: number = 10;
        showOnlyActive: boolean = false;

        get pageCount() {
            this.page = 1;
            if (this.listType == 1) {
                let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().startsWith(this.searchString.toLowerCase()));
                if (this.showOnlyActive) {
                    filteredItems = filteredItems.filter(obj => obj.isSelected);
                }
                return Math.ceil(filteredItems.length / this.pageLength);
            }
            else {
                let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
                if (this.showOnlyActive) {
                    filteredItems = filteredItems.filter(obj => obj.isSelected);
                }
                return Math.ceil(filteredItems.length / this.pageLength);
            }
        }

        get displayList() {
            if (this.listType == 1) {
                let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().startsWith(this.searchString.toLowerCase()));
                if (this.showOnlyActive) {
                    filteredItems = filteredItems.filter(obj => obj.isSelected);
                }
                let startIndex = (this.page - 1) * this.pageLength;
                let result = filteredItems.slice(startIndex, startIndex + this.pageLength);
                return result;
            }
            else {
                let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
                if (this.showOnlyActive) {
                    filteredItems = filteredItems.filter(obj => obj.isSelected);
                }
                let startIndex = (this.page - 1) * this.pageLength;
                let result = filteredItems.slice(startIndex, startIndex + this.pageLength);
                return result;
            }
        }

        singleItem: ListDetailDto = new ListDetailDto;

        updateModelList(item: ListDetailDto) {
            this.$emit("updateModelList", item, this.listType);
        }
    }
</script>
