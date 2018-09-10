using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmugMug.NET
{
    public class Album : SmugMugObject
    {
        public string AlbumKey;
        public bool AllowDownloads;
        public string Backprinting;
        public string BoutiquePackaging;
        public bool CanRank;
        public bool CanShare;
        public bool Clean;
        public bool Comments;
        public string CommunityUri;
        public string Date;
        public string Description;
        public string DownloadPassword;
        public bool EXIF;
        public bool FamilyEdit;
        public bool Filenames;
        public bool FriendEdit;
        public bool Geography;
        public bool HasDownloadPassword;
        public Header Header;
        public bool HideOwner;
        public string HighlightImageUri;
        public int ImageCount;
        public DateTime ImagesLastUpdated;
        public string InterceptShipping;
        public string Keywords;
        public Size LargestSize;
        public DateTime LastUpdated;
        public string Name;
        public string NodeID;
        public long OriginalSizes;
        public bool PackagingBrand;
        public string Password;
        public string PasswordHint;
        public bool Printable;
        public string PrintmarkUri;
        public PrivacyType Privacy;
        public int ProofDays;
        public bool Protected;
        public SecurityType SecurityType;
        public bool Share;
        public string SmugSearchable;
        public SortDirection SortDirection;
        public AlbumSortMethod SortMethod;
        public string TemplateUri;
        public string ThemeUri;
        public long TotalSizes;
        public string UploadKey;
        public string UrlName;
        public string UrlPath;
        public bool Watermark;
        public string WatermarkUri;
        public Uri WebUri;
        public bool WorldSearchable;
        public AlbumUris Uris;

        public override string ToString()
        {
            return string.Format("Album: {0}, {1}", Name, JsonConvert.SerializeObject(this));
        }
    }

    public class AlbumUris
    {
        public SmugMugUri AlbumShareUris;
        public SmugMugUri Node;
        public SmugMugUri User;
        public SmugMugUri Folder;
        public SmugMugUri ParentFolders;
        public SmugMugUri HighlightImage;
        public SmugMugUri AlbumHighlightImage;
        public SmugMugUri AlbumImages;
        public SmugMugUri AlbumPopularMedia;
        public SmugMugUri AlbumGeoMedia;
        public SmugMugUri AlbumComments;
        public SmugMugUri AlbumDownload;
        public SmugMugUri AlbumPrices;
    }

    public class AlbumGetResponse : SmugMugUri
    {
        public string DocUri;
        public Album Album;
    }

    public class AlbumPostResponse : SmugMugUri
    {
        public Album Album;

        public override string ToString()
        {
            return Album.ToString();
        }
    }

    public class AlbumPagesResponse : SmugMugPagesObject
    {
        public IEnumerable<Album> Album;
    }
}
