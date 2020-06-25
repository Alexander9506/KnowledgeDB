using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.Repositories.Language;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KnowledgeDB
{

    public static class SeedMainDB
    {
        public static void EnsurePopulated(IApplicationBuilder builder)
        {
            EFContext context = builder.ApplicationServices.GetService<EFContext>();

            if (!context.Articles.Any())
            {

                ArticleTag artTag1 = new ArticleTag
                {
                    Name = "FirstTag"
                };

                ArticleTag artTag2 = new ArticleTag
                {
                    Name = "SecondTag"
                };

                DateTime date = DateTime.Now;

                for (int i = 0; i < 20; i++)
                {
                    date = date.AddDays(-i);
                    Article art = new Article
                    {
                        Content = $"Das hier ist Content {i}",
                        Summary = $"Das hier ist die Zusammenfassung {i}",
                        Title = $"Titel {i}",
                        RefToTags = i % 2 == 0 ? new List<RefArticleArticleTag> { new RefArticleArticleTag { ArticelTag = artTag1 } } : new List<RefArticleArticleTag> { new RefArticleArticleTag { ArticelTag = artTag1 }, new RefArticleArticleTag { ArticelTag = artTag2 } },
                        CreatedAt = date,
                        ModifiedAt = date
                        
                    };
                    context.Articles.Add(art);
                }
                context.SaveChanges();
            }
        }
    }
}
