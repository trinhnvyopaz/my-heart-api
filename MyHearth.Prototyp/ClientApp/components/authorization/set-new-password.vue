<template>
    <div class="background">
        <div class="layer">

            <h1 v-localize="{i: 'setNewPassword.title'}" />

            <div v-if="!user || loading" style="margin: 10% 0">
                <div class="loader"></div>
            </div>

            <div v-if="user && !loading" class="content-container" v-on:keyup.enter="SetNewPassword()">

                <label class="label label-warning">{{msg}}</label>

                <div class="form-group">
                    <label for="inputCurrentPassword" v-localize="{i: 'setNewPassword.currentPassword'}" />
                    <input type="password" class="form-control" id="inputCurrentPassword" v-model="user.currentPassword" v-on:keyup.enter="SetNewPassword()" :placeholder="$locale({i: 'placeholders.currentPassword'})" maxlength="100">
                    <label class="label label-warning" v-show="validationCurrentPassword">{{validationCurrentPassword}}</label>
                </div>

                <div class="form-group">
                    <label for="inputPassword" v-localize="{i: 'setNewPassword.newPassword'}" />
                    <input type="password" class="form-control" id="inputPassword" v-model="user.password" v-on:keyup.enter="SetNewPassword()" :placeholder="$locale({i: 'placeholders.passwordRequirements'})" maxlength="100">
                    <label class="label label-warning" v-show="validationPassword">{{validationPassword}}</label>
                </div>

                <div class="form-group">
                    <label for="inputConfirmPassword" v-localize="{i: 'setNewPassword.confirmNewPassword'}" />
                    <input type="password" class="form-control" id="inputConfirmPassword" v-model="user.confirmPassword" v-on:keyup.enter="SetNewPassword()" :placeholder="$locale({i: 'placeholders.confirmPassword'})" maxlength="100">
                    <label class="label label-warning" v-show="validationConfirmPassword">{{validationConfirmPassword}}</label>
                </div>

                <button class="btn btn-lg btn-red" v-localize="{i: 'setNewPassword.save'}" v-on:click="SetNewPassword()" />
                <router-link to="/settings" v-localize="{i: 'common.back'}" />

            </div>
        </div>
    </div>
</template>

<script>

    import { decodeFlag } from '../../utils/utils'
    import userApi from '@backend/api/user'
    import authStore from '@backend/store/authStore'

    export default {
        data()
        {
            return {
                loading: false,

                msg: null,

                validationCurrentPassword: null,
                validationPassword: null,
                validationConfirmPassword: null,

                user: null,

            }
        },

        methods: {
            Validate: function ()
            {
                this.validationCurrentPassword = null;
                this.validationPassword = null;
                this.validationConfirmPassword = null;

                var isValid = true;

                //currentPassword
                if (this.user.currentPassword.length == 0)
                {
                    this.validationCurrentPassword = this.$locale({
                        i: 'errors.emptyCurrentPassword'
                    });
                    isValid = false;
                }

                //password
                var passwordRegex = /^.{8,}$/;
                if (!passwordRegex.test(this.user.password))
                {
                    this.validationPassword = this.$locale({
                        i: 'errors.invalidPassword'
                    });
                    isValid = false;
                }

                //confirm password
                if (this.user.confirmPassword != this.user.password)
                {
                    this.validationConfirmPassword = this.$locale({
                        i: 'errors.invalidConfirmPassword'
                    });
                    isValid = false;
                }

                return isValid;

            },

            SetNewPassword: function ()
            {
                this.loading = true;
                var vm = this;

                if (this.Validate())
                {
                    api.put('users', this.user).then(result =>
                    {
                        console.log('SAVE PASSWORD RESULT', result);
                        if (result.status == 202)
                        {

                            var status = result.data.modelStatus;
                            if (status == 0)
                            {
                                vm.msg = vm.$locale({ i: 'setNewPassword.passwordChanged' });
                            }
                            else
                            {
                                var flags = decodeFlag(status, 'userModel');
                                if (flags.indexOf('WrongPassword') > -1)
                                {
                                    vm.validationCurrentPassword = vm.$locale({ i: 'errors.invalidCurrentPassword' });
                                }
                            }
                        }
                    })
                }

                this.loading = false;
            },

        },
        async created()
        {
            var vm = this;
            try
            {
                userApi.getCurrentUser().then(response =>
                {
                    var data = response.data;
                    console.log('DATA', data);
                    vm.userId = data.id;                    
                    var bday = (data.birthday) ? data.birthday.substring(0, 10) : null;
                    vm.user = {
                        userName: data.userName,
                        birthday: bday,
                        culture: data.culture,
                        currentPassword: '',
                        password: '',
                        confirmPassword: '',
                        email: data.email,
                        phone: data.phone,
                        id: data.id,
                        termsAgreement: true,
                        marketingAgreement: data.marketingAgreement
                    };
                    vm.oldUsername = data.userName;
                    console.log('PARSED USER', vm.user);
                }).catch(err =>
                {
                    console.log('Err', err);
                });
            }
            catch (error)
            {
                console.log('Error while retrieving current user -> logout', error);
                authStore.logout();
            }
        }

    }
</script>

<style scoped>
</style>
