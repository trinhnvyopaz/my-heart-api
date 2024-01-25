import Api from "./api";
import CreateCheckoutSessionDTO from "../../models/CreateCheckoutSessionDTO"
import BaseResponse from "../../models/BaseResponse";
import CancelSubscriptionRequest from "../../models/CancelSubscriptionRequest";

export default {
    async fetchCheckoutSession(dto: CreateCheckoutSessionDTO) : Promise<BaseResponse> {
        return await Api.post("/PaymentGateway/createCheckoutSession", dto);
    },
    async cancelSubscription(dto: CancelSubscriptionRequest) : Promise<BaseResponse> {
        return await Api.put("/PaymentGateway/cancelSubscription", dto);
    }
};