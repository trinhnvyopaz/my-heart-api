import api from "./api";
import axios, { AxiosError, AxiosResponse } from "axios";
import BaseResponse from "../../models/BaseResponse";
import PatchDto from "../../models/shared/PatchDto";
import router from "../../router";
import AuthStore from "@backend/store/authStore.ts";
import Events from "@models/shared/Events";
import EventBus from "@models/EventBus";
import { jsonDateParser } from "json-date-parser"

export default {
    async getDiseases(params: any) {
        return await api.get(`/Selection/Diseases`, params);
    },

    async getPharmaceuticals(params: any) {
        return await api.get(`/Selection/Pharmaceuticals`, params);
    },

    async getPreventiveMeasures(params: any) {
        return await api.get(`/Selection/PreventiveMeasures`, params);
    },

    async getNonpharmaticTherapies(params: any) {
        return await api.get(`/Selection/NonpharmaticTherapies`, params);
    },
}
