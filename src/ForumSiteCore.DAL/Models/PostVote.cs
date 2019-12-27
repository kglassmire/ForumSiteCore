using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class PostVote
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public bool? Direction { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }

        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
