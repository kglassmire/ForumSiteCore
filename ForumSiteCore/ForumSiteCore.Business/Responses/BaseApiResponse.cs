using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public abstract class BaseApiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
