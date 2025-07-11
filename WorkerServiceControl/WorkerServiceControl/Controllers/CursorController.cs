
using Microsoft.AspNetCore.Mvc;
using WorkerServiceControl.Models;
using WorkerServiceControl.Services;

namespace WorkerServiceControl.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CursorController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendRightClick(Cursor cursor)
        {
            Sender.SendRightClick(cursor.X, cursor.Y);
            return Ok(new { cursor });
        }
    }
}
