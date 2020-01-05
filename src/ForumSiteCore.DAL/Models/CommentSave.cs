using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class CommentSave
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CommentId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public bool Saved { get; set; }
        public long PostId { get; set; }

        public Post Post { get; set; }
        public Comment Comment { get; set; }
        public ApplicationUser User { get; set; }
    }
}
