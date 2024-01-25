using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces {
    public interface IOpenData {
        public Task<bool> UpdateData();
    }
}
