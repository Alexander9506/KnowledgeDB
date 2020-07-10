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


namespace KnowledgeDB.Middleware
{
    public class RequestScaledImageMiddleware
    {
        private const string QUERY_KEY_KEEP_RATIO = "keepRatio";
        private const string QUERY_KEY_IMBED_IN_BACKGROUND = "imbedInBackground";
        private const string QUERY_KEY_WIDTH = "width";
        private const string QUERY_KEY_HEIGHT = "height";


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
            bool imbedInBackground = false;

            if (query.ContainsKey(QUERY_KEY_KEEP_RATIO) && bool.TryParse(query[QUERY_KEY_KEEP_RATIO], out bool keepRatioAspect))
            {
                keepRatio = keepRatioAspect;
            }

            if (query.ContainsKey(QUERY_KEY_IMBED_IN_BACKGROUND) && bool.TryParse(query[QUERY_KEY_IMBED_IN_BACKGROUND], out bool imbed))
            {
                imbedInBackground = imbed;
            }

            if (query.ContainsKey(QUERY_KEY_WIDTH) && query.ContainsKey(QUERY_KEY_HEIGHT))
            {
                if (int.TryParse(query[QUERY_KEY_WIDTH], out int width) && int.TryParse(query[QUERY_KEY_HEIGHT], out int height))
                {
                    string imagePath = context.Request.Path;
                    IFileProvider fileProvider = _env.WebRootFileProvider;
                    IFileInfo fileInfo = fileProvider.GetFileInfo(imagePath);
                    if (fileInfo.Exists)
                    {
                        Image i = Image.FromStream(fileInfo.CreateReadStream());
                        Bitmap scaledImage = ResizeImage(i, width, height, keepRatio, imbedInBackground);

                        MemoryStream memStream;
                        using (memStream = new MemoryStream())
                        {
                            scaledImage.Save(memStream, ImageFormat.Png);
                            memStream.Seek(0, SeekOrigin.Begin);//Set stream to begin so the complete mem stream is copied

                            await memStream.CopyToAsync(context.Response.Body);
                        }
                        return;
                    }
                    else
                    {
                        //Bad Request                
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Image not available");
                        return;
                    }
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Resize Image
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <param name="keepRatio">Keep the original Ratio -> Images may not be the exact desired size</param>
        /// <param name="imbedInBackground">returns the Image in exactly the desired size by imbedding the smaller image in background</param>
        /// <returns></returns>
        private Bitmap ResizeImage(Image image, int desiredWidth, int desiredHeight, bool keepRatio = false, bool imbedInBackground = true)
        {
            int width = desiredWidth;
            int height = desiredHeight;

            if (keepRatio)
            {
                double widthPercent = desiredWidth / (double)image.Width;
                double heightPercent = desiredHeight / (double)image.Height;

                double minPercent = Math.Min(widthPercent, heightPercent);

                width = (int)(image.Width * minPercent);
                height = (int)(image.Height * minPercent);
            }

            var destRect = new Rectangle(0, 0, width, height); // Where to place the original image on the "canvas"
            var destImage = new Bitmap(width, height); // Size of the "canvas"

            if (keepRatio && imbedInBackground)
            {
                //place the smaller imager in the image with desired size
                int startInnerImageX = (int)((desiredWidth - (double)width) / 2d);
                int startInnerImageY = (int)((desiredHeight - (double)height) / 2d);

                destRect = new Rectangle(startInnerImageX, startInnerImageY, width, height);
                destImage = new Bitmap(desiredWidth, desiredHeight);
            }

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
}
