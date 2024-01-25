import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import AtcDto from "../../models/professionInfo/AtcDto";
import PagedResourceRequest from "../../models/PagedResourceRequest";


export default {

    async getAtcs(): Promise<BaseResponse> {
        return await Api.get(`/Atc`);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/Atc/getByIds', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/Atc/getPagedResource', request);
    }
    
};
