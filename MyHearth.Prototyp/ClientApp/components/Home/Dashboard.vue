<template>
    <div class="background">
        <div class="layer">
            <div class="layer-content">
                <div class="col-12">
                    <h1 class="page-title d-flex justify-content-end text-uppercase" v-localize="{i: 'dashboard.title'}" />
                    <hr/>
                </div>                
            </div>
            <div class="content-container">

                <project-index></project-index>

            </div>
        </div>
    </div>
</template>

<script>
    
    import projectList from "@components/project/project-list.vue";
    import projectIndex from "@components/project/project-index.vue";

    export default {
        data()
        {
            return {
                loading: false,
            }
        },
        components: {
            'project-list': projectList,
            'project-index': projectIndex
        }

    }
</script>

<style scoped>
</style>
