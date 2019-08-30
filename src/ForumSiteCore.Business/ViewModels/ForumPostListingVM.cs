using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ForumSiteCore.Business.ViewModels
{    
    public class ForumPostListingVM : BaseVM
    {
        public ForumDto Forum { get; set; }
        public List<PostDto> Posts { get; set; }
        public String ForumListingType { get; set; }        
    }
}
