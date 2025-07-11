
using Microsoft.AspNetCore.Mvc;

namespace WorkerServiceControl.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        [HttpGet]
        public ContentResult Get()
        {
            string someContent = "Some Content";

            return new ContentResult
            {
                Content = someContent,
                ContentType = "text/html"
            };
        }
    }
}
