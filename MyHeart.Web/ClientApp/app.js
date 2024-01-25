import "babel-polyfill";
import Vue from "vue";
import router from "./router";
import Application from "@components/app-root.vue";
import vuetify from "./utils/vuetify";

// Plugins
import axios from "axios";
import VueRouter from "vue-router";
import { library } from "@fortawesome/fontawesome-svg-core";
import { fas } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import SvgIcon from "vue-svgicon";
import wysiwyg from "vue-wysiwyg";
import "vue-wysiwyg/dist/vueWysiwyg.css";
import Masonry from "vue-masonry-css";
import Vue2Editor from "vue2-editor";

import appsettings from "../appsettings.json";

import "vuetify/dist/vuetify.min.css";
import "@mdi/font/css/materialdesignicons.css"; // Ensure you are using css-loader
import store from "./backend/store/authStore";
import PerfectScrollbar from 'vue2-perfect-scrollbar'
import 'vue2-perfect-scrollbar/dist/vue2-perfect-scrollbar.css'
import $ from 'jquery';
import getter from "./backend/ApiUrlGetter.js"
import LocalizedDataTable from '@components/Shared/localizedDataTable.vue';

console.log("Booting Up .... ");


axios.interceptors.request.use(
  (config) => {
    let token = store.getToken();

    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }

    return config;
  },

  (error) => {
    return Promise.reject(error);
  }
);

let wysiwygOptions = {
    hideModules: { "link": true, "image": true  }
}

console.log(process.env)

Vue.use(Vue2Editor);
Vue.use(VueRouter);
Vue.use(wysiwyg, wysiwygOptions);
Vue.use(Masonry);
Vue.use(SvgIcon, {
  tagName: "svgicon",
  defaultWidth: "1.2rem",
  defaultHeight: "1.2rem",
});
Vue.use(PerfectScrollbar);



console.log(process.env.BASE_URL);
//Axios
//AXIOS
Vue.prototype.$http = axios;
console.log(process.env)
axios.defaults.baseURL = getter.getApiUrl(process.env.MODE);
console.log("BaseUrl:", axios.defaults.baseURL);

//font awesome
library.add(fas);
Vue.component("font-awesome-icon", FontAwesomeIcon);
Vue.component("localized-data-table", LocalizedDataTable);
Vue.component('font-awesome-icon', FontAwesomeIcon);

let app = new Vue({
  el: "#app-root",
  router: router,
  render: (h) => h(Application),
  vuetify,
});

console.log("Booting Done ");
export { app, router };
