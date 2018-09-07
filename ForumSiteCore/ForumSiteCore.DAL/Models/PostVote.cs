﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class PostVote
    {
        public Int64 Id { get; set; }
        public Int64 PostId { get; set; }
        public Int64 UserId { get; set; }
        public Boolean Direction { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Boolean Inactive { get; set; }

        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
