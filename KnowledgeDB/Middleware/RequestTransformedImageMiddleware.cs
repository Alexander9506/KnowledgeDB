using KnowledgeDB.Development;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;


namespace KnowledgeDB.Middleware
{
    public class RequestTransformedImageMiddleware
    {
        private const string REQUEST_TRANSFORMATION_KEY = "transform";

        private readonly RequestDelegate _next;
        private IWebHostEnvironment _env;

        private IList<TransformImageAddIn> AddIns = new List<TransformImageAddIn>();

        public RequestTransformedImageMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var query = context.Request.Query;
            if (query.ContainsKey(REQUEST_TRANSFORMATION_KEY))
            {
                string imagePath = context.Request.Path;
                IFileProvider fileProvider = _env.WebRootFileProvider;
                IFileInfo fileInfo = fileProvider.GetFileInfo(imagePath);
                if (fileInfo.Exists)
                {
                    Image i = Image.FromStream(fileInfo.CreateReadStream());

                    OnImageLoaded(i);

                    Bitmap transformedImage = OnTransformImage(i);

                    OnImageTransformed();

                    MemoryStream memStream;
                    using (memStream = new MemoryStream())
                    {
                        transformedImage.Save(memStream, ImageFormat.Png);
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

            await _next(context);
        }

        private void OnImageTransformed()
        {
            foreach (var addInn in AddIns)
            {
                addInn.ImageTransformed();
            }
        }

        private Bitmap OnTransformImage(Bitmap bitmap, ImageRequestContext context)
        {
            Bitmap last = bitmap;
            foreach (var addInn in AddIns)
            {
                last = addInn.TransformImage(last, context);
            }
            return last;
        }

        private void OnImageLoaded(Bitmap bitmap)
        {
            foreach (var addInn in AddIns)
            {
                addInn.ImageLoaded(bitmap);
            }
        }
    }


    public interface TransformImageAddIn
    {
        public void PreImageLoad();
        public void ImageLoaded(Bitmap bitmap);

        public Bitmap TransformImage(Bitmap bitmap, ImageRequestContext context);
        public void ImageTransformed();
    }


    public class ImageRequestContext
    {
        public IFileInfo FileInfo { get; set; }
        public Dictionary<String,String> Attributes { get; set; }
         
    }

    public class ScaleImageAddIn : TransformImageAddIn
    {
        private const string QUERY_KEY_KEEP_RATIO = "keepRatio";
        private const string QUERY_KEY_IMBED_IN_BACKGROUND = "imbedInBackground";
        private const string QUERY_KEY_WIDTH = "width";
        private const string QUERY_KEY_HEIGHT = "height";
        private const string QUERY_KEY_FAST = "fast";


        public void ImageLoaded(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public void ImageTransformed()
        {
            throw new NotImplementedException();
        }

        public void PreImageLoad()
        {
            throw new NotImplementedException();
        }

        public Bitmap TransformImage(Bitmap bitmap, ImageRequestContext context)
        {
            int width = 0;
            int height = 0;
            bool keepRatio = false;
            bool imbedInBackground = false;
            bool fastMode = false;

            fastMode = RetrieveFromDicionary(context.Attributes, QUERY_KEY_FAST, false);
            keepRatio = RetrieveFromDicionary(context.Attributes, QUERY_KEY_KEEP_RATIO, false);
            imbedInBackground = RetrieveFromDicionary(context.Attributes, QUERY_KEY_IMBED_IN_BACKGROUND, false);
            width = RetrieveFromDicionary(context.Attributes, QUERY_KEY_WIDTH, 100);
            height = RetrieveFromDicionary(context.Attributes, QUERY_KEY_HEIGHT, 100);

            Bitmap scaledImage = ResizeImage(bitmap, width, height, keepRatio, imbedInBackground, fastMode);
            return scaledImage;
        }

        private T RetrieveFromDicionary<T>(Dictionary<string, string> dict, string key, T defaultValue)
        {
            T result = defaultValue;
            if (dict != null && dict.ContainsKey(key))
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if(converter != null)
                    {
                        result = (T) converter.ConvertFromString(dict[key]);
                    }
                }catch(Exception e)
                {
                    //TODO:Log
                }
            }
            return result;
        }

        /// <summary>
        /// Resize Image
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <param name="keepRatio">Keep the original Ratio -> Images may not be the exact desired size</param>
        /// <param name="imbedInBackground">returns the Image in exactly the desired size by imbedding the smaller image in background</param>
        /// <param name="fastMode">image scalled faster but quality is worse</param>
        /// <returns></returns>
        private Bitmap ResizeImage(Bitmap image, int desiredWidth, int desiredHeight, bool keepRatio = false, bool imbedInBackground = true, bool fastMode = false)
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
                graphics.CompositingQuality = fastMode ? CompositingQuality.HighSpeed : CompositingQuality.HighSpeed;
                graphics.InterpolationMode = fastMode ? InterpolationMode.Default : InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = fastMode ? SmoothingMode.HighSpeed : SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = fastMode ? PixelOffsetMode.HighSpeed : PixelOffsetMode.HighQuality;

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
