using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class DiscardedPharmaciesResponse<T> {
        public int Group { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IList<T> Data { get; set; }
    }
}
