using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.ModelExtensions
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
