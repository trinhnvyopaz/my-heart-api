<template>

    <div class="search">

        <form class="my-0">
            <div class="row">

                <div class="col">
                    <input class="form-control mr-sm-2" type="search" :placeholder="placeholder" aria-label="Search" v-model='productName' @keyup="showMockAjaxSearch">

                    <div class="search-wrapper" v-if="isDropdownVisible">
                        <search-dropdown :products="data.products"
                                         :categories="data.categories" />
                    </div>

                </div>

                <div class="mt-1"
                     :class="{'mt-sm-0':!qr_show, 'mt-md-0':qr_show,
                    'col-xs-2': !qr_show, 'col-sm-2': !qr_show, 'col-lg-1': !qr_show, 'pl-sm-0': !qr_show, 'pl-md-0': qr_show,
                    'col-12': qr_show, 'col-md-4' : qr_show, 'col-lg-3': qr_show,
                    'col-sm-2 col-md-2 col-lg-2': project_world
                     }">

                    <b-button-group class="w-100">
                        <!-- class="my-2 my-sm-0" -->
                        <b-button block variant="outline-success" class="mt-0" @click="searchProduct()">
                            <i class="fa fa-search" />                            
                        </b-button>
                        <b-button v-if="qr_show" block variant="outline-info" class="mt-0" v-b-modal="'qrModal'">
                            Scan QR Code
                        </b-button>
                    </b-button-group>
                    <b-modal v-if="qr_show"
                             id="qrModal">
                        <b-img fluid src="img/dummy/qr.png" />
                    </b-modal>

                </div>

            </div>

            <div class="row mt-1">
                <div class="col">
                    <div class="col" style="z-index: 1">
                        <b-card no-body style="border:none; padding: 0;margin-left: -15px;">
                            <b-row v-if="foundTools!=null && foundTools.length">
                                <b-col>
                                    <h5>We found following tools:</h5>
                                </b-col>
                            </b-row>
                            <products-grid :toolbox_add="true" :tools="foundTools" />
                        </b-card>
                    </div>
                </div>
            </div>

        </form>

    </div> 

</template>

<script>    
    
    import SearchDropdown from './toolbox-search-dropdown.vue';
    import ProductsGrid from '@components/product/ProductsGrid.vue';
    //import { EventBus } from '../../EventBus.js';
    
    import productsApi from '@backend/api/products'

    export default {
        data() {
            return {
                productName: "",                
                foundTools: null,
                isDropdownVisible: false
            };
        },
        props: {
            data: {},
            qr_show: {},
            project_world: {},
            placeholder: {
                default: 'Enter model name or serial number'
            }
        },
        components: {            
            SearchDropdown,
            ProductsGrid
        },
        methods: {
            searchProduct(searchString, type) {

                var toFind = (searchString ? searchString : this.productName);

                this.productName = toFind;

                if (toFind) {                    

                    var that = this;

                    if (type == "category") {
                        productsApi.FindProductsInCategories(toFind).then(function (r) {
                            that.foundTools = r.data;                            
                        });
                    }
                    else{
                        productsApi.FindProducts(toFind).then(function (r) {
                            that.foundTools = r.data;
                        });
                    }                    
                    
                }                
            },
            showMockAjaxSearch() {
                var that = this;
                this.data = {};
                productsApi.FindProductsAndCategories(this.productName).then(function (r) {
                    that.data = r.data;
                    that.isDropdownVisible = true;                    
                });                
            },
            hideMockAjaxSearch() {                
                this.isDropdownVisible = false;                
            }
            
        }
        ,
        mounted() {
            var that = this;
            this.$eventBus.$on('toolbox-item-selected', function (data) {

                console.log("EVENT BUS");
                that.searchProduct(data.name, data.type);                
                that.hideMockAjaxSearch();
            });        
        }
    };

</script>

<style>

    .search-wrapper
    {
        position: relative;
        width: 100%;
        height: 0;
    }
</style>
