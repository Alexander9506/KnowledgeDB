using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace KnowledgeDB.Infrastructure
{
    [HtmlTargetElement(Attributes = "image-path-on-server")]
    public class TransformedImageRequestTagHelper : TagHelper
    {
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;

        public bool KeepRatio { get; set; } = false;
        public bool ImbedInBackground { get; set; } = false;
        public bool Fast { get; set; } = false;

        public string ImagePathOnServer { get; set; }

        private readonly IWebHostEnvironment _environment;
        public TransformedImageRequestTagHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(ImagePathOnServer))
            {
                string FileUrl = "/" + ImagePathOnServer;
                string URLAdditions = $"?transform&width={Width}&height={Height}&keepRatio={KeepRatio}&imbedInBackground={ImbedInBackground}&fast={Fast}";

                output.Attributes.Add("src", FileUrl + URLAdditions);
            }

            return Task.CompletedTask;
        }
    }
}
