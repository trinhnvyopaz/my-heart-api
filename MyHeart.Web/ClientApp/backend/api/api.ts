import axios, { AxiosError, AxiosResponse } from "axios";
import BaseResponse from "../../models/BaseResponse";
import PatchDto from "../../models/shared/PatchDto";
import router from "../../router";
import AuthStore from "@backend/store/authStore.ts";
import Events from "@models/shared/Events";
import EventBus from "@models/EventBus";
import { jsonDateParser } from "json-date-parser"

export default {
    async post(url: string, data: any): Promise<BaseResponse> {
        try {
            var axiosResponse = await axios.post(url, data);

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                console.error(error);
                return this.handleConnectionError();
            }
        }
    },

    async postString(url: string, data: string): Promise<BaseResponse> {
        try {
            var axiosResponse = await axios.post(url, data, {
                headers: {
                    "Accept": "application/json",
                    "Content-type": "application/json",
                    "Authorization": "Bearer " + sessionStorage.getItem('jwt')
                }
            });

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                console.error(error);
                return this.handleConnectionError();
            }
        }
    },

    async put(url: string, data: any): Promise<BaseResponse> {
        try {
            var axiosResponse = await axios.put(url, data);

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                console.error(error);
                return this.handleConnectionError();
            }
        }
    },
    async get(url: string, data: any = null, reviveDates = false): Promise<BaseResponse> {
        try {
            if (reviveDates) {
                var axiosResponse = await axios.get(url, {
                    params: data,
                    transformResponse: [data => JSON.parse(data, jsonDateParser)]
                });
            }
            else {
                var axiosResponse = await axios.get(url, {
                    params: data
                });
            }

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                return this.handleConnectionError();
            }
        }
    },
    async patch(url: string, data: any = null): Promise<BaseResponse> {
        try {
            var axiosResponse = await axios.patch(url, data);

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                return this.handleConnectionError();
            }
        }
    },

    async delete(url: string): Promise<BaseResponse> {
        try {
            var axiosResponse = await axios.delete(url);

            return this.handleAxiosResponse(axiosResponse);
        } catch (error) {
            if (error.response) {
                return this.handleAxiosError(error);
            } else {
                return this.handleConnectionError();
            }
        }
    },

    async download(url: string, filename) {
        axios.get(url, {
            responseType: 'blob'
        }).then(response => {
            console.log(response)
            if (window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(response, filename);
            }
            else {
                var fileURL = window.URL.createObjectURL(new Blob([response.data]));
                var fileLink = document.createElement('a');

                fileLink.href = fileURL;
                fileLink.setAttribute('download', filename);
                document.body.appendChild(fileLink);

                fileLink.click();
            }

        })
    },

    handleAxiosResponse(axiosResponse: AxiosResponse): BaseResponse {
        var response: BaseResponse = {
            data: axiosResponse.data,
            errors: null,
            message: axiosResponse.statusText,
            status: axiosResponse.status,
            success: true
        };

        return response;
    },
    handleAxiosError(axiosError: AxiosError): BaseResponse {
        if(axiosError.response.status == 401){
            AuthStore.clearToken();
            AuthStore.clearCurrentUser();
            EventBus.$emit(Events.UserLoggedOut);
            router.push("/login");
        }
        

        var response: BaseResponse = {
            success: false,
            message: axiosError.response.data.title,
            data: axiosError.response.data,
            errors: axiosError.response.data.errors,
            status: axiosError.response.status
        };

        return response;
    },
    handleConnectionError(): BaseResponse {
        var response: BaseResponse = {
            success: false,
            message: "Connection error, please try again later",
            data: null,
            errors: null,
            status: null
        };

        return response;
    }
};
