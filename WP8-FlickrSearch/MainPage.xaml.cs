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

namespace WP8_FlickrSearch
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string sSearch = "";
            if (NavigationContext.QueryString.TryGetValue("search", out sSearch))
            {
                try
                {
                    App.DataHelperInstance.LoadData(sSearch);
                    DataContext = App.DataHelperInstance;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oops! We couldn't load your image! " + ex.Message.ToString());
                }
            }
        }

        // Handle selection changed on LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as FlickrItem).Id, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
        }

    }
}