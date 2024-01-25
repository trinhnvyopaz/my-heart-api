<template>
    <div>
        <div class="d-flex flex-row align-center">
            <v-text-field @keyup.enter="addItem" ref="synonymField" label="Nové synonymum" v-model="synonymName" />
            <v-btn @click="addItem" class="ml-2">Přidat</v-btn>
        </div>

        <div class="mb-2">
        {{ error }}
        </div>

        <div v-for="item in sortedItems" :key="item.id">
            <v-icon @click="deleteItem(item.name)" color="error">mdi-delete</v-icon>
            <label class="synonym-label">{{ item.name }}</label>
        </div>
    </div>
</template>
<script lang="ts">
import { Component, Prop, Vue, Watch} from "vue-property-decorator";
import SynonymDetailModel from "@models/professionInfo/bonds/SynonymDetailModel"
import DiseaseSynonymDto from "../../models/professionInfo/bonds/DiseaseSynonymDto";

@Component({
    name: "SynonymDetail",
    components: {
        
    },
})

export default class SynonymDetail extends Vue {
    @Prop({default: () => []})
    items;

    error: string = ""

    synonymName: string = ""

    deleteItem(name: string){
        this.$emit('delete-item', name)
    }

    mounted(){
        console.log(this.items)
    }

    get sortedItems(){
        console.log(this.items)
        return this.items.slice().sort((a, b) => a.name.localeCompare(b.name));
    }

    addItem(){
        if(this.synonymName == ""){
            return;
        }

        const exists =this.items.some(x => x.name == this.synonymName)
        if(exists){
            this.error = "Synonymum už existuje!"
            return;
        }
      
        this.error = ""
        const data = {
            name: this.synonymName,
            isManual: true
        } 
    
        this.synonymName = ""

        this.$emit('add-item', data)      
    }
}

</script>
<style scoped>
.synonym-label{
    color: black;
    font-weight: 800;
    font-size: 18px;
}
</style>