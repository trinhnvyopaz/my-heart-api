<template>
    <b-navbar class="margin-15 shadow" variant="light" type="light" toggleable="md">
        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
        <b-navbar-brand @click="showMainPage"><font-awesome-icon class="brand-logo" icon="heartbeat" />&nbsp;Moje srdce</b-navbar-brand>
        <b-collapse is-nav id="nav_collapse">
            <b-navbar-nav pills>
                <!--<b-nav-item class="ml-2" @click="showAtlas">Atlas škůdců</b-nav-item> <div class="splitter d-none d-lg-block">|</div>-->
            </b-navbar-nav>
            <b-navbar-nav class="ml-auto">
                <div class="splitter d-none d-lg-block">|</div>
                <b-nav-item-dropdown :text="$locale({i: 'placeholders.user'})" v-if="authorized" right>
                    <b-dropdown-item @click="userProfile">{{ $locale({i: 'placeholders.profile'}) }}</b-dropdown-item>
                    <b-dropdown-item @click="logoutUser">{{ $locale({i: 'placeholders.logOut'}) }}</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item v-if="!authorized" @click="logintUser" right>
                    {{ $locale({i: 'placeholders.signIn'}) }}
                </b-nav-item>
            </b-navbar-nav>
        </b-collapse>
    </b-navbar>

</template>
<script>
    import userApi from '@backend/api/user'
    import authApi from '@backend/api/auth'
    import { EventBus } from "@utils/event-bus.js";

    export default {
        props: ['auth'],
        data() {
            return {
                //routes,
                authorized: false,
                collapsed: true,
                UserName: 'User'
            }
        },
        mounted() {
            this.authorized = this.auth;

            // catches login events
            EventBus.$on("user-loged-in", () => {
                this.authorized = true;
            });

            // catches logout events
            EventBus.$on("user-loged-out", () => {
                this.authorized = false;
            });
        },
        methods: {
            logoutUser: function () {
                try {
                    FB.getLoginStatus(function (response) {
                        if (response.status === 'connected') {
                            FB.logout();
                        }
                    });
                }
                catch (e) {

                }

                EventBus.$emit("user-loged-out");
                authApi.logout();
                this.$router.push('login')
            },
            showMainPage: function () {
                this.$router.push({ path: '/mainPage' });
            },

            userProfile: function () {
                this.$router.push({ path: '/clientProfile' });
            },
            logintUser: function () {
                this.$router.push('login');
            }
        }
    }
</script>
<style>

    .full-height {
        height: 100%;
    }

    .splitter {
        color: darkorange;
        margin-top: 8px;
    }

    .brand-logo {
        color: darkorange;
    }

    .page-title {
        color: whitesmoke;
    }

    .navbar-brand {
        cursor: pointer;
    }
</style>
