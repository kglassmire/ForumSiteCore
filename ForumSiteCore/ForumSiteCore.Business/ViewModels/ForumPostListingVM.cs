using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ForumSiteCore.Business.ViewModels
{    
    public class ForumPostListingVM
    {
        public ForumPostListingVM(ForumDto forum, IList<PostDto> posts, String forumListingType)
        {
            Posts = posts;
            Forum = forum;
            ForumListingType = forumListingType;
        }
        public ForumDto Forum { get; set; }
        public IList<PostDto> Posts { get; set; }
        public String ForumListingType { get; set; }
    }
}
