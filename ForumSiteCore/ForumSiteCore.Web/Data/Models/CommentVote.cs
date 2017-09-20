﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.Web.Data.Models
{
    public class CommentVote
    {
        public Int64 Id { get; set; }
        public Int64 CommentId { get; set; }
        public Int64 UserId { get; set; }
        public Boolean Direction { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }

        public Comment Comment { get; set; }
        public ApplicationUser User { get; set; }
    }
}
