using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class ForumSaveVM : BaseVM
    {        
        public long ForumId { get; set; }
        public bool Saved { get; set; }
    }
}
