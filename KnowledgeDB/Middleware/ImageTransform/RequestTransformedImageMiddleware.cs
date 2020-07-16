using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Middleware.ImageTransform
{
    public class RequestTransformedImageMiddleware
    {
        private const string REQUEST_TRANSFORMATION_KEY = "transform";

        private readonly RequestDelegate _next;
        private IWebHostEnvironment _env;
        private IList<ITransformImageAddIn> _addIns = new List<ITransformImageAddIn>();

        public RequestTransformedImageMiddleware(RequestDelegate next, IWebHostEnvironment env, IOptions<RequestTransformedImageOptions> options)
        {
            _next = next;
            _env = env;
            RequestTransformedImageOptions requestScaledImageOptions = options.Value;
            _addIns = requestScaledImageOptions.AddIns;
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
                    OnPreImageLoad(fileInfo);
                    Bitmap bitmap = (Bitmap)Image.FromStream(fileInfo.CreateReadStream());

                    OnImageLoaded(bitmap);

                    ImageRequestContext imageContext = new ImageRequestContext { FileInfo = fileInfo, Attributes = query.AsQueryable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()) };
                    Bitmap transformedImage = OnTransformImage(bitmap, imageContext);

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

        private void OnPreImageLoad(IFileInfo fileInfo)
        {
            foreach (var addInn in _addIns)
            {
                addInn.PreImageLoad(new ImageRequestContext { FileInfo = fileInfo});
            }
        }

        private void OnImageTransformed()
        {
            foreach (var addInn in _addIns)
            {
                addInn.ImageTransformed();
            }
        }

        private Bitmap OnTransformImage(Bitmap bitmap, ImageRequestContext context)
        {
            Bitmap last = bitmap;
            foreach (var addInn in _addIns)
            {
                last = addInn.TransformImage(last, context);
            }
            return last;
        }

        private void OnImageLoaded(Bitmap bitmap)
        {
            foreach (var addInn in _addIns)
            {
                addInn.ImageLoaded(bitmap);
            }
        }
    }
}
