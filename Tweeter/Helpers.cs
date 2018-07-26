using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Tweeter.Converters
{
	public class DateFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
				return null;

			DateTime dt = DateTime.Parse(value.ToString());
			DateTime now = DateTime.Now;

			TimeSpan diff = now - dt;

			if (diff.TotalHours > 24)
			{
				// if tweet is more than 24 hours old, display as MMM/dd
				return dt.ToString("MMM/dd");
			}
			else
			{
				// if tweet is less than 24 hours old, display time
				// follow hours/minutes/seconds format of website
				double h = Math.Round(diff.TotalHours);
				double m = Math.Round(diff.TotalMinutes);
				double s = Math.Round(diff.TotalSeconds);

				return h > 0 ? h + "h" : ( m > 0 ? m + "m" : s + "s");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class GetTwitterHandle : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
				return null;

			return "@" + value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

    public class GetUrlCard : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class GetTweet : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            
            // example of changing tweet object data mid-stream
            Tweet t = value as Tweet;

            // hashtags, usermentions, and URLs all exist in the entities member
            // indices point to the location of each inside the tweet's text string.

            // todo - overload Tweet class to add a new Tweet object containing EITHER the tweet or the retweet
            // then we can use one DataTemplate with one set of bindings that display different members
            
            // display_text_range provides the index of the first character in a tweet's text
            // this is nonzero for tweets that are replies to other tweets (NOT RT or QT)
            // For some reason twitter's DisplayTextRange doesn't always match the length of the tweet.
            // I don't know why!
            if(t.Text != null)
            {
                t.Text = t.Text.Substring(t.DisplayTextRange[0], (t.DisplayTextRange[1] - t.DisplayTextRange[0]));
            }

            // we need to do the same thing for retweets
            if (t.RetweetedStatus != null)
            {
                int start = t.RetweetedStatus.DisplayTextRange[0];
                int end = t.RetweetedStatus.DisplayTextRange[1];
                string txt = t.RetweetedStatus.Text;
                t.RetweetedStatus.Text = txt.Substring(start, (end - start));

                                
            }

            // html decode text, because Twitter escapes < and >.
            // this caused a JSON error and I don't know, because Newtonsoft.Json kind of sucks at error trapping.
            // maybe if text is null it breaks? hell if I know.
            if (t.Text != null)
                t.Text = WebUtility.HtmlDecode(t.Text);

            return t;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsRetweet : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Tweeter.Templates
{
	public class TweetTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TweetTemplate { get; set; }
		public DataTemplate RetweetTemplate { get; set; }
		public DataTemplate QuotedTemplate { get; set; }
		public DataTemplate VideoTemplate { get; set; }
		public DataTemplate ImageTemplate { get; set; }
		public DataTemplate LinkTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object obj)
		{
            // this is to facilitate using different DataTemplates for different tweet types.
            // Sauce: http://www.teixeira-soft.com/bluescreen/2016/03/23/c-how-to-dinamicaly-select-a-datatemplate-for-each-item-in-a-list/
            // if TweetData includes a Retweet member, use that!
            Tweet TweetData = obj as Tweet;

			// urls are in an array under TweetData.Entities.Urls

			// Tweet : TweetData.Text
			// Retweet : TweetData.RetweetedStatus
			// Quoted : TweetData.QuotedStatus
			// Video: TweetData.Entities.Media[0].MediaUrl (thumbnail) & .Url (tweet)
			// Image : TweetData.ExtendedEntities.Media[i].MediaUrl (direct link) & .Url (tweet)
			//		Image only: .URL should be identical to TweetData.Text
			//		Image with text: TweetData.Text will END with .URL
			//		Multiple images: first image is in TweetData.Entities.Media[0]
			// Link : TweetData.Entities.Urls
			//		This array may have more than one URL member

			if (TweetData.RetweetedStatus != null)
			{
                // this determines the datatemplate to return
                // return RetweetTemplate;
                return TweetTemplate;
			}
			else
			{
                return TweetTemplate;
			}
		}

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Tweet TweetData = item as Tweet;
            if (TweetData.RetweetedStatus != null)
            {
                // this determines the datatemplate to return
                // return RetweetTemplate;
                return TweetTemplate;
            }
            else
            {
                return TweetTemplate;
            }
        }
    }
}

namespace Tweeter.Utils { 

	public class Login
	{
		public async void TwitterLogin()
		{
			// set parameters
            // I'm going to have to do something about this so I'm not exposing the API key on GitHub...
			string CallbackUri = "app://";
			string ConsumerKey = "MhpRmlMvGTaVrs2cTPMPzGRKB";
			string ConsumerSecret = "jpbqyiLzv35x3gD6VpdjSEgrhu9p8T0q0yorjYlAf0nNPdOwB8";

			// create an instance of the Twitter service
			TwitterService.Instance.Initialize(ConsumerKey, ConsumerSecret, CallbackUri);

			// try to login and fail nicely if Twitter is being an asshole
			if (!await TwitterService.Instance.LoginAsync())
			{
                await new MessageDialog("Login failed.").ShowAsync();
				return;
			}
            else
            {
                // now we're logged in, so let's get down to business!
                (App.Current as App).IsLoggedIn = true;

                // if the token is expired or invalid, this breaks
                try
                {
                    (App.Current as App).TheUser = await TwitterService.Instance.GetUserAsync();
                }
                catch (TwitterException ex)
                {
                    if ((ex.Errors?.Errors?.Length > 0) && (ex.Errors.Errors[0].Code == 89))
                    {
                        await new MessageDialog("Access token expired! Logging out.").ShowAsync();
                        TwitterService.Instance.Logout();
                        return;
                    }
                    else
                    {
                        throw ex;
                    }
                }
                
            }
		}
	}

    public class Loaders
    {
        public async Task<List<Tweet>> GetFeedAsync()
        {
            if (!await TwitterService.Instance.LoginAsync())
            {
                // login failed. probably should do something here.
                return null;
            }
            else
            {
                try
                {
                    TwitterUser TwUser = await TwitterService.Instance.GetUserAsync();
                }
                catch (TwitterException ex)
                {
                    if ((ex.Errors?.Errors?.Length > 0) && (ex.Errors.Errors[0].Code == 89))
                    {
                        await new MessageDialog("Access token expired! Logging out.").ShowAsync();
                        TwitterService.Instance.Logout();
                        return null;
                    }
                    else
                    {
                        throw ex;
                    }
                }

                var TwConfig = new TwitterDataConfig
				{
					Query = "statuses/home_timeline.json",
					QueryType = TwitterQueryType.Home
                };

                //IEnumerable<Tweet> TwTweets = await TwitterService.Instance.GetUserTimeLineAsync(TwUser.ScreenName, 50);
                List<Tweet> TwTweets = await TwitterService.Instance.RequestAsync(TwConfig, 100);

                // todo: validate JSON coming from Twitter for a retweeted retweet or quote tweet
                return TwTweets;
            }
        }
    }
}
