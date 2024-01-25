using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class ColumnOrder
    {
        public string PropertyPath { get; set; }
        public SortDirection Direction { get; set; }
    }
    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
