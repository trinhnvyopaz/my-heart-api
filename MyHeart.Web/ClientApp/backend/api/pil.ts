import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import AtcDto from "../../models/professionInfo/AtcDto";
import PagedResourceRequest from "../../models/PagedResourceRequest";


export default {

    async getPillBySukl(sukl: number): Promise<BaseResponse> {
        return await Api.get(`/Pil/${sukl}`);
    },

    async getFileBySukl(sukl: number, fileName: string) {
        await Api.download(`/Pil/file/${sukl}`, fileName);
    }
    
};
