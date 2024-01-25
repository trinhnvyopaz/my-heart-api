<template>
    <div style="height: 100%">
        <v-row>
            <v-col cols="10" md="10">
                <v-text-field v-model="searchString"
                              label="Hledat"
                              clearable />
            </v-col>
        </v-row>
        <v-row v-for="(item, index) in displayList" :key="item.id" style="height: 48px">
            <v-col cols="10" md="10">
                <v-checkbox :label="item.name"
                            v-model="item.assigned"
                            color="error"
                            dense
                            style="margin: 0"
                            @change="toggleDiscard(item)"/>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="10" md="10">
                <div class="text-center">
                    <v-pagination v-model="page"
                                  :length="pageCount"
                                  color="error" />
                </div>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";
    import PharmacyDiscardRequest from "../../models/PharmacyDiscardRequest";

    @Component({
        name: "PharmacyDiscardDetail"
    })
    export default class PharmacyDiscardDetail extends Vue {

        @Prop({ default: -1 })
        medicamentGroupId: number;

        @Prop({ default: false })
        isShown: boolean;

        @Watch("isShown", { deep: true })
        onValueChanged() {
            if (this.isShown) {
                console.warn('LOAD DATA');
                this.page = 1;
            }
            
            //Load Data
        }

        loading: boolean = false;

        items: any[] = [];
        searchString: string = null;
        pageLength: number = 10;
        apiItemCount: number = 0;
        request: PharmacyDiscardRequest = { filter: "", group: this.medicamentGroupId, page: 0, pageSize: 10 };

        get page() {
            return this.request.page;
        }

        set page(page: number) {
            if (this.page !== page || this.items.length == 0) {
                this.request.page = page;
                console.log("setting discard page");
                this.loadData();
            }
        }

        get pageCount() {
            return Math.ceil(this.apiItemCount / this.pageLength);
        }

        get displayList() {
            var result = this.items;
            result = result.map(obj => { return { pharmacyId: obj.pharmacyId, medicamentGroupId: obj.medicamentGroupId, assigned: !obj.discarded, name: obj.name } })
            return result;
        }

        async loadData() {
            console.log('loading data');
            this.loading = true;
            var result = await MedicamentGroupApi.getDiscardedPharmacies(this.request);
            //console.warn(result);

            if (result.success) {
                this.items = result.data.data;
                this.apiItemCount = result.data.totalCount;
            }

        }

        async toggleDiscard(item) {

            let toggledItem = this.items.filter(obj => obj.pharmacyId == item.pharmacyId)[0];
            console.warn('toggled item', item);
            toggledItem.discarded = !toggledItem.discarded;

            console.log('toggle', item, this.items);
            var result = await MedicamentGroupApi.toggleDiscard(item);
            console.log(result);

            let list = this.items.filter(obj => !obj.discarded).map(obj => { return { id: obj.pharmacyId, nameReg: obj.name } });

            this.$emit('updatePharmacies', list);
        }

    }
</script>
