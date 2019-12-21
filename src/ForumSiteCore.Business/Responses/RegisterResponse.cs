using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Responses
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string ReturnUrl { get; set; }       
        public string Message { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
