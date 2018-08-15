using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.Data.Json;
using Windows.Data.Html;
using HtmlAgilityPack;
using Windows.Web.Http;

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
                return dt.ToString("MMM dd");
            }
            else
            {
                // if tweet is less than 24 hours old, display time
                // follow hours/minutes/seconds format of website
                double h = Math.Round(diff.TotalHours);
                double m = Math.Round(diff.TotalMinutes);
                double s = Math.Round(diff.TotalSeconds);

                return h > 0 ? h + "h" : (m > 0 ? m + "m" : s + "s");
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

    public class DebugThis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
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

            return value as Tweet2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


}

namespace Tweeter.Utils { 

	public class Login
	{
		public async void TwitterLogin()
		{
			// set parameters
			string CallbackUri = "app://";

            // pull the API key and secret from API_Keys.resw
            ResourceLoader resources = ResourceLoader.GetForViewIndependentUse("API_Keys");
            string ConsumerKey = resources.GetString("key");
            string ConsumerSecret = resources.GetString("secret");

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
                        TwitterLogin();
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
        public async Task<List<Tweet2>> GetFeedAsync()
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
                
                // create a list of Tweet objects
                List<Tweet> TwTweets = await TwitterService.Instance.RequestAsync(TwConfig, 100);

                // instantiate a list of Tweet2 objects, which will allow adding additional metadata to the tweet object
                List<Tweet2> Tweets = new List<Tweet2>();

                // iterate through the list of Tweet objects and convert them to Tweet2 objects
                // then add the Tweet2 objects to the new list
                foreach (Tweet t in TwTweets)
                {
                    Tweet2 t2 = new Tweet2();
                    t2.BaseTweet = t;

                    Tweets.Add(t2);
                }

                return Tweets;
            }
        }

        public async Task<List<Tweet2>> GetTweetRepliesAsync(TwitterUser theUser, string theTweetId)
        {
            List<Tweet2> tweets = new List<Tweet2>();

            Uri requestUri = new Uri("https://twitter.com/i/" + theUser.ScreenName + "/conversation/" + theTweetId +
                "?include_available_features=1&include_entitites&reset_error_state=false&max_position=" + theTweetId);

            HttpClient httpClient = new HttpClient();

            
            //var responseBody = "";

            try
            {                
                HttpResponseMessage httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();

                // if the response is a 404, the tweet is gone or protected
                if (httpResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    // this is a json file
                    string result = await httpClient.GetStringAsync(requestUri);

                    // parse the result into a JsonObject
                    JsonObject theJson = JsonObject.Parse(result);

                    // this is unnecessary if I can figure out how to deserialize JsonObject into an object.
                    bool hasMore = theJson["has_more_items"].GetBoolean();
                    string rawText = theJson["items_html"].GetString();
                    string nextKey = theJson["min_position"].GetString();

                    // this might make it easier to traverse the stupid HTML Twitter returns.
                    // ...maybe.
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(rawText);
                }

            }
            catch (Exception ex)
            {

            }

            return tweets;
        }
    }

    public class TwitterJson
    {
        bool hasMore { get; set; }
        string rawText { get; set; }
        string nextKey { get; set; }
    }

    public static class InlineXaml
    {
        public static readonly DependencyProperty InlinesProperty = DependencyProperty.RegisterAttached("Inlines", typeof(ICollection<Inline>), typeof(InlineXaml), new PropertyMetadata(default(ICollection<Inline>), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBlock tb)) return;

            tb.Inlines.Clear();

            if (!(e.NewValue is ICollection<Inline> inlines)) return;

            foreach (var inline in inlines)
            {
                try
                {
                    tb.Inlines.Add(inline);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                    
            }
        }

        public static void SetInlines(DependencyObject element, ICollection<Inline> value)
        {
            element.SetValue(InlinesProperty, value);
        }

        public static ICollection<Inline> GetInlines(DependencyObject element)
        {
            return (ICollection<Inline>)element.GetValue(InlinesProperty);
        }
    }
}