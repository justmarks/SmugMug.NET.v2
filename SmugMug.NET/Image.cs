using Newtonsoft.Json;
using System;

namespace SmugMug.NET
{
    public class Image : SmugMugObject
    {
        public int Altitude;
        public string ArchivedMD5;
        public int ArchivedSize;
        public string ArchivedUri;
        public bool CanEdit;
        public string Caption;
        public bool Collectable;
        public DateTime Date;
        public bool EZProject;
        public string FileName;
        public string Format;
        public bool Hidden;
        public string ImageKey;
        public bool IsArchive;
        public bool IsVideo;
        public string[] KeywordArray;
        public string Keywords;
        public DateTime LastUpdated;
        public double Latitude;
        public double Longitude;
        public int OriginalHeight;
        public int OriginalSize;
        public int OriginalWidth;
        public bool Processing;
        public bool Protected;
        public string ThumbnailUrl;
        public string Title;
        public string UploadKey;
        public bool Watermarked;
        public Uri WebUri;
        public ImageUris Uris;

        public override string ToString()
        {
            return string.Format("Image: {0}, {1}", FileName, JsonConvert.SerializeObject(this));
        }
    }

    public class ImageUris
    {
        public SmugMugUri ImageAlbum;
        public SmugMugUri ImageComments;
        public SmugMugUri ImageDownload;
        public SmugMugUri ImageMetadata;
        public SmugMugUri ImageOwner;
        public SmugMugUri ImagePrices;
        public SmugMugUri ImageSizeDetails;
        public SmugMugUri ImageSizes;
        public SmugMugUri LargestImage;
    }

    public class ImageGetResponse : SmugMugUri
    {
        public string DocUri;
        public Image Image;
    }

    public class ImageUpload
    {
        public string StatusImageReplaceUri;
        public string ImageUri;
        public string AlbumImageUri;
        public string URL;
    }

    public class ImagePostResponse 
    {
        public string Stat;
        public string Method;
        public ImageUpload Image;

        public override string ToString()
        {
            return Image.ToString();
        }
    }

    public class ImagePatchResponse : SmugMugUri
    {
        public Image Image;

        public override string ToString()
        {
            return Image.ToString();
        }
    }
}