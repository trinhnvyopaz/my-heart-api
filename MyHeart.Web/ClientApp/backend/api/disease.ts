import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import DiseaseDTO from "../../models/professionInfo/DiseaseDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";

export default {
    async getDisease(): Promise<BaseResponse> {
        return await Api.get(`/Disease`);
    },

    async getDiseaseDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/Disease/${id}`);
    },

    async createDisease(disease: DiseaseDTO): Promise<BaseResponse> {
        return await Api.post(`/Disease`, disease);
    },

    async updateDisease(disease: DiseaseDTO): Promise<BaseResponse> {
        return await Api.put(`/Disease`, disease);
    },

    async deleteDisease(diseaseId: number): Promise<BaseResponse> {
        return await Api.delete(`/Disease/${diseaseId}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Disease/getDataTable', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/Disease/getByIds', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/Disease/getPagedResource', request);
    }
};
