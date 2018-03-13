using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class PostDto
    {
        public Int64 Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Url { get; set; }
        public Decimal HotScore { get; set; }
        public Decimal ControversyScore { get; set; }
        public Int64 TopScore => Upvotes - Downvotes;
        public Int64 Upvotes { get; set; }
        public Int64 Downvotes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Int64 SavesCount { get; set; }
        public String UserName { get; set; }
        public string ForumName
        {
            get => forumName;
            set => forumName = value;
        }

        private String forumName;

    }
}
