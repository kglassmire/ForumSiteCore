using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class CreatePostVM
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public String Url { get; set; }
        public String ForumName { get; set; }
    }
}
