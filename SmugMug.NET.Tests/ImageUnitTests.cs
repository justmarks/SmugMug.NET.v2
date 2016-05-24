using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace SmugMug.NET.Tests
{
    [TestClass]
    public class ImageUnitTests
    {
        private ISmugMugAPI api;

        [TestInitialize()]
        public void InitializeAnonymous()
        {
            var mock = new Mock<ISmugMugAPI>();

            Image invalidImage = null;
            Image validImage = new Image() { FileName = "ValidFileName.jpg", Title = "Valid Image", Format="JPG" };
            Image unownedImage = new Image() { FileName = "UnownedFileName.png", Title = "Unowned Image", Format="PNG" };
            Image updatedImage = new Image() { FileName = "UnownedFileName.png", Title = "Unowned Image", Format = "PNG", Caption = "Updated caption" };

            ImageUpload invalidImageUpload = null;
            ImageUpload validImageUpload = new ImageUpload() { };

            mock.Setup(api => api.GetImage("ValidImage")).ReturnsAsync(validImage);
            mock.Setup(api => api.GetImage("InvalidImage")).ReturnsAsync(invalidImage);
            mock.Setup(api => api.GetImage("UnownedImage")).ReturnsAsync(unownedImage);

            mock.Setup(api => api.UploadImage("ValidAlbum", "ValidImage")).ReturnsAsync(validImageUpload);
            mock.Setup(api => api.UploadImage("ValidAlbum", "InvalidImage")).ReturnsAsync(invalidImageUpload);

            Node invalidNode = null;
            Node validNode = new Node() { Name = "ValidNode", NodeID = "ABCDE", HasChildren = true };
            mock.Setup(api => api.GetNode("ValidNode")).ReturnsAsync(validNode);
            mock.Setup(api => api.GetNode("InvalidNode")).ReturnsAsync(invalidNode);
            mock.Setup(api => api.UploadImage(validNode, "ValidImage")).ReturnsAsync(validImageUpload);
            mock.Setup(api => api.UploadImage(validNode, "InvalidImage")).ReturnsAsync(invalidImageUpload);
            mock.Setup(api => api.UploadImage(invalidNode, "ValidImage")).Throws<ArgumentNullException>();
            mock.Setup(api => api.UploadImage(invalidNode, "InvalidImage")).Throws<ArgumentNullException>();

            Album invalidAlbum = null;
            Album validAlbum = new Album() { Name = "ValidAlbum", ImageCount = 5 };
            mock.Setup(api => api.GetAlbum("ValidAlbum")).ReturnsAsync(validAlbum);
            mock.Setup(api => api.GetAlbum("InvalidAlbum")).ReturnsAsync(invalidAlbum);
            mock.Setup(api => api.UploadImage(validAlbum, "ValidImage")).ReturnsAsync(validImageUpload);
            mock.Setup(api => api.UploadImage(validAlbum, "InvalidImage")).ReturnsAsync(invalidImageUpload);
            mock.Setup(api => api.UploadImage(invalidAlbum, "ValidImage")).Throws<ArgumentNullException>();
            mock.Setup(api => api.UploadImage(invalidAlbum, "InvalidImage")).Throws<ArgumentNullException>();

            mock.Setup(api => api.GetImage(validImageUpload)).ReturnsAsync(validImage);
            mock.Setup(api => api.GetImage(invalidImageUpload)).Throws<ArgumentNullException>();

            mock.Setup(api => api.UpdateImage(validImage, It.Is<Dictionary<string, string>>(i => i.ContainsKey("Caption")))).ReturnsAsync(updatedImage);
            mock.Setup(api => api.UpdateImage(validImage, It.Is<Dictionary<string, string>>(i => i.ContainsKey("Invalid")))).Throws<HttpRequestException>();

            mock.Setup(api => api.UpdateImage((Image)null, It.IsAny<Dictionary<string, string>>())).Throws<ArgumentNullException>();
            mock.Setup(api => api.UpdateImage(validImage, null)).Throws<ArgumentNullException>();

            mock.Setup(api => api.DeleteImage(invalidImage)).Throws<ArgumentNullException>();
            mock.Setup(api => api.DeleteImage(unownedImage)).Throws<HttpRequestException>();

            api = mock.Object;
        }

        [TestMethod]
        public async Task GetImage()
        {
            Image image= await api.GetImage("ValidImage");
            Assert.IsNotNull(image);
            Assert.AreEqual("ValidFileName.jpg", image.FileName);
            Assert.AreEqual("JPG", image.Format);
            Assert.AreEqual("Valid Image", image.Title);
        }

        [TestMethod]
        public async Task GetImage_Invalid()
        {
            Image image = await api.GetImage("InvalidImage");
            Assert.IsNull(image);
        }

        [TestMethod]
        public async Task UploadImage()
        {
            ImageUpload imageUpload = await api.UploadImage("ValidAlbum", "ValidImage");
            Assert.IsNotNull(imageUpload);

            Image image = await api.GetImage(imageUpload);
            Assert.IsNotNull(image);
            Assert.AreEqual("ValidFileName.jpg", image.FileName);
            Assert.AreEqual("JPG", image.Format);
            Assert.AreEqual("Valid Image", image.Title);
        }

        [TestMethod]
        public async Task UploadImage_InvalidImage()
        {
            ImageUpload imageUpload = await api.UploadImage("ValidAlbum", "InvalidImage");
            Assert.IsNull(imageUpload);
        }

        [TestMethod]
        public async Task UploadImage_ByNode()
        {
            Node node = await api.GetNode("ValidNode");
            Assert.IsNotNull(node);

            ImageUpload imageUpload = await api.UploadImage(node, "ValidImage");
            Assert.IsNotNull(imageUpload);

            Image image = await api.GetImage(imageUpload);
            Assert.IsNotNull(image);
            Assert.AreEqual("ValidFileName.jpg", image.FileName);
            Assert.AreEqual("JPG", image.Format);
            Assert.AreEqual("Valid Image", image.Title);
        }

        [TestMethod]
        public async Task UploadImage_ByNode_InvalidImage()
        {
            Node node = await api.GetNode("ValidNode");
            Assert.IsNotNull(node);

            ImageUpload imageUpload = await api.UploadImage(node, "InvalidImage");
            Assert.IsNull(imageUpload);
        }

        [TestMethod]
        public async Task UploadImage_ByAlbum()
        {
            Album album = await api.GetAlbum("ValidAlbum");
            Assert.IsNotNull(album);

            ImageUpload imageUpload = await api.UploadImage(album, "ValidImage");
            Assert.IsNotNull(imageUpload);

            Image image = await api.GetImage(imageUpload);
            Assert.IsNotNull(image);
            Assert.AreEqual("ValidFileName.jpg", image.FileName);
            Assert.AreEqual("JPG", image.Format);
            Assert.AreEqual("Valid Image", image.Title);
        }

        [TestMethod]
        public async Task UploadImage_ByAlbum_InvalidImage()
        {
            Album album = await api.GetAlbum("ValidAlbum");
            Assert.IsNotNull(album);

            ImageUpload imageUpload = await api.UploadImage(album, "InvalidImage");
            Assert.IsNull(imageUpload);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UploadImage_ByAlbum_InvalidAlbum()
        {
            Album album = await api.GetAlbum("InvalidAlbum");
            Assert.IsNull(album);

            ImageUpload imageUpload = await api.UploadImage(album, "ValidImage");
        }

        [TestMethod]
        public async Task UpdateImage()
        {
            Image image = await api.GetImage("ValidImage");
            Assert.IsNotNull(image);

            Dictionary<string, string> updates = new Dictionary<string, string>() { { "Caption", "Updated caption" } };

            Image updatedImage = await api.UpdateImage(image, updates);
            Assert.IsNotNull(updatedImage);
            Assert.AreEqual(updates["Caption"], updatedImage.Caption);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateImage_InvalidAlbum()
        {
            Dictionary<string, string> updates = new Dictionary<string, string>() { { "Invalid", "Invalid" } };
            Image updatedImage = await api.UpdateImage((Image)null, updates);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateImage_InvalidAlbumNullArguments()
        {
            Image image = await api.GetImage("InvalidImage");
            Image updatedImage = await api.UpdateImage(image, null);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task UpdateImage_InvalidArguments()
        {
            Image image = await api.GetImage("ValidImage");
            Dictionary<string, string> updates = new Dictionary<string, string>() { { "Invalid", "Invalid" } };
            Image updatedImage = await api.UpdateImage(image, updates);
        }

        [TestMethod]
        public async Task DeleteImage()
        {
            Image image = await api.GetImage("ValidImage");
            await api.DeleteImage(image);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteImage_Invalid()
        {
            Image image = await api.GetImage("InvalidImage");
            await api.DeleteImage(image);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task DeleteImage_Unowned()
        {
            Image image = await api.GetImage("UnownedImage");
            await api.DeleteImage(image);
        }
    }
}
