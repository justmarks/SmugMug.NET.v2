using Newtonsoft.Json;

namespace SmugMug.NET
{
    public class UserProfile : SmugMugObject
    {
        public string BioText;
        public string Blogger;
        public string Custom;
        public string DisplayName;
        public string Facebook;
        public string FirstName;
        public string Flickr;
        public string GooglePlus;
        public string Instagram;
        public string LastName;
        public string LinkedIn;
        public string Pinterest;
        public string Tumblr;
        public string Twitter;
        public string Vimeo;
        public string Wordpress;
        public string YouTube;
        public UserProfileUris Uris;

        public override string ToString()
        {
            return string.Format("UserProfile: {0}, {1}", DisplayName, JsonConvert.SerializeObject(this));
        }
    }

    public class UserProfileUris
    {
        public SmugMugUri BioImage;
        public SmugMugUri CoverImage;
        public SmugMugUri User;
    }

    public class UserProfileGetResponse : SmugMugUri
    {
        public string DocUri;
        public UserProfile UserProfile;
    }

    public class UserProfilePostResponse : SmugMugUri
    {
        public UserProfile UserProfile;

        public override string ToString()
        {
            return UserProfile.ToString();
        }
    }
}