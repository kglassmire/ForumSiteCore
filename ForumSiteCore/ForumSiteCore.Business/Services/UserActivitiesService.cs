using CacheManager.Core;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForumSiteCore.Business.Services
{
    public class UserActivitiesService
    {
        private readonly ApplicationDbContext _context;
        private ICacheManager<object> _cache;
        private IUserAccessor<Int64> _userAccessor;
        
        public UserActivitiesService(ApplicationDbContext context, ICacheManager<object> cache, IUserAccessor<Int64> userAccessor)
        {
            _context = context;
            _cache = cache;
            _userAccessor = userAccessor;
        }

        public void ProcessPosts(IEnumerable<PostDto> posts)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userPostsVoted = UserPostsVoted(_userAccessor.UserId);
            var userPostsCreated = UserPostsCreated(_userAccessor.UserId);
            var userPostsSaved = UserPostsSaved(_userAccessor.UserId);

            foreach(var post in posts)
            {
                if (userPostsVoted.ContainsKey(post.Id))
                {
                    if (userPostsVoted[post.Id] == true)
                    {
                        post.UserVote = Enums.VotedType.Up;
                    }                        
                    else
                    {
                        post.UserVote = Enums.VotedType.Down;
                    }                        
                }

                if (userPostsCreated.Contains(post.Id))
                {
                    post.UserCreated = true;
                }

                if (userPostsSaved.Contains(post.Id))
                {
                    post.UserSaved = true;
                }
            }
        }

        public void ProcessComments()
        {
            throw new NotImplementedException();
        }

        public void ProcessForums(IEnumerable<ForumDto> forums)
        {

        }

        public Dictionary<Int64, Boolean> UserPostsVoted(Int64 userId)
        {
            return (Dictionary<Int64, Boolean>)_cache.GetOrAdd(String.Format("user-{0}-posts-voted", userId), valueFactory => UserPostsVotedInternal(userId));
        }

        public Dictionary<Int64, Boolean> UserCommentsVoted(Int64 userId)
        {
            return (Dictionary<Int64, Boolean>)_cache.GetOrAdd(String.Format("user-{0}-comments-voted", userId), valueFactory => UserCommentsVotedInternal(userId));
        }

        public HashSet<Int64> UserPostsCreated(Int64 userId)
        {
            return (HashSet<Int64>)_cache.GetOrAdd(String.Format("user-{0}-posts-created", userId), valueFactory => UserPostsCreatedInternal(userId));
        }

        public HashSet<Int64> UserCommentsCreated(Int64 userId)
        {
            return (HashSet<Int64>)_cache.GetOrAdd(String.Format("user-{0}-comments-created", userId), valueFactory => UserCommentsCreatedInternal(userId));
        }

        public HashSet<Int64> UserPostsSaved(Int64 userId)
        {
            return (HashSet<Int64>)_cache.GetOrAdd(String.Format("user-{0}-posts-saved", userId), valueFactory => UserPostsSavedInternal(userId));
        }

        public HashSet<Int64> UserCommentsSaved(Int64 userId)
        {
            return (HashSet<Int64>)_cache.GetOrAdd(String.Format("user-{0}-comments-saved", userId), valueFactory => UserCommentsSavedInternal(userId));
        }

        private Dictionary<Int64, Boolean> UserPostsVotedInternal(Int64 userId)
        {
            var query = _context.PostVotes
                .Where(x => x.UserId.Equals(userId));

            return query.Select(x => new { x.PostId, x.Direction }).ToDictionary(kvp => kvp.PostId, kvp => kvp.Direction);
        }

        private Dictionary<Int64, Boolean> UserCommentsVotedInternal(Int64 userId)
        {
            var query = _context.CommentVotes               
                .Where(x => x.UserId.Equals(userId)).ToList();

            return query.Select(x => new { x.CommentId, x.Direction }).ToDictionary(kvp => kvp.CommentId, kvp => kvp.Direction);
        }

        private HashSet<Int64> UserPostsCreatedInternal(Int64 userId)
        {
            return _context.Posts
                .Where(x => x.UserId.Equals(userId)).Select(x => x.Id).ToHashSet();
        }

        private HashSet<Int64> UserCommentsCreatedInternal(Int64 userId)
        {
            return _context.Comments
                .Where(x => x.UserId.Equals(userId)).Select(x => x.Id).ToHashSet();
        }

        private HashSet<Int64> UserPostsSavedInternal(Int64 userId)
        {
            return _context.PostSaves
                .Where(x => x.UserId.Equals(userId)).Select(x => x.PostId).ToHashSet();
        }

        private HashSet<Int64> UserCommentsSavedInternal(Int64 userId)
        {
            return _context.CommentSaves
                .Where(x => x.UserId.Equals(userId)).Select(x => x.CommentId).ToHashSet();
        }
    }
}