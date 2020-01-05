using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class PostDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
        public string Url { get; set; }
        public decimal HotScore { get; set; }
        public decimal ControversyScore { get; set; }
        public long TotalScore => Upvotes - Downvotes;
        public long Upvotes { get; set; }
        public long Downvotes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public long CommentsCount { get; set; }
        public long SavesCount { get; set; }
        public string UserName { get; set; }
        public VoteType UserVote { get; set; }
        public bool UserCreated { get; set; }
        public bool UserSaved { get; set; }
        public string ForumName { get; set; }
        public bool ShowForumName { get; set; }
    }
}
