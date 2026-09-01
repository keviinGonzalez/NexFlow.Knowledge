using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NexFlow.Knowledge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebaController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("¡Hola, Mundo!");
        }
    }
}
