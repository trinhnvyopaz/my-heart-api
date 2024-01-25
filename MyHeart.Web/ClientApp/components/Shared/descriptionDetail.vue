<template >
    <div id="height" style="height:100%; border: 1px solid lightgray; width:99%;">
        <!--<perfect-scrollbar>
            <wysiwyg v-model="data" style="height: 100%; border: none" @change="updateDescription" />
        </perfect-scrollbar>-->
        <div id="sticky" class="sticky-top">
            <vue-editor id="editor" v-model="data" style="height: 100%; border: none" @input="updateDescription" />
        </div>
        <div></div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
    @Component({
        name: "DescriptionDetail"
    })
    export default class DescriptionDetail extends Vue {
        @Prop({ default: "" })
        value: string;

        data: string = "";
        height: number = 0;

        updateDescription() {
            console.log('emit description', this.data);
            this.$emit("input", this.data);
            this.$emit("changed");
        }

        /*.ps {
        position: relative;
        height: 830px;
        }*/
        mounted() {
            this.data = this.value;

            const nodes = $(".heightProvider");

            for (var i = 0; i < nodes.length; i++) {
                this.height = nodes[i].scrollHeight;
                if (this.height > 0) break;
            }

            this.height -= 150;

            if (this.height == -150) {
                this.height = 600;
            }

            console.log(`before min this.height ${this.height} from window it is ${window.screen.height * 0.76}`);
            this.height = Math.min(this.height, window.screen.height * 0.76);
            console.log(`after min ${this.height}`)

            console.log("height is:" + this.height);

            $("#editor").css({ "height": this.height + "px" });
            $(".ql-editor").css({ "height": this.height + "px" });
        }

        beforeMount() {
            /*class="heightProvider"
            const nodes = $(".heightProvider");

            for (var i = 0; i < nodes.length; i++) {
                this.height = nodes[i].scrollHeight;
                if (this.height > 0) break;
            }

            this.height -= 150;

            if (this.height == -150) {
                this.height = 600;
            }

            console.log(`before min this.height ${this.height} from window it is ${window.screen.height * 0.60}`);
            this.height = Math.min(this.height, window.screen.height * 0.78);
            console.log(`after min ${this.height}`)*/
        }
    }
</script>

<style>
    .ql-toolbar {
        position: -webkit-sticky;
        position: sticky;
        top: 0;
        z-index: 2;
        background-color: #fff;
    }
</style>