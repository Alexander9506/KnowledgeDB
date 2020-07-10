using Microsoft.AspNetCore.Builder;


namespace KnowledgeDB.Middleware
{
    public static class RequestScaledImageMiddlewareExtensions
    {
        public static IApplicationBuilder UseScaledImageMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestScaledImageMiddleware>();
        }
    }
}
