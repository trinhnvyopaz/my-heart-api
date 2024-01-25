<template>
    <b-container>

        <div class="content-container login col-lg-4 col-md-12 col-sm-12">
            <b-card header-tag="header">

                <h1 slot="header">{{$locale({i: 'placeholders.signIn'})}}</h1>

                <div>
                    <label class="label text-danger" v-show="msg">
                        {{msg}}
                    </label>

                    <div class="loader" v-show="loading"></div>

                    <div class="form-group">
                        <label for="inputUserName" v-localize="{i: 'placeholders.username'}"></label>
                        <b-form-input type="text" id="inputUserName" v-model="userName" :placeholder="$locale({ i: 'placeholders.username' })" v-on:keyup.enter="Login()" autofocus maxlength="100"></b-form-input>
                    </div>
                    <div class="form-group">
                        <label for="inputPassword" v-localize="{i: 'placeholders.password'}"></label>
                        <b-form-input type="password" id="inputPassword" v-model="password" :placeholder="$locale({i: 'placeholders.password'})" v-on:keyup.enter="Login()" maxlength="100"></b-form-input>
                        <!--<router-link to="/reset-password" style="padding: 8px" v-localize="{i: 'login.forgotPassword'}"></router-link>-->
                    </div>
                    <b-button variant="success lg" @click="Login()" v-localize="{i: 'placeholders.signIn'}"></b-button>

                    <!--<div>
                        <label v-localize="{i: 'login.dontHaveAccount'}"></label>
                        <label><router-link to="/register" v-localize="{i: 'login.register'}">Register</router-link></label>
                    </div>-->
                </div>
            </b-card>
        </div>

    </b-container>
</template>


<script>
    import Vue from 'vue'
    import { router } from '../../app'
    import { mapActions, mapState } from 'vuex'

    import authApi from '@backend/api/auth'
    import authStore from '@backend/store/authStore'

    import userStore from '@backend/store/userStore'
    import { EventBus } from "@utils/event-bus.js";

    export default {
        beforeRouteEnter(to, from, next) {
            if (authStore.isLoggedIn()) {
                next("/MainPage");
            } else {
                next();
            }
        },
        data() {
            return {
                userName: '',
                password: '',
                loading: false,
                token: null,

                msg: null,

                facebookAccessToken: null,
            }
        },
        methods: {
            //BASIC AUTHORIZATION
            Login: function () {
                this.msg = null;
                if (this.userName.length == 0 || this.password.length == 0) {
                    this.msg = this.$locale({ i: 'errors.invalidCredentials' });
                    return;
                }
                else {
                    this.loading = true;
                    var vm = this;
                    authApi.login(this.userName, this.password).then(result => {
                        if (result.status == 200) {
                            console.log("BASIC AUTHORIZATION", result);
                            vm.token = result.data;
                            authStore.setToken(vm.token);
                            vm.Authorize();
                        }
                        else {
                            //vm.msg = result.status;
                            console.log("RESULT IS NOT OK", result);
                            if (result.response.status === 401) {
                                this.msg = this.$locale({ i: 'errors.invalidCredentials' });
                                console.log("Test");
                            }
                        }

                    }).catch(function (error) {
                        console.log('LOGIN ERROR', error);
                        vm.msg = "Exception occured, check console log for more info";
                        if (error.response.status === 401) {
                            vm.msg = vm.$locale({
                                i: 'errors.invalidCredentials'
                            });
                        }
                        if (error.response.status == 400) {
                            console.log('LOGIN ERROR RESPONSE', error.response);
                            vm.msg = vm.$locale({ i: 'errors.accountBlocked' }).replace('TIME_PLACEHOLDER', error.response.data);
                        }
                        vm.loading = false;
                    });
                    vm.loading = false;
                }
            },

            Authorize: function () {
                var vm = this;
                authApi.authorize().then(user => {
                    if (user.data.culture == undefined)
                        userStore.setCulture("cs-CZ");
                    else
                        userStore.setCulture(user.data.culture);

                    vm.$locale({ l: user.data.culture });
                    userStore.setUser(user);

                    EventBus.$emit("user-loged-in");
                });
                this.$router.push({ path: '/mainPage' });
            },
        },

    }
</script>


<style scoped>

    .login {
        margin: auto;
    }

    .login-or {
        position: relative;
        font-size: 18px;
        color: #aaa;
        margin-top: 10px;
        margin-bottom: 10px;
        padding-top: 10px;
        padding-bottom: 10px;
    }

    .span-or {
        display: block;
        position: absolute;
        left: 50%;
        top: -2px;
        margin-left: -25px;
        background-color: #fff;
        width: 50px;
        text-align: center;
    }

    .hr-or {
        background-color: #cdcdcd;
        height: 1px;
        margin-top: 0px !important;
        margin-bottom: 0px !important;
    }

    /* Shared */
    .loginBtn {
        box-sizing: border-box;
        position: relative;
        /* width: 13em;  - apply for fixed size */
        margin: 0.2em;
        padding: 0 15px 0 46px;
        border: none;
        text-align: left;
        line-height: 34px;
        white-space: nowrap;
        border-radius: 0.2em;
        font-size: 16px;
        color: #FFF;
    }

        .loginBtn:before {
            content: "";
            box-sizing: border-box;
            position: absolute;
            top: 0;
            left: 0;
            width: 34px;
            height: 100%;
        }

        .loginBtn:focus {
            outline: none;
        }

        .loginBtn:active {
            box-shadow: inset 0 0 0 32px rgba(0,0,0,0.1);
        }

    @media (max-width: 767px) {
        .loginBtn {
            width: 256px;
        }
    }

    /* Facebook */
    .loginBtn--facebook {
        background-color: #4C69BA;
        background-image: linear-gradient(#4C69BA, #3B55A0);
        /*font-family: "Helvetica neue", Helvetica Neue, Helvetica, Arial, sans-serif;*/
        text-shadow: 0 -1px 0 #354C8C;
    }

        .loginBtn--facebook:before {
            border-right: #364e92 1px solid;
            background: url('https://s3-us-west-2.amazonaws.com/s.cdpn.io/14082/icon_facebook.png') 6px 6px no-repeat;
        }

        .loginBtn--facebook:hover,
        .loginBtn--facebook:focus {
            background-color: #5B7BD5;
            background-image: linear-gradient(#5B7BD5, #4864B1);
        }


    /* Google */
    .loginBtn--google {
        /*font-family: "Roboto", Roboto, arial, sans-serif;*/
        background: #DD4B39;
    }

        .loginBtn--google:before {
            border-right: #BB3F30 1px solid;
            background: url('https://s3-us-west-2.amazonaws.com/s.cdpn.io/14082/icon_google.png') 6px 6px no-repeat;
        }

        .loginBtn--google:hover,
        .loginBtn--google:focus {
            background: #E74B37;
        }
</style>
