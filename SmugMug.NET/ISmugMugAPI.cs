using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmugMug.NET
{
    public interface ISmugMugAPI
    {
        Task<Album> GetAlbum(string albumId);
        Task<List<Album>> GetAlbums(User user, int maxAlbumCount = int.MaxValue);
        Task<List<Album>> GetFeaturedAlbums(User user, int maxAlbumCount = int.MaxValue);

        Task<Album> CreateAlbum(string albumTitle, string userNickName, string folderPath, Dictionary<string, string> arguments = null);
        Task<Album> CreateAlbum(string albumTitle, User user, string folderPath, Dictionary<string, string> arguments = null);
        Task<Album> CreateAlbum(string albumTitle, Folder folder, Dictionary<string, string> arguments = null);

        Task<Album> UpdateAlbum(Album album, Dictionary<string, string> arguments);

        Task DeleteAlbum(Album album);

        Task<User> GetUser(string userNickName);
        Task<User> GetAuthenticatedUser();
        Task<User> GetSiteUser();
        
        Task<UserProfile> GetUserProfile(string userNickName);
        Task<UserProfile> GetUserProfile(User user);

        Task<UserProfile> UpdateUserProfile(UserProfile userProfile, Dictionary<string, string> updates);
        Task<UserProfile> UpdateUserProfile(User user, Dictionary<string, string> updates);

        Task<Image> GetImage(string imageKey);
        Task<Image> GetImage(ImageUpload imageUpload);
        String GetImageKey(ImageUpload imageUpload);
        Task<ImageUpload> UploadImage(string albumUri, string filePath);
        Task<ImageUpload> UploadImage(Node node, string filePath);
        Task<ImageUpload> UploadImage(Album album, string filePath);
        Task<Image> UpdateImage(Image image, Dictionary<string, string> arguments);
        Task DeleteImage(Image image);

        Task<AlbumImage> GetAlbumImage(Album album, string imageKey);
        Task<AlbumImagesWithSizes> GetAlbumImagesWithSizes(Album album, int maxAlbumImageCount = int.MaxValue);
        Task<List<AlbumImage>> GetAlbumImages(Album album, int maxAlbumImageCount = int.MaxValue);

        Task<Node> GetNode(string nodeId);
        Task<Node> GetRootNode(User user);
        Task<List<Node>> GetChildNodes(Node node, int maxNodeCount = int.MaxValue);

        String GetDefaultNodeID(User user);

        Task<Node> CreateNode(NodeType type, string nodeName, string folderNodeId, Dictionary<string, string> arguments = null);

        Task<Node> UpdateNode(Node node, Dictionary<string, string> updates);

        Task DeleteNode(Node node);

        Task<Folder> GetFolder(string userNickName, string folderPath);
        Task<Folder> GetFolder(User user, string folderPath);

        Task<Folder> CreateFolder(string folderName, string userNickName, string folderPath, Dictionary<string, string> arguments = null);
        Task<Folder> CreateFolder(string folderName, User user, string folderPath, Dictionary<string, string> arguments = null);
        Task<Folder> CreateFolder(string folderName, Folder folder, Dictionary<string, string> arguments = null);

        Task<Folder> UpdateFolder(Folder folder, Dictionary<string, string> arguments);

        Task DeleteFolder(Folder folder);   
    }
}
