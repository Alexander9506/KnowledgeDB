using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeDB.Middleware;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.Repositories.Language;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace KnowledgeDB
{
    public class Startup
    {
        public IConfiguration configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Models.Repositories.EFContext>();
            services.AddDbContext<EFLanguageContext>(options => options.UseNpgsql(configuration.GetConnectionString("LanguageDatabase")));

            services.AddTransient<IStringRetriever, StringRetrieverRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IFileRepository, FileRepository>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            app.UseScaledImageMiddleware();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedMainDB.EnsurePopulated(app);
            SeedLanguageDB.EnsurePopulated(app);
        }
    }
}
