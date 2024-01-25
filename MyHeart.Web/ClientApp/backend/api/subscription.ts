import Api from "./api";
import BaseResponse from "../../models/BaseResponse";

export default {

    async getSubscriptions(): Promise<BaseResponse> {
        return await Api.get(`/Subscription`);
    },
    async getSubscription(id: number): Promise<BaseResponse> {
        return await Api.get(`/Subscription/${id}`);
    },
};
