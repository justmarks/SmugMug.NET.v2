using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmugMug.NET
{
    public class SmugMugAPI : ISmugMugAPI
    {
        private InMemoryTokenManager smugmugTokenManager = new InMemoryTokenManager();
        private DesktopConsumer smugmugConsumer;
        private ServiceProviderDescription smugmugServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("http://api.smugmug.com/services/oauth/1.0a/getRequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://api.smugmug.com/services/oauth/1.0a/authorize", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("http://api.smugmug.com/services/oauth/1.0a/getAccessToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
            ProtocolVersion = ProtocolVersion.V10a
        };

        const string SMUGMUG_API_v2_BaseEndpoint = @"https://api.smugmug.com";
        const string SMUGMUG_API_v2_ApiEndpoint = @"https://api.smugmug.com/api/v2/";
        const string SMUGMUG_API_v2_UploadEndpoint = @"https://upload.smugmug.com/";

        public LoginType LoginType;

        public SmugMugAPI(LoginType loginType, OAuthCredentials creds)
        {
            LoginType = loginType;
            smugmugTokenManager.ConsumerKey = creds.ConsumerKey;
            smugmugTokenManager.ConsumerSecret = creds.ConsumerSecret;

            if (loginType == LoginType.OAuth)
            {
                smugmugTokenManager.AccessToken = creds.AccessToken;
                smugmugTokenManager.StoreNewAccessToken(creds.AccessToken, creds.AccessTokenSecret);
            }
        }

        #region REST Requests
        private async Task<T> GetRequest<T>(string endpoint)
        {
            return await GetRequest<T>(SMUGMUG_API_v2_BaseEndpoint, endpoint);
        }

        private async Task<T> GetRequest<T>(string baseAddress, string endpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (LoginType == LoginType.Anonymous)
                {
                    endpoint = string.Format("{0}{2}APIKey={1}", endpoint, smugmugTokenManager.ConsumerKey, endpoint.Contains('?') ? "&" : "?");
                }
                else if (LoginType == LoginType.OAuth)
                {
                    smugmugConsumer = new DesktopConsumer(smugmugServiceDescription, smugmugTokenManager);
                    HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest;

                    var resourceEndpoint = new MessageReceivingEndpoint(baseAddress + endpoint, resourceHttpMethod);
                    var httpRequest = smugmugConsumer.PrepareAuthorizedRequest(resourceEndpoint, smugmugTokenManager.AccessToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", httpRequest.Headers["Authorization"].Substring(6));
                }
                else
                {
                    throw new NotSupportedException(string.Format("LoginType {0} is unsupported", LoginType));
                }

                HttpResponseMessage httpResponse = client.GetAsync(endpoint).Result;
                System.Diagnostics.Trace.WriteLine(string.Format("GET {0}", httpResponse.RequestMessage.RequestUri));
                httpResponse.EnsureSuccessStatusCode();
                GetResponseStub<T> contentResponse = await httpResponse.Content.ReadAsAsync<GetResponseStub<T>>();
                System.Diagnostics.Trace.WriteLine(string.Format("---{0}:{1}", contentResponse.Code, contentResponse.Message));

                return contentResponse.Response;
            }
        }

        private async Task<T> PostRequest<T>(string endpoint, string jsonContent)
        {
            return await PostRequest<T>(SMUGMUG_API_v2_BaseEndpoint, endpoint, jsonContent);
        }

        private async Task<T> PostRequest<T>(string baseAddress, string endpoint, string jsonContent)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (LoginType == LoginType.Anonymous)
                {
                    endpoint = string.Format("{0}{2}APIKey={1}", endpoint, smugmugTokenManager.ConsumerKey, endpoint.Contains('?') ? "&" : "?");
                }
                else if (LoginType == LoginType.OAuth)
                {
                    smugmugConsumer = new DesktopConsumer(smugmugServiceDescription, smugmugTokenManager);
                    HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest;

                    var resourceEndpoint = new MessageReceivingEndpoint(baseAddress + endpoint, resourceHttpMethod);
                    var httpRequest = smugmugConsumer.PrepareAuthorizedRequest(resourceEndpoint, smugmugTokenManager.AccessToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", httpRequest.Headers["Authorization"].Substring(6));
                }
                else
                {
                    throw new NotSupportedException(string.Format("LoginType {0} is unsupported", LoginType));
                }

                HttpResponseMessage httpResponse = client.PostAsync(endpoint, new StringContent(jsonContent)).Result;
                System.Diagnostics.Trace.WriteLine(string.Format("POST {0}: {1}", httpResponse.RequestMessage.RequestUri, jsonContent));
                httpResponse.EnsureSuccessStatusCode();
                PostResponseStub<T> contentResponse = await httpResponse.Content.ReadAsAsync<PostResponseStub<T>>();
                System.Diagnostics.Trace.WriteLine(string.Format("---{0} {1}: {2}", contentResponse.Code, contentResponse.Message, contentResponse.Response));

                return contentResponse.Response;
            }
        }

        private async Task<ImageUpload> UploadImage(string albumUri, string fileName, byte[] image, CancellationToken cancellationToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(SMUGMUG_API_v2_UploadEndpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Smug-AlbumUri", albumUri);
                client.DefaultRequestHeaders.Add("X-Smug-FileName", fileName);
                client.DefaultRequestHeaders.Add("X-Smug-ResponseType", "JSON");
                client.DefaultRequestHeaders.Add("X-Smug-Version", "v2");

                if (LoginType == LoginType.OAuth)
                {
                    smugmugConsumer = new DesktopConsumer(smugmugServiceDescription, smugmugTokenManager);
                    HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest;

                    var resourceEndpoint = new MessageReceivingEndpoint(SMUGMUG_API_v2_UploadEndpoint, resourceHttpMethod);
                    var httpRequest = smugmugConsumer.PrepareAuthorizedRequest(resourceEndpoint, smugmugTokenManager.AccessToken);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", httpRequest.Headers["Authorization"].Substring(6));
                }
                else
                {
                    throw new NotSupportedException(string.Format("LoginType {0} is unsupported", LoginType));
                }

                var content = new StreamContent(new MemoryStream(image));

                HttpResponseMessage httpResponse = client.PostAsync(SMUGMUG_API_v2_UploadEndpoint, content, cancellationToken).Result;
                System.Diagnostics.Trace.WriteLine(string.Format("POST {0}", httpResponse.RequestMessage.RequestUri));
                httpResponse.EnsureSuccessStatusCode();
                ImagePostResponse contentResponse = await httpResponse.Content.ReadAsAsync<ImagePostResponse>();
                System.Diagnostics.Trace.WriteLine(string.Format("---{0} {1}: {2}", contentResponse.Stat, contentResponse.Method, contentResponse.Image));

                return contentResponse.Image;
            }
        }

        private async Task<T> PatchRequest<T>(string endpoint, string jsonContent)
        {
            return await PatchRequest<T>(SMUGMUG_API_v2_BaseEndpoint, endpoint, jsonContent);
        }

        private async Task<T> PatchRequest<T>(string baseAddress, string endpoint, string jsonContent)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (LoginType == LoginType.Anonymous)
                {
                    endpoint = string.Format("{0}{2}APIKey={1}", endpoint, smugmugTokenManager.ConsumerKey, endpoint.Contains('?') ? "&" : "?");
                }
                else if (LoginType == LoginType.OAuth)
                {
                    smugmugConsumer = new DesktopConsumer(smugmugServiceDescription, smugmugTokenManager);
                    HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.PatchRequest | HttpDeliveryMethods.AuthorizationHeaderRequest;

                    var resourceEndpoint = new MessageReceivingEndpoint(baseAddress + endpoint, resourceHttpMethod);
                    var httpRequest = smugmugConsumer.PrepareAuthorizedRequest(resourceEndpoint, smugmugTokenManager.AccessToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", httpRequest.Headers["Authorization"].Substring(6));
                }
                else
                {
                    throw new NotSupportedException(string.Format("LoginType {0} is unsupported", LoginType));
                }

                HttpResponseMessage httpResponse = client.PatchAsync(endpoint, new StringContent(jsonContent)).Result;
                System.Diagnostics.Trace.WriteLine(string.Format("PATCH {0}: {1}", httpResponse.RequestMessage.RequestUri, jsonContent));
                httpResponse.EnsureSuccessStatusCode();
                PostResponseStub<T> contentResponse = await httpResponse.Content.ReadAsAsync<PostResponseStub<T>>();
                System.Diagnostics.Trace.WriteLine(string.Format("---{0} {1}: {2}", contentResponse.Code, contentResponse.Message, contentResponse.Response));

                return contentResponse.Response;
            }
        }

        private async Task DeleteRequest(string endpoint)
        {
            await DeleteRequest(SMUGMUG_API_v2_BaseEndpoint, endpoint);
        }

        private async Task DeleteRequest(string baseAddress, string endpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (LoginType == LoginType.Anonymous)
                {
                    endpoint = string.Format("{0}{2}APIKey={1}", endpoint, smugmugTokenManager.ConsumerKey, endpoint.Contains('?') ? "&" : "?");
                }
                else if (LoginType == LoginType.OAuth)
                {
                    smugmugConsumer = new DesktopConsumer(smugmugServiceDescription, smugmugTokenManager);
                    HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.DeleteRequest | HttpDeliveryMethods.AuthorizationHeaderRequest;

                    var resourceEndpoint = new MessageReceivingEndpoint(baseAddress + endpoint, resourceHttpMethod);
                    var httpRequest = smugmugConsumer.PrepareAuthorizedRequest(resourceEndpoint, smugmugTokenManager.AccessToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", httpRequest.Headers["Authorization"].Substring(6));
                }
                else
                {
                    throw new NotSupportedException(string.Format("LoginType {0} is unsupported", LoginType));
                }

                HttpResponseMessage httpResponse = client.DeleteAsync(endpoint).Result;
                System.Diagnostics.Trace.WriteLine(string.Format("DELETE {0}", httpResponse.RequestMessage.RequestUri));
                httpResponse.EnsureSuccessStatusCode();
                DeleteResponseStub contentResponse = await httpResponse.Content.ReadAsAsync<DeleteResponseStub>();
                System.Diagnostics.Trace.WriteLine(string.Format("---{0}:{1}", httpResponse.StatusCode, httpResponse.ReasonPhrase));
            }
        }
        #endregion

        #region Helpers
        private string GenerateNodeJson(string name, Dictionary<string, string> arguments = null)
        {
            var jsonContent = new StringBuilder();
            jsonContent.Append("{");

            //Remove all non alpha-numeric characters from the urlName
            char[] arr = name.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c))).ToArray();
            arr[0] = new string(arr).ToUpper()[0];
            string urlName = new string(arr);
            urlName = urlName.Replace(" ", "-");

            jsonContent.Append(string.Format("\"Name\":\"{0}\", \"UrlName\":\"{0}\"", name));

            //For each argument, append to Folder
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    jsonContent.Append(string.Format(", \"{0}\":\"{1}\"", argument.Key, argument.Value));
                }
            }

            jsonContent.Append("}");

            return jsonContent.ToString();
        }

        private string GenerateJson(Dictionary<string, string> arguments)
        {
            JObject obj = JObject.FromObject(arguments);
            return obj.ToString();
        }
        #endregion

        #region User
        public async Task<User> GetUser(string userNickName)
        {
            string endpoint = string.Format("user/{0}", userNickName);
            UserGetResponse response = await GetRequest<UserGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.User;
        }

        public async Task<User> GetAuthenticatedUser()
        {
            string endpoint = "/api/v2!authuser";
            UserGetResponse response = await GetRequest<UserGetResponse>(endpoint);
            return response.User;
        }

        public async Task<User> GetSiteUser()
        {
            string endpoint = "/api/v2!siteuser";
            UserGetResponse response = await GetRequest<UserGetResponse>(endpoint);
            return response.User;
        }
        #endregion

        #region UserProfile
        public async Task<UserProfile> GetUserProfile(string userNickName)
        {
            string endpoint = string.Format("user/{0}!profile", userNickName);
            UserProfileGetResponse response = await GetRequest<UserProfileGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.UserProfile;
        }

        public async Task<UserProfile> GetUserProfile(User user)
        {
            if (user != null)
            {
                string endpoint = user.Uris.UserProfile.Uri;
                UserProfileGetResponse response = await GetRequest<UserProfileGetResponse>(endpoint);
                return response.UserProfile;
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        private async Task<UserProfile> UpdateUserProfile(string uri, Dictionary<string, string> updates)
        {
            if (updates != null && updates.Count > 0)
            {
                string content = GenerateJson(updates);
                string endpoint = uri;
                var response = await PatchRequest<UserProfilePostResponse>(endpoint, content);
                return response.UserProfile;
            }
            else
            {
                throw new ArgumentException(string.Format("Updates can not be null or empty", updates), "updates");
            }
        }

        public async Task<UserProfile> UpdateUserProfile(UserProfile userProfile, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update a user profile.");

            if (userProfile != null)
            {
                return await UpdateUserProfile(userProfile.Uri, updates);
            }
            else
            {
                throw new ArgumentException(string.Format("UserProfile {0} not found", userProfile), "userProfile");
            }
        }

        public async Task<UserProfile> UpdateUserProfile(User user, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update a user profile.");

            if (user != null)
            {
                return await UpdateUserProfile(user.Uris.UserProfile.Uri, updates);
            }
            else
            {
                throw new ArgumentException(string.Format("User {0} not found", user), "user");
            }
        }
        #endregion

        #region Node
        public async Task<Node> GetNode(string nodeId)
        {
            string endpoint = string.Format("node/{0}", nodeId);
            NodeGetResponse response = await GetRequest<NodeGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.Node;
        }

        public async Task<Node> GetRootNode(User user)
        {
            if (user != null)
            {
                string endpoint = user.Uris.Node.Uri;
                NodeGetResponse response = await GetRequest<NodeGetResponse>(endpoint);
                return response.Node;
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        public String GetDefaultNodeID(User user)
        {
            if (user != null)
            {
                string[] splitUri = user.Uris.Node.Uri.Split(new char[] { '/' });
                return splitUri[splitUri.Length - 1];
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        private async Task<List<Node>> GetPagedNodes(string initialUri, int maxCount)
        {
            List<Node> results = new List<Node>();
            string nextPage = initialUri;
            NodePagesResponse nodePagesResponse;
            do
            {
                nodePagesResponse = await GetRequest<NodePagesResponse>(nextPage);
                results.AddRange(nodePagesResponse.Node);

                if (nodePagesResponse.Pages != null)
                {
                    nextPage = nodePagesResponse.Pages.NextPage;
                }
                else
                {
                    break;
                }
                //TODO: Update nextPage to Ensure we don't return more than maxCount
            }
            while (!String.IsNullOrEmpty(nextPage) && (results.Count < maxCount));

            return results;
        }

        public async Task<List<Node>> GetChildNodes(Node node, int maxNodeCount = int.MaxValue)
        {
            if (node.HasChildren)
                return await GetPagedNodes(node.Uris.ChildNodes.Uri, maxNodeCount);
            else
                return null;
        }

        public async Task<Node> CreateNode(NodeType nodeType, string nodeName, string folderNodeId, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create a node.");

            Node parentNode = await GetNode(folderNodeId);
            if (parentNode != null)
            {
                if (nodeName.Length > 32)
                    throw new ArgumentException("Node names must be less than 32 characters long.", nodeName);

                if (arguments == null)
                {
                    arguments = new Dictionary<string, string>();
                }
                arguments.Add("Type", nodeType.ToString());

                string content = GenerateNodeJson(nodeName, arguments);
                string endpoint = parentNode.Uris.ChildNodes.Uri;
                var response = await PostRequest<NodePostResponse>(endpoint, content);
                return response.Node;
            }
            else
            {
                throw new ArgumentException(string.Format("Node {0} not found", folderNodeId), "folderNodeId");
            }
        }

        public async Task<Node> UpdateNode(Node node, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update a node.");

            if (node != null)
            {
                if (updates != null && updates.Count > 0)
                {
                    string content = GenerateJson(updates);
                    string endpoint = node.Uri;
                    var response = await PatchRequest<NodePostResponse>(endpoint, content);
                    return response.Node;
                }
                else
                {
                    throw new ArgumentException(string.Format("Updates can not be null or empty", updates), "updates");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Node {0} not found", node), "node");
            }
        }

        public async Task DeleteNode(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "You must provide a valid node to delete.");
            }

            await DeleteRequest(node.Uri);
        }

        #endregion

        #region Folder
        public async Task<Folder> GetFolder(string userNickName, string folderPath)
        {
            StringBuilder sb = new StringBuilder("folder/user/");
            sb.Append(userNickName);
            if (!String.IsNullOrEmpty(folderPath))
            {
                sb.Append("/").Append(folderPath);
            }
            string endpoint = sb.ToString();
            FolderGetResponse response = await GetRequest<FolderGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.Folder;
        }

        public async Task<Folder> GetFolder(User user, string folderPath)
        {
            if (user != null)
            {
                StringBuilder sb = new StringBuilder(user.Uris.Folder.Uri);
                if (!String.IsNullOrEmpty(folderPath))
                {
                    sb.Append("/").Append(folderPath);
                }
                string endpoint = sb.ToString();
                FolderGetResponse response = await GetRequest<FolderGetResponse>(endpoint);
                return response.Folder;
            }
            else
            {
                throw new ArgumentNullException(string.Format("User {0} not found", user), "user");
            }
        }

        public async Task<Folder> CreateFolder(string folderName, string userNickName, string folderPath, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create a folder.");

            Folder parentFolder = await GetFolder(userNickName, folderPath);
            if (parentFolder != null)
            {
                return await CreateFolder(folderName, parentFolder, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Folder {0} not found", folderPath), "folderPath");
            }
        }

        public async Task<Folder> CreateFolder(string folderName, User user, string folderPath, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create a folder.");

            Folder parentFolder = await GetFolder(user, folderPath);
            if (parentFolder != null)
            {
                return await CreateFolder(folderName, parentFolder, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Folder {0} not found", folderPath), "folderPath");
            }
        }

        public async Task<Folder> CreateFolder(string folderName, Folder folder, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create a folder.");

            if (folderName.Length > 32)
                throw new ArgumentException("Folder names must be less than 32 characters long.", folderName);

            string content = GenerateNodeJson(folderName, arguments);
            string endpoint = folder.Uris.Folders.Uri;
            var response = await PostRequest<FolderPostResponse>(endpoint, content);
            return response.Folder;
        }

        public async Task<Folder> UpdateFolder(Folder folder, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update a folder.");

            if (folder != null)
            {
                if (updates != null && updates.Count > 0)
                {
                    string content = GenerateJson(updates);
                    string endpoint = folder.Uri;
                    var response = await PatchRequest<FolderPostResponse>(endpoint, content);
                    return response.Folder;
                }
                else
                {
                    throw new ArgumentException(string.Format("Updates can not be null or empty", updates), "updates");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Folder {0} not found", folder), "folder");
            }
        }

        public async Task DeleteFolder(Folder folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder", "You must provide a valid folder to delete.");
            }

            await DeleteRequest(folder.Uri);
        }
        #endregion

        #region Album
        public async Task<Album> GetAlbum(string albumId)
        {
            string endpoint = string.Format("album/{0}", albumId);
            AlbumGetResponse response = await GetRequest<AlbumGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.Album;
        }

        private async Task<List<Album>> GetPagedAlbums(string initialUri, int maxCount)
        {
            List<Album> results = new List<Album>();
            string nextPage = initialUri;
            AlbumPagesResponse albumPagesResponse;
            do
            {
                albumPagesResponse = await GetRequest<AlbumPagesResponse>(nextPage);
                if (albumPagesResponse.Album != null)
                {
                    results.AddRange(albumPagesResponse.Album);

                    if (albumPagesResponse.Pages != null)
                    {
                        nextPage = albumPagesResponse.Pages.NextPage;
                    }
                    else
                    {
                        break;
                    }
                    //TODO: Update nextPage to Ensure we don't return more than maxCount
                }
                else
                    throw new NullReferenceException("The user has not specified anys albums.");

            }
            while (!String.IsNullOrEmpty(nextPage) && (results.Count < maxCount));

            return results;
        }

        public async Task<List<Album>> GetAlbums(User user, int maxAlbumCount = int.MaxValue)
        {
            return await GetPagedAlbums(user.Uris.UserAlbums.Uri, maxAlbumCount);
        }

        public async Task<List<Album>> GetFeaturedAlbums(User user, int maxAlbumCount = int.MaxValue)
        {
            return await GetPagedAlbums(user.Uris.UserFeaturedAlbums.Uri, maxAlbumCount);
        }

        private async Task<List<AlbumImage>> GetPagedAlbumImages(string initialUri, int maxCount)
        {
            List<AlbumImage> results = new List<AlbumImage>();
            string nextPage = initialUri;
            AlbumImagePagesResponse albumImagePagesResponse;
            do
            {
                albumImagePagesResponse = await GetRequest<AlbumImagePagesResponse>(nextPage);
                results.AddRange(albumImagePagesResponse.AlbumImage);

                if (albumImagePagesResponse.Pages != null)
                {
                    nextPage = albumImagePagesResponse.Pages.NextPage;
                }
                else
                {
                    break;
                }
                //TODO: Update nextPage to Ensure we don't return more than maxCount
            }
            while (!String.IsNullOrEmpty(nextPage) && (results.Count < maxCount));

            return results;
        }

        public async Task<List<AlbumImage>> GetAlbumImages(Album album, int maxAlbumImageCount = int.MaxValue)
        {
            return await GetPagedAlbumImages(album.Uris.AlbumImages.Uri, maxAlbumImageCount);
        }

        public async Task<Album> CreateAlbum(string albumTitle, string userNickName, string folderPath, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create a folder.");

            User user = await GetUser(userNickName);
            return await CreateAlbum(albumTitle, user, folderPath, arguments);
        }

        public async Task<Album> CreateAlbum(string albumTitle, User user, string folderPath, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create an album.");

            Folder parentFolder = await GetFolder(user, folderPath);
            if (parentFolder != null)
            {
                return await CreateAlbum(albumTitle, parentFolder, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Folder {0} not found", folderPath), "folderPath");
            }
        }

        public async Task<Album> CreateAlbum(string albumTitle, Folder folder, Dictionary<string, string> arguments = null)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to create an album.");

            if (albumTitle.Length > 32)
                throw new ArgumentException("Album titles must be less than 32 characters long.", albumTitle);

            string content = GenerateNodeJson(albumTitle, arguments);
            string endpoint = folder.Uris.FolderAlbums.Uri;
            var response = await PostRequest<AlbumPostResponse>(endpoint, content);
            return response.Album;
        }

        public async Task<Album> UpdateAlbum(Album album, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update an album.");

            if (album != null)
            {
                if (updates != null && updates.Count > 0)
                {
                    string content = GenerateJson(updates);
                    string endpoint = album.Uri;
                    var response = await PatchRequest<AlbumPostResponse>(endpoint, content);
                    return response.Album;
                }
                else
                {
                    throw new ArgumentException(string.Format("Updates can not be null or empty", updates), "updates");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Album {0} not found", album), "album");
            }
        }

        public async Task DeleteAlbum(Album album)
        {
            if (album == null)
            {
                throw new ArgumentNullException("album", "You must provide a valid album to delete.");
            }

            await DeleteRequest(album.Uri);
        }

        #endregion

        #region Image
        public async Task<Image> GetImage(string imageKey)
        {
            string endpoint = string.Format("image/{0}", imageKey);
            ImageGetResponse response = await GetRequest<ImageGetResponse>(SMUGMUG_API_v2_ApiEndpoint, endpoint);
            return response.Image;
        }

        public async Task<Image> GetImage(ImageUpload imageUpload)
        {
            var imageKey = GetImageKey(imageUpload);

            return await GetImage(imageKey);
        }

        public async Task<AlbumImage> GetAlbumImage(Album album, string imageKey)
        {
            if (album != null)
            {
                string endpoint = string.Format(album.Uri + "/image/{0}", imageKey);
                AlbumImageResponse response = await GetRequest<AlbumImageResponse>(endpoint);
                return response.AlbumImage;
            }
            else
            {
                throw new ArgumentNullException("album");
            }
        }

        public String GetImageKey(ImageUpload imageUpload)
        {
            if (imageUpload != null)
            {
                string[] splitUri = imageUpload.ImageUri.Split(new char[] { '/' });
                return splitUri[splitUri.Length - 1];
            }
            else
            {
                throw new ArgumentNullException("imageUpload");
            }
        }

        public async Task<ImageUpload> UploadImage(string albumUri, string filePath)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to upload an image.");

            if (File.Exists(filePath))
            {
                FileInfo fi = new FileInfo(filePath);
                byte[] fileContents = File.ReadAllBytes(fi.FullName);

                ImageUpload response = await UploadImage(albumUri, fi.Name, fileContents, CancellationToken.None);
                return response;
            }
            else
            {
                throw new FileNotFoundException("Cannot find the file to upload", filePath);
            }
        }

        public async Task<ImageUpload> UploadImage(Node node, string filePath)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to upload an image.");

            if (node.Type != NodeType.Album)
                throw new ArgumentException("Images can only be uploaded to album nodes.");

            return await UploadImage(node.Uris.Album.Uri, filePath);
        }

        public async Task<ImageUpload> UploadImage(Album album, string filePath)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to upload an image.");

            return await UploadImage(album.Uri, filePath);
        }

        public async Task<Image> UpdateImage(Image image, Dictionary<string, string> updates)
        {
            if (LoginType == LoginType.Anonymous)
                throw new UnauthorizedAccessException("You must be logged in using OAuth to update an image.");

            if (image != null)
            {
                if (updates != null && updates.Count > 0)
                {
                    string content = GenerateJson(updates);
                    string endpoint = image.Uri;
                    var response = await PatchRequest<ImagePatchResponse>(endpoint, content);
                    return response.Image;
                }
                else
                {
                    throw new ArgumentException(string.Format("Updates can not be null or empty", updates), "updates");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Image {0} not found", image), "image");
            }
        }

        public async Task DeleteImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image", "You must provide a valid image to delete.");
            }

            await DeleteRequest(image.Uri);
        }
        #endregion        
    }
}
 