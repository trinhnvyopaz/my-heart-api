import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import NonpharmacyDto from "../../models/professionInfo/NonpharmacyDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";

export default {
   
    async getNonpharmacy(): Promise<BaseResponse> {
        return await Api.get(`/Nonpharmacy`);
    },
    
    async getNonpharmacyDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/Nonpharmacy/${id}`);
    },

    async createNonpharmacy(Nonpharmacy: NonpharmacyDto): Promise<BaseResponse> {
        return await Api.post(`/Nonpharmacy`, Nonpharmacy);
    },

    async updateNonpharmacy(Nonpharmacy: NonpharmacyDto): Promise<BaseResponse> {
        return await Api.put(`/Nonpharmacy`, Nonpharmacy);
    },

    async deleteNonpharmacy(id: number): Promise<BaseResponse> {
        return await Api.delete(`/Nonpharmacy/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Nonpharmacy/getDataTable', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/Nonpharmacy/getPagedResource', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/Nonpharmacy/getByIds', request);
    }
};
