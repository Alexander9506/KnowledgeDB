using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            string originalPath = "";
            if (feature != null)
            {
                originalPath = feature.OriginalPath;
            }

            PathString requestedPath = new PathString(originalPath);
            IEnumerable<PathString> assetsFolders = 
                _configuration.GetSection("AssestsFolders").Get<String[]>()
                .Select(p => new PathString(Path.Combine("/", p)));

            //Exclude images to keep loading times low
            if (assetsFolders.Any(ps => requestedPath.StartsWithSegments(ps)))
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
