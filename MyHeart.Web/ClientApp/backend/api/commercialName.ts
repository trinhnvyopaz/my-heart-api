import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import CommercialNameDto from "../../models/professionInfo/CommercialNameDto";

export default {
    async getCommercialNames(): Promise<BaseResponse> {
        return await Api.get(`/CommercialName`);
    },

    async getCommercialNameDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/CommercialName/${id}`);
    },

    async updateCommercialName(CommercialName: CommercialNameDto): Promise<BaseResponse> {
        return await Api.put(`/CommercialName`, CommercialName);
    },

    async createCommercialName(CommercialName: CommercialNameDto): Promise<BaseResponse> {
        return await Api.post(`/CommercialName`, CommercialName);
    },

    async deleteCommercialName(id: number): Promise<BaseResponse> {
        return await Api.delete(`/CommercialName/${id}`);
    }
 
};
