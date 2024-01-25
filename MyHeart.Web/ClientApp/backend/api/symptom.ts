import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import SymptomDto from "../../models/professionInfo/SymptomDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";

export default {

    async getSymptoms(): Promise<BaseResponse> {
        return await Api.get(`/Symptom`);
    },

    async getSymptomDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/Symptom/${id}`);
    },

    async updateSymptom(symptom: SymptomDto): Promise<BaseResponse> {
        return await Api.patch(`/Symptom`, symptom);
    },

    async addSymptom(symptom: SymptomDto): Promise<BaseResponse> {
        return await Api.post(`/Symptom`, symptom);
    },

    async deleteSymptom(id: number): Promise<BaseResponse> {
        return await Api.delete(`/Symptom/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Symptom/getDataTable', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/Symptom/getByIds', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/Symptom/getPagedResource', request);
    }
};
