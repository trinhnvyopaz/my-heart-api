import SortDirection from "./SortDirection";

export default interface ColumnOrder {
    propertyPath: string;
    direction: SortDirection;
}
