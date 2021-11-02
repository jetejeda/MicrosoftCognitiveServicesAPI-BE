using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoginitiveServicesClient.Models;
using CoginitiveServicesClient.Services;
using Microsoft.EntityFrameworkCore;

namespace CoginitiveServicesClient.Controllers
{
    [ApiController]
    public class CognitiveServicesController : ControllerBase
    {
        AppDbContext context;
        public CognitiveServicesController()
        {
            if (context == null)
            {
                context = new AppDbContext();
            }
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Get()
        {
            return Ok("File upload running API");
        }

        [HttpPost]
        [Route("/getRelationship")]
        public async Task<IActionResult> GetRelationship()
        {
            try
            {
                string rawValue = string.Empty;
                var data = new DataModel();
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    rawValue = await reader.ReadToEndAsync();
                    data = JsonConvert.DeserializeObject<DataModel>(rawValue);
                }

                var imageOne = ImageOperations.Base64ToImage(data.Image1);
                var imageTwo = ImageOperations.Base64ToImage(data.Image2);               
                var kinshipPercentage = await CognitiveServicesAux.GetRelationship(imageOne, imageTwo) * 100;
                kinshipPercentage = Math.Round(kinshipPercentage,2);
                var kinship = CognitiveServicesAux.GetKinship(kinshipPercentage);
                var textAnswer = "La similitud es: " + kinshipPercentage + "% El parentesco entre las personas es de " + kinship;
                var imageResult = ImageOperations.AssembleAnswerImage(imageOne, imageTwo, textAnswer);
                var imageResultBase64 = ImageOperations.ImageToBase64(imageResult);

                MemoryStream ms = new MemoryStream();
                context.Imagenes.Add(new ImagenesModel { 
                        ImageData = Convert.FromBase64String(imageResultBase64)
                });
                context.SaveChanges();

                var lastElement = await context.Imagenes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                return Ok(new AnswerModel { 
                        Answer = imageResultBase64,
                        Id = lastElement.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }


        [HttpGet]
        [Route("/getImageById")]
        public async Task<IActionResult> GetImageById(int id)
        {
            try
            {
                var image = await context.Imagenes.FirstOrDefaultAsync(x => x.Id == id);
                string base64String = Convert.ToBase64String(image.ImageData, 0, image.ImageData.Length);

                return Ok(new AnswerModel
                {
                    Answer = base64String
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
