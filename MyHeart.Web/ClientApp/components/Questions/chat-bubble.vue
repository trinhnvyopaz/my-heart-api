<template>
    <div>
        <div v-if="!mine" class="text--secondary" style="font-size: x-small">
            {{user}}
        </div>
        <v-card>
            {{text}}
        </v-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch, Prop } from "vue-property-decorator"; 

    @Component({
        name: "ChatBubble",
        components: {
            
        },
    })
    export default class ChatBubble extends Vue {
        @Prop({ default: "" })
        text: string = "";

        @Prop({ default: "" })
        date: string = "";

        @Prop({ default: "" })
        user: string = "";

        @Prop({ default: false })
        mine: boolean = false;

        mounted() {
            const d = new Date(Date.parse(this.date));
            this.date = d.getHours() + ":" + d.getMinutes() + " " + d.getDay + "." + d.getMonth + "." + d.getFullYear(); 
        }
    }
</script>

<style scoped>
    
</style>
