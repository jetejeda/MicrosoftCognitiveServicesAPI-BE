using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoginitiveServicesClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CognitiveServicesController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("File upload running API");
        }

        [HttpPost]
        [Route("getRelationship")]
        public IActionResult Upload(IFormCollection data, IFormFile image1, IFormFile image2)
        {
            try
            {
                var email = data["email"];
                var sendToEmail = Convert.ToBoolean(data["sendToEmail"]);
                return Ok("Datos recibidos con éxito");
            }
            catch (Exception)
            {
                return BadRequest("Datos incorrectos");
            }
        }

    }
}
