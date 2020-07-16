using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace KnowledgeDB.Middleware.ImageTransform.AddIns
{
    public class ScaleImageAddIn : ITransformImageAddIn
    {
        private const string QUERY_KEY_KEEP_RATIO = "keepRatio";
        private const string QUERY_KEY_IMBED_IN_BACKGROUND = "imbedInBackground";
        private const string QUERY_KEY_WIDTH = "width";
        private const string QUERY_KEY_HEIGHT = "height";
        private const string QUERY_KEY_FAST = "fast";


        public void ImageLoaded(Bitmap bitmap)
        {

        }

        public void ImageTransformed()
        {

        }

        public void PreImageLoad()
        {

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
                    if (converter != null)
                    {
                        result = (T)converter.ConvertFromString(dict[key]);
                    }
                }
                catch (Exception e)
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

        public void PreImageLoad(ImageRequestContext context)
        {
            
        }
    }
}
