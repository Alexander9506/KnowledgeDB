using System.Drawing;


namespace KnowledgeDB.Middleware.ImageTransform
{
    public interface ITransformImageAddIn
    {
        public void PreImageLoad(ImageRequestContext context);
        public void ImageLoaded(Bitmap bitmap);
        public Bitmap TransformImage(Bitmap bitmap, ImageRequestContext context);
        public void ImageTransformed();
    }
}
