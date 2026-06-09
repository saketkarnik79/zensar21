using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        //[Authorize]
        public IActionResult Get()
        {
            return Ok(new[]
            {
                new { Id = 1, Name = "Laptop" }
            });
        }
    }
}
