using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public long PostId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public List<CommentDto> Children { get; set; }
        public long TotalScore => Upvotes - Downvotes;
        public long Upvotes { get; set; }
        public long Downvotes { get; set; }
        public int Level { get; set; }
        public long[] Path { get; set; }

        public long CommentsCount { get; set; }
        public long SavesCount { get; set; }
        public decimal BestScore { get; set; }
        public decimal ControversyScore { get; set; }
        public string UserName { get; set; }
        public VoteType UserVote { get; set; }
        public bool UserCreated { get; set; }
        public bool UserSaved { get; set; }


    }
}
