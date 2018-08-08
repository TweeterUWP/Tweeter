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

                            inlines.Add(runtemp as Inline);
                        }

                        Hyperlink link = new Hyperlink();
                        link.UnderlineStyle = UnderlineStyle.None;
                        link.NavigateUri = new Uri("http://bing.com");

                        Run run = new Run();
                        run.Text = t.Tweet.Text.Substring(start, len);

                        link.Inlines.Add(run);
                        
                        inlines.Add(link as Inline);

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
                        
                        inlines.Add(runtemp as Inline);
                    }
                    t.Inlines = inlines;
                }
                else
                {
                    //tweet is just text, so we need a single inline
                    Run runtemp = new Run { Text = t.Text };

                    List<Inline> inlines = new List<Inline>();
                    inlines.Add(runtemp);
                    t.Inlines = inlines;
                }
            }

            return TweetList;
        }

        private void LoadBlade (object sender, SelectionChangedEventArgs e)
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

            // find all blades after the sender's blade and remove them
            // then create a new blade and populate it

            // BladeItem > ListView

            // when a blade is CLOSED, the tweet that spawned it should no longer be selected in the ListView
            // pick the item by the Tag property - it will be the tweet ID

            IList<BladeItem> bladeList = bladeView.ActiveBlades;

            // FirstOrDefault doesn't work with lists; it only works with arrays
            // so we need to convert our list to an array
            BladeItem[] activeBlades = bladeList.ToArray();


            // this is the BladeItem that contains the selected ListViewItem
            BladeItem currentBlade = theSender.Parent as BladeItem;

            // the index of the current blade in the BladeView
            int currentIndex = Array.IndexOf(activeBlades, currentBlade);

            // everything after this index should be removed
            for (int i = currentIndex + 1; i < activeBlades.Count(); i++)
            {
                try
                {
                     bladeView.Items.RemoveAt(i);
                    //bladeView.Items.Remove(activeBlades[i]);
                }
                catch
                {
                    // do nothing
                }
            }

            BladeItem newBlade = new BladeItem();

            newList.ItemTemplate = (DataTemplate)App.Current.Resources["TweetTemplate"];
            newList.SelectionChanged += LoadBlade;

            newBlade.Content = newList;

            newBlade.Style = (Style)App.Current.Resources["BladeStyle"];

            bladeView.Items.Add(newBlade);
        }

        private void UnloadBlade(object sender, BladeItem e)
        {
            throw new NotImplementedException();
        }
    }
}
