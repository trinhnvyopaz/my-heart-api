using Foundation;
using MyHeart.MobileApp.iOS.PlatformServices;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using UIKit;
using Vision;
using CoreImage;
using CoreGraphics;
using MyHeart.MobileApp.iOS.Extensions;
using System.IO;
using System.Linq;
using ImageIO;
using System.Drawing;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(VisionServiceiOS))]
namespace MyHeart.MobileApp.iOS.PlatformServices
{
    public class VisionServiceiOS : IVisionService
    {
        private uint maxObservations = 10;
        private readonly List<VNRectangleObservation> observations = new List<VNRectangleObservation>();
        CIImage InputImage;
        UIImage RawImage;
        VNDetectRectanglesRequest rectangleRequest;

        public Stream DetectAndCropDocuments(byte[] content)
        {
            var detectedRecatangles = DetectRectangle(content);

            if (detectedRecatangles == null)
                return null;


            var observation = detectedRecatangles[0];

            var width = InputImage.Extent.Width;
            var height = InputImage.Extent.Height;
            var topLeft = new CGPoint(observation.TopLeft.X * width, observation.TopLeft.Y * height);
            var bottomLeft = new CGPoint(observation.BottomLeft.X * width, observation.BottomLeft.Y * height);
            var bottomRight = new CGPoint(observation.BottomRight.X * width, observation.BottomRight.Y * height);


            var cropY = height - topLeft.Y;

            var newHeight = height - bottomRight.Y - cropY;
            var newWidth = width - bottomLeft.X - (width - bottomRight.X);

            UIGraphics.BeginImageContext(new CGSize(newWidth, newHeight));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new CGRect(0, 0, newWidth, newHeight);
            context.ClipToRect(clippedRect);

            var drawRect = new CGRect(-bottomLeft.X, -cropY, width, height);

            RawImage.Draw(drawRect);

            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var imageStream = modifiedImage.AsPNG().AsStream();

            return imageStream;
        }

        public IList<VNRectangleObservation> DetectRectangle(byte[] content)
        {
            try
            {
                rectangleRequest = new VNDetectRectanglesRequest(HandleRectangles);
                rectangleRequest.MaximumObservations = new nuint(maxObservations);
                var imageData = NSData.FromArray(content);
                RawImage = UIImage.LoadFromData(imageData);
                var ciImage = new CIImage(RawImage);
                InputImage = ciImage.CreateWithOrientation(RawImage.Orientation.ToCIImageOrientation());

                var uiImage = new UIImage(InputImage);

                var handler = new VNImageRequestHandler(InputImage,
                    uiImage.Orientation.ToCGImagePropertyOrientation(),
                    new VNImageOptions());

                handler.Perform(new VNRequest[] { rectangleRequest }, out NSError error);

                return observations;

            }
            catch (Exception ex)
            {
                return null;

            }

            //});
        }
        private void HandleRectangles(VNRequest request, NSError error)
        {
            observations.Clear();
            var imageSize = InputImage.Extent.Size;
            foreach (var observation in request.GetResults<VNRectangleObservation>())
            {
                observations.Add(observation);
            }
        }
    }
}