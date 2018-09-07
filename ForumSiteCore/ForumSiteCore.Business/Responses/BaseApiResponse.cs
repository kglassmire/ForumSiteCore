using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public abstract class BaseApiResponse
    {
        public String Status { get; set; }
        public String Message { get; set; }
    }
}
