<template>
    <div id="app-root">
        <div class="container-fluid">

            <div class="m-md-2" id="content-box">
                Hello all samll poeple
            </div>

        </div>
    </div>
</template>
<script>
    export default {
        data() {
            return {
                authorized: false,
            }
        },
        mounted() {

            console.log("App-Root mounted!");        },
    }
</script>
<style>
    #app {
        background-color: gray;
    }
</style>
