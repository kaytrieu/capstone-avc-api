using Microsoft.AspNetCore.Mvc;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        [HttpGet]
        public ActionResult checkConnection()
        {
            return Ok();
        }
    }
}
