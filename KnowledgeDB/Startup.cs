using System.Collections.Generic;
using KnowledgeDB.Infrastructure;
using KnowledgeDB.Middleware.ImageTransform;
using KnowledgeDB.Middleware.ImageTransform.AddIns;
using KnowledgeDB.Models.Context;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.Repositories.Language;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

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
            services.AddDbContext<EFContext>();
            services.AddDbContext<EFLanguageContext>(options => options.UseNpgsql(configuration.GetConnectionString("LanguageDatabase")));

            services.AddRequestTransformedImageOptions(options => options.AddIns = new List<ITransformImageAddIn>() { new ScaleImageAddIn() });
            services.AddTransient<IStringRetriever, StringRetrieverRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IFileHelper, FileHelper>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            
            app.UseTransformedImageMiddleware();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            SeedMainDB.EnsurePopulated(app);
            SeedLanguageDB.EnsurePopulated(app);
        }
    }
}
