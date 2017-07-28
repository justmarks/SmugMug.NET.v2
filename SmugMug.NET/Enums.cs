using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace SmugMug.NET
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountStatus
    {
        Active,
        PastDue,
        Suspended,
        Closed
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoginType
    {
        Anonymous,
        OAuth
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortMethod
    {
        Position,
        Caption,
        Filename,
        [EnumMember(Value = "Date Uploaded")]
        DateUploaded,
        [EnumMember(Value = "Date Modified")]
        DateModified,
        [EnumMember(Value = "Date Taken")]
        DateTaken,
        LastUpdated,
        SortIndex
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrivacyType
    {
        Public,
        Unlisted,
        Private
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SecurityType
    {
        None,
        Password,
        GrantAccess
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SmugSearchable
    
    {
        No,
        Local,
        LocalUser,
        Yes,
        [EnumMember(Value = "Inherit from user")]
        InheritfromUser
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Header
    {
        Custom,
        SmugMug
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorldSearchable
    {
        No,
        HomeOnly,
        Yes,
        [EnumMember(Value = "Inherit from user")]
        InheritfromUser
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Size
    {
        Medium,
        Large,
        XLarge,
        X2Large,
        X3Large,
        X4Large,
        X5Large,
        4K,
        5K,
        Original
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponseLevel
    {
        Default,
        Full,
        Password,
        Public,
        GrantAccess
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NodeType
    {
        Folder,
        Album,
        Page
    }
}
