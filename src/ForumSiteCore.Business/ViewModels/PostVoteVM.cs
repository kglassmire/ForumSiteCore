using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.ViewModels
{
    public class PostVoteVM : BaseVM
    {
        public long PostId { get; set; }
        public VoteType VoteType { get; set; }        
    }
}
