
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;

namespace MyHeart.MobileApp.Services
{
    public interface IVisionService
    {
        Stream DetectAndCropDocuments(byte[] content);
    }
}
