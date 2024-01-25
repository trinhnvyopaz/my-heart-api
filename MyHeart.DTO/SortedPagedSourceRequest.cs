using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class SortedPagedSourceRequest {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<int> Selected { get; set; }
        public string Filter { get; set; }
    }
}
