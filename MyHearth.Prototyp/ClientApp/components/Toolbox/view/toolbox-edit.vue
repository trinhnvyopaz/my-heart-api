<template>

    <!-- určitě by se to mělo rozdělit, ale je to prototyp, tak to asi není úplně potřeba dělat -->
    
    <div class="row">

        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">

            <div class="row">

                <div class="col-sm-6 col-md-4 col-lg-4 product-image-container">

                    <img :src="toolboxProduct.image" class="product-image">

                </div>

                <div class="col-sm-6 col-md-8 col-lg-8 mt-xs-1">

                    <div class="card product-card">

                        <div style="line-height: 32px;">
                            <h1 style="display: inline-block;margin-bottom: 0;">{{ toolboxProduct.productName }}</h1>

                            <span v-if="showModalButton">
                                <a v-b-modal="toolboxProduct.productId" class="btn btn-xs text-light btn-success ml-2 p-2" style="margin-bottom: 10px"><font-awesome-icon icon="edit" />&nbsp;Edit</a>
                                <toolbox-modal :id="toolboxProduct.productId" :tool="toolboxProduct.productName" :toolboxProduct="toolboxProduct" />
                            </span>

                        </div>

                        <h6>{{ toolboxProduct.description }}</h6>

                        <p v-if="toolboxProduct.toolBuyDate">Tool buyed: {{ toolboxProduct.toolBuyDate }}</p>

                    </div>

                </div>

            </div>

        </div>

        <div class="col">

            <div class="card mt-5">

                <div class="card-body toolboxProduct">

                    <b-tabs>

                        <b-tab title="Tool actions" title-item-class="list-group-item-success"
                                title-link-class="text-muted">

                            <section class="product-tab-content">
                                <toolbox-actions :toolboxId="tool_id" />
                            </section>
                        </b-tab>                            

                    </b-tabs>

                </div>

            </div>

        </div>

    </div>

</template>

<script>
    
    import ToolboxActions from '../component/toolbox-actions'
    import ToolboxModal from '@components/toolbox/component/toolbox-modal'
    import toolboxApi from '@backend/api/toolbox'

    export default {
        props:[],
        created: function () {
            this.tool_id = this.$route.query.id;
            this.loadProductDetail()
        },
        data()
        {
            return {
                showModalButton: false,
                loading: false,                                
                toolboxProduct: [],
            }
        },
        methods: {
            loadProductDetail: function () {

                var that = this;

                toolboxApi.getProduct(this.tool_id).then(data => {

                    if (data != null) {

                        if (data.toolBuyDate) {
                            data.toolBuyDate = data.toolBuyDate.substring(0, 10);
                        }

                        that.toolboxProduct = data;                        
                        that.showModalButton = true;

                    } else {
                        console.log("--- Failed List");
                    }


                });

            }
        },
        components:
        {
            ToolboxActions,
            ToolboxModal
        },
        mounted() {
            var that = this;
            this.$eventBus.$on('toolbox-product-updated', function (payLoad) {                
                that.loadProductDetail()
            });
      }
    }
</script>

<style scoped>

    .product-tab-content
    {
        padding: 1rem;
        border: 1px solid #dee2e6;
        border-top: none;
    }

    .toolboxProduct .nav-link.active
    {
        color: #495057 !important;
    }
</style>
