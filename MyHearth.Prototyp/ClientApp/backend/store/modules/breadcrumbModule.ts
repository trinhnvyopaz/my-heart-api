import { Mutation, State, Action, Getter, Module } from "vuex-simple";
import { Persistance } from "./persistance";

export interface Breadcrumb {
    text: string;
    to: any;
}

export class BreadcrumbModule {
    @State()
    public _breadcrumbs: Breadcrumb[] = [];

    // GETTERS
    @Getter()
    public get breadcrumbs(): Breadcrumb[] {
        return this._breadcrumbs;
    }

    // ACTIONS
    @Action()
    private Init() {
        this._breadcrumbs = JSON.parse(Persistance.loadBreadcrumbs());
    }

    @Action()
    private Add(value) {
        this.setBreadcrumbs(value);
    }

    @Action()
    private Remove(value) {
        this.removeBreadcrumbs(value);
    }

    @Action()
    private Clear() {
        this.clear();
    }

    // MUTATIONS
    @Mutation()
    public setBreadcrumbs(value) {
        if (!this.breadcrumbs.map(({ text }) => text).includes(value.text))
            this.breadcrumbs.push(value);

        this.saveBreadcrumbs();
    }

    @Mutation()
    public removeBreadcrumbs(value) {
        this.breadcrumbs.splice(this.breadcrumbs.indexOf(value), 1);
    }

    @Mutation()
    public clear() {
        this._breadcrumbs = [];
    }

    @Mutation()
    public saveBreadcrumbs() {
        let breadcrumbsToSave = JSON.stringify(this.breadcrumbs);
        Persistance.saveBreadcrumbs(breadcrumbsToSave);
    }
}