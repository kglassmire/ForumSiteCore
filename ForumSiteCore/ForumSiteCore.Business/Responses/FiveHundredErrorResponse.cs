using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class FiveHundredErrorResponse : BaseApiResponse
    {
        public Exception Data { get; set; }
    }
}
