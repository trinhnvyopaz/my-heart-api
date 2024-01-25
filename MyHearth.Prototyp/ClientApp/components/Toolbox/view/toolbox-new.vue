<template>

    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">

        <b-card>
            <b-row>
                <b-col cols="12">
                    <h3>Find your product</h3>
                </b-col>

                <div class="col-12 mb-2">
                    <search :qr_show="true" />
                </div>

                <div class="col-sm-4 col-xs-5 col-lg-3 mb-3">
                    <!--<multi-menu />-->
                </div>

            </b-row>

        </b-card>

    </div>

</template>

<script>

    import Search from '../component/toolbox-search.vue';    

    export default {
        data() {
            return {                
                //searchData: {
                //    products: [
                //        {
                //            id: 0,
                //            name: 'Cordless circular saw',
                //            image: 'img/dummy/market/ccs.jpg',
                //            description: 'A very short description',
                //            vendor: 'Einhell'
                //        },
                //        {
                //            id: 1,
                //            name: 'Cordless circular drill',
                //            image: 'img/dummy/market/cd.jpg',
                //            description: 'A very short description',
                //            vendor: 'Bosch'
                //        }
                //    ]
                //},
                isEinhellProductSelected: false,                
            }
        },
        components: {            
            Search,            
            //MultiMenu,            
            //ProductSorter
        },
        methods: {
            onEinhellProductSelected() {
                this.isEinhellProductSelected = !this.isEinhellProductSelected;
            }
        },
        mounted() {
            var that = this;
            //EventBus.$on('searchItemSelected', function (d) {
            //    that.isEinhellProductSelected = (d.vendor === 'Einhell');
            //});
        }
    };

</script>

<style scoped>

</style>
