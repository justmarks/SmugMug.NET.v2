using SmugMug;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmugMug.NET.Samples
{
    class ImagesSample
    {
        public static async Task WorkingWithAlbumImages(SmugMugAPI api)
        {
            //Get a specific album node, "TrBCmb"
            Album album = await api.GetAlbum("TrBCmb");
            Console.WriteLine("Album '{0}' has {1} images", album.Name, album.ImageCount);

            //Get all the images in the given album
            var albumImages = await api.GetAlbumImages(album);
            
            //Get a specific image in the given album
            var albumImage = await api.GetAlbumImage(album, "ktwWSFX-0");
            Console.WriteLine("'{0}' ({1}) with keywords \"{2}\"", albumImage.Title, albumImage.FileName, albumImage.Keywords);
        }
    }
}