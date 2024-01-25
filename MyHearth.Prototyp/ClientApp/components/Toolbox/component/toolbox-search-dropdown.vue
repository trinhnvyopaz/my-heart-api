<template>

    <section class="search-dropdown">

        <div v-if="products.length" class="products">

            <small class="search-dropdown-header">Products</small>

            <search-item-product v-for="p in products"
                                 :name="p.name"
                                 :key="p.id"
                                 :image="p.image"
                                 :description="p.description" />

        </div>

        <div v-if="categories" class="categories">

            <small class="search-dropdown-header">Categories</small>

            <search-item-category v-for="category in categories"
                                  :key="category.id"
                                  :name="category.name" />

        </div>

    </section>

</template>

<script>

    import SearchItemProduct from './toolbox-search-item.vue';
    import SearchItemCategory from './toolbox-search-item-category.vue';

    export default {
        props: { products: Array, categories: Array },
        components: {
            SearchItemProduct,
            SearchItemCategory
        }

    };

</script>

<style>

    .search-dropdown
    {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        background: #fff;
        border: 1px solid #ccc;
        border-top: 0;
        z-index: 3;
    }

    .categories
    {
        margin-top: 3px;
        border-top: 1px solid #ccc;
    }

    .search-dropdown-header
    {
        margin-left: 5px;
    }

</style>
