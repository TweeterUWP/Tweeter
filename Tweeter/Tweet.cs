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

        public string Text
        {
            get
            {
                _Text = _Tweet.Text;

                if (_Text != null)
                {
                    // display only the text in the DisplayTextRange indices
                    _Text = _Tweet.Text.Substring(_Tweet.DisplayTextRange[0], (_Tweet.DisplayTextRange[1] - _Tweet.DisplayTextRange[0]));

                    // html decode text, because Twitter escapes < and >.
                    _Text = WebUtility.HtmlDecode(_Text);

                    // should probably link @usernames and #hashtags somehow.

                    // if hashtags are present, this won't be null
                    if (_Tweet.Entities.Hashtags != null)
                    {
                        // iterate through hashtags
                        // each hashtag has an int[] pair of indices and a string containing the hashtag
                        // _Text should be updated to turn hashtags into hyperlinks

                        // text before first hyperlink needs to be wrapped in <run></run> tags
                        // then the hyperlink
                        // then the next block of text before the next hyperlink
                        // then the next hyperlink
                        // finish when at the end of the string

                        TwitterHashtag[] hashtags = _Tweet.Entities.Hashtags;

                        // get the Tweet text up to the first hashtag
                        List<string> strings = new List<string>();

                        string strStart = _Text.Substring(0, hashtags[0].Indices[0]);

                        strings.Add(strStart);

                        foreach (TwitterHashtag h in hashtags)
                        {
                            int start = h.Indices[0];
                            int end = h.Indices[1];
                            int length = end - start;

                            Hyperlink link = new Hyperlink();

                        }
                    }
                }

                _Tweet.Text = _Text;

                return _Text;
            }
        }

        public Entities[] Entities
        {
            get
            {
                TwitterHashtag[] hashtags = _Tweet.Entities.Hashtags;
                TwitterUserMention[] usermentions = _Tweet.Entities.UserMentions;

                List<Entities> ents = new List<Entities>();

                foreach (TwitterHashtag h in hashtags)
                {
                    Entities e = new Entities();
                    e.Type = "#";
                    e.Value = h.Text;
                    e.Indices = h.Indices;

                    ents.Add(e);
                }

                return ents.ToArray();
            }
        }
    }
}
