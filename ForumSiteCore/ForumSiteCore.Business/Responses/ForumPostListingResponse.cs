using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class ForumPostListingResponse : BaseApiResponse
    {
        public ForumPostListingVM Data { get; set; }
    }
}
