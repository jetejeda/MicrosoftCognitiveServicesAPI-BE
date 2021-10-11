using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
