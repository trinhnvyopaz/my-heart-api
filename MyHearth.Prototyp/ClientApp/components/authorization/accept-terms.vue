<template>
    <div class="layer">
        <div class="content-container">

            <h3 v-localize="{i: 'acceptTerms.acceptTermsOfService'}" />
            <p v-localize="{i: 'acceptTerms.inputUsername'}" />

            <label class="label label-danger" v-show="msg">
                {{msg}}
            </label>

            <div class="form-group">
                <label for="inputUsername" v-localize="{i: 'acceptTerms.username'}" />
                <input type="text" class="form-control" id="inputUsername" v-model="model.username" :placeholder="$locale({i: 'placeholders.username'})" maxlength="100" />
                <label class="label label-warning">{{validationUsername}}</label>
            </div>
            <div class="form-group">
                <label for="inputLanguage" v-localize="{i: 'acceptTerms.language'}" />
                <select class="form-control" v-model="model.culture">
                    <option v-for="culture in cultures" v-bind:value="culture.value">
                        {{ culture.text }}
                    </option>
                </select>
            </div>

            <div class="form-group">
                <input type="checkbox" id=" acceptTerms" v-model="model.termsAgreement" />
                <label v-localize="{i: 'acceptTerms.iAgree'}" />
                <label><router-link to="/terms" v-localize="{i: 'acceptTerms.termsOfService'}" target="_blank"></router-link></label>
                <br />
                <label class="label label-warning">{{validationTerms}}</label>
            </div>
            <button class="btn btn-lg btn-red" v-on:click="Confirm" v-localize="{i: 'acceptTerms.confirm'}" />
        </div>

    </div>
</template>


<script>
    import Vue from 'vue'
    import axios from 'axios'
    import { router } from '../../app'
    import { mapActions, mapState } from 'vuex'
    //import { externalLogin, currentUser } from '../../utils/consts'

    import api from '@backend/api/api'
    import authApi from '@backend/api/auth'
    import authStore from '@backend/store/authStore'

    //import { setCulture } from '../../utils/utils'
    

    export default {
        data()
        {
            return {
                msg: null,
                validationUsername: null,
                validationTerms: null,

                cultures: [
                    { text: 'Deutch', value: 'de-DE' },
                    { text: 'English', value: 'en-GB' },
                    { text: 'Czech', value: 'cs-CZ' }
                ],
            }
        },

        computed: {
            /* ...mapState({
                model: state => state.externalRegistrationModel
            }) */
        },

        methods: {

            Validate: function ()
            {
                var isValid = true;

                this.validationUsername = null;
                this.validationTerms = null;

                //username
                if (this.model.username == null || this.model.username.length < 4)
                {
                    this.validationUsername = this.$locale({
                        i: 'errors.invalidUsername'
                    });
                    this.isValid = false;
                }

                //terms
                if (!this.model.termsAgreement)
                {
                    this.validationTerms = this.$locale({
                        i: 'errors.invalidTerms'
                    });
                    isValid = false;
                }

                return isValid;
            },

            Confirm: function ()
            {

                if (!this.Validate())
                {
                    return;
                }

                var vm = this;



                //api.post('login/external', this.model)
                authApi.externalLogin(this.mode).then(result =>
                {
                    console.log("CONFIRM REGISTRATION", result.data);
                    authStore.setToken(result.data);

                    authApi.authorize().then(user =>
                    {
                        console.log('LOGIN USER:', user);

                        //TODO Set Culture
                        //setCulture(user.data.culture);


                        vm.$locale({ l: user.data.culture });
                        authStore.setUser(user.data);
                    });


                    vm.$router.push('/my-pumps');
                }).catch(error =>
                {
                    console.log("EXTERNAL REGISTRATION ERROR", error);
                    vm.validationUsername = vm.$locale({
                        i: 'errors.usernameInUse'
                    });
                });
            }
        },

        created()
        {
            if (this.model.culture == null || this.model.culture.length == 0)
            {
                console.log('link culture!');
                this.model.culture = this.$locale();
            }
        }

    }
</script>


<style scoped>
</style>
