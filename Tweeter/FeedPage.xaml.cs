using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tweeter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedPage : Page
    {
        public FeedPage()
        {
            this.InitializeComponent();
            LoadFeed(); 
        }

        private async void LoadFeed()
        {
            Utils.Loaders Loader = new Utils.Loaders();
            List<Tweet> lstTweets = await Loader.GetFeedAsync();

            lstFeed.ItemsSource = lstTweets;
        }

        private void lstFeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // instantiate sender into a listview object
            ListView theSender = (ListView)sender;
            //Load a blade for the selected tweet
            //Tweet SelectedTweet = (sender as ListView).SelectedItem as Tweet;

            Tweet SelectedTweet = theSender.SelectedItem as Tweet;

            // Create a new list of Tweet objects
            List<Tweet> theTweet = new List<Tweet>();

            // Add selected tweet to list
            theTweet.Add(SelectedTweet);

            // create a new listview control
            ListView newList = new ListView();

            // populate listview with selected tweet
            newList.ItemsSource = theTweet;

            // if the sender is lstFeed and Tweet01 already exists, replace Tweet01 content
            // if Tweet01 doesn't exist, create a new blade and populate it.

            if (theSender.Name == "lstFeed")
            {

            }

            BladeItem newBlade = new BladeItem();

            newList.ItemTemplateSelector = (DataTemplateSelector)Resources["TweetTemplateSelector"];

            newBlade.Content = newList;

            newBlade.Style = (Style)App.Current.Resources["BladeStyle"];

            bladeView.Items.Add(newBlade);
        }

        private void lstFeed_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadTweet(sender);
        }

        private void LoadTweet(object sender)
        {
            // instantiate sender into a listview object
            ListView theSender = (ListView)sender;
            //Load a blade for the selected tweet
            //Tweet SelectedTweet = (sender as ListView).SelectedItem as Tweet;

            Tweet SelectedTweet = theSender.SelectedItem as Tweet;

            // Create a new list of Tweet objects
            List<Tweet> theTweet = new List<Tweet>();

            // Add selected tweet to list
            theTweet.Add(SelectedTweet);

            // create a new listview control
            ListView newList = new ListView();

            // populate listview with selected tweet
            newList.ItemsSource = theTweet;

            // if the sender is lstFeed and Tweet01 already exists, replace Tweet01 content
            // if Tweet01 doesn't exist, create a new blade and populate it.

            if (theSender.Name == "lstFeed")
            {

            }

            BladeItem newBlade = new BladeItem();

            newList.ItemTemplateSelector = (DataTemplateSelector)Resources["TweetTemplateSelector"];

            newBlade.Content = newList;

            newBlade.Style = (Style)App.Current.Resources["BladeStyle"];

            bladeView.Items.Add(newBlade);
        }
    }
}
