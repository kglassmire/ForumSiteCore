using ForumSiteCore.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class UserProfileVM : BaseVM
    {
        public List<PostDto> Posts { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
