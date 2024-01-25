<template>
    <div>
        <v-tabs v-model="tab">
            <v-tab>Dle onemocnění</v-tab>
            <v-tab @change="backToDiseases()">Všechny otázky</v-tab>
        </v-tabs>

        <v-tabs-items v-model="tab">
            <v-tab-item>
                <div>
                    <v-container>
                        <div v-if="!selected">
                            <v-row>
                                <v-row>
                                    <v-col cols="10">
                                        <v-text-field v-model="searchString"
                                                      label="Hledat"
                                                      clearable
                                                      ref="searchBar" />
                                    </v-col>
                                </v-row>
                            </v-row>
                            <v-row @click="getDetail(item.id)" v-for="(item, index) in displayList" v-bind:key="item.id" style="height: 48px">
                                <div>
                                    <span class="regular-text">{{item.name}} <font-awesome-icon icon="search"></font-awesome-icon></span>
                                </div>
                            </v-row>
                            <v-row>
                                <v-col cols="12">
                                    <div class="text-center">
                                        <v-pagination v-model="page"
                                                      :length="pageCount"
                                                      color="error" />
                                    </div>
                                </v-col>
                            </v-row>
                        </div>
                        <div v-if="selected">
                            <v-row>
                                <v-btn @click="backToDiseases"
                                       text>
                                    <span><font-awesome-icon icon="backward"></font-awesome-icon> zpět</span>
                                </v-btn>
                                <span class="regular-text">
                                    {{selectedDisease.name}}
                                </span>
                            </v-row>
                            <v-divider></v-divider>
                            <v-text-field v-model="allQuestionSearchString"
                                        label="Hledat"
                                        clearable />
                            <template v-for="(item, index) in questionList">
                                <div class="regular-text mt-1 d-flex justify-space-between">
                                    <span>{{item.text}}</span>
                                    <v-icon @click="addQuestion(item)" color="primary">mdi-plus</v-icon>
                                </div>
                                <v-divider></v-divider>
                            </template>
                            <v-row>
                                <v-col cols="12">
                                    <div class="text-center">
                                        <v-pagination v-model="page"
                                                      :length="pageCount"
                                                      color="error" />
                                    </div>
                                </v-col>
                            </v-row>
                        </div>
                    </v-container>
                </div>
            </v-tab-item>

            <v-tab-item>
                <v-divider></v-divider>
                <v-text-field v-model="allQuestionSearchString"
                                        label="Hledat"
                                        clearable />
                <div style="height: 450px;">
                    <template v-for="(item, index) in AllQuestiondisplayList" >
                        <div class="regular-text mt-1 d-flex justify-space-between">
                            <span>{{item.text}}</span>
                            <v-icon @click="addQuestion(item)" color="primary">mdi-plus</v-icon>
                        </div>
                        <v-divider></v-divider>
                    </template>
                </div>
                
            </v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from "vue-property-decorator";
    import DiseaseDto from "../../models/professionInfo/DiseaseDto";
    import SymptomQuestionsDto from "../../models/professionInfo/SymptomQuestionsDto";
    import SymptomQuestionsApi from "@backend/api/symptomQuestions";
    import DataTableRequest from "../../models/DataTableRequest";
    import QuestionDto from "@models/question/QuestionDto";

    @Component({
        name: "DiseaseQuestionDetail"
    })
    export default class DiseaseQuestionDetail extends Vue {
        @Prop({ default: null })
        items: DiseaseDto[];

        @Prop({default: []})
        assignedQuestionIds: number[]


        loading: boolean = false;
        selected: boolean = false;
        tab: number = 0;

        searchString: string = "";
        allQuestionSearchString: string = ""
        page: number = 1;
        pageLength: number = 10;
        get pageCount() {
            this.page = 1;
            let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
            return Math.ceil(filteredItems.length / this.pageLength);
        }
        get displayList() {
            let filteredItems = this.items.filter(obj => !this.searchString || obj.name.toLowerCase().includes(this.searchString.toLowerCase()));
            let startIndex = (this.page - 1) * this.pageLength;
            let result = filteredItems.slice(startIndex, startIndex + this.pageLength);
            return result;
        }

        @Watch("assignedQuestionIds")
        onAssignedQuestionIds(newValue){
            this.allQuestions = this.allQuestions.filter(x => !this.assignedQuestionIds.find(q => q == x.id));
        }

        get AllQuestiondisplayList() {
            let filteredItems = this.allQuestions.filter(obj => !this.allQuestionSearchString || obj.text.toLowerCase().includes(this.allQuestionSearchString.toLowerCase()));
            return filteredItems;
        }
        questionList: SymptomQuestionsDto[] = [];
        allQuestions: SymptomQuestionsDto[] = [];
        allQuestionsList: Map<number, SymptomQuestionsDto[]> = new Map<number, SymptomQuestionsDto[]>();
        request: DataTableRequest = { page: 1, pageSize: 10, filter: "", secondFilter: "" };

        pageDetail: number = 1;
        curID: number = -1;
        selectedDisease: DiseaseDto = null;
        get pageDetailCount() {
            this.pageDetail = 1;
            return (Math.ceil(this.questionList.length / this.pageLength));
        }
        get displayDetailList() {
            let startIndex = (this.page - 1) * this.pageLength;
            let result = this.questionList.slice(startIndex, startIndex + this.pageLength);
            return result;
        }

        reload() {
            console.log("questionReload");
            if (this.curID == -1) return;
            this.mounted();
        }

        getDetail(id: number) {
            this.questionList = this.allQuestionsList.get(id);
            this.curID = id;

            this.selectedDisease = this.items.filter(x => x.id == this.curID)[0];

            this.selected = true;
        }

        async getDetailData(id: number) : Promise<SymptomQuestionsDto[]> {
            this.loading = true;

            var result = await SymptomQuestionsApi.getSymptomQuestionsByDiseaseId(id);

            if (result.success) {
                var data = result.data as SymptomQuestionsDto[];

                for (var single of data) {
                    single.text = single.text.replace(/(<([^>]+)>)/gi, "");
                }

                this.allQuestions = this.allQuestions.concat(data.filter(x => !this.assignedQuestionIds.find(q => q == x.id)));

                return data;
            }

            return [];
        }

        backToDiseases() {
            this.selected = false;
        }

        async mounted() {
            for (var disease of this.items) {
                this.allQuestionsList.set(disease.id, (await this.getDetailData(disease.id)))
            }
        }

        addQuestion(question: SymptomQuestionsDto){
            this.$emit('add-question', question)
        }

    }
</script>
<style scoped>
.regular-text{
    color: #47474A;
    font-size: 18px;
    font-family: nunito;
}
</style>
