using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmugMug.NET
{
    public class OAuthCredentials
    {
        public string ConsumerKey;
        public string ConsumerSecret;
        public string AccessToken;
        public string AccessTokenSecret;

        public OAuthCredentials(string consumerKey)
        {
            ConsumerKey = consumerKey;
        }

        public OAuthCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }
    }
}
