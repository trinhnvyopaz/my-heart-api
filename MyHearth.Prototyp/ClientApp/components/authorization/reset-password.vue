<template>
    <div class="layer">
        <div class="content-container">

            <h3 v-localize="{i: 'resetPassword.resetPassword'}" />
            <label class="label label-warning warning" v-show="msg">
                {{msg}}
            </label>
            <div class="loader" v-show="loading"></div>

            <div class="form-group">
                <label for="inputEmail" v-localize="{i: 'resetPassword.email'}" />
                <input id="inputEmail" class="form-control" type="email" v-model="email" :placeholder="$locale({i: 'placeholders.email'})" maxlength="100" />
                <label class="label label-warning warning">{{validationEmail}}</label>
            </div>

            <div>
                <b-button variant="primary" value="Reset password" @click="ResetPassword()" v-localize="{i: 'resetPassword.resetPassword'}" />
            </div>

            <div style="padding:8px">
                <label v-localize="{i: 'resetPassword.dontReset'}" />
                <label><router-link to="/login" v-localize="{i: 'resetPassword.signIn'}" /></label>
            </div>

        </div>
    </div>
</template>
<script>

    import userApi from '@backend/api/user'

    export default {
        data() {
            return {
                email: null,
                validationEmail: null,
                msg: null,
                loading: false,
            }
        },
        methods: {

            Validate: function () {
                this.validationEmail = null;
                var isValid = true;

                // TODO validation of email should be a util function
                //email
                var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w+)+$/;
                if (!emailRegex.test(this.email)) {
                    this.validationEmail = this.$locale({
                        i: 'errors.invalidEmail'
                    });
                    isValid = false;
                }

                return isValid;
            },

            ResetPassword: function () {
                this.loading = true;
                console.log('Password reset');
                console.log(this.email);

                var vm = this;
                if (this.Validate()) {

                    userApi.resetPassword(this.email).then(result => {
                        vm.loading = false;
                        console.log(result)
                        if (result.status == 200) {
                            vm.$router.push('/reset-password-complete');
                        }
                    }).catch(function (error) {
                        vm.loading = false;
                        console.log(error);
                        vm.msg = error;
                    })
                }

            }
        }
    }
</script>
<style scoped>

    .form-group {
        text-align: left
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

    .warning {
        color: red !important;
    }
</style>