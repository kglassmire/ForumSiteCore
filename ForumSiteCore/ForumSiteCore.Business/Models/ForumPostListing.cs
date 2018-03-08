using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{    
    public class ForumPostListing
    {
        public ForumPostListing(Forum forum, IList<Post> posts, String forumListingType)
        {
            Forum = forum;
            Posts = posts;
            ForumListingType = forumListingType;
        }
        public Forum Forum { get; set; }
        public IList<Post> Posts { get; set; }
        public String ForumListingType { get; set; }
    }
}
