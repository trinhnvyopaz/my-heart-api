import ColumnOrder from "./ColumnOrder";

export default interface DataTableRequest {
    page: number;
    pageSize: number;
    filter: string;
    secondFilter: string;
    columnOrders: ColumnOrder[] | null;
}
