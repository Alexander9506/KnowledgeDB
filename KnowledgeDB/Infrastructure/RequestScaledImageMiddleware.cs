using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;


namespace KnowledgeDB.Infrastructure
{
    public class RequestScaledImageMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebHostEnvironment _env;

        public RequestScaledImageMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var query = context.Request.Query;
            bool keepRatio = false;

            if (query.ContainsKey("keepRatio") && bool.TryParse(query["keepRatio"], out bool keepRatioAspect)) {
                keepRatio = keepRatioAspect;
            }

            if (query.ContainsKey("width") && query.ContainsKey("height")){
                if(int.TryParse(query["height"], out int height) && int.TryParse(query["width"], out int width))
                {
                    string imagePath = context.Request.Path;
                    IFileProvider fileProvider = _env.WebRootFileProvider;
                    IFileInfo fileInfo = fileProvider.GetFileInfo(imagePath);
                    if (fileInfo.Exists) {
                        Image i = Image.FromStream(fileInfo.CreateReadStream());
                        Bitmap scaledImage = ResizeImage(i, width, height, keepRatio);

                        MemoryStream memStream;
                        using (memStream = new MemoryStream()) {
                            scaledImage.Save(memStream, ImageFormat.Png);
                            memStream.Seek(0, SeekOrigin.Begin);
                            await memStream.CopyToAsync(context.Response.Body);
                        }
                        return;
                    } else {
                        context.Response.StatusCode = 400; //Bad Request                
                        await context.Response.WriteAsync("Image not available");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private Bitmap ResizeImage(Image image, int desiredWidth, int desiredHeight, bool keepRation)
        {
            int width = desiredWidth;
            int height = desiredHeight;

            if (keepRation) {
                double widthPercent = desiredWidth / ((double)image.Width);
                double heightPercent = desiredHeight / ((double)image.Height);

                double minPercent = Math.Min(widthPercent, heightPercent);

                width =  (int)(((double)image.Width) * minPercent);
                height = (int)(((double)image.Height) * minPercent);
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }

    public static class TestMiddlewareExtensions
    {
        public static IApplicationBuilder UseTestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestScaledImageMiddleware>();
        }
    }
}
