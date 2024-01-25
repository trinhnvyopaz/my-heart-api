using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public interface IHealthKitService
    {
        Task<double?> FetchPreviousDayAverageHeartRate();
        Task<double?> FetchPreviousDayAverageBodyMass();
        Task<double?> FetchPreviousDayAverageBloodPressureSystolic();
        Task<double?> FetchPreviousDayAverageBloodPressureDiastolic();
        bool IsEnabled();
        bool Disconnect();
        Task<bool?> ConnectToAccount();
    }
}
