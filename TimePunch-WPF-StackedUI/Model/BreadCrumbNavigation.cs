using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimePunch.StackedUI.Model
{
    public class BreadCrumbNavigation
    {
        public BreadCrumbNavigation(string frameKey, string frameTitle)
        {
            FrameKey = frameKey;
            FrameTitle = frameTitle;
        }

        public string FrameKey { get; }
        public string FrameTitle { get; }
    }
}
