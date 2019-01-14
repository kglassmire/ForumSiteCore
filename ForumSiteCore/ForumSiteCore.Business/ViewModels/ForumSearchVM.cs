using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class ForumSearchVM : BaseVM
    {
        public IList<String> Results { get; set; }
    }
}
