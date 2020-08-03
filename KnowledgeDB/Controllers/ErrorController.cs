using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace KnowledgeDB.Controllers
{
    [ApiController]
    public class ErrorController : Controller
    {
        private readonly IConfiguration _configuration;

        public ErrorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("/ServerError")]
        public IActionResult ServerError() {
            return View();
        }

        [Route("/Error/{statusCode?}")]
        public IActionResult Error(int statusCode = 0)
        {
            string viewName = "Error";

            //HttpContext.Response.StatusCode = statusCode;
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var originalPath = feature?.OriginalPath;
            
            var imageBasePath = Path.Combine("/", _configuration.GetValue<string>("ImageBasePath"));
            
            //Exclude Assests to keep loading times low
            if (originalPath.StartsWith(imageBasePath))
            {
                return StatusCode(statusCode);
            }

            if(statusCode == 404)
            {
                viewName += "404";
            }

            return View(viewName);
        }
    }
}
