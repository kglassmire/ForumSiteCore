using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class FiveHundredErrorResponse
    {
        public Exception Data { get; set; }
        public String Message { get; set; }
        public String Status { get; set; }
    }
}
