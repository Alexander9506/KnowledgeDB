using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;


namespace KnowledgeDB.Middleware.ImageTransform
{
    public class ImageRequestContext
    {
        public IFileInfo FileInfo { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

    }
}
