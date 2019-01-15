using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class CommentDto
    {
        public Int64 Id { get; set; }
        public Int64? ParentId { get; set; }
        public Int64 PostId { get; set; }
        public String Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }

        //public Int64 TotalScore => Upvotes - Downvotes;
        //public Int64 Upvotes { get; set; }
        //public Int64 Downvotes { get; set; }
        public Int32 Level { get; set; }
        public Int64[] Path { get; set; }

        public Int64 CommentsCount { get; set; }
        public Int64 SavesCount { get; set; }
        public Decimal BestScore { get; set; }
        public Decimal ControversyScore { get; set; }
        public String UserName { get; set; }
        public VotedType UserVote { get; set; }
        public Boolean UserCreated { get; set; }
        public Boolean UserSaved { get; set; }


    }
}
