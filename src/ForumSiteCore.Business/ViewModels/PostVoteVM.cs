using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class PostVoteVM : BaseVM
    {
        public Int64 PostId { get; set; }
        public VoteType VoteType { get; set; }        
    }
}
