
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class DataTableRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string Filter { get; set; }
        public string SecondFilter { get; set; }
        public List<ColumnOrder> ColumnOrders { get; set; }
    }
}
