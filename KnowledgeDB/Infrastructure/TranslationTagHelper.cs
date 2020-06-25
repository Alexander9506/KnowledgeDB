using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories.Language;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Infrastructure
{
    [HtmlTargetElement("span", Attributes = "translate-key")]
    public class TranslationTagHelper : TagHelper
    {
        private IStringRetriever stringRetriever;
        private IConfiguration configuration;

        public string TranslateKey { get; set; }
        public string ToLanguage { get; set; }

        public TranslationTagHelper(IStringRetriever stringRetriever, IConfiguration configuration)
        {
            this.stringRetriever = stringRetriever;
            this.configuration = configuration;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            LanguagePack lp = GetLanguagePack(ToLanguage);
            StringEntry entry = null;

            if (lp != null)
            {
                entry = stringRetriever.StringEntries.FirstOrDefault(e => e.LanguagePack.ID == lp.ID && e.Key.KeyName == TranslateKey);
            }

            if (entry != null)
            {
                output.Content.AppendHtml(entry.Text);
            }
            else
            {
                output.Content.AppendHtml($"!!No Entry found for: {TranslateKey} !!");
            }

            return Task.CompletedTask;
        }

        private LanguagePack GetLanguagePack(string language)
        {
            LanguagePack lp = null;
            if (!string.IsNullOrWhiteSpace(language))
            {
                lp = stringRetriever.LanguagePacks.FirstOrDefault(lp => lp.EnglishLanguageName == language);
            }

            if(lp == null)
            {
                string configuredLanguage = configuration.GetSection("Language").GetSection("Default").Value;
                lp = stringRetriever.LanguagePacks.FirstOrDefault(lp => lp.EnglishLanguageName == configuredLanguage);
            }

            return lp;
        }
    }
}
