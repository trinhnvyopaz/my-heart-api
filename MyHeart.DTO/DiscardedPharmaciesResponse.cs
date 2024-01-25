using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class DiscardedPharmaciesRequest {
        public int Group { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }

        public string Filter { get; set; }
    }
}
