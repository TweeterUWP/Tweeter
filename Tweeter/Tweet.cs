using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweeter
{
    public class Tweet2 : Microsoft.Toolkit.Uwp.Services.Twitter.Tweet
    {
        public bool IsRetweet
        {
            get
            {
                return true;
            }
        }
    }
}
