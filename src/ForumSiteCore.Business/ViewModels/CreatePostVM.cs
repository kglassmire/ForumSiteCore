using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class CreatePostVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ForumName { get; set; }
    }
}
