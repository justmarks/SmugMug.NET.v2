using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmugMug.NET
{
    public class SmugMugObject
    {
        public string Uri;
        public string UriDescription;
        public ResponseLevel ResponseLevel;
    }

    public class SmugMugUri : SmugMugObject
    {
        public string EndpointType;
        public string Locator; //This is the data type of the object or objects in the response
        public string LocatorType; //This tells you how many objects are in the response (Object means you get one object; Objects means an array of objects)
    }

    public class Pages
    {
        public int Count;
        public string FirstPage;
        public string LastPage;
        public string NextPage;
        public int RequestedCount;
        public int Start;
        public int Total;
    }

    public class SmugMugPagesObject : SmugMugUri
    {
        public Pages Pages;
    }
}
