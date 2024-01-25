import ColumnOrder from "./ColumnOrder";

export default interface GroupedDataTableRequest {
    page: number;
    pageSize: number;
    filter: string;
    groups: number[];
    columnOrders: ColumnOrder[] | null;
}
