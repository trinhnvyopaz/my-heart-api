<template>
    <div class="container">
        <div class="content-container">

            <b-card>

                <b-card-header>
                    <h3 v-localize="{i: 'register.signUp'}"></h3>
                </b-card-header>

                <b-card-body>

                    <label class="label label-warning warning" v-show="msg">
                        {{msg}}
                    </label>
                    <div class="loader" v-show="loading"></div>

                    <div class="form-group">
                        <label for="inputUserName" v-localize="{i: 'register.username'}" />
                        <input type="text" class="form-control" id="inputUserName" v-model="userName" :placeholder="$locale({i: 'placeholders.username'})" maxlength="100">
                        <label class="label label-warning warning" v-show="validationUsername">{{validationUsername}}</label>
                    </div>

                    <div class="form-group">
                        <label for="inputEmail" v-localize="{i: 'register.email'}" />
                        <input type="text" class="form-control" id="inputEmail" v-model="email" :placeholder="$locale({i: 'placeholders.email'})" maxlength="100">
                        <label class="label label-warning warning" v-show="validationEmail">{{validationEmail}}</label>
                    </div>

                    <div class="form-group">
                        <label for="inputBirthday" v-localize="{i: 'register.dateOfBirth'}" />
                        <input type="text" onfocus="(this.type='date')" class="form-control" id="inputBirthday" v-model="birthday" :placeholder="$locale({i: 'placeholders.dateOfBirth'})" maxlength="100">
                        <label class="label label-warning warning" v-show="validationBirthday">{{validationBirthday}}</label>
                    </div>

                    <!--<div class="form-group">
                        <label v-localize="{i: 'register.selectLanguage'}" />

                        <select v-model="culture" class="form-control">
                            <option v-for="culture in cultures" v-bind:value="culture.value">
                                {{ culture.text }}
                            </option>
                        </select>
                    </div>-->

                    <div class="form-group">
                        <label for="inputPassword" v-localize="{i: 'register.password'}" />
                        <input type="password" class="form-control" id="inputPassword" v-model="password" :placeholder="$locale({i: 'placeholders.passwordRequirements'})" maxlength="100">
                        <label class="label label-warning warning" v-show="validationPassword">{{validationPassword}}</label>
                    </div>

                    <div class="form-group">
                        <label for="inputConfirmPassword" v-localize="{i: 'register.confirmPassword'}" />
                        <input type="password" class="form-control" id="inputConfirmPassword" v-model="confirmPassword" :placeholder="$locale({i: 'placeholders.confirmPassword'})" maxlength="100">
                        <label class="label label-warning warning" v-show="validationConfirmPassword">{{validationConfirmPassword}}</label>
                    </div>

                    <div class="form-group">
                        <input type="checkbox" v-model="termsAgreement" />
                        <span>
                            <label v-localize="{i: 'register.iAgreeWith'}" />
                            <label><router-link to="/terms" v-localize="{i: 'register.termsAndConditions'}" target="_blank" /></label>
                            <label v-localize="{i: 'register.forRegistration'}" />
                            <br />
                            <label class="label label-warning warning" v-show="validationTerms">{{validationTerms}}</label>
                        </span>
                    </div>

                    <!--<div class="form-group">
                        <input type="checkbox" v-model="marketingAgreement" />
                        <label v-localize="{i: 'register.marketingAgreement'}" />
                    </div>-->
                </b-card-body>

                <b-card-footer>
                    <div>
                        <button class="btn btn-lg btn-success" v-localize="{i: 'register.register'}" @click="Register()" />
                    </div>

                    <div>
                        <label style="padding: 8px" v-localize="{i: 'register.haveAccount'}" />
                        <label><router-link to="/login" v-localize="{i: 'register.signIn'}"></router-link></label>
                    </div>
                </b-card-footer>

            </b-card>

        </div>
    </div>
</template>
<script>

    import authApi from '@backend/api/auth'
    import userApi from '@backend/api/user'
    import authStore from '@backend/store/authStore'
    //import api from '@backend/api/api'


    export default {
        data()
        {
            return {
                userName: '',
                validationUsername: '',

                email: '',
                validationEmail: '',

                birthday: null,
                validationBirthday: '',

                culture: this.$locale(),

                password: '',
                validationPassword: '',

                confirmPassword: '',
                validationConfirmPassword: '',

                termsAgreement: false,
                validationTerms: '',

                marketingAgreement: false,

                cultures: [
                    { text: 'Deutsch', value: 'de-DE' },
                    { text: 'English', value: 'en-GB' },
                    { text: 'čeština', value: 'cs-CZ' }
                ],

                msg: '',

                loading: false,
            }
        },

        methods: {

            Validate: function ()
            {
                var isValid = true;

                this.validationUsername = null;
                this.validationEmail = null;
                this.validationBirthday = null;
                this.validationPassword = null;
                this.validationConfirmPassword = null;
                this.validationTerms = null;

                //username
                var usernameRegex = /^([a-zA-Z0-9]){5,15}$/;
                if (!usernameRegex.test(this.userName))
                {
                    //TODO
                    this.validationUsername = this.$locale({
                        i: 'errors.invalidUsername'
                    });
                    isValid = false;
                }

                //email
                var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w+)+$/;
                if (!emailRegex.test(this.email))
                {
                    this.validationEmail = this.$locale({
                        i: 'errors.invalidEmail'
                    });
                    isValid = false;
                }

                //birthday
                if (this.birthday == null || this.birthday.length == 0)
                {
                    console.log('BDAY NULL OR LENGTH 0')
                    this.validationBirthday = this.$locale({
                        i: 'errors.invalidBirthday'
                    });
                    isValid = false;
                }
                else
                {
                    var today = new Date();
                    var date = Date.parse(this.birthday);
                    var age = (new Date(today - date).getUTCFullYear() - 1970);
                    console.log("AGE", age);
                    if (age < 12 || age > 99)
                    {
                        this.validationBirthday = this.$locale({
                            i: 'errors.invalidBirthday'
                        });
                        isValid = false;
                    }
                }

                //password
                var passwordRegex = /^.{8,}$/;
                if (!passwordRegex.test(this.password))
                {
                    this.validationPassword = this.$locale({
                        i: 'errors.invalidPassword'
                    });
                    isValid = false;
                }

                //confirm password
                if (this.confirmPassword != this.password)
                {
                    this.validationConfirmPassword = this.$locale({
                        i: 'errors.invalidConfirmPassword'
                    });
                    isValid = false;
                }

                //terms
                if (!this.termsAgreement)
                {
                    this.validationTerms = this.$locale({
                        i: 'errors.invalidTerms'
                    });
                    isValid = false;
                }

                return isValid;
            },

            Register: function ()
            {
                this.loading = true;
                var vm = this;

                if (!this.Validate())
                {
                    vm.loading = false;
                    return;
                }

                let userModel = {
                    firstName: this.firstName,
                    lastName: this.lastName,
                    userName: this.userName,
                    birthday: this.birthday,
                    phone: this.phone,
                    email: this.email,
                    password: this.password,
                    termsAgreement: this.termsAgreement,
                    marketingAgreement: this.marketingAgreement,
                    culture: 'cs-CZ'
                };

                userApi.register(userModel).then(result =>
                {
                    vm.loading = false;
                    console.log('registration result', result);
                    if (result.status === 201)
                    {
                        var status = result.data.modelStatus;
                        if (status == 0)
                        {
                            this.$router.push('/registration-complete');
                        }
                        else
                        {
                            console.log('REGISTARTION STATUS', status)
                            if (status % 2 == 1)
                            {
                                vm.validationUsername = this.$locale({
                                    i: 'errors.usernameInUse'
                                });
                            }
                            if (status >= 2)
                            {
                                vm.validationEmail = this.$locale({
                                    i: 'errors.emailInUse'
                                });
                            }
                        }
                    }
                }).catch(function (error)
                {
                    this.msg = this$locale({i: 'errors.serviceUnavailable'}) + "Error: " + error;

                    console.log(error);
                })
                vm.loading = false;
            }
        }
    }
</script>
<style scoped>

    .login-container {
        background-color: #ffffff;
        width: 50%;
        padding: 32px;
        border-radius: 8px;
        text-align: center;
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

    h3 {
        text-align: center;
        line-height: 300%;
    }

    .warning {
        color: red !important;
    }
</style>
