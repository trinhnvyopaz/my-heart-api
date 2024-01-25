<template>
    <div :class="{'main-layout-checked': checked}" class="answer-main-layout d-flex align-center">
        <span v-if="!checked" class="dot">
        </span>
        <div v-if="checked" class="dot-inner d-flex justify-center">
            <v-icon color="#4CAF50">mdi-check</v-icon>
        </div>
        <span :class="{'text-container-checked': checked}" class="text-container">{{text}}</span>
    </div>
</template>
<script lang="ts">
import Vue from "vue";
import { Component, Prop, Watch} from "vue-property-decorator";

@Component({
    name: "QuestionaireQuestionAnswer",
    components: {
    },
})

export default class QuestionaireQuestionAnswer extends Vue {
    @Prop({default: false}) checked: boolean
    @Prop({default: ""}) text: string

    mounted(){
        console.log(this.checked)
    }

    @Watch("checked")
    onPropertyChanged(value: boolean, oldValue: boolean) {
        console.log(value)
        
    }
}

</script>
<style scoped>
.answer-main-layout{       
    height: 50px;
    width: 480px;
    border-radius: 10px;
    background-color: white;
}
.dot {
    height: 28px;
    width: 34px;
    border-radius: 50%;
    border: 1px var(--blue) solid;
    margin-left: 20px;
    padding: 4px;
}
span{
    width: 100%;
}
.dot-inner{
    background-color: #F2F2F2;
    border-radius: 50%;
    padding: 4px;
    height: 28px;
    margin-left: 20px;
    width: 32px;
}
.text-container{
    margin-left: 16px;
    font-weight: bold;
    font-size: 18px;
    color: #47474A;
}

.main-layout-checked {
    background-color: #0666EB;
}

.text-container-checked {
    color: #ffffff;
}

</style>
