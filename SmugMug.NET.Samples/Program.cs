

namespace SmugMug.NET.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiAnonymous = AuthenticationSample.AuthenticateUsingAnonymous();

            var apiOAuth = AuthenticationSample.AuthenticateUsingOAuth();
        }
    }
}
