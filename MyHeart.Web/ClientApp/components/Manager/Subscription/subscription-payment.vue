<template>
    <div class="mt-6 d-flex flex-column main-container justify-space-between align-center">
        <label class="page-title">Tarif Kompletní péče</label>
        <div class="d-flex flex-column card">
            <label class="big-title">Potvrďte platbu</label>
            <div class="d-flex justify-space-between">
                <label class="payment-form-title">Platba: </label>
                <label class="payment-form-value">{{subscription.price}} Kč/měsíc</label>
            </div>
            <div class="d-flex justify-space-between">
                <label class="payment-form-title">Platnost do: </label>
                <label class="payment-form-value">{{ subscriptionPeriod }}</label>
            </div>
            <div class="d-flex justify-space-between">
                <label class="payment-form-title">Platební metoda: </label>
                <div class="d-flex align-center flex-md-row">
                    <VisaSVG/>
                    <MastercardSVG class="ml-4"/>
                    <label class="ml-4 payment-form-value">Platba kartou</label>
                </div>              
            </div>
            <label class="mt-4 payment-form-note">Pokud tarif zrušíte, pokračuje nárok na jeho užívání do doby další plánované platby”.</label>
            <label class="mt-4 payment-form-note">Po potvrzení budete přesměrování na platební bránu</label>
            <v-btn @click="confirmPayment">Potvrdit</v-btn>
        </div>     
        <div/>
    </div>
    
</template>
<script lang="ts">
    import Vue from "vue";
    import { Component, Prop } from "vue-property-decorator";
    import MastercardSVG from "@resources/mastercard-color.svg"
    import VisaSVG from "@resources/visa-color.svg"
    import PaymnetApi from "@backend/api/payments"
    import SubscriptionApi from "@backend/api/subscription"
    import CreateCheckoutSessionDTO from "../../../models/CreateCheckoutSessionDTO";
    import {loadStripe} from '@stripe/stripe-js/pure';
    import config from '../../../../appsettings.json';
    import SubscriptionDto from "@models/subscription/subscriptionDto";
    import Formatting from '@utils/formatting';
    import moment from 'moment';

    @Component({
        name: "SubscriptionPayment",
        components: {
            MastercardSVG, VisaSVG
        },
    })
    export default class SubscriptionPayment extends Vue {
        userId: number = 0
        subscriptionId: number = 0
        subscription: SubscriptionDto = new SubscriptionDto
        formatting: Formatting = Formatting

        mounted(){

            if (this.$route.params.subscriptionId) {
                this.subscriptionId = Number(this.$route.params.subscriptionId);             
                this.loadData();                   
            }
        }

        get subscriptionPeriod(){
            var currentDate = moment();
            var dateWithOneMonthAdded = currentDate.add(1, 'months');
            return Formatting.formatDate(dateWithOneMonthAdded)
        }

        async loadData(){
            var response = await SubscriptionApi.getSubscription(this.subscriptionId)
            if(response.success){
                this.subscription = response.data
            }
        }

        async confirmPayment(){
            var createSessionDTO = new CreateCheckoutSessionDTO
            createSessionDTO.productStripeId = this.subscription.productStripeId
            
            var sessionResponse = await PaymnetApi.fetchCheckoutSession(createSessionDTO)
            if(sessionResponse.success){
                console.log(sessionResponse)
                var stripe = await loadStripe(config.StripePublicKey)
                stripe?.redirectToCheckout({
                    sessionId: sessionResponse.data.id
                }).then(function(result) {
                    if (result.error) {
                        console.log(result.error);
                    }
                });

            }   
            // this.$emit("payment-processed");
        }
    }
</script>
<style scoped>
.card{
    background-color: white;
    border-radius: 10px;
    padding: 16px;
    gap: 10px;
    max-width: 684px;
    width: 85%;
}
.payment-form-title{
    font-size: 20px;
    font-family: nunito;
    color: #97989D;
}
.payment-form-value{
    font-size: 20px;
    font-family: nunito;
    color: #2D2957;
    font-weight: 600;
}
.payment-form-note{
    font-size: 16px;
    font-family: nunito;
    color: #97989D;
    font-weight: 300;
}
.main-container{
    height: 100%;
}
</style>