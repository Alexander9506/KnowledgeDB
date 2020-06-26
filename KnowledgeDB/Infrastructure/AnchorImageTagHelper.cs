using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KnowledgeDB.Infrastructure
{
    [HtmlTargetElement("a", Attributes = "image-class")]
    public class AnchorImageTagHelper : TagHelper
    {
        public bool TextAsTitle { get; set; } = true;
        public string ButtonClass { get; set; }
        public string ImageClass { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperContent childContext = output.GetChildContentAsync().Result;
            string ButtonText = childContext.GetContent();

            TagBuilder imageTag = new TagBuilder("i");
            imageTag.AddCssClass(ImageClass);
            if (TextAsTitle)
            {
                output.Attributes.Add("title", ButtonText);
            } else
            {
                imageTag.InnerHtml.Append(ButtonText);
            }

            output.Attributes.Add(new TagHelperAttribute("type","button"));
            output.Content.AppendHtml(imageTag);
            if (!String.IsNullOrWhiteSpace(ButtonClass))
            {
                foreach (var className in ButtonClass.Split(' '))
                {
                    output.AddClass(className, HtmlEncoder.Default);
                }
            }
            return Task.CompletedTask;
        }
    }
}
