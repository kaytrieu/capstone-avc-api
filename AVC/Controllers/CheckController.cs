using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

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

        [HttpGet("log")]
        public ActionResult getLogFile()
        {
            var directory = new DirectoryInfo("D:\\home\\LogFiles\\http\\RawLogs");
            var myFile = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .FirstOrDefault();

            if (myFile != null && myFile.Extension.Contains("json"))
            {
                var stream = myFile.OpenText();
                var content = stream.ReadToEnd();
                stream.Close();
                return Ok(content);
            }

            return Ok();
        }
    }
}
