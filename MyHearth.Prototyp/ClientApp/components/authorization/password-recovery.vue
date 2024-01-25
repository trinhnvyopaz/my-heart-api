<template>
    <div class="layer">

        <h1 v-localize="{i: 'passwordRecovery.title'}"></h1>

        <div class="content-container">

            <div class="form-group" v-show="!completed">
                <label v-localize="{i : 'passwordRecovery.newPassword'}" />
                <input id="inputPassword" type="password" :placeholder="$locale({i: 'placeholders.passwordRequirements'})" class="form-control" v-model="password" maxlength="100"/>
                <label class="label label-warning">{{validationPassword}}</label>
            </div>

            <div class="form-group" v-show="!completed">
                <label v-localize="{i : 'passwordRecovery.confirmPassword'}" />
                <input id="inputConfirmPassword" type="password" :placeholder="$locale({i: 'placeholders.confirmPassword'})" class="form-control" v-model="confirmPassword" maxlength="100"/>
                <label class="label label-warning">{{validationConfirmPassword}}</label>
            </div>

            <div v-show="!completed">
                <button class="btn btn-lg btn-red" v-on:click="SetNewPassword" v-localize="{i: 'passwordRecovery.setNewPassword'}" />
            </div>

            <div style="padding: 8px" v-show="completed">
                <label v-localize="{i: 'passwordRecovery.success'}" />
                <br />
                <label><router-link to="login" v-localize="{i: 'passwordRecovery.signIn'}" /></label>
            </div>
        </div>
    </div>
</template>
<script>

    //import { resetPasswordUrl } from '../../utils/consts'
    import authApi from '@backend/api/auth'

    export default {

        data()
        {
            return {
                password: '',
                confirmPassword: '',
                confirmString: null,

                validationPassword: null,
                validationConfirmPassword: null,

                msg: null,

                completed: false,

            }
        },
        methods: {

            SetNewPassword: function ()
            {
                if (!this.Validate())
                {
                    return;
                }

                var vm = this;
                authApi.recoverPassword().then(result =>
                {                   
                    if (result.status == 202)
                    {
                        this.completed = true;
                    }
                });
            },

            Validate: function ()
            {

                this.validationPassword = null;
                this.validationConfirmPassword = null;

                var isValid = true

                //password
                var passwordRegex = /^.{5,}$/;
                if (!passwordRegex.test(this.password))
                {
                    this.validationPassword = this.$locale({
                        i: 'errors.invalidPassword'
                    });
                    isValid = false;
                }

                //confirm passwordS
                if (this.confirmPassword != this.password)
                {
                    this.validationConfirmPassword = this.$locale({ i: 'errors.invalidConfirmPassword' });
                    isValid = false;
                }

                return isValid;
            },

        },

        created()
        {
            this.confirmString = this.$route.query.confirmString;
            var culture = this.$route.query.culture;
            this.$locale({ l: culture });
        }
    }
</script>
<style scoped>
</style>
