using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoginitiveServicesClient.Services;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CoginitiveServicesClient
{
    public static class CognitiveServicesAux
    {
        public static string GetKinship(double cognitiveServicesResult)
        {            
            if (cognitiveServicesResult <= 20)
            {
                return "ninguno";
            }
            else if (cognitiveServicesResult <= 40)
            {
                return "primos lejanos";
            }
            else if (cognitiveServicesResult <= 60)
            {
                return "primos o tíos";
            }
            else if (cognitiveServicesResult <= 80)
            {
                return "hermanos";
            }
            else if (cognitiveServicesResult <= 90)
            {
                return "papá - mamá / hijo";
            }
            else
            {
                return "la misma persona";
            }
        }


        public static async Task<double> GetRelationship (Bitmap imageOne, Bitmap imageTwo)
        {
            const string SUBSCRIPTION_KEY = "e938e0743b224d8fbe79fdc152e079dd";
            const string ENDPOINT = "https://facecomparisonapi.cognitiveservices.azure.com/";

            // Authenticate.
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);


            var cognitiveServicesResult = await FindSimilar(client, imageOne, imageTwo, RecognitionModel.Recognition04);
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
            var pngImageOne = ImageOperations.ConvertToPng(imageOne);
            pngImageOne.Save(streamImageOne, ImageFormat.Png);
            streamImageOne.Position = 0;
            var faces = await DetectFaceRecognizeWithStream(client, streamImageOne, RecognitionModel.Recognition04);
            streamImageOne.Dispose();

            if (faces.Count == 0)
            {
                return 0.0;
            }

            // Add detected faceId to list of GUIDs.
            targetFaceIds.Add(faces[0].FaceId.Value);

            // Detect faces from source image two.
           
            var streamImageTwo = new MemoryStream();
            var pngImageTwo = ImageOperations.ConvertToPng(imageTwo);
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
    }
}
