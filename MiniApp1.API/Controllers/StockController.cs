using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [Authorize(Roles = "Admin", Policy = "AgePolicy")]
        [Authorize(Roles = "Admin", Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var username = HttpContext.User.Identity.Name;
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            // Veri tabanında userId veya username alanları üzerinden gerekli datalar çekilebilir.

            return Ok($"Username: {username} -- UserId: {userId}");
        }
    }
}
