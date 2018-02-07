using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PicController : Controller
    {
        private readonly IHostingEnvironment _env;

        public PicController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpGet]
        [Route("{id}")]
        [Produces("image/png")]
        public IActionResult GetImage(int id)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine($"{webRoot}/Pics/", $"shoes-{id}.png");

            try
            {
                var buffer = System.IO.File.ReadAllBytes(path);
                return File(buffer, "image/png");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return NotFound();
            }
        }
    }
}