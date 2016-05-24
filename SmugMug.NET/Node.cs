using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmugMug.NET
{
    public class Node : SmugMugObject
    {
        public bool AutoRename;
        public DateTime DateAdded;
        public DateTime DateModified;
        public string Description;
        public PrivacyType EffectivePrivacy;
        public SecurityType EffectiveSecurityType;
        public FormattedValues FormattedValues; //TODO: These are not being read properly
        public bool HasChildren;
        public bool HideOwner;
        public string HighlightImageUri;
        public bool IsRoot;
        public string[] Keywords;
        public string Name;
        public string NodeID;
        public string Password;
        public string PasswordHint;
        public PrivacyType Privacy;
        public SecurityType SecurityType;
        public SmugSearchable SmugSearchable;
        public SortDirection SortDirection;
        public int SortIndex;
        public SortMethod SortMethod;
        public NodeType Type;
        public string UrlName;
        public string UrlPath;
        public WorldSearchable WorldSearchable;
        public Uri WebUri;
        public NodeUris Uris;

        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}", Type, Name, JsonConvert.SerializeObject(this));
        }
    }

    public class FormattedValues
    {
        public class Name
        {
            public string html;
        }

        public class Description
        {
            public string html;
            public string text;
        }
    }

    public class NodeUris
    {
        public SmugMugUri Album;
        public SmugMugUri ChildNodes;
        public SmugMugUri FolderById;
        public SmugMugUri HighlightImage;
        public SmugMugUri MoveNodes;
        public SmugMugUri NodeGrants;
        public SmugMugUri ParentNode;
        public SmugMugUri ParentNodes;
        public SmugMugUri User;
    }

    public class NodeGetResponse : SmugMugUri
    {
        public string DocUri;
        public Node Node;
    }

    public class NodePostResponse :  SmugMugUri
    {
        public Node Node;

        public override string ToString()
        {
            return Node.ToString();
        }
    }

    public class NodePagesResponse : SmugMugPagesObject
    {
        public IEnumerable<Node> Node;
    }
}