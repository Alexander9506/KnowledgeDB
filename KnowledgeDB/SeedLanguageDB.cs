using KnowledgeDB.Models;
using KnowledgeDB.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB
{
    /// <summary>
    /// Seeds the LanguageDB
    /// </summary>
    public static class SeedLanguageDB
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            EFLanguageContext context = app.ApplicationServices.GetService<EFLanguageContext>();

            if (!context.LanguagePacks.Any())
            {

                StringKey sitenameKey = new StringKey { KeyName = "placeholder_sitename" };

                IEnumerable<StringEntry> germanEntries = new StringEntry[] {
                    new StringEntry{Text = "KnowledgeDB", Key = sitenameKey},
                };
                LanguagePack german = new LanguagePack { EnglishLanguageName = "German", NativeLanguageName = "Deutsch", StringEntries = germanEntries };

                context.LanguagePacks.Add(german);
                context.SaveChanges();
            }
        }
    }
}
