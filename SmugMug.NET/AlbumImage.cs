using System;
using System.Collections.Generic;

namespace SmugMug.NET
{
    public class AlbumImage : Image
    {
    }
    

    public class AlbumImageResponse : SmugMugUri
    {
        public string DocUri;
        public AlbumImage AlbumImage;
    }

    public class AlbumImagePagesResponse : SmugMugPagesObject
    {
        public IEnumerable<AlbumImage> AlbumImage;
    }

    public class AlbumImagesWithSizes
    {
        public List<AlbumImage> AlbumImages;
        public Dictionary<string, ImageSizesGetResponse> ImageSizes;
    }

}