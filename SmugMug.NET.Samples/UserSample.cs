using SmugMug;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmugMug.NET.Samples
{
    class UserSample
    {
        public static async Task WorkingWithUsers(SmugMugAPI api)
        {
            //Get a given user
            User user = await api.GetUser("justmarks");
            Console.WriteLine(user.Name);

            //Get the user's profile
            UserProfile userProfile = await api.GetUserProfile(user);
            Console.WriteLine("{0} - Twitter:", userProfile.DisplayName, userProfile.Twitter);
        }
    }
}