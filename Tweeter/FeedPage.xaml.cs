using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Documents;

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
            List<Tweet2> lstTweets = await Loader.GetFeedAsync();

            lstFeed.ItemsSource = FormatEntities(lstTweets);
        }

        private List<Tweet2> FormatEntities(List<Tweet2> TweetList)
        {
            foreach (Tweet2 t in TweetList)
            {
                // should probably link @usernames and #hashtags somehow. also symbols.

                // if entities exist, iterate through them

                if (t.TweetEntities.Count() > 0 && t.Text != null)
                {
                    // this will be the index of the end of the previous entity and should be set in our loop
                    int i = 0;

                    // this will be our list of objects - either <run> or <hyperlink> XAML elements
                    List<object> things = new List<object>();
                    List<Inline> inlines = new List<Inline>();

                    foreach (TweetEntity tx in t.TweetEntities)
                    {
                        int start = tx.Indices[0];
                        int end = tx.Indices[1];
                        int len = end - start;

                        // if the end of the last entity is NOT index of the start of this entity
                        // pull out the text into a run block

                        if (start > i)
                        {
                            string temp = t.Tweet.Text.Substring(i, start - i);
                            Run runtemp = new Run
                            {
                                Text = temp
                            };

                            Inline iline = runtemp;
                            inlines.Add(iline);
                        }

                        Hyperlink link = new Hyperlink();
                        link.NavigateUri = new Uri("http://bing.com");

                        Run run = new Run();
                        run.Text = t.Tweet.Text.Substring(start, len);

                        link.Inlines.Add(run);

                        Inline iline2 = link;
                        inlines.Add(iline2);

                        // set i to equal the end of the entity, so we know where to start the next block of text
                        i = end;
                    }

                    if (i < t.Tweet.Text.Length)
                    {
                        // tweet ends with text, not an entity
                        string temp = t.Tweet.Text.Substring(i, t.Tweet.Text.Length - i);
                        Run runtemp = new Run
                        {
                            Text = temp
                        };

                        Inline iline = runtemp;
                        inlines.Add(iline);
                    }
                    t.Inlines = inlines;
                }
                else
                {
                    //tweet is just text, so we need a single inline
                    Run runtemp = new Run { Text = t.Text };

                    List<Inline> inlines = new List<Inline>();
                    Inline iline = runtemp;
                    inlines.Add(iline);
                    t.Inlines = inlines;
                }
            }

            return TweetList;
        }

        private void lstFeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // instantiate sender into a listview object
            ListView theSender = (ListView)sender;
            //Load a blade for the selected tweet
            //Tweet SelectedTweet = (sender as ListView).SelectedItem as Tweet;

            Tweet2 SelectedTweet = (theSender.SelectedItem as Tweet2).ShallowCopy();


            // Create a new list of Tweet objects
            List<Tweet2> theTweet = new List<Tweet2>();

            // Add selected tweet to list
            theTweet.Add(SelectedTweet);

            // TODO
            // - use search API to retreive tweet responses. this will be inexact at best, but whatevs.

            // create a new listview control
            ListView newList = new ListView();

            // populate listview with selected tweet
            newList.ItemsSource = FormatEntities(theTweet);

            // if the sender is lstFeed and Tweet01 already exists, replace Tweet01 content
            // if Tweet01 doesn't exist, create a new blade and populate it.

            if (theSender.Name == "lstFeed")
            {
                IList<BladeItem> bi = bladeView.ActiveBlades;

                try
                {
                    // this SHOULD return the BladeItem named Tweet01, but it isn't.
                    // I don't know why.
                    BladeItem theItem = bi.First(item => item.Name.Equals("Tweet01"));
                }
                catch
                {
                    // ain't nothing there
                }
            }

            BladeItem newBlade = new BladeItem();

            //newList.ItemTemplateSelector = (DataTemplateSelector)Resources["TweetTemplateSelector"];
            newList.ItemTemplate = (DataTemplate)App.Current.Resources["TweetTemplate"];

            newBlade.Content = newList;

            newBlade.Style = (Style)App.Current.Resources["BladeStyle"];

            newBlade.Name = "Tweet01";

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
