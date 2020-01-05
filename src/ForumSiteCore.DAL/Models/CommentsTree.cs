using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL.Models
{
    public class CommentTree
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public long? UserId { get; set; }
        public long? PostId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public long Upvotes { get; set; }
        public long Downvotes { get; set; }
        public long CommentsCount { get; set; }
        public long SavesCount { get; set; }
        public decimal BestScore { get; set; }
        public decimal ControversyScore { get; set; }
        public int Level { get; set; }
        public long[] Path { get; set; }

        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
