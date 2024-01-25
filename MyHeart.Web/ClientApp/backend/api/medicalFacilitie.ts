import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import MedicalFacilitiesDto from "../../models/professionInfo/TherapyNewsDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";

export default {

    async getMedicalFacilities(): Promise<BaseResponse> {
        return await Api.get(`/MedicalFacilities`);
    },
    
    async getMedicalFacilitiesDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/MedicalFacilities/${id}`);
    },

    async updateMedicalFacilities(medicalFacilities: MedicalFacilitiesDto): Promise<BaseResponse> {
        return await Api.put(`/MedicalFacilities`, medicalFacilities);
    },

    async createMedicalFacilities(medicalFacilities: MedicalFacilitiesDto): Promise<BaseResponse> {
        return await Api.post(`/MedicalFacilities`, medicalFacilities);
    },

    async deleteMedicalFacilities(id: number): Promise<BaseResponse> {
        return await Api.delete(`/MedicalFacilities/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/MedicalFacilities/getDataTable', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/MedicalFacilities/getPagedResource', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/MedicalFacilities/getByIds', request);
    }
};
