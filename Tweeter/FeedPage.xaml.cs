using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Documents;
using Windows.Foundation;

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

            if (theSender.SelectedItem != null)
            {
                Tweet2 SelectedTweet = (theSender.SelectedItem as Tweet2).ShallowCopy();


                // Create a new list of Tweet objects
                List<Tweet2> theTweet = new List<Tweet2>();

                // Add selected tweet to list
                theTweet.Add(SelectedTweet);

                // TODO
                // - use search API to retreive tweet responses. this will be inexact at best, but whatevs.

                // create a new listview control
                ListView newList = new ListView();
                newList.Name = "lstFeed";
                newList.Margin = new Thickness{ Bottom=0, Top=7, Left=0, Right=0 };

                // populate listview with selected tweet
                newList.ItemsSource = FormatEntities(theTweet);

                // unload any existing blades spawned by this view

                UnloadBlade(bladeView, theSender.Parent as BladeItem);

                BladeItem newBlade = new BladeItem();

                newBlade.Name = "bldHome";
                newBlade.Width = 430;

                newList.ItemTemplate = (DataTemplate)App.Current.Resources["TweetTemplate"];
                newList.SelectionChanged += LoadBlade;

                newBlade.Content = newList;

                newBlade.Style = (Style)App.Current.Resources["BladeStyle"];

                bladeView.Items.Add(newBlade);

                // TODO: scroll bladeView to newly-created blade.
                if (bladeView.BladeMode == BladeMode.Fullscreen)
                {
                    // current window width
                    Rect windowBounds = Window.Current.Bounds;
                    int currentWidth = (int)windowBounds.Width;

                    // scroll to view
                    BringIntoViewOptions opts = new BringIntoViewOptions();
                    Rect target = new Rect { Height = windowBounds.Height, Width = windowBounds.Width, X = currentWidth, Y = 0 };
                    opts.TargetRect = target;

                    // this doesn't actually do anything
                    newBlade.StartBringIntoView(opts);
                }
            }
        }

        private void UnloadBlade(object sender, BladeItem e)
        {
            // sender is the BladeView, e is the BladeItem itself
            // get the current index
            // index before the current index should invoke ListView.SelectedItems.Clear()

            // active blades
            BladeItem[] activeBlades = bladeView.ActiveBlades.ToArray();

            // blade to be unloaded
            BladeItem currentBlade = e;

            // index of blade to be unloaded
            int currentIndex = Array.IndexOf(activeBlades, currentBlade);

            // index of previous blade in ActiveBlades
            int previousIndex = currentIndex - 1;

            if (previousIndex > -1)
            {
                // previous BladeItem
                BladeItem previousBlade = activeBlades[previousIndex];

                // ListView contained in previous BladeItem
                ListView previousList = previousBlade.Content as ListView;

                // Clear selected item in ListView
                // previousList.SelectedItems.Clear();
                previousList.SelectedItem = null;
            }

            // we also need to close all the blades after the current blade

            // everything after this index should be removed
            if (currentIndex + 1 >= activeBlades.Count() - 1)
            {
                for (int i = currentIndex + 1; i < activeBlades.Count(); i++)
                {
                    bladeView.Items.RemoveAt(i);
                }
            }
        }
    }
}
