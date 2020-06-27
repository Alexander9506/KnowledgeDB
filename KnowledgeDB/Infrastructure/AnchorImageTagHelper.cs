
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KnowledgeDB.Infrastructure
{
    [HtmlTargetElement("a", Attributes = "image")]
    public class AnchorImageTagHelper : TagHelper
    {
        private IConfiguration configuration;
        public bool TextAsTitle { get; set; } = true;
        public string ButtonClasses { get; set; }
        public string ImageClasses { get; set; }
        [HtmlAttributeName("image")]
        public string ImageKeyword { get; set; }

        public AnchorImageTagHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperContent childContext = output.GetChildContentAsync().Result;
            string ButtonText = childContext.GetContent();

            TagBuilder imageTag = new TagBuilder("i");

            //Add configured CssClasses
            if (!String.IsNullOrWhiteSpace(ImageKeyword))
            {
                foreach (var cssClass in GetDefaultImageClasses(ImageKeyword))
                {
                    imageTag.AddCssClass(cssClass);
                }
                imageTag.AddCssClass(GetImageClasses(ImageKeyword));
            }

            if (!String.IsNullOrWhiteSpace(ImageClasses))
            {
                imageTag.AddCssClass(ImageClasses);
            }

            if (TextAsTitle)
            {
                output.Attributes.Add("title", ButtonText);
            } else
            {
                imageTag.InnerHtml.Append(ButtonText);
            }

            output.Attributes.Add(new TagHelperAttribute("type","button"));
            output.Content.AppendHtml(imageTag);
            if (!String.IsNullOrWhiteSpace(ButtonClasses))
            {
                foreach (var className in ButtonClasses.Split(' '))
                {
                    output.AddClass(className, HtmlEncoder.Default);
                }
            }
            return Task.CompletedTask;
        }

        private IEnumerable<string> GetDefaultImageClasses(string imageKeyword)
        {
            return configuration.GetSection("Taghelper").GetSection("Default").GetChildren().Select(s => s.Value);
        }

        private string GetImageClasses(string keyword)
        {
            return configuration.GetSection("Taghelper").GetSection("Keywords").GetValue<string>(keyword);
        }
    }
}
