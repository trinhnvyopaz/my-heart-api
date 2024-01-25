<template>
    <split-dialog :isNew="isNew"
                  :isModified="isModified"
                  v-model="dialogShown"
                  @save="savePharmacy"
                  @delete="deletePharmacy"
                  @addNew="addNew"
                  :addButtonText="addButtonText"
                  :showAddButton="showAddButton"
                  :title="isNew ? 'Přidat farmakum' : 'Upravit farmakum'"
                  :focusedDetail="focusedDetail">
        <div v-html="error"></div>
        <v-form ref="form">
            <v-text-field label="Název účiné látky" v-model="pharmacy.name" @focus="changeFocus(-1)" :rules="requiredRules" />
            <v-text-field label="Léková skupina" :value="medicamentGroupString" @focus="showListDetail(medicamentGroup, pharmacy.activeSubstance, 0, showMedicamentAdd, 'Přidat skupinu');changeFocus(0)" readonly />
            <v-row>
                <v-col cols="4"><v-text-field label="Sukl" type="number" v-model="pharmacy.suklCode" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="Síla" v-model="pharmacy.strength" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="Forma" v-model="pharmacy.form" @focus="changeFocus(-1)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="4"><v-text-field label="Balení" v-model="pharmacy.package" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="Cesta" v-model="pharmacy.path" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="Doplněk" v-model="pharmacy.supplement" @focus="changeFocus(-1)" /></v-col>
            </v-row>
            <v-row>
                <v-col cols="4"><v-text-field label="ATC WHO" v-model="pharmacy.atcWho" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="DDDAMNT WHO" type="number" v-model="pharmacy.dddamntWho" @focus="changeFocus(-1)" /></v-col>
                <v-col cols="4"><v-text-field label="DDDUN WHO" v-model="pharmacy.dddunWho" @focus="changeFocus(-1)" /></v-col>
            </v-row>
        </v-form>


        <h3>Dávkování</h3>
        <v-text-field label="Dávkování" v-model="pharmacy.dosage" @focus="changeFocus(-1)" />
        <v-text-field label="Minimální dávka" v-model="pharmacy.minimumDose" @focus="changeFocus(-1)" />
        <v-text-field label="Maximální dávka" v-model="pharmacy.maximumDose" @focus="changeFocus(-1)" />
        <v-text-field label="Název Reg" v-model="pharmacy.nameReg" @focus="changeFocus(-1)" />

        <v-btn class="custom" v-if="pil != null && pil.pilAvailable" @click="getPilDoc">
            Stáhnout příbalový leták
        </v-btn>

        <template #rightPanel>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="listType" :strength="strength" v-on:updateBonds="updateModelList" /></v-tab-item>
        </template>

        <medicament-group-detail-dialog-deep :isNew="deepNew"
                                            v-model="isMedicamentGroupAddActive"
                                            :symptomId="deepId"
                                            :levelsDeep="levelsDeep + 1"
                                            @updateMedicamentGroup="updateMedicamentGroup" />
    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import PharmacyApi from "@backend/api/pharmacy";
    import PilApi from "@backend/api/pil";
    import MedicamentGroupApi from "@backend/api/medicamentGroup";
    import PharmacyDto from "../../../../models/professionInfo/PharmacyDto";
    import PilDto from "../../../../models/professionInfo/PilDto";
    import MedicamentGroupDto from "../../../../models/professionInfo/MedicamentGroupDto";
    import ListDetail from "../../../Shared/listDetail.vue";
    import ListDetailButton from "../../../Shared/listDetailButton.vue";
    import BondDetailDto from "../../../../models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "../../../Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "../../../Shared/bondDetailButtonsPaged.vue";
    import PagedStickyList from "../../../../backend/tools/PagedStickyList";
    import SplitDialog from "@components/Shared/splitDialog.vue";
    import FocusHelper from "@utils/FocusHelper"

    @Component({
        name: "PharmacyDetailDialogDeep",
        components: {
            BondDetail, BondDetailButtons, ListDetail, ListDetailButton, SplitDialog
        },
    })
    export default class PharmacyDetailDialogDeep extends Vue {
        deepId: number = -1;
        deepNew: boolean = true;
        loading: boolean = false;
        error: string | null = null;
        saving: boolean = false;
        isPharmacyLoaded: boolean = false;
        isModified: boolean = false;
        showDialogSave: boolean = false;
        isMedicamentGroupAddActive: boolean = false;        

        //disease: DiseaseDto[] = [];
        medicamentGroup: PagedStickyList = new PagedStickyList("medicamentgroup");
        //symptoms: SymptomDto[] = [];

        pharmacy: PharmacyDto = new PharmacyDto();
        pil: PilDto = null;

        listDetailDto: BondDetailDto[] = [];
        singleItem: BondDetailDto = new BondDetailDto;
        valueList: PagedStickyList = null;
        listType: number = -1;
        isListDetailActive: boolean = false;
        strength = false;

        addButtonText: string = "";
        showAddButton: boolean = false;
        addButtonAction = null

        focusedDetail = -1;

        requiredRules = [v => (v != null && v !="") || 'Povinné pole']

        @Prop({ default: false }) value: boolean = false; // v-model = show or not
        get dialogShown() {
            return this.value;
        }
        set dialogShown(val) {
            this.$emit("input", val);
        }

        @Prop({default: 0})
        levelsDeep: number 

        @Prop({ default: "" })
        isNew: boolean = false;

        @Prop({ default: "" })
        pharmacyId: number = -1;

        @Watch("pharmacy", { deep: true })
        onModified() {
            this.isModified = true;
        }

        @Watch("dialogShown")
        async dialogShownChanged() {
            this.isPharmacyLoaded = false;

            this.medicamentGroup = new PagedStickyList("medicamentgroup", true);

            this.changeFocus(-1);
            this.error = ""

            if(this.$refs.form){
                this.$refs.form.resetValidation()
            }

            if (!this.isNew) {
                if (this.dialogShown) {
                    await this.loadPharmacy();
                }
            }
            else {
                this.isPharmacyLoaded = true;
                this.pharmacy = new PharmacyDto;
                this.pharmacy.activeSubstance = [];
            }
            this.isListDetailActive = false;
            this.isModified = false;
        }

        async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)
                         
            if(this.focusedDetail == 0){
                this.$refs.bondDetail.$refs.searchBar.focus()                
            }              
        }

        get medicamentGroupString() {
            var medicamentGroupStringTemp = "";
            for (var key in this.pharmacy.activeSubstance) {
                if (!this.medicamentGroup.selectedNative.find(x => x.id == this.pharmacy.activeSubstance[key].medicamentGroupId))
                    continue;

                    if(medicamentGroupStringTemp != ""){
                        medicamentGroupStringTemp += ", ";
                    }

                    medicamentGroupStringTemp = medicamentGroupStringTemp + this.medicamentGroup.selectedNative.find(x => x.id == this.pharmacy.activeSubstance[key].medicamentGroupId).name ;
            }
            return medicamentGroupStringTemp;
        }

        showMedicamentAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isMedicamentGroupAddActive = true;
        }

        async updateMedicamentGroup(message: string, updatedGroup){
            await this.medicamentGroup.loadPage(updatedGroup);    
            console.log(updatedGroup)
            console.log(this.medicamentGroup.displayList.map(x => x.id))        
            var bondDetail = this.medicamentGroup.displayList.find(x => x.id == updatedGroup.id);
            console.log(bondDetail)
            this.updateModelList(bondDetail, 0)
            this.showListDetail(this.medicamentGroup, this.pharmacy.activeSubstance, 0, this.showMedicamentAdd, 'Přidat skupinu')
        }

        beforeCreate(){
            this.$options.components.MedicamentGroupDetailDialogDeep = require('@components/Manager/ProfessionInformation/MedicamentGroup/medicament-group-detail-dialog-deep.vue').default
        }

        showListDetail(valueList: PagedStickyList, selectedList: object[], listType: number, addNewFunction, addButtonText: string) {
            if (!this.isListDetailActive) {
                this.getListDetailList(valueList, selectedList, listType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.listType = listType;
                this.isListDetailActive = !this.isListDetailActive;
            }
            else {
                this.isListDetailActive = !this.isListDetailActive;
            }

            if(this.levelsDeep <= 1){
                this.showAddButton = true;
            }

            this.addButtonText = addButtonText;
            this.addButtonAction = addNewFunction;
        }

        getListDetailList(valueList: PagedStickyList, selectedList: object[], listType: number) {
            this.listDetailDto = [];
            for (var key in valueList.nativeList) {
                this.singleItem.id = valueList.nativeList[key].id;
                this.singleItem.name = valueList.nativeList[key].name;
                switch (listType) {
                    case 0:
                        if (selectedList.length > 0 && selectedList.find(x => x.pharmacyId == valueList.nativeList[key].id) != null) {
                            this.singleItem.bondStr = 0;
                            this.singleItem.isSelected = true;
                        }
                        else {
                            this.singleItem.bondStr = 0;
                            this.singleItem.isSelected = false;
                        }
                        break;
                }
                this.listDetailDto.push(this.singleItem);
                this.singleItem = new BondDetailDto;
            }
        }

        updateModelList(item: BondDetailDto, listType: number) {
            switch (listType) {
                case 0:
                    console.log(this.medicamentGroup)
                    this.pharmacy.activeSubstance = this.pharmacy.activeSubstance.filter(x => x.medicamentGroupId != item.id);
                    this.medicamentGroup.selected = this.medicamentGroup.selected.filter(x => x.id != item.id);
                    this.medicamentGroup.selectedNative = this.medicamentGroup.selectedNative.filter(x => x.id != item.id);
                    console.log(this.medicamentGroup)
                    if (item.isSelected) {
                        this.pharmacy.activeSubstance.push({ pharmacyId: this.pharmacyId, medicamentGroupId: item.id, pharmacy: null, bondStrength: 10 });

                        this.medicamentGroup.selectedNative.push(this.medicamentGroup.nativeList.find(x => x.id === item.id));
                        this.medicamentGroup.selected.push(item);
                        console.log(this.medicamentGroup)
                    }
                    //this.medicamentGroup.sort();
                    break;
            }
            let temp = Object.assign({}, this.pharmacy);
            this.pharmacy = temp;
            this.isModified = true;
        }

        async loadPharmacy() {
            this.loading = true;

            var result = await PharmacyApi.getPharmacyDetail(this.pharmacyId);

            if (result.success) {
                this.pharmacy = result.data;
                this.isPharmacyLoaded = true;
            }

            await Promise.all([
                this.getMedicamentGroups(),
                this.getPil()
            ])

            this.loading = false;
        }

        async getPilDoc() {
            PilApi.getFileBySukl(this.pil.sukl, this.pil.pilFileName);
        }

        async getPil() {
            const sukl = this.pharmacy.suklCode;
            var result = await PilApi.getPillBySukl(sukl);

            if (result.success) {
                this.pil = new PilDto();
                this.pil = result.data;
            }

        }

        addNew(){
            this.addButtonAction()
        }

        async getMedicamentGroups() {
            if (this.pharmacy.activeSubstance != null && this.pharmacy.activeSubstance.length > 0) {
                const pharmacyIds = this.pharmacy.activeSubstance.map(x => x.medicamentGroupId);
                console.log(pharmacyIds)
                var result = await MedicamentGroupApi.getByIds(pharmacyIds);
                console.log(result)
                if (result.success) {
                    var substances = result.data;
                    var substanceBonds = this.pharmacy.activeSubstance;

                    for (var key in substanceBonds) {
                        var medicamentGroup: MedicamentGroupDto = substances.find(x => x.id == substanceBonds[key].medicamentGroupId);
                        
                        if (medicamentGroup != null && medicamentGroup != undefined) {
                            this.medicamentGroup.selected.push({ bondStr: 0, id: substanceBonds[key].medicamentGroupId, isSelected: true, name: medicamentGroup.name });
                        }
                    }

                    this.medicamentGroup.selectedNative = this.medicamentGroup.selectedNative.concat(substances);
                }
            }
        }

        async savePharmacy() {
            this.loading = true;
            this.error = "";
            this.saving = true;

            console.log(this.pharmacy)

            var result = this.isNew ? await PharmacyApi.createPharmacy(this.pharmacy)
                : await PharmacyApi.updatePharmacy(this.pharmacy)
            if (result.success) {
                this.pharmacy = result.data;
                this.isModified = false;
                this.$emit("updatePharmacy", "Záznam uložen.", this.pharmacy);
                this.dialogShown = false;

            }
            else {
                for (var key in result.errors) {
                    var errors = result.errors[key];
                    for (var index in errors) {
                        this.error += `${errors[index]} `;
                        this.error += "<br>";
                    }
                }
            }
            this.saving = false;
            this.loading = false;
        }

        async deletePharmacy() {
            this.loading = true;
            this.error = "";
            this.saving = true;
            var result = await PharmacyApi.deletePharmacy(this.pharmacyId);
            if (result.success) {
                this.$emit("updatePharmacy", "Záznam odstraněn.");
            }
            this.saving = false;
            this.loading = false;
        }
    }
</script>
<style scoped>
.row{
    margin-left: 0;
    margin-right: 0;
}
</style>