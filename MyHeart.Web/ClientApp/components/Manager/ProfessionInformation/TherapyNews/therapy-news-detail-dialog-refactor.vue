<template>
    <split-dialog :isNew="isNew"
                 :isModified="isModified"
                 v-model="dialogShown"
                 :therapyNewsId="therapyNewsId"
                 @save="saveTherapyNews"
                 @delete="deleteTherapyNews"
                 @addNew="addNew"
                 :addButtonText="addButtonText"
                 :showAddButton="showAddButton"
                 :title="isNew ? 'Přidat novinku' : 'Upravit novinku'"
                 :focusedDetail="focusedDetail">
        <div>{{error}}</div>
        <v-form ref="form">
            <v-text-field label="Název" v-model="therapyNews.text" @focus="changeFocus(-1)" :rules="requiredRules" />
            <html-text-field :value="therapyNews.description" @focus="changeFocus(0)" />
            <v-text-field label="Externí odkaz" v-model="therapyNews.webLink" @focus="changeFocus(-1)" :rules="requiredRules" />
            <v-text-field label="Onemocnění" :value="diseaseString" @focus="showBondDetail(diseaseList,therapyNews.diseases,0, showDiseaseAdd, 'Přidat onemocnění' );changeFocus(1)" readonly/>
        </v-form>

        <template #rightPanel>
            <v-tab-item><vue-editor ref="descriptionEditor" v-model="therapyNews.description" /></v-tab-item>
            <v-tab-item><bond-detail ref="bondDetail" :items="valueList" :bondType="bondType" @updateBonds="updateBonds" :strength="strength" /></v-tab-item>
        </template>

        <disease-detail-dialog-deep :isNew="deepNew"
                                    v-model="isDiseaseAddActive"
                                    :symptomId="deepId"
                                    :levelsDeep="levelsDeep + 1"
                                    @updateDisease="updateDeepDisease"/>
    </split-dialog>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import TherapyNewsApi from "@backend/api/therapyNews";
    import TherapyNewsDto from "@models/professionInfo/TherapyNewsDto";
    import DiseaseDto from "@models/professionInfo/DiseaseDto";
    import TherapyNewsDiseaseDto from "@models/professionInfo/bonds/TherapyNewsDiseaseDto";
    import DiseaseApi from "@backend/api/disease";

    import BondDetailDto from "@models/professionInfo/helpClass/bondDetailDto";
    import BondDetail from "@components/Shared/bondDetailSingleSelect.vue";
    import BondDetailButtons from "@components/Shared/bondDetailButtonsPaged.vue";

    import DescriptionDetail from "@components/Shared/descriptionDetail.vue";
    import DescriptionDetailButtons from "@components/Shared/descriptionDetailButtons.vue";
    import DescriptionHtmlOutput from "@components/Shared/descriptionHtmlOutput.vue";
    import HtmlTextField from "@components/Shared/htmlTextField.vue";
    import PagedStickyList from "@backend/tools/PagedStickyList";

    import SplitDialog from "@components/Shared/splitDialog.vue";
    import FocusHelper from "@utils/FocusHelper"
@Component({
    name: "TherapyNewsDetailDialogRefactor",
    components: {
        BondDetail, BondDetailButtons, DescriptionDetail, DescriptionDetailButtons,
        DescriptionHtmlOutput, HtmlTextField, SplitDialog
    },
})
export default class TherapyNewsDetailDialogRefactor extends Vue {
    deepId: number = -1;
    deepNew: boolean = true;
    loading: boolean = false;
    error: string | null = null;
    isModified: boolean = false;
    showDialogSave: boolean = false;
    isDiseaseAddActive: boolean = false;
    focusedDetail = -1;

    @Prop({ default: false }) value: boolean = false; // v-model = show or not
    get dialogShown() {
        return this.value;
    }
    set dialogShown(val) {
        this.$emit("input", val);
    }

    @Prop({ default: "" })
    isNew: boolean = false;

    @Prop({ default: 0 })
    therapyNewsId: number = -1;

    saving: boolean = false;
    therapyNews: TherapyNewsDto = new TherapyNewsDto();
    diseaseList: PagedStickyList = new PagedStickyList("disease");
    disease: TherapyNewsDiseaseDto = new TherapyNewsDiseaseDto;

    bondDetailDto: BondDetailDto[] = [];
    singleBondDetail: BondDetailDto = new BondDetailDto;
    isDetailActive: boolean = false;
    isDescriptionDetailActive: boolean = false;
    bondType: number = -1;
    valueList: PagedStickyList = null;
    selectedList: object[] = null;
    isTaken: boolean = false;
    strength: boolean = false;

    addButtonText: string = "";
    showAddButton: boolean = false;
    addButtonAction = null

    requiredRules = [v => (v != null && v !="") || 'Povinné pole']

    @Watch("therapyNews", { deep: true })
    onTherapyNewsModified() {
        this.isModified = true;
    }

    @Prop({default: 0})
    levelsDeep: number 

    @Watch("dialogShown")
    async dialogShownChanged() {
        this.diseaseList = new PagedStickyList("disease", true);

        this.error = "";
        this.changeFocus(-1);
        if(this.$refs.form){
            this.$refs.form.resetValidation()
        }

        if (!this.isNew) {
            if (this.dialogShown) {
                await this.loadTherapyNews();
                this.isModified = false;
            }

        }
        else {
            this.therapyNews = new TherapyNewsDto;
            this.therapyNews.diseases = [];
        }
        this.isDetailActive = false;
        this.isDescriptionDetailActive = false;
        this.showDialogSave = false;
    }

    async changeFocus(index){
            this.focusedDetail = index
            await FocusHelper.delay(100)

            if(this.focusedDetail == 0){                  
                FocusHelper.focusEditor(this.$refs.descriptionEditor.quill)
            }                          
            else if(this.focusedDetail == 1){
                this.$refs.bondDetail.$refs.searchBar.focus()            
            }               
        }

    get diseaseString() {
        var disease = "";
        for (var key in this.therapyNews.diseases) {
            if (!this.diseaseList.selectedNative.find(x => x.id == this.therapyNews.diseases[key].diseaseId))
                continue;
            disease = disease + this.diseaseList.selectedNative.find(x => x.id == this.therapyNews.diseases[key].diseaseId).name + ", ";
        }

        return disease.substring(0, disease.length - 2);
    }

    showDiseaseAdd() {
            this.deepNew = true;
            this.deepId = -1;
            this.isDiseaseAddActive = true;
        }

    beforeCreate(){
        this.$options.components.DiseaseDetailDialogDeep = require('@components/Manager/ProfessionInformation/Disease/disease-detail-dialog-deep.vue').default
    }

    async updateDeepDisease(message: string, updatedDisease){
        await this.diseaseList.loadPage(updatedDisease);
        var bondDetail = this.diseaseList.displayList.find(x => x.id == updatedDisease.id);
        this.updateBonds(bondDetail, 0)
        this.showBondDetail(this.diseaseList, this.therapyNews.diseases,0, this.showDiseaseAdd, 'Přidat příčinu');  
    }

    getBondDetailList(valueList: PagedStickyList, selectedList: object[], bondType: number) {
        this.bondDetailDto = [];
        for (var key in valueList.nativeList) {
            this.singleBondDetail.id = valueList.nativeList[key].id;
            this.singleBondDetail.name = valueList.nativeList[key].name;
            switch (bondType) {
                case 0: {
                    if (selectedList.length > 0 && selectedList.find(x => x.diseaseId == valueList.nativeList[key].id) != null) {
                        this.singleBondDetail.isSelected = true;
                    }
                    else {
                        this.singleBondDetail.bondStr = 0;
                        this.singleBondDetail.isSelected = false;
                    }
                    break;
                }

            }
            this.bondDetailDto.push(this.singleBondDetail);

            this.singleBondDetail = new BondDetailDto;
        }
    }

    updateBonds(item: BondDetailDto, bondType: number) {
        this.isTaken = false;
        switch (bondType) {
            case 0: {
                this.therapyNews.diseases = this.therapyNews.diseases.filter(x => x.diseaseId != item.id);
                this.diseaseList.selected = this.diseaseList.selected.filter(x => x.id != item.id);
                this.diseaseList.selectedNative = this.diseaseList.selectedNative.filter(x => x.id !== item.id);
                if (item.isSelected) {
                    this.therapyNews.diseases.push({ diseaseId: item.id, therapyNewsId: this.therapyNewsId });
                    this.diseaseList.selectedNative.push(this.diseaseList.nativeList.find(x => x.id === item.id));
                    this.diseaseList.selected.push(item);
                }
                //this.diseaseList.sort();
                break;
            }
        }

        this.therapyNews = Object.assign({}, this.therapyNews);
    }

    addNew(){
        this.addButtonAction()
    }

    showBondDetail(valueList: PagedStickyList, selectedList: object[], bondType: number, addNewFunction, addButtonText: string) {
        this.isDescriptionDetailActive = false;

        if (!this.isDetailActive) {
            this.getBondDetailList(valueList, selectedList, bondType);
            this.valueList = valueList;
            if (valueList.page === 0) {
                this.valueList.page = 1;
            }
            this.bondType = bondType;
            this.isDetailActive = !this.isDetailActive;
        }
        else {
            if (bondType != this.bondType) {
                this.getBondDetailList(valueList, selectedList, bondType);
                this.valueList = valueList;
                if (valueList.page === 0) {
                    this.valueList.page = 1;
                }
                this.bondType = bondType;
            }
            else {
                this.isDetailActive = !this.isDetailActive;
            }
        }

        if(this.levelsDeep <= 1){
            this.showAddButton = true;
        }

        this.addButtonText = addButtonText;
        this.addButtonAction = addNewFunction;
    }    
    
    async getDisease() {
        if (this.therapyNews.diseases != null && this.therapyNews.diseases.length > 0) {
            const diseaseIds = this.therapyNews.diseases.map(x => x.diseaseId);
            var result = await DiseaseApi.getByIds(diseaseIds);

            if (result.success) {
                var diseases = result.data;

                var diseaseBonds = this.therapyNews.diseases;
                for (var key in diseaseBonds) {
                    var disease: DiseaseDto = diseases.find(x => x.id == diseaseBonds[key].diseaseId);

                    if (disease != null && disease != undefined) {
                        this.diseaseList.selected.push({ bondStr: 0, id: diseaseBonds[key].diseaseId, isSelected: true, name: disease.name });
                    }
                }

                this.diseaseList.selectedNative = this.diseaseList.selectedNative.concat(diseases);
            }
        }
    }

    async loadTherapyNews() {
        this.loading = true;

        var result = await TherapyNewsApi.getTherapyNewsDetail(this.therapyNewsId);

        if (result.success) {
            this.therapyNews = result.data;

            this.getDisease();
        }

        this.loading = false;
        this.isModified = false;
    }

    async saveTherapyNews() {
        this.loading = true;
        this.error = "";
        this.saving = true;

        var result = this.isNew ?
            await TherapyNewsApi.createTherapyNews(this.therapyNews) :
            await TherapyNewsApi.updateTherapyNews(this.therapyNews);
        console.log(result);
        if (result.success) {
            this.therapyNews = result.data;
            this.isModified = false;
            this.$emit("updateTherapyNews", "Záznam uložen.", this.therapyNews);
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

    async deleteTherapyNews() {
        this.loading = true;
        this.error = "";
        this.saving = true;
        var result = await TherapyNewsApi.deleteTherapyNews(this.therapyNewsId);
        if (result.success) {
            this.$emit("updateTherapyNews", "Záznam odstraněn.");
        }
        this.saving = false;
        this.loading = false;
    }
}
</script>
<style scoped>
    .dialog-footer{
        font-size:13px;
        color:rgb(150,150,150);
    }

</style>
