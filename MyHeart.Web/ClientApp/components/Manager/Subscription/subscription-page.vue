<template>
    <div>
        <div class="d-flex flex-column flex-md-row main-container mb-2 justify-center align-center align-md-start">
            <v-progress-circular
                v-if="loading"
                indeterminate
                color="primary"
                size="64"
            ></v-progress-circular>
            <div v-else v-for="subscription in subscriptions" :key="subscription.id">
                <subscription-card @subscription-cancelled="onSubscriptionCalncelled" :isSelected="userDetail.subscriptionId == subscription.id" :user="userDetail" :subscription="subscription" :pricing="subscription.price" :itemList="features(subscription.id)"/>
            </div>          
        </div>
        <subscription-result-dialog :success="paymentSuccess" v-model="showPaymentResult" />
    </div>
    
</template>
<script lang="ts">
    import Vue from "vue";
    import { Component, Prop, Watch } from "vue-property-decorator";
    import SubscriptionCard from "@components/Manager/Subscription/subscription-card.vue"
    import SubscriptionFeatures from "@utils/SubscriptionFeatures"
    import SubscriptionResultDialog from "@components/Manager/Subscription/subscription-result-dialog.vue"
    import UserDetailDto from "../../../models/user/UserDetailDto";
    import UserApi from "@backend/api/user"
    import SubscriptionApi from "@backend/api/subscription"
    import SubscriptionDto from "models/subscription/subscriptionDto"

    @Component({
        name: "SubscriptionPage",
        components: {
            SubscriptionCard, SubscriptionResultDialog
        },
    })
    export default class SubscriptionPage extends Vue {

        @Prop()
        userDetail: UserDetailDto

        showPayment: boolean = false
        basicFeatures: string[] = []
        fullFeatures: string[] = []
        userId: number
        subscriptions: SubscriptionDto[] = []        
        showPaymentResult: boolean = false
        paymentSuccess: boolean = false
        loading: boolean = false

        mounted(){
            this.basicFeatures = SubscriptionFeatures.getBasicFeatures();
            this.fullFeatures = SubscriptionFeatures.getFullFeatures();
            this.loadData()
        }

        onSubscriptionCalncelled(){
            this.$emit('subscription-cancelled')
            this.loadData();
        }

        async loadData(){
            this.loading = true;

            var subscriptionResponse = await SubscriptionApi.getSubscriptions()
            if (subscriptionResponse.success) {
                console.log(subscriptionResponse.data);
                this.subscriptions = subscriptionResponse.data
            }

            if(this.$route.query.result){
                this.paymentSuccess = this.$route.query.result == 'success' ? true : false;
                this.showPaymentResult = true;
                const newPath = `/user/detail/${this.userId}`;
                this.$router.replace(newPath);
                this.$router.replace({ query: null });
            }   

            this.loading = false;
        }

        features(id: number){
            return id == 1 ? this.basicFeatures : this.fullFeatures;
        }
    }
</script>
<style scoped>
.main-container{
    gap: 26px!important;
}
</style>