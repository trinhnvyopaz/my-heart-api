<template>
    <div class="background">
        <div class="layer">
            <div class="layer-content">

                <h1>{{ $locale({i: 'common.notInDemo'}) }}</h1>

            </div>

            <div class="content-container">
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data()
        {
            return {
                loading: false,
            }
        },
    }
</script>

<style scoped>
</style>
