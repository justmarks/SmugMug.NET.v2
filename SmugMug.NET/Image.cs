using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public Dictionary<string, SmugMugUri> Uris;

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
        public SmugMugUri CSMILVideo;
        public SmugMugUri EmbedVideo;
        public SmugMugUri Regions;
        public SmugMugUri PointOfInterestCrops;
        public SmugMugUri PointOfInterest;
        public SmugMugUri LargestVideo;
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

    public class ImageSizesGetResponse : SmugMugUri
    {
        public ImageSizes ImageSizes;
    }

    public class ImageSizes
    {
        [JsonProperty("110VideoUrl")]
        public string VideoUrl110;

        [JsonProperty("1280VideoUrl")]
        public string VideoUrl1280;

        [JsonProperty("1920VideoUrl")]
        public string VideoUrl1920;

        [JsonProperty("200VideoUrl")]
        public string VideoUrl200;

        [JsonProperty("320VideoUrl")]
        public string VideoUrl320;

        [JsonProperty("4KImageUrl")]
        public string ImageUrl4k;

        [JsonProperty("5KImageUrl")]
        public string ImageUrl5k;

        [JsonProperty("640VideoUrl")]
        public string VideoUrl640;

        [JsonProperty("960VideoUrl")]
        public string VideoUrl960;

        public string LargeImageUrl;

        public string LargestImageUrl;
        public string LargestVideoUrl;
        public string MediumImageUrl;
        public string OriginalImageUrl;
        public string SMILVideoUrl;
        public string SmallImageUrl;
        public string ThumbImageUrl;
        public string TinyImageUrl;
        public string X2LargeImageUrl;
        public string X3LargeImageUrl;
        public string X4LargeImageUrl;
        public string X5LargeImageUrl;
        public string XLargeImageUrl;
    }

}