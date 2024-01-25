export default interface PagedResourceRequest {
    page: number;
    pageSize: number;
    selected: number[];
    filter: string;
}
