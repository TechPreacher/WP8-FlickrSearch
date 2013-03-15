using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WP8_FlickrSearch
{
    public class DataHelper : INotifyPropertyChanged
    {
        public string sFlickrAPIKey = ""; // To do: ENTER YOUR FLICKR API KEY HERE!
        public ObservableCollection<FlickrItem> Items { get; private set; }
        public bool IsDataLoaded { get; private set; }

        public DataHelper()
        {
            this.Items = new ObservableCollection<FlickrItem>();
        }

        public void LoadData(string query)
        {
            this.Items.Clear();
            WebClient flickr = new WebClient();
            flickr.DownloadStringCompleted += new DownloadStringCompletedEventHandler(flickr_DownloadStringCompleted);
            flickr.DownloadStringAsync(new Uri("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key="
                          + sFlickrAPIKey + "&page=1&per_page=30&text=" + query));
        }

        void flickr_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            
            XElement xmlPhotos = XElement.Parse(e.Result);
            int iId = 0;

            IEnumerable<FlickrItem> list;
            list = from photo in xmlPhotos.Descendants("photo")
                   select new FlickrItem()
                   {
                       Id = iId++.ToString(),
                       ImageSource = "http://farm" + photo.Attribute("farm").Value + ".static.flickr.com/" + photo.Attribute("server").Value + "/" + photo.Attribute("id").Value + "_" + photo.Attribute("secret").Value + "_m.jpg",
                       Description = photo.Attribute("title").Value,
                   };

            foreach (FlickrItem p in list)
                this.Items.Add(p);

            this.IsDataLoaded = true;
        }

        public void PersistData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fs = isf.CreateFile("FlickrItems.dat"))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<FlickrItem>));
                    ser.Serialize(fs, this.Items);
                }
            }
        }

        public void ResumeData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists("FlickrItems.dat"))
                {
                    using (IsolatedStorageFileStream fs = isf.OpenFile("FlickrItems.dat", System.IO.FileMode.Open))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<FlickrItem>));
                        object list = ser.Deserialize(fs);

                        if (null != list && list is ObservableCollection<FlickrItem>)
                            this.Items = list as ObservableCollection<FlickrItem>;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
