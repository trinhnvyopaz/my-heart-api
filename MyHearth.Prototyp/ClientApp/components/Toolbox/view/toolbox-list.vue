<template>

    <b-tabs class="full-height" vertical no-nav-style nav-class="h-100 border-right no-outline" nav-wrapper-class="full-height">
        <b-tab title-link-class="text-center text-secondary" active>
            <template slot="title">
                <i class="fa fa-th-large"></i>
            </template>
            <b-row>
                <toolbox-item-add-new :showTiles="true" />
                <toolbox-item v-for="item of toolboxItems"
                              :key="item.id"
                              :id="item.id"
                              :count="item.count"
                              :name="item.productName"
                              :description="item.description"
                              :type="item.type"
                              :image="item.image"
                              :showTiles="true" />
            </b-row>
        </b-tab>
        <b-tab title-link-class="text-center text-secondary">
            <template slot="title">
                <i class="fa fa-th-list"></i>
            </template>
            <b-row>
                <toolbox-item-add-new :showTiles="false" />
                <toolbox-item v-for="item of toolboxItems"
                              :key="item.id"
                              :id="item.id"
                              :count="item.count"
                              :name="item.productName"                              
                              :type="item.type"
                              :image="item.image"
                              :showTiles="false" />
            </b-row>
        </b-tab>
    </b-tabs>

</template>

<script>

    import ToolboxItem from '../component/toolbox-item.vue';
    import ToolboxItemAddNew from '../component/toolbox-item-add-new.vue';
    import toolboxApi from '@backend/api/toolbox'

    export default {
        data()
        {
            return {
                loading: false,
                toolboxItems: []
            }
        },
        created: function () {

            this.loading = true;
            var that = this;
            
            var data = [];

            toolboxApi.getProductsList().then(r => {
                
                data = r;
                
                if (data != null) {
                    
                    that.toolboxItems = data;

                } else {
                    console.log("--- Failed List");
                }


            });            

            this.loading = false;
        },
        components:
        {
            ToolboxItemAddNew,
            ToolboxItem
        }
    }
</script>

<style scoped>

    .toolbox_box
    {
        display: block;
        width: 300px;
        border: 1px solid gray;
        margin: 20px;
        box-shadow: 0px 0px 20px rgba(0, 0, 0, 0.1);
        position: relative;
    }

    .project_img
    {
        width: 300px;
        z-index: -1;
        clear: both;
    }

    .project_tools
    {
        position: absolute;
        top: 230px;
    }
</style>
