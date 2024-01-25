<template>
   
    <div class="layer-content">
        <div class="col-12">
            <h1 class="page-title d-flex justify-content-end text-uppercase" v-localize="{i: 'toolbox.title'}" />
            <hr />
        </div>
        
        <div class="row">
            <div class="col full-height">

                <toolbox-list />

            </div>

        </div>

    </div>
        
</template>

<script>
    
    import toolboxListVue from "../Toolbox/view/toolbox-list.vue";

    export default {
        data()
        {
            return {
                loading: false,
            }
        },
        components: {
            'toolbox-list': toolboxListVue
        }

    }
</script>

<style scoped>
   

</style>
