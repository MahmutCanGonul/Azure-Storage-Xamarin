# Azure-Storage-Xamarin
Send photo Azure Storage with Xamarin

💣CREATE AZURE STORAGE ACCOUNT IN AZURE PORTAL:

▶️1.Step: We need create a Azure Storage Account in Azure Portal:
![image](https://user-images.githubusercontent.com/75094927/133793545-9935b44b-cc8b-4af5-9041-17d273ed837e.png)

▶️2.Step: Create Container in Azure Storage Account:
![image](https://user-images.githubusercontent.com/75094927/133793705-9244edf4-3f0e-4a7a-8e80-b9d8f32350a7.png)

Now we create our Storage Container!


💣SETUP XAM.PLUGIN.MEDIA [This part about picking a photo on Xamarin]



▶️1.Step: Install Xam.Plugin.Media
![image](https://user-images.githubusercontent.com/75094927/133794925-c491ad1f-84ce-4e05-8fb0-c9483232ecb6.png)


▶️2.Step: In your AndroidManifest.xml file write this code in <Application>....</Application> Tags:

       <provider android:name="android.support.v4.content.FileProvider" 
				android:authorities="${applicationId}.fileprovider" 
				android:exported="false" 
				android:grantUriPermissions="true">
			<meta-data android:name="android.support.FILE_PROVIDER_PATHS" 
				android:resource="@xml/file_paths"></meta-data>
       </provider>

⁉️Note: If you error like this in your project: 
        
    Error Attribute provider#android.support.v4.content.FileProvider@authorities at AndroidManifest.xml:11:70-138 requires a placeholder substitution but no value for <project_package_name> is provided.     
  
  🟢You must delete ${} this symbols on android:authorities="${applicationId}.fileprovider" text.  



▶️3.Step: In your Resources File you need to add xml file and in the xml file create file_paths.xml.
  In your file_paths.xml you must enter this code: 
  
           <paths xmlns:android="http://schemas.android.com/apk/res/android">
    <external-files-path name="my_images" path="Pictures" />
    <external-files-path name="my_movies" path="Movies" />
          </paths>
  


▶️4.Step: MainActivity.cs scprit you must enter this code in OnCreate() method and download  Plugin.CurrentActivity package:

     CrossCurrentActivity.Current.Init(this.Application);

 Well done your tools are ready now you need to write some code:)
 
 
💣PICK IMAGE LOGIC
 
▶️1.Step: Firstly create Xamarin forms in your project and enter this code:
 
        <StackLayout>
            <Image x:Name="myImage"/>
            <Button x:Name="pick_image_but" Text="Pick" Clicked="pick_image_but_Clicked"/>
            <Label x:Name="test" TextColor="Black" FontSize="15"/>
        </StackLayout>


▶️2.Step: Then Open Script and typing logic in pick_image_but_Clicked() method:


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


<img src="https://user-images.githubusercontent.com/75094927/133798062-3a58236f-0694-421b-b0ad-b9d4d4d6fd5d.png" width="500" height="500">



💣UPLOAD AZURE STORAGE YOUR PHOTO


▶️1.Step: Copy Connection String Key on Azure Storage Account [Please copy key1, Bağlantı dizesi(Connection string)]: 
   
   
   <img src="https://user-images.githubusercontent.com/75094927/133803292-3a112b32-25fe-4df4-8062-3803b2e34c36.png" width="800" height="500">


▶️2.Step: Download WindowsAzure.Storage package on your project and  you must create CloudStorageAccount object for connection string key integration:
           
          public CloudStorageAccount storage = CloudStorageAccount.Parse("CONNECTION_STRING"); // Paste this string key on CONNECTION_STRING 

▶️3.Step: Write Task method like UploadAzureToFile() and enter this logic in this Task method:

      public async Task UploadAzureToFile(MediaFile file)
        {
             CloudBlobClient cloudBlobClient = storage.CreateCloudBlobClient();
             CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("Your_Container_Name");
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
              test.Text = blockBlob.Uri.ToString(); // blockBlob.Uri.ToString() => this is your photo url in website
        }
          


▶️3.Step: Write Task Method agein like UploadImage() and enter this logic in this task method:

         private static async Task UploadImage(CloudBlockBlob blob, string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                await blob.UploadFromStreamAsync(fileStream);
            }
          
        }


▶️Last Step: You must be call UploadAzureToFile() method in your pick_image_but_Clicked() method like this:

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

            await UploadAzureToFile(file); // WE ADD THAT OUR AZURE TASK METHOD!!!
            
        }
        
        
  🥇 Well done your project is ready!!!
 
  <img src="https://user-images.githubusercontent.com/75094927/133801037-4bd14431-4206-4d82-a2e8-f2c2ad7e9931.png" width="500" height="500">







