using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class RegisterResponse : BaseResponse
    {        
        public string ReturnUrl { get; set; }       
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
