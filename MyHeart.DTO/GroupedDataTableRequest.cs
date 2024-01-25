using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class GroupedDataTableRequest {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public string Filter { get; set; }
        public int[] Groups { get; set; }
        public List<string> Includes { get; set; }
        public List<ColumnOrder> ColumnOrders { get; set; }
    }
}
