using System;

namespace MyHeart.DTO
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MappingExcel : Attribute
    {
        public string ColumnName { get; set; }

        public MappingExcel(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
