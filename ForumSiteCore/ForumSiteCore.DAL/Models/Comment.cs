using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class Comment
    {
        public Int64 Id { get; set; }
        public Int64? ParentId { get; set; }
        public Int64 UserId { get; set; }
        public Int64 PostId { get; set; }
        public String Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Int64 Upvotes { get; set; }
        public Int64 Downvotes { get; set; }
        public Int64 CommentsCount { get; set; }
        public Int64 SavesCount { get; set; }
        public Decimal BestScore { get; set; }
        public Decimal ControversyScore { get; set; }

        public List<CommentSave> CommentSaves { get; set; }
        public List<CommentVote> CommentVotes { get; set; }
        public Comment Parent { get; set; }
        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
