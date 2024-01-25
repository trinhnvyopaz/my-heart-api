<template>
   
    <b-card
        :title="ourTitle"
        header-bg="dark"
        @click="Ouch"
    >

    </b-card>
        
</template>

<script lang="ts">
        
    import {Component, Vue} from "vue-property-decorator"

    @Component({})
    export default class Welcome extends Vue {
        
        loading: boolean = false;
        ourTitle: string = "Welcom to little mission";
        
        public Ouch(e) {
            this.ourTitle = "You touched me";
        }

        ready() {
            console.log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX   welcome to all voayers");
            
        }
        
    }

</script>

<style scoped>
   

</style>
