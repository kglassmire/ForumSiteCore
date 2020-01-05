using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class FiveHundredErrorResponse : BaseResponse
    {
        public Exception Data { get; set; }
    }
}
