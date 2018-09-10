using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmugMug.NET
{
    public class GetResponseStub<T>
    {
        public T Response;
        public int Code;
        public string Message;
    }
    public class GetResponseWithExpansionStub<T,TE>
    {
        public T Response;
        public Dictionary<string, TE> Expansions;
        public int Code;
        public string Message;
    }
    public class PostResponseStub<T>
    {
        public T Response;
        public int Code;
        public string Message;

        public override string ToString()
        {
            return Response.ToString();
        }
    }

    public class DeleteResponseStub
    {
        public SmugMugUri Response;
        public int Code;
        public string Message;
    }
}
