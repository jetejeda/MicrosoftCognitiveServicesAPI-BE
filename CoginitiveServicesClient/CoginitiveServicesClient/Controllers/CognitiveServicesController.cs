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

namespace CoginitiveServicesClient.Controllers
{
    [ApiController]
    public class CognitiveServicesController : ControllerBase
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Get()
        {
            return Ok("File upload running API");
        }

        [HttpPost]
        [Route("/getRelationship")]
        public async Task<IActionResult> ProcessPayment()
        {
            string rawValue = string.Empty;
            var data = new DataModel();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                rawValue = await reader.ReadToEndAsync();
                data = JsonConvert.DeserializeObject<DataModel>(rawValue);
            }

            try
            {
                var imageOne = CognitiveServicesAux.Base64ToImage(data.Image1);
                var imageTwo = CognitiveServicesAux.Base64ToImage(data.Image2);
                var kinshipPercentage = await CognitiveServicesAux.GetRelationship(imageOne, imageTwo) * 100;
                var kinship = ImageOperations.GetKinship(kinshipPercentage);
                return Ok("La similitud es: " + kinshipPercentage + "% El parentesco entre las personas es de " + kinship);
            }
            catch (Exception e)
            {
                return BadRequest("Datos incorrectos");
            }
        }

    }
}
