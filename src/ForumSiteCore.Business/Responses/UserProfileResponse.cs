using ForumSiteCore.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class UserProfileResponse : BaseResponse
    {
        public List<PostDto> Posts { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
