using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CoginitiveServicesClient
{
    public static class CognitiveServicesAux
    {
        public static Bitmap Base64ToImage (string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = (Bitmap)Bitmap.FromStream(ms, true);
                return image;
            }
        }

        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        // Recognition model 4 was released in 2021 February.
        // It is recommended since its accuracy is improved
        // on faces wearing masks compared with model 3,
        // and its overall accuracy is improved compared
        // with models 1 and 2.
        const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
        public static async Task<double> GetRelationship (Bitmap imageOne, Bitmap imageTwo)
        {
            const string SUBSCRIPTION_KEY = "e938e0743b224d8fbe79fdc152e079dd";
            const string ENDPOINT = "https://facecomparisonapi.cognitiveservices.azure.com/";

            // Authenticate.
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);


            var cognitiveServicesResult = await FindSimilar(client, imageOne, imageTwo, RECOGNITION_MODEL4);
            return cognitiveServicesResult;
        }

        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        /*
         * FIND SIMILAR
         * This example will take an image and find a similar one to it in another image.
         */
        public static async Task<double> FindSimilar(IFaceClient client, Bitmap imageOne, Bitmap imageTwo, string recognition_model)
        {
            IList<Guid?> targetFaceIds = new List<Guid?>();
            // Detect faces from image one and save GUID.

            var streamImageOne = new MemoryStream();
            var pngImageOne = ConvertToPng(imageOne);
            pngImageOne.Save(streamImageOne, ImageFormat.Png);
            streamImageOne.Position = 0;
            var faces = await DetectFaceRecognizeWithStream(client, streamImageOne, recognition_model);
            streamImageOne.Dispose();

            if (faces.Count == 0)
            {
                return 0.0;
            }

            // Add detected faceId to list of GUIDs.
            targetFaceIds.Add(faces[0].FaceId.Value);

            // Detect faces from source image two.
           
            var streamImageTwo = new MemoryStream();
            var pngImageTwo = ConvertToPng(imageTwo);
            pngImageTwo.Save(streamImageTwo, ImageFormat.Png);
            streamImageTwo.Position = 0;
            IList<DetectedFace> detectedFaces = await DetectFaceRecognizeWithStream(client, streamImageTwo, recognition_model);            
            streamImageTwo.Dispose();

            // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
            IList<SimilarFace> similarResults;
            if (detectedFaces.Count > 0)
            {
                similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);
                return similarResults.Count > 0 ? similarResults[0].Confidence : 0.0;
            }
            else
            {
                return 0.0;
            }

        }

        private static async Task<List<DetectedFace>> DetectFaceRecognizeWithStream(IFaceClient faceClient, Stream image, string recognition_model)
        {   
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithStreamAsync(image, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection03);
            return detectedFaces.ToList();
        }

        static Image ConvertToPng(Image imageToConvert)
        {
            Image bmpNewImage = new Bitmap(imageToConvert.Width,
                                      imageToConvert.Height);
            Graphics gfxNewImage = Graphics.FromImage(bmpNewImage);
            gfxNewImage.DrawImage(imageToConvert,
                                  new Rectangle(0, 0, bmpNewImage.Width,
                                                bmpNewImage.Height),
                                  0, 0,
                                  imageToConvert.
                                  Width, imageToConvert.Height,
                                  GraphicsUnit.Pixel);
            return bmpNewImage;
        }
    }
}
