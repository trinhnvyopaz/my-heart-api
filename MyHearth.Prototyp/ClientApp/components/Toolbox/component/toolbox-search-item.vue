<template>
    
    <div class="search-product-container" @click="onSelect">

        <a href="#"><img :src="image" class="search-product-image">{{ name }}</a>
        <span v-if="vendor" class="search-product-vendor">{{ vendor }}</span>

    </div>

</template>

<script>

    //import { EventBus } from '../../EventBus.js';

    export default {
        props: ['id', 'name', 'description', 'rating', 'image', 'vendor'],
        methods: {
            onSelect(e)
            {                
                var that = this;
                this.$eventBus.$emit('toolbox-item-selected', { name: that.name, type: "product" });                
                e.preventDefault();
                return false;
            }
        }
    };

</script>

<style>

    .search-product-container
    {
        padding: 5px;
        line-height: 25px;
        font-size: 120%;
    }

    .search-product-image
    {
        max-width: 25px;
        max-height: 25px;
        margin-right: 10px;
        margin-top: 0;
        vertical-align: top;
    }

    .search-product-vendor
    {
        display: block;
        font-size: 75%;
        margin-left: 35px;
    }

    .search-product-container:hover
    {
        background-color: #ccc;
    }

</style>
