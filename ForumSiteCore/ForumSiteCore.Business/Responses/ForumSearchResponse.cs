using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class ForumSearchResponse : BaseApiResponse
    {
        public IList<String> Data { get; set; }
    }
}
