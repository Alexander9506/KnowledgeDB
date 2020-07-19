using System;
using KnowledgeDB.Areas.Identity.Data;
using KnowledgeDB.Models.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(KnowledgeDB.Areas.Identity.IdentityHostingStartup))]
namespace KnowledgeDB.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityContext>(options => options.UseNpgsql(
                        context.Configuration.GetConnectionString("IdentityDatabase")));

                services.AddDefaultIdentity<KnowledgeDBUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<IdentityContext>();
            });
        }
    }
}