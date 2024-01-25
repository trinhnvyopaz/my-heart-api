import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import TherapyNewsDto from "../../models/professionInfo/TherapyNewsDto";
import DataTableRequest from "../../models/DataTableRequest";

export default {
    async getTherapyNews(): Promise<BaseResponse> {
        return await Api.get(`/TherapyNews`);
    },

    async getTherapyNewsDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/TherapyNews/${id}`);
    },

    async updateTherapyNews(therapyNews: TherapyNewsDto): Promise<BaseResponse> {
        return await Api.put(`/TherapyNews`, therapyNews);
    },

    async createTherapyNews(therapyNews: TherapyNewsDto): Promise<BaseResponse> {
        return await Api.post(`/TherapyNews`, therapyNews);
    },

    async deleteTherapyNews(id: number): Promise<BaseResponse> {
        return await Api.delete(`/TherapyNews/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/TherapyNews/getDataTable', request);
    },

    async getSubscriptionPreferences(): Promise<BaseResponse> {
        return await Api.get("/TherapyNews/getSubscriptionPreferences");
    },

    async getSubscriptionPreferencesForPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/TherapyNews/getSubscriptionPreferences/${id}`);
    },

    async updateSubscriptionPreferences(TherapyNewsSubscriptionSettingsDto: any): Promise<BaseResponse> {
        return await Api.post("TherapyNews/updateSubscriptionPreferences", TherapyNewsSubscriptionSettingsDto);
    }
};
