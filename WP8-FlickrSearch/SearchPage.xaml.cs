using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

namespace WP8_FlickrSearch
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private void btnSearchAppBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?search=" + tbSearch.Text, UriKind.Relative));
        }

        private async void btnVoiceAppBar_Click(object sender, EventArgs e)
        {
            App.SpeechRecognizerWithUI.Settings.ExampleText = "Flower";
            App.SpeechRecognizerWithUI.Settings.ShowConfirmation = true;
            App.SpeechRecognizerWithUI.Settings.ListenText = "What picture are you looking for?";
            var result = await App.SpeechRecognizerWithUI.RecognizeWithUIAsync();

            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();

                if (result.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.High || result.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Medium)
                {
                    var sSearch = result.RecognitionResult.Text;
                    NavigationService.Navigate(new Uri("/MainPage.xaml?search=" + sSearch, UriKind.Relative));
                }
                else
                {
                    await synth.SpeakTextAsync("I am not too sure what you just said. Please try again!");
                }
            }
        }

    }
}