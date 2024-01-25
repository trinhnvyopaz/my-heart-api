<template>
    
    <div class="search-category-container"  @click="onSelect">

        <a href="#">{{ name }}</a>

    </div>

</template>

<script>

    export default {
        props: ['name'],
        methods: {
            onSelect(e) {
                var that = this;
                this.$eventBus.$emit('toolbox-item-selected', { name: that.name, type: "category" });
                e.preventDefault();
                return false;
            }
        }
    };

</script>

<style>

    .search-category-container
    {
        padding: 5px;
    }
    .search-category-container:hover
    {
        background-color: #ccc;
    }

</style>
