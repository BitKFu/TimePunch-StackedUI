using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.StackedUI.Model;

namespace TimePunch.StackedUI.Events
{
    public class UpdateBreadCrumbNavigation
    {
        public UpdateBreadCrumbNavigation(BreadCrumbNavigation[] breadCrumbs)
        {
            BreadCrumbs = breadCrumbs;
        }

        public BreadCrumbNavigation[] BreadCrumbs { get; }
    }
}
