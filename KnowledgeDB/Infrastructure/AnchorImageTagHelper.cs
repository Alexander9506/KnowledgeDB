
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
    [HtmlTargetElement(Attributes = "image")]
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
            TagBuilder imageTag = new TagBuilder("i");

            string ButtonText = GetButtonText(output);
            SetButtonTitle(TextAsTitle, ButtonText, output, imageTag);

            //Add Configured classes
            Dictionary<string,string> defaultClasses = GetConfiguredDefaultClasses();

            //Override configured classes if declared
            SetInDictionary(defaultClasses, "Button", ButtonClasses);
            SetInDictionary(defaultClasses, "Image", ImageClasses);

            Dictionary<string, string> classes = GetConfiguredClasses(ImageKeyword);
            AddToDictionary(classes, defaultClasses);

            if (classes.ContainsKey("Button"))
            {
                AddClassesToOutput(output, classes["Button"]);
            }

            if (classes.ContainsKey("Image"))
            {
                AddClassesToTag(imageTag, classes["Image"]);
            }

            output.Attributes.Add(new TagHelperAttribute("type","button"));
            output.Content.AppendHtml(imageTag);
           
            return Task.CompletedTask;
        }

        private void SetInDictionary(Dictionary<string, string> result, string key, string value)
        {
            //Abort if value is empty
            if (String.IsNullOrWhiteSpace(value))
            {
                return;
            }
            result[key] = value;
        }

        private void AddClassesToTag(TagBuilder tag, string classesString)
        {
            if (!String.IsNullOrWhiteSpace(classesString))
            {
                tag.AddCssClass(classesString);
            }
        }

        private void AddClassesToOutput(TagHelperOutput output, string classesString)
        {
            if (!String.IsNullOrWhiteSpace(classesString))
            {
                foreach (var className in classesString.Split(' '))
                {
                    output.AddClass(className, HtmlEncoder.Default);
                }
            }
        }

        private void SetButtonTitle(bool textAsTitle, string buttonText, TagHelperOutput output, TagBuilder imageTag)
        {
            if (textAsTitle)
            {
                output.Attributes.Add("title", buttonText);
            }
            else
            {
                imageTag.InnerHtml.Append(buttonText);
            }
        }

        private string GetButtonText(TagHelperOutput output)
        {
            TagHelperContent childContext = output.GetChildContentAsync().Result;
            return childContext.GetContent();
        }

        private Dictionary<string,string> GetConfiguredClasses (string keyword){
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordSection = configuration.GetSection("Taghelper").GetSection("Keywords").GetSection(keyword);
                AddToDictionary(result, keywordSection);
            }

            return result;
        }

        private Dictionary<string, string> GetConfiguredDefaultClasses()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var imageDefault = configuration.GetSection("Taghelper").GetSection("Default");
            AddToDictionary(result, imageDefault);

            return result;
        }
        private void AddToDictionary(Dictionary<string, string> result, IConfigurationSection section)
        {
            var children = section.GetChildren();
            if (children.Any())
            {
                AddToDictionary(result, children.ToDictionary(k => k.Key, v => v.Value));
            }
            else
            {
                AddToDictionary(result, "Image", section.Value);
            }
        }
        private void AddToDictionary(Dictionary<string, string> result, Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                AddToDictionary(result, item.Key, item.Value);
            }
        }
        private void AddToDictionary(Dictionary<string, string> result, string key, string value)
        {
            //Abort if value is empty
            if (String.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (result.ContainsKey(key))
            {
                result[key] = string.Join(" ", new String[] { result[key], value });
            }
            else
            {
                result[key] = value;
            }
        }

    }
}
