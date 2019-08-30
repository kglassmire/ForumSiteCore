using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class LoginVM
    {
        public string UserName { get; set; }        
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
