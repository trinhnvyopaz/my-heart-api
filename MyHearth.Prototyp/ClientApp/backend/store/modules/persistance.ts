export class Persistance {
    private static BREADCRUMBS_KEY: string = "breadcrumbs_key";

    public static loadBreadcrumbs() {
        return localStorage.getItem(this.BREADCRUMBS_KEY);
    }

    public static saveBreadcrumbs(value) {
        localStorage.setItem(this.BREADCRUMBS_KEY, value);
    }
}