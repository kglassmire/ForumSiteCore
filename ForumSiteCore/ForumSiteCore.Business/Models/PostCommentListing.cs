using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class PostCommentListing
    {
        public PostCommentListing(Post post, IList<Comment> comments, String postListingType)
        {
            Post = post;
            Comments = comments;
            PostListingType = postListingType;
        }

        public Post Post { get; set; }
        public IList<Comment> Comments { get; set; }
        public String PostListingType { get; set; }
    }
}
