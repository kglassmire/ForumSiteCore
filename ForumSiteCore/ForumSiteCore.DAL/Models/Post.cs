using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class Post
    {
        public Int64 Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Url { get; set; }
        public Int64 ForumId { get; set; }
        public Int64 UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Int64 Upvotes { get; set; }
        public Int64 Downvotes { get; set; }
        public Decimal HotScore { get; set; }
        public Int64 CommentsCount { get; set; }
        public Int64 SavesCount { get; set; }

        public Forum Forum { get; set; }
        public ApplicationUser User { get; set; }
        public List<PostVote> PostVotes { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
