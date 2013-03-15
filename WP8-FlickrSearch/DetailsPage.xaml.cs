using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP8_FlickrSearch.Resources;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml.Linq;
using Windows.Phone.System.UserProfile;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;

namespace WP8_FlickrSearch
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                
                // Navigation via Touch
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    try
                    {
                        DataContext = App.DataHelperInstance.Items[index];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Oops! We couldn't load your image! " + ex.Message.ToString());
                    }
                }

                else
                {
                    string sId = "", sDescription = "";

                    // Navigation via File Association
                    if (NavigationContext.QueryString.ContainsKey("HandleFile"))
                    {
                        string fileToken = NavigationContext.QueryString["ID"];
                        var filename = SharedStorageAccessManager.GetSharedFileName(fileToken);

                        var file = await SharedStorageAccessManager.CopySharedFileAsync(
                           ApplicationData.Current.LocalFolder,
                                fileToken + ".flickr", NameCollisionOption.ReplaceExisting,
                                fileToken);

                        var content = await file.OpenAsync(FileAccessMode.Read);
                        DataReader dr = new DataReader(content);
                        await dr.LoadAsync((uint)content.Size);

                        //Get XML from file content
                        string xml = dr.ReadString((uint)content.Size);

                        //Load XML document
                        XDocument doc = XDocument.Parse(xml);
                        XName attName = XName.Get("ID");
                        XAttribute att = doc.Root.Attribute(attName);

                        //Get UniqueId from file
                        sId = att.Value;
                        sDescription = "Image loaded through File Mapper";
                    }

                    // Navigation via Uri Mapper
                    if (NavigationContext.QueryString.ContainsKey("HandleUri"))
                    {
                        sId = NavigationContext.QueryString["ID"];
                        sDescription = "Image loaded through URL Mapper";
                    }

                    // Execute navigation
                    try
                    {
                        FlickrItem fli = new FlickrItem();
                        fli.ImageSource = sId;
                        fli.Description = sDescription;
                        DataContext = fli;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Oops! We couldn't load your image! " + ex.Message.ToString());
                    }
                }
            }
        }

        private async void btnSetLock_Click(object sender, EventArgs e)
        {
            string fileName = "lockscreen.jpg";

            // Setup lockscreen.
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                //If you're not the provider, this call will prompt the user for permission.
                //Calling RequestAccessAsync from a background agent is not allowed.
                await LockScreenManager.RequestAccessAsync();
            }

            if (LockScreenManager.IsProvidedByCurrentApplication)
            {
                SaveImage(fileName);
                //Uri imageUri = new Uri("ms-appx://" + fileName, UriKind.RelativeOrAbsolute);
                Uri imageUri = new Uri("ms-appdata:///local/" + fileName, UriKind.RelativeOrAbsolute);
                LockScreen.SetImageUri(imageUri);
            }

        }

        public void SaveImage(string fileName)
        {
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (isolatedStorage.FileExists(fileName))
                isolatedStorage.DeleteFile(fileName);

            WriteableBitmap bitmapSource = new WriteableBitmap((BitmapSource)image.Source);
            MemoryStream stream = new MemoryStream();
            bitmapSource.SaveJpeg(stream, bitmapSource.PixelWidth, bitmapSource.PixelHeight, 0, 100);


            BitmapImage bitmapStream = new BitmapImage();
            bitmapStream.SetSource(stream);
            WriteableBitmap bitmapFile = new WriteableBitmap(bitmapStream);

            IsolatedStorageFileStream fileStream = isolatedStorage.CreateFile(fileName);
            bitmapFile.SaveJpeg(fileStream, bitmapFile.PixelWidth, bitmapFile.PixelHeight, 0, 100);
            fileStream.Close();
        }

    }
}