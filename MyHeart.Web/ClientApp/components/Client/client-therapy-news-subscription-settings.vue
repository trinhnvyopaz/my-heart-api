<template>
    <v-card class="rounded-xl" color="#F2F2F2">
        <v-card-title>Nastavení odběru</v-card-title>

        <v-card-text>

            <div v-if="preferences">

                <v-radio-group v-model="preferences.subscriptionPreferences">
                    <v-radio v-for="(subOption, index) in subOptions" :key="index" :label="subOption.text" :value="subOption.value" />
                </v-radio-group>

                <v-checkbox v-model="preferences.therapyNewsEmailNotification" label="Upozornit emailem" />

            </div>

        </v-card-text>

        <v-card-actions>
            <v-btn justify-end @click="savePreferences" dark>Uložit</v-btn>
        </v-card-actions>

    </v-card>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import therapyApi from "@backend/api/therapyNews"

    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../../models/user/UserDto";
    @Component({
        name: "ClientTherapyNewsSubscriptionSettings"
    })
    export default class ClientTherapyNewsSubscriptionSettings extends Vue {
        loading: boolean = false;

        preferences: any = null;

        subOptions: any[] = [
            { text: 'Všechny novinky', value: 0 },
            { text: 'Pouze moje onemocnění', value: 1 },
            { text: 'Nepřeji si dostávat žádné informace', value: 2 },
        ];

        notifyByEmail: boolean = false;
        subPreferences: number = 0;

        mounted() {
            this.loadPreferences();
        }

        async loadPreferences() {
            if (AuthStore.isImpersonating()) {
                const impersonated: UserDto = AuthStore.getImpersonatedUser() as UserDto;
                var result = await therapyApi.getSubscriptionPreferencesForPerson(impersonated.id);
            } else {
                var result = await therapyApi.getSubscriptionPreferences();
            }

            this.preferences = result.data;
        }

        async savePreferences() {
            var result = await therapyApi.updateSubscriptionPreferences(this.preferences);
            this.hideDialog();
        }

        hideDialog() {
            this.$emit('hide');
        }
    }
</script>


<style scoped>
</style>
