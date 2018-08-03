using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Microsoft.Toolkit.Uwp.Services.Twitter;

namespace Tweeter
{
    public class Tweet2
    {
        private bool _IsRetweet = false;
        private Tweet _BaseTweet;
        private Tweet _Tweet;
        private string _Text;

        private List<object> _temp;

        public object obj;

        /// <summary>
        /// The original tweet object
        /// </summary>
        public Tweet BaseTweet
        {
            get
            {
                return _BaseTweet;
            }
            set
            {
                _BaseTweet = value;
                _Tweet = _BaseTweet;

                if (_BaseTweet.RetweetedStatus != null)
                {
                    _IsRetweet = true;
                    _Tweet = _BaseTweet.RetweetedStatus;
                }
            }
        }
        
        /// <summary>
        /// Gets whether or not the tweet is a retweet; use with x:Load in XAML
        /// </summary>
        public bool IsRetweet
        {
            get
            {
                return _IsRetweet;
            }
        }

        /// <summary>
        /// Gets the user object of a tweet
        /// </summary>
        public TwitterUser User
        {
            get { return _BaseTweet.User; }
        }

        /// <summary>
        /// Gets the timestamp of the tweet
        /// </summary>
       public DateTime CreationDate
        {
            get { return _BaseTweet.CreationDate; }
        }

        /// <summary>
        /// Gets the tweet object - either the original tweet or the retweet
        /// </summary>
        public Tweet Tweet
        {
            get
            {

                return _Tweet;
            }
        }

        public TweetEntity[] TweetEntities
        {
            get
            {
                TwitterHashtag[] hashtags = _Tweet.Entities.Hashtags;
                TwitterUserMention[] usermentions = _Tweet.Entities.UserMentions;
                TwitterSymbol[] symbols = _Tweet.Entities.Symbols;
                
                List<TweetEntity> ents = new List<TweetEntity>();

                foreach (TwitterHashtag h in hashtags)
                {
                    TweetEntity e = new TweetEntity
                    {
                        Type = "#",
                        Value = h.Text,
                        Indices = h.Indices
                    };

                    ents.Add(e);
                }

                foreach (TwitterUserMention m in usermentions)
                {
                    TweetEntity e = new TweetEntity
                    {
                        Type = "@",
                        Value = m.ScreenName,
                        Indices = m.Indices
                    };

                    ents.Add(e);
                }

                foreach (TwitterSymbol s in symbols)
                {
                    TweetEntity e = new TweetEntity
                    {
                        Type = "$",
                        Value = s.Text,
                        Indices = s.Indices
                    };

                    ents.Add(e);
                }

                // turn the list into an array
                TweetEntity[] te = ents.ToArray();

                // sort the array by starting index
                // this might make it easier to do the needful with this data
                Array.Sort(te, delegate (TweetEntity a, TweetEntity b) { return a.Indices[0].CompareTo(b.Indices[0]); });

                return te;
            }
        }

        public string Text
        {
            get
            {
                _Text = _Tweet.Text;

                if (_Text != null)
                {
                    // display only the text in the DisplayTextRange indices

                    // this is barfing because somehow the Text attribute from the toolkit is converting HTML encoded entities back into regular characters
                    // this breaks the DisplayTextRange attribute, so we'll have to go fix that shit first.
                    _Text = _Tweet.Text.Substring(_Tweet.DisplayTextRange[0], (_Tweet.DisplayTextRange[1] - _Tweet.DisplayTextRange[0]));

                    // html decode text, because Twitter escapes < and >.
                    _Text = WebUtility.HtmlDecode(_Text);
                }

                _Tweet.Text = _Text;

                return _Text;
            }
        }
    }
}
