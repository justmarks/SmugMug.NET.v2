using Newtonsoft.Json;
using System;

namespace SmugMug.NET
{
    public class Folder : SmugMugObject
    {
        public DateTime DateAdded;
        public DateTime DateModified;
        public string Description;
        public string HighlightImageUri;
        public bool IsEmpty;
        public string Keywords;
        public string Name;
        public string NodeID;
        public string Password;
        public string PasswordHint;
        public PrivacyType Privacy;
        public SecurityType SecurityType;
        public SmugSearchable SmugSearchable;
        public SortDirection SortDirection;
        public SortMethod SortMethod;
        public string UrlName;
        public string UrlPath;
        public WorldSearchable WorldSearchable;
        public Uri WebUri;
        public FolderUris Uris;

        public override string ToString()
        {
            return string.Format("Folder: {0}, {1}", Name, JsonConvert.SerializeObject(this));
        }
    }

    public class FolderUris
    {
        public SmugMugUri AlbumList;
        public SmugMugUri FolderAlbums;
        public SmugMugUri FolderById;
        public SmugMugUri FolderHighlightImage;
        public SmugMugUri FolderList;
        public SmugMugUri FolderPages;
        public SmugMugUri Folders;
        public SmugMugUri HighlightImage;
        public SmugMugUri Node;
        public SmugMugUri ParentFolder;
        public SmugMugUri ParentFolders;
        public SmugMugUri Size;
        public SmugMugUri User;
    }

    public class FolderGetResponse : SmugMugUri
    {
        public string DocUri;
        public Folder Folder;
    }

    public class FolderPostResponse : SmugMugUri
    {
        public Folder Folder;

        public override string ToString()
        {
            return Folder.ToString();
        }
    }

    public class POSTParameter
    {
        public string ParameterName { get; set; }
        public string Problem { get; set; }
    }
}