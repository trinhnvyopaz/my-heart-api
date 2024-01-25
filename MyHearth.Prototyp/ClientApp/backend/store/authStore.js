import { Date } from "core-js";

const authStore = {
    BEARER_TOKEN: 'token_key',
    USER_KEY: 'user_key',
    getToken: function () {
        return localStorage.getItem(this.BEARER_TOKEN);
    },
    setToken: function (token) {
        localStorage.setItem(this.BEARER_TOKEN, token);
    },
    clear: function () {
        localStorage.clear();
    },
    isLoggedIn: function () {
        return localStorage.getItem(this.BEARER_TOKEN) != null;
    },
    getHeaders: function () {
        return { 'Authorization': 'Bearer ' + this.getToken() };
    },
    isTokenExp: function () {
        let token = this.getToken();
        let pt = JSON.parse(atob(token));
        let exp = pt.exp;

        return exp < Date.now() / 1000;
    }
};

export default authStore;
