using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces {
    public interface ISMSService {
        Task SendSMS(string message, string number);
    }
}
