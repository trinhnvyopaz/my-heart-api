using Foundation;
using HealthKit;
using MyHeart.MobileApp.iOS.PlatformServices;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(HealthKitiOS))]
namespace MyHeart.MobileApp.iOS.PlatformServices
{
    public class HealthKitiOS : IHealthKitService
    {
        readonly nuint MAX_LIMIT_VALUE = 2147483647;

        public async Task<double?> FetchPreviousDayAverageHeartRate()
        {
            var heartRate = await FetchPreviousDayHealthDataRate(HKQuantityTypeIdentifier.HeartRate);
            return heartRate;
        }

        public async Task<double?> FetchPreviousDayAverageBodyMass()
        {
            var bodyMass = await FetchPreviousDayHealthDataRate(HKQuantityTypeIdentifier.BodyMass);
            if (bodyMass != null)
            {
                bodyMass = Math.Round(bodyMass.Value, 1);
            }
            return bodyMass;
        }

        public async Task<double?> FetchPreviousDayAverageBloodPressureSystolic()
        {
            var systolicHeartRate = await FetchPreviousDayHealthDataRate(HKQuantityTypeIdentifier.BloodPressureSystolic);
            return systolicHeartRate;
        }

        public async Task<double?> FetchPreviousDayAverageBloodPressureDiastolic()
        {
            var dystolicHeartRate = await FetchPreviousDayHealthDataRate(HKQuantityTypeIdentifier.BloodPressureDiastolic);
            return dystolicHeartRate;
        }

        private async Task<double?> FetchPreviousDayHealthDataRate(HKQuantityTypeIdentifier type)
        {
            var now = DateTime.Now.Date;
            var startDate = now.AddDays(-1).ToNSDate();
            var endDate = now.ToNSDate();

            var predicate = HKQuery.GetPredicateForSamples(startDate, endDate, HKQueryOptions.None);

            var heartRateValues = await FetchHealthData(type, predicate, null);
            if (heartRateValues == null)
                return null;

            var heartRateAverage = heartRateValues.Average();

            return heartRateAverage;
        }

        private async Task<List<double?>> FetchHealthData(HKQuantityTypeIdentifier type, NSPredicate predicate, NSSortDescriptor[] descriptors)
        {
            var quantityType = HKQuantityType.Create(type);

            NSSet<HKObjectType> readTypes = new NSSet<HKObjectType>(new HKObjectType[] { quantityType });

            var heartRateList = await FetchHealthDataAsync(readTypes, quantityType, predicate, descriptors);
            return heartRateList;
        }
        private async Task<List<double?>> FetchHealthDataAsync(NSSet<HKObjectType> readTypes, HKSampleType sampleType, NSPredicate predicate, NSSortDescriptor[] descriptors)
        {
            var tcs = new TaskCompletionSource<List<double?>>();

            // Request authorization to read heart rate data
            HKHealthStore healthStore = new HKHealthStore();

            NSSet<HKObjectType> requestTypes = new NSSet<HKObjectType>(new HKObjectType[]
            {
                HKQuantityType.Create(HKQuantityTypeIdentifier.BloodPressureDiastolic),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BloodPressureSystolic),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass),
                HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate),
            });

            healthStore.RequestAuthorizationToShare(new NSSet<HKObjectType>(), requestTypes, (success, error) =>
            {
                if (success)
                {
                    // Authorization succeeded, read heart rate data

                    HKSampleQuery queryResult = new HKSampleQuery(sampleType: sampleType, predicate: predicate, limit: MAX_LIMIT_VALUE, sortDescriptors: descriptors, (query, results, queryError) =>
                    {
                        if (results.Length > 0)
                        {
                            var values = results.Select(r => GetSampleDoubleValue(r, sampleType)).ToList();

                            tcs.SetResult(values);
                        }
                        else
                        {
                            tcs.SetResult(null);
                        }
                    });

                    healthStore.ExecuteQuery(queryResult);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }

        private double? GetSampleDoubleValue(HKSample sample, HKSampleType type)
        {
            var quantitySample = sample as HKQuantitySample;

            switch (type.Identifier)
            {
                case "HKQuantityTypeIdentifierHeartRate":
                    return quantitySample.Quantity.GetDoubleValue(HKUnit.Count.UnitDividedBy(HKUnit.Minute));

                case "HKQuantityTypeIdentifierBodyMass":
                    return quantitySample.Quantity.GetDoubleValue(HKUnit.Gram) / 1000;

                case "HKQuantityTypeIdentifierBloodPressureSystolic":
                    return quantitySample.Quantity.GetDoubleValue(HKUnit.MillimeterOfMercury);

                case "HKQuantityTypeIdentifierBloodPressureDiastolic":
                    return quantitySample.Quantity.GetDoubleValue(HKUnit.MillimeterOfMercury);

                default:
                    return null;
            }
        }

        public bool IsEnabled()
        {
            return true;
        }

        public bool Disconnect()
        {
            return false;
        }

        public async Task<bool?> ConnectToAccount()
        {
            return false;
        }
    }
}