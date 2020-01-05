using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL
{
    public static class Const
    {
        public const string DisableAllTriggersCommentVote = "ALTER TABLE comment_votes DISABLE TRIGGER ALL";
        public const string DisableAllTriggersPostVote = "ALTER TABLE post_votes DISABLE TRIGGER ALL";
        public const string EnableAllTriggersCommentVote = "ALTER TABLE comment_votes ENABLE TRIGGER ALL";
        public const string EnableAllTriggersPostVote = "ALTER TABLE post_votes ENABLE TRIGGER ALL";        
    }
}
