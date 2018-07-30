using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Services.Twitter;

namespace Tweeter
{
    public class Tweet2
    {
        private bool _IsRetweet = false;
        private Tweet _BaseTweet;
        public Tweet BaseTweet
        {
            get
            {
                return _BaseTweet;
            }
            set
            {
                _BaseTweet = value;

                if (_BaseTweet.RetweetedStatus != null)
                {
                    _IsRetweet = true;
                }
            }
        }
        public bool IsRetweet
        {
            get
            {
                return _IsRetweet;
            }
        }

        public TwitterUser User
        {
            get { return _BaseTweet.User; }
        }

    }
}
