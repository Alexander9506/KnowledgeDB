using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
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

        public RequestScaledImageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //if request goes to => ~/images/
                //example URL => ~/images/fasdf.png?width=1024&height=1024
                //work on
                    //check if the requested image is available
                    //scale down and send back
                //short circuit
            //else
                //next delegate

            var query = context.Request.Query;

            if(query.ContainsKey("width") && query.ContainsKey("height")){
                if(int.TryParse(query["height"], out int height) && int.TryParse(query["width"], out int width))
                {
                    //TODO: load the correct image
                    Image i = Image.FromFile("C:\\Users\\alexa\\source\\repos\\KnowledgeDB\\KnowledgeDB\\wwwroot\\images\\uploads\\03vy0nt.jpg");
                    Bitmap scaledImage = ResizeImage(i, width, height);

                    MemoryStream memStream;
                    using (memStream = new MemoryStream()){
                        scaledImage.Save(memStream, ImageFormat.Png);
                        memStream.Seek(0, SeekOrigin.Begin);
                        await memStream.CopyToAsync(context.Response.Body);
                    }
                    return;
                }
            }

            await _next(context);
        }

        //TODO: should'nt change aspect ratio
        private Bitmap ResizeImage(Image image, int width, int height)
        {
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
