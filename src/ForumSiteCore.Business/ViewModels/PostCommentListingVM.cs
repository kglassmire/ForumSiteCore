using ForumSiteCore.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class PostCommentListingVM
    {
        public PostCommentListingVM()
        {

        }

        public PostCommentListingVM(PostDto post, List<CommentDto> comments, String commentListingType)
        {
            Post = post;
            Comments = comments;
            CommentListingType = commentListingType;
        }

        public PostDto Post { get; set; }
        public List<CommentDto> Comments { get; set; }
        public String CommentListingType { get; set; }
    }
}
