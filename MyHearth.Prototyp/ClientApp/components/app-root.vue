<template>
    <div id="app-root" class="app">
        <nav-menu :auth="authorized" params="route: route"></nav-menu>
        <div class="x">
            <div class="m-md-2" id="content-box">
                <v-app>
                    <router-view></router-view>
                </v-app>
            </div>
        </div>
    </div>
</template>
<script lang="js">

    import Vue from 'vue'
    import { useModule } from "vuex-simple";
    import NavMenu from '@components/nav-menu.vue'
    import authApi from '@backend/api/auth'
    import authStore from '@backend/store/authStore'
    import userStore from '@backend/store/userStore'

    export default {
        data() {
            return {
                authorized: false,
                breadcrumbStore: useModule(this.$store, ["Breadcrumb"])
            }
        },
        created() {
            this.breadcrumbStore.Init();

            if (userStore.getCulture() == null)
                userStore.setCulture('cs-CZ');

            var culture = userStore.getCulture();

            console.log('CULTURE = ', culture);

            this.$locale({ l: culture });

            this.authorized = authStore.isLoggedIn();
        },
        mounted() {
            console.log("App-Root mounted!");
        },
        components: {
            'nav-menu': NavMenu
        }
    }
</script>
<style lang="css">
    .invert-color {
        -webkit-filter: invert(100%);
    }

    .margin-15 {
        margin: 15px;
    }

    .shadow {
        box-shadow: 5px 5px 10px 5px lightgrey !important;
    }

    .manager-table {
        margin-left: 15px;
        margin-right: 15px;
        margin-bottom: 15px;
    }

    .delete-button {
        margin-right: 15px;
    }
</style>
