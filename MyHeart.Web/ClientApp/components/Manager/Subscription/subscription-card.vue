<template>
    <div class="d-flex flex-column main-container">
        <div>
            <label style="color: #97989D; font-size: 20px;">{{subscription.name}}</label>
            <div class="d-flex align-center">
                <label style="font-family: nunito; font-weight: bold;font-size: 60px;">{{ pricing == 0 ? "Zdarma" : pricing }}</label>
                <span class="ml-4" v-if="pricing != 0" style="font-family: nunito;font-size: 50px;">kč</span>
                <label class="ml-4 mt-2" v-if="period != null" style="font-family: nunito;font-size: 12px;color: #7C7C84;">{{ "/ " + period }}</label>
            </div>
        </div>

        <v-btn @click="pickSubscription" class="empty">
            {{ isSelected? "Vybráno" : "Vybrat tarif " + subscription.name }}
         </v-btn>
        
        <label class="period-end-text" v-if="user.subscriptionCancelAtPeriodEnd && isSelected">Ukončeno k {{ formating.formatDate(user.subscriptionValidTo) }}</label>

        <div v-for="item in itemList">    
            <div class="d-flex justify-start align-start">
                <SuccessSVG class="mr-2 flex-grow-0 flex-shrink-0"/>
                <label class="flex-grow-1 flex-shrink-1">{{item}}</label>
            </div>       
        </div>

        <simple-dialog v-model="confirmDialogShown"
                   title=""
                   positive="Zrušit předplatné"
                   @positiveClick="cancelSubscirption"
                   width="600"
                   :autoclose="false">
        <v-card-text>
            <label class="confirm-dialog-text">Opravdu chcete zrušit předplatné? Předplatné bude aktivní do {{ formating.formatDate(user.subscriptionValidTo) }}</label>
        </v-card-text>
    </simple-dialog>

    <v-snackbar right top v-model="snackbarShown" :color="snackBarColor">{{snackBarText}}</v-snackbar>

    </div>
</template>
<script lang="ts">
    import Vue from "vue";
    import { Component, Prop } from "vue-property-decorator";
    import SuccessSVG from "@resources/success.svg"
    import SubscriptionDto from "@models/subscription/subscriptionDto"
    import UserDetailDto from "@models/user/UserDetailDto";
    import PaymentApi from "@backend/api/payments"
    import CancelSubscriptionRequest from "@models/CancelSubscriptionRequest";
    import SimpleDialog from "@components/Shared/simpleDialog.vue";
    import Formatting from '@utils/formatting';

    @Component({
        name: "SubscriptionCard",
        components: {
            SuccessSVG, SimpleDialog
        },
    })
    export default class SubscriptionCard extends Vue {
        @Prop()
        itemList: string[];


        @Prop()
        subscription: SubscriptionDto

        @Prop()
        pricing: number

        @Prop()
        period: string

        @Prop()
        user: UserDetailDto

        @Prop()
        isSelected: boolean

        formating = Formatting

        userId: number
        confirmDialogShown: boolean = false
        snackbarShown: boolean = false
        snackBarColor: string = ""
        snackBarText: string = ""

        mounted(){
           
        }

        pickSubscription(){
            if(!this.isSelected && this.pricing != 0){
                this.$router.push("/user/detail/" + this.userId + "/payment/" + this.subscription.id)
            }else if(!this.isSelected && this.pricing == 0){                             
                this.confirmDialogShown = true;            
            }
            
        }

        async cancelSubscirption(){
            var request : CancelSubscriptionRequest = {
                stripeUserId: this.user.stripeId
            }

            var response = await PaymentApi.cancelSubscription(request)
            if(response.success){
                this.snackBarText = "Tarif zrušen"
                this.snackBarColor = "success"
                this.snackbarShown = true
                this.confirmDialogShown = false;

                this.$emit('subscription-cancelled')

            }else{
                this.snackBarText = "Nepodařilo se zrušit tarif"
                this.snackBarColor = "error"
                this.snackbarShown = true
            }
        }
    }
</script>
<style scoped>
.main-container{
    background-color: white!important;
    padding: 16px;
    border-radius: 10px;
    max-width: 330px;
}
.confirm-dialog-text{
    color: #47474A;
    font-size: 24px;
}
.period-end-text{
    color:  #2474e3;
    font-size: 24px;   
}
</style>