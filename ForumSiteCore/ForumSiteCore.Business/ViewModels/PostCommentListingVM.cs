using ForumSiteCore.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class PostCommentListingVM
    {
        public PostCommentListingVM(PostDto post, IList<CommentDto> comments, String postListingType)
        {
            Post = post;
            Comments = comments;
            PostListingType = postListingType;
        }

        public PostDto Post { get; set; }
        public IList<CommentDto> Comments { get; set; }
        public String PostListingType { get; set; }
    }
}
