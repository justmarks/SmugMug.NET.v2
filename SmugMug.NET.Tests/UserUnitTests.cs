using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http;

namespace SmugMug.NET.Tests
{
    [TestClass]
    public class UserUnitTests
    {
        private ISmugMugAPI api;

        [TestInitialize()]
        public void InitializeAnonymous()
        {
            var mock = new Mock<ISmugMugAPI>();

            SmugMugUri defaultNodeUri = new SmugMugUri() { Uri = "/api/v2/node/ABCDE" };

            User nullUser = null;
            User validUser = new User() { Name = "Valid User", NickName = "ValidUser", Uris = new UserUris { Node = defaultNodeUri } };
            User updatedUser = new User() { Name = "Valid User", NickName = "NickName" };

            mock.Setup(api => api.GetUser("ValidUser")).ReturnsAsync(validUser);
            mock.Setup(api => api.GetUser("InvalidUser")).ReturnsAsync(nullUser);

            mock.Setup(api => api.GetDefaultNodeID(validUser)).Returns("ABCDE");

            List<Album> validAlbums = new List<Album>() { new Album() { Name = "ValidAlbum", ImageCount = 5 }, new Album() { Name = "AnotherValidAlbum", ImageCount = 10 }, new Album() { Name = "ThirdValidAlbum", ImageCount = 15 } };
            List<Album> invalidAlbums = null;

            mock.Setup(api => api.GetAlbums(validUser, It.IsInRange<int>(0, int.MaxValue, Range.Inclusive))).ReturnsAsync(validAlbums);
            mock.Setup(api => api.GetAlbums(validUser, It.IsInRange<int>(int.MinValue, 0, Range.Inclusive))).ReturnsAsync(invalidAlbums);
            mock.Setup(api => api.GetAlbums(nullUser, It.IsInRange<int>(0, int.MaxValue, Range.Inclusive))).ReturnsAsync(invalidAlbums);
            mock.Setup(api => api.GetAlbums(nullUser, It.IsInRange<int>(int.MinValue, 0, Range.Inclusive))).ReturnsAsync(invalidAlbums);

            mock.Setup(api => api.GetFeaturedAlbums(validUser, It.IsInRange<int>(0, int.MaxValue, Range.Inclusive))).ReturnsAsync(validAlbums);
            mock.Setup(api => api.GetFeaturedAlbums(validUser, It.IsInRange<int>(int.MinValue, 0, Range.Inclusive))).ReturnsAsync(invalidAlbums);
            mock.Setup(api => api.GetFeaturedAlbums(nullUser, It.IsInRange<int>(0, int.MaxValue, Range.Inclusive))).ReturnsAsync(invalidAlbums);
            mock.Setup(api => api.GetFeaturedAlbums(nullUser, It.IsInRange<int>(int.MinValue, 0, Range.Inclusive))).ReturnsAsync(invalidAlbums);
            
            api = mock.Object;
        }

        [TestMethod]
        public async Task GetUser()
        {
            User user = await api.GetUser("ValidUser");
            Assert.IsNotNull(user);
            Assert.AreEqual("Valid User", user.Name);
            Assert.AreEqual("ValidUser", user.NickName);
            Assert.AreEqual("/api/v2/node/ABCDE", user.Uris.Node.Uri);
        }

        [TestMethod]
        public async Task GetUser_Invalid()
        {
            User user = await api.GetUser("InvalidUser");
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetUserAlbums()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetAlbums(user);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count > 0);
        }

        [TestMethod]
        public async Task GetUserAlbums_Invalid()
        {
            User user = await api.GetUser("InvalidUser");
            List<Album> albums = await api.GetAlbums(user);
            Assert.IsNull(albums);
        }

        [TestMethod]
        public async Task GetUserAlbums_withLimit()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetAlbums(user, 3);
            Assert.IsNotNull(albums);
            Assert.AreEqual(3, albums.Count);
        }

        [TestMethod]
        public async Task GetUserAlbums_withInvalidLimit()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetAlbums(user, -1);
            Assert.IsNull(albums);
        }

        [TestMethod]
        public async Task GetUserFeaturedAlbums()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetFeaturedAlbums(user);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count > 0);
        }

        [TestMethod]
        public async Task GetUserFeaturedAlbums_withLimit()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetFeaturedAlbums(user, 3);
            Assert.IsNotNull(albums);
            Assert.AreEqual(3, albums.Count());
        }

        [TestMethod]
        public async Task GetUserFeaturedAlbums_withInvalidLimit()
        {
            User user = await api.GetUser("ValidUser");
            List<Album> albums = await api.GetFeaturedAlbums(user, -1);
            Assert.IsNull(albums);
        }

        [TestMethod]
        public async Task GetDefaultNodeId()
        {
            User user = await api.GetUser("ValidUser");
            Assert.IsNotNull(user);

            string defaultNodeId = api.GetDefaultNodeID(user);
            Assert.AreEqual("ABCDE", defaultNodeId);
        }
    }
}
