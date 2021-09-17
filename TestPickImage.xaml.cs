using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.Media.Abstractions;
using System.IO;
using System.Net.Cache;

namespace LoveCalculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPickImage : ContentPage
    {
        public CloudStorageAccount storage = CloudStorageAccount.Parse("CONNECTION_STRING");
       
        public TestPickImage()
        {
            InitializeComponent();
        }

        private async void pick_image_but_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("ERROR", "Pick Photo is NOT supported", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
            {
                return;
            }

            myImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                //file.Dispose();
                return stream;
                 
            });

            await UploadAzureToFile(file);
            
        }

        public async Task UploadAzureToFile(MediaFile file)
        {
             CloudBlobClient cloudBlobClient = storage.CreateCloudBlobClient();
             CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("your_blob_container_name");
            string filePath = file.Path;
            string fileName = Path.GetFileName(filePath);
            await cloudBlobContainer.CreateIfNotExistsAsync();

            await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            var blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            await UploadImage(blockBlob, filePath);
           if(blockBlob.ExistsAsync().Result)
              test.Text = blockBlob.Uri.ToString();
           
           
         


        }

        private static async Task UploadImage(CloudBlockBlob blob, string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                await blob.UploadFromStreamAsync(fileStream);
            }
          
         
        }


    }
}