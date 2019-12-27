using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class Post
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public long ForumId { get; set; }
        public long UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public long Upvotes { get; set; }
        public long Downvotes { get; set; }
        public decimal HotScore { get; set; }
        public decimal ControversyScore { get; set; }
        public long CommentsCount { get; set; }
        public long SavesCount { get; set; }

        public Forum Forum { get; set; }
        public ApplicationUser User { get; set; }
        public List<PostVote> PostVotes { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
