using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweeter
{
    public class TweetEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int[] Indices { get; set; }
    }
}
