using ForumSiteCore.DAL.Models;
using ForumSiteCore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using Microsoft.EntityFrameworkCore.Query;

namespace ForumSiteCore.DAL.Repositories.Implementations
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;
        public ForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Post> New(Int64 forumId, Int32 limit = 0, DateTimeOffset? prevDate = null)
        {
            var predicate = PredicateBuilder.New<Post>();
            predicate = predicate.And(x => x.ForumId.Equals(forumId));
            if (prevDate != null)
                predicate = predicate.And(x => x.Created < prevDate);

            IQueryable<Post> query = _context.Posts
                .Include(x => x.User)
                .Where(predicate)
                .OrderByDescending(x => x.Created);

            if (limit != 0)
                query = query.Take(limit);

            return query.ToList();
        }
    }
}
