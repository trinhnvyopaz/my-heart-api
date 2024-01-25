<template>
    <div class="layer">
        <div class="content-container">
            <h3 v-localize="{i: 'registrationComplete.registrationComplete'}" />
            <label v-localize="{i: 'registrationComplete.checkEmail'}" />
            <label><router-link to="/login"  v-localize="{i: 'registrationComplete.signIn'}" /></label>
        </div>
    </div>
</template>
<script>

    export default {
        data()
        {
            return {
            }
        },
        methods: {
        }
    }
</script>
<style scoped>
</style>
