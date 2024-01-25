<template>
    <v-container class="main-layout" grid-list-lg>
        <v-layout row wrap>
            <v-flex xs12 class="d-flex justify-center">
                <label class="questionaire-title">Vyberte ze seznamu Vaše hlavní potíže</label>  
            </v-flex>
            <v-flex xs12>
                <v-text-field v-model="request.filter" placeholder="Vyhledejte potíže" class="search" prepend-inner-icon="mdi-magnify" @click:append="search" @keyup.enter="search">
                    <template v-slot:append>
                        <v-btn class="placeholder-btn">Vyhledat</v-btn>
                    </template>
                </v-text-field>
            </v-flex>
            <v-flex xs12 md4 v-for="item in checkableSymptoms" :key="item.symptom.id">
                <v-checkbox v-model="item.checked" :label="item.symptom.name" />
            </v-flex>
            <v-flex  style="padding: 0;" xs12>
                <v-divider></v-divider>
            </v-flex>
            <v-flex xs12 class="d-flex justify-end">
                <v-btn x-large @click="next" class="justify-end">
                    <span>Pokračovat</span>
                    <v-icon style="margin-left: 20px;">mdi-arrow-right</v-icon></v-btn>  
            </v-flex>  
            <v-snackbar right top v-model="showValidationSnackbar" color="error">Není vybrán žádný příznak</v-snackbar>
        <!-- <v-snackbar right top v-model="showUpdateSnackbar" color="success">Příznak byl upraven</v-snackbar>
        <v-snackbar right top v-model="showCreateSnackbar" color="success">Příznak byl vytvořen</v-snackbar> -->

        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    import SymptomDto from "@models/professionInfo/SymptomDto";
    import ProfessionInfoApi from "@backend/api/symptom";
    import DataTableRequest from "@models/DataTableRequest";
    import EventBus from "@models/EventBus";
    import CheckBox from "@resources/checkbox.svg"

    @Component({
        name: "QuestionaireSypmtom",
        components: {
            
        },
    })

    export default class QuestionaireSypmtom extends Vue {
        symptoms: SymptomDto[] = [];

        checkbox = CheckBox
        //max int value
        request: DataTableRequest = { page: 1, pageSize: 2147483647, filter: "" };
        totalCount: number = 0;
        pageCount: number = 0;
        loading: boolean = false;
        sortQuery: string = "";
        checkableSymptoms: any[] = [];
        showValidationSnackbar: boolean = false;

        mounted() {         
            this.getSymptoms();
            EventBus.$on("reset-questionairesymptoms", this.reset);
        }

        search() {
            this.request.page = 1;
            this.getSymptoms();
        }
        reset(){
            this.checkableSymptoms = [];
            this.getSymptoms();
        }
        next(){
            let valid = this.validate();

            if(valid){
                 this.showValidationSnackbar = false;
                 this.$emit("goToQuestions", this.checkableSymptoms.filter(s => s.checked).map(s => s.symptom.id));
            }
            else{
                this.showValidationSnackbar = true;
            }
        }
        validate(){
            if(this.checkableSymptoms.filter(s => s.checked).length == 0){
                return false;
            }

            return true;
        }
        async getSymptoms() {
            this.loading = true;


            var result = await ProfessionInfoApi.getDataTable(this.request);

            if (result.success) {
                this.symptoms = result.data.data;

                this.checkableSymptoms = this.symptoms.map(s =>  ({
                    symptom: s,
                    checked: false
		        }));

                this.totalCount = result.data.totalCount;
                this.pageCount = Math.ceil(this.totalCount / this.request.pageSize);
            }

            this.loading = false;
        }
    }
</script>


<style scoped>
    .v-checkbox{
        color: #1976d2 !important
    }
    .placeholder-btn{
        background-color: #F2F2F2!important; 
        color: #47474A!important;
        margin: 0;
        pointer-events: none;
    }
    .main-layout{
        background-color: #F2F2F2;
        border-radius: 20px;
    }
    .questionaire-title{
        color: var(--dark);
        font-size: 22px;
        font-weight: bold;
    }
</style>
