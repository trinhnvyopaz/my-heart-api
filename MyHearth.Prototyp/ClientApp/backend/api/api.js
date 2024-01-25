import axios from 'axios'
import authStore from '../store/authStore'

export default {
    get(url)
    {
        console.log('GET');
        return axios.get(url, { headers: authStore.getHeaders() }).then(r => {
            console.log('API GET RESPONSE (' + axios.defaults.baseURL + url + ')', r);
            return r;
        }).catch(e => {
            console.log('API GET ERROR (' + axios.defaults.baseURL + url + ')', e);
            return e;
        });
    },

    getNoCache(url) {
        console.log('GET NOCACHE');
        let h = authStore.getHeaders();
        h["Cache-Control"] = "no-cache";
        return axios.get(url, { headers: h }).then(r => {
            console.log('API GET RESPONSE (' + axios.defaults.baseURL + url + ')', r);
            return r;
        }).catch(e => {
            console.log('API GET ERROR (' + axios.defaults.baseURL + url + ')', e);
            return e;
        });
    },
    post(url, data)
    {
        console.log('POST ', data);
        return axios.post(url, data, { headers: authStore.getHeaders() }).then(r =>
        {
            console.log('API POST RESPONSE (' + url + ')', r);
            return r;
        }).catch(e =>
        {
            console.log('API POST ERROR (' + url + ')', e);
            return e;
        });
    },
    postWithMessage(url, data) {
        return axios.post(url, data).then(r => {
            console.log('API postWithMessage RESPONSE (' + url + ')', r);
            return r;
        }).catch(e => {
            console.log('API postWithMessage ERROR (' + url + ')', e, e);
            throw e;
        });
    },
    upload(url, file) {
        return axios.post(url, file, {
            headers: { 'Content-Type': 'multipart/form-data' }
        }).then(r => {
            console.log('API UPLOAD FILE  RESPONSE (' + url + ')', r);
            return r;
        }).catch(e => {
            console.log('API UPLOAD FILE  ERROR (' + url + ')', e, e);
            throw e;
        });
    },
    put(url, data)
    {
        console.log('PUT ', data);
        return axios.put(url, data, { headers: authStore.getHeaders() }).then(r =>
        {
            console.log('API PUT RESPONSE (' + url + ')', r);
            return r;
        }).catch(e =>
        {
            console.log('API PUT ERROR (' + url + ')', e);
            return e;
        });
    },
    delete(url, data)
    {
        return axios.delete(url, { headers: authStore.getHeaders() }, data).then(r =>
        {
            console.log('API DELETE RESPONSE (' + url + ')', r);
            return r;
        }).catch(e =>
        {
            console.log('API DELETE ERROR (' + url + ')', e);
            return e;
        });
    },
    deleteWithMessage(url) {
        return axios.delete(url).then(r => {
            console.log('API deleteWithMessage RESPONSE (' + url + ')', r);
            return [true, r.data];
        }).catch(e => {
            console.log('API deleteWithMessage ERROR (' + url + ')', e, e.response);
            return [false, e.response.data];
        });
    }

}
