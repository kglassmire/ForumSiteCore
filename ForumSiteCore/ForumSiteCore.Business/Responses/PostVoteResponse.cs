using ForumSiteCore.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class PostVoteResponse : BaseApiResponse
    {
        public PostDto Data { get; set; }
    }
}
