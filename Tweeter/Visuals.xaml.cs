using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweeter
{
    public partial class Visuals
    {
        public Visuals()
        {
            InitializeComponent();
        }
        public bool Not(bool? value)
        {
            return !(value == null);
        }
    }
}
