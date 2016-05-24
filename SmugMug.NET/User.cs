using Newtonsoft.Json;
using System;

namespace SmugMug.NET
{
    public class User : SmugMugObject
    {
        public AccountStatus AccountStatus;
        public string Domain;
        public string DomainOnly;
        public string FirstName;
        public bool FriendsView;
        public int ImageCount;
        public bool IsTrial;
        public string LastName;
        public string Name;
        public string NickName;
        public string Plan;
        public bool QuickShare;
        public string RefTag;
        public SortMethod SortBy;
        public string TotalAccountSize;
        public string TotalUploadedSize;
        public string ViewPassHint;
        public string ViewPassword;
        public Uri WebUri;
        public UserUris Uris;

        public override string ToString()
        {
            return string.Format("User: {0}, {1}", Name, JsonConvert.SerializeObject(this));
        }
    }

    public class UserUris
    {
        public SmugMugUri BioImage;
        public SmugMugUri CoverImage;
        public SmugMugUri DuplicateImageSearch;
        public SmugMugUri Features;
        public SmugMugUri Folder;
        public SmugMugUri Node;
        public SmugMugUri SortUserFeaturedAlbums;
        public SmugMugUri UnlockUser;
        public SmugMugUri UrlPathLookup;
        public SmugMugUri UserAlbumTemplates;
        public SmugMugUri UserAlbums;
        public SmugMugUri UserContacts;
        public SmugMugUri UserDeletedAlbums;
        public SmugMugUri UserDeletedFolders;
        public SmugMugUri UserDeletedPages;
        public SmugMugUri UserFeaturedAlbums;
        public SmugMugUri UserGeoMedia;
        public SmugMugUri UserGrants;
        public SmugMugUri UserGuideSates;
        public SmugMugUri UserHideGuides;
        public SmugMugUri UserImageSearch;
        public SmugMugUri UserLatestQuickNews;
        public SmugMugUri UserPopularMedia;
        public SmugMugUri UserProfile;
        public SmugMugUri UserRecentImages;
        public SmugMugUri UserTasks;
        public SmugMugUri UserTopKeywords;
        public SmugMugUri UserUploadLimits;
        public SmugMugUri UserWatermarks;
    }

    public class UserGetResponse : SmugMugUri
    {
        public string DocUri;
        public User User;
    }

    public class UserPostResponse : SmugMugUri
    {
        public User User;

        public override string ToString()
        {
            return User.ToString();
        }
    }
}