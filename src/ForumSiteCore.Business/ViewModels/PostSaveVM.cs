using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class PostSaveVM : BaseVM
    {
        public long PostId { get; set; }
        public bool Saved { get; set; }
    }
}
