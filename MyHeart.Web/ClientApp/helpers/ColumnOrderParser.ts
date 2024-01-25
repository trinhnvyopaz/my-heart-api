import ColumnOrder from "@models/ColumnOrder"
import SortDirection from "@models/SortDirection"

export default class ColumnOrderParser{

    static parseOptions(options: any){
        var columnOrders: ColumnOrder[] = [];
        for (let index = 0; index < options.sortDesc.length; index++) {
            const sortDesc = options.sortDesc[index];
            const sortBy = options.sortBy[index];

            var columnOrder: ColumnOrder = {
                propertyPath: sortBy,
                direction: sortDesc == true ? SortDirection.descending : SortDirection.ascending
            };
            
            columnOrders.push(columnOrder)
        }

        return columnOrders;
    }

}
