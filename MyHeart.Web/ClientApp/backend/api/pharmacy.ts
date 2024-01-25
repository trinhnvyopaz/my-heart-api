import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import PharmacyDto from "../../models/professionInfo/PharmacyDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";


export default {

    async getPharmacy(): Promise<BaseResponse> {
        return await Api.get(`/Pharmacy`);
    },

    async getPharmacyDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/Pharmacy/${id}`);
    },

    async createPharmacy(Pharmacy: PharmacyDto): Promise<BaseResponse> {
        return await Api.post(`/Pharmacy`, Pharmacy);
    },

    async updatePharmacy(Pharmacy: PharmacyDto): Promise<BaseResponse> {
        return await Api.put(`/Pharmacy`, Pharmacy);
    },

    async deletePharmacy(id: number): Promise<BaseResponse> {
        return await Api.delete(`/Pharmacy/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Pharmacy/getDataTable', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/Pharmacy/getByIds', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/Pharmacy/getPagedResource', request);
    }
    
};
