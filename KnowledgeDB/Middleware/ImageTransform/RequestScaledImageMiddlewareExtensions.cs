using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace KnowledgeDB.Middleware.ImageTransform
{
    public static class RequestScaledImageMiddlewareExtensions
    {

        public static IServiceCollection AddRequestTransformedImageOptions(this IServiceCollection service, Action<RequestTransformedImageOptions> options = default)
        {
            options = options ?? (opts => { });
            service.Configure(options);

            return service;
        }
        public static IApplicationBuilder UseScaledImageMiddleware(this IApplicationBuilder builder, Action<RequestLocalizationOptions> options = default)
        {
            return builder.UseMiddleware<RequestTransformedImageMiddleware>();
        }
    }

    public class RequestTransformedImageOptions
    {
        public IList<ITransformImageAddIn> AddIns { get; set; } = null;
    }

}
