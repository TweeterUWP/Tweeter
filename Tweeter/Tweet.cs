using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Services.Twitter;

namespace Tweeter
{
    public class Tweet2
    {
        private bool _IsRetweet = false;
        private Tweet _BaseTweet;
        private Tweet _Tweet;
        private string _Text;

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

        public bool? IsRT = true;

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

        public string Text
        {
            get
            {
                _Text = _Tweet.Text;

                if (_Text != null)
                {
                    _Text = _Tweet.Text.Substring(_Tweet.DisplayTextRange[0], (_Tweet.DisplayTextRange[1] - _Tweet.DisplayTextRange[0]));

                    // html decode text, because Twitter escapes < and >.
                    _Text = WebUtility.HtmlDecode(_Text);
                }

                return _Text;
            }
        }
    }
}
