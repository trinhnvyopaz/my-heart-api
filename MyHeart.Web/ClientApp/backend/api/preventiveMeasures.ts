import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import PreventiveMeasuresDto from "../../models/professionInfo/PreventiveMeasuresDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";


export default {

    async getPreventiveMeasures(): Promise<BaseResponse> {
        return await Api.get(`/PreventiveMeasure`);
    },

    async getPreventiveMeasuresDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/PreventiveMeasure/${id}`);
    },

    async updatePreventiveMeasures(PreventiveMeasure: PreventiveMeasuresDto): Promise<BaseResponse> {
        return await Api.put(`/PreventiveMeasure`, PreventiveMeasure);
    },

    async createPreventiveMeasures(PreventiveMeasure: PreventiveMeasuresDto): Promise<BaseResponse> {
        return await Api.post(`/PreventiveMeasure`, PreventiveMeasure);
    },

    async deletePreventiveMeasures(id: number): Promise<BaseResponse> {
        return await Api.delete(`/PreventiveMeasure/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/PreventiveMeasure/getDataTable', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/PreventiveMeasure/getPagedResource', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/PreventiveMeasure/getByIds', request);
    }
};
