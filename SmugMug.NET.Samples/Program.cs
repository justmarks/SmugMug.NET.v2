

namespace SmugMug.NET.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiAnonymous = AuthenticationSample.AuthenticateUsingAnonymous();
            FoldersSample.WorkingWithFoldersAndAlbums(apiAnonymous).Wait();
            NodesSample.WorkingWithNodes(apiAnonymous).Wait();
            ImagesSample.WorkingWithAlbumImages(apiAnonymous).Wait();
            UserSample.WorkingWithUsers(apiAnonymous).Wait();

            var apiOAuth = AuthenticationSample.AuthenticateUsingOAuth();
        }
    }
}
