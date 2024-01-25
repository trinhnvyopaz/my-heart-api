import Vue from "vue";
import Vuex from "vuex";

import { Module, createVuexStore } from "vuex-simple";

import { BreadcrumbModule } from "./modules/breadcrumbModule";

export class MainStore {
    @Module()
    public Breadcrumb: BreadcrumbModule = new BreadcrumbModule();
}

Vue.use(Vuex);

const main_store = new MainStore();

export default createVuexStore(main_store, {
    strict: false,
    modules: {},
    plugins: []
});