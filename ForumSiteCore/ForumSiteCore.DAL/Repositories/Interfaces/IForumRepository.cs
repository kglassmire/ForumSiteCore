using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL.Repositories.Interfaces
{
    public interface IForumRepository
    {
        List<Post> New(Int64 forumId, Int32 limit = 0, DateTimeOffset? prevDate = null);
    }
}
