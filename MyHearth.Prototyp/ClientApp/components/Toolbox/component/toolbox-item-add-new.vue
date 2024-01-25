<template>

    <b-col cols="12" :class="{'col-lg-3 col-md-6 pr-1 pl-1 mb-2' : showTiles}"  class="pr-1 pl-1 mb-2">
        <router-link tag="a" to="/toolbox-new" class="card bg-default card-toolbox card-toolbox-static">
            <div class="card-body text-center card-inactive d-flex">
                <div class="align-self-center mx-auto">
                    <i class="fa fa-plus fa-2x" />
                    <div class="mt-2">
                        <strong class="card-title">Add Tool</strong>
                    </div>
                </div>
            </div>
        </router-link>
    </b-col>

</template >

<script>    
        
    export default {
            props: ['showTiles'],            
        }
</script>


<style>

    .card-inactive strong
    {
        white-space: nowrap;
        color: #000;
        opacity: 0.4;
    }

    .card-toolbox-static
    {
        height: 100%;
    }

</style>
