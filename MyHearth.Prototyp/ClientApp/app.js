//import 'bootstrap';
import "babel-polyfill";
import Vue from "vue";
import router from "./router";
import Vuex from "vuex"
import Application from "@components/app-root.vue";
import Vuetify from 'vuetify';

// Plugins
import axios from "axios";
import VueRouter from "vue-router";
import BootstrapVue from "bootstrap-vue";
import VueLocalize from "v-localize";
import { translations } from "@utils/translations";
import { library } from "@fortawesome/fontawesome-svg-core";
import { fas } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import SvgIcon from "vue-svgicon";
import wysiwyg from "vue-wysiwyg";
import "vue-wysiwyg/dist/vueWysiwyg.css";
import Masonry from "vue-masonry-css";
import Snotify, { SnotifyPosition } from "vue-snotify";
import "vue-snotify/styles/material.css";
import 'vuetify/dist/vuetify.min.css';
import '@mdi/font/css/materialdesignicons.css'; // Ensure you are using css-loader
import mainStore from "@backend/store/store.ts";

import appsettings from "../appsettings.json";
import { useModule } from "vuex-simple";

//TODO make event-source-polyfill work
//import { NativeEventSource, EventSourcePolyfill } from 'event-source-polyfill';
//window.EventSource = NativeEventSource || EventSourcePolyfill;


console.log("Booting Up .... ");

// Event bus for whole app
//Vue.prototype.$eventBus = new Vue()

Vue.use(Vuex);
Vue.use(Vuetify);
Vue.use(VueRouter);
Vue.use(BootstrapVue);
Vue.use(VueLocalize);
Vue.use(wysiwyg);
Vue.use(Masonry);
Vue.use(SvgIcon, {
    tagName: 'svgicon',
    defaultWidth: '1.2rem',
    defaultHeight: '1.2rem'
})

//AXIOS
Vue.prototype.$http = axios;
axios.defaults.baseURL = appsettings.BaseUrl;
console.debug("BaseUrl:", appsettings.BaseUrl);

//localize
let localize = VueLocalize.config({
    default: 'cs-CZ',
    available: ["cs-CZ"],
    fallback: '?',
    localizations: translations
});

//font awesome
library.add(fas)
Vue.component('font-awesome-icon', FontAwesomeIcon)

//Vuetify options
const opts = {
    icons: {
        iconfont: 'mdi'
    },
}

let app = new Vue({
    el: '#app-root',
    store: mainStore,
    router: router,
    render: h => h(Application),
    localize,
    vuetify: new Vuetify(opts)
});

console.log("Booting Done ");

export { app, router };