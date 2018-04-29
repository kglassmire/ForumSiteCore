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
        private const string UserCommentsCreatedCacheKeyTemplate = "user-{0}-comments-created";
        private const string UserCommentsSavedCacheKeyTemplate = "user-{0}-comments-saved";
        private const string UserCommentsVotedCacheKeyTemplate = "user-{0}-comments-voted";
        private const string UserPostsCreatedCacheKeyTemplate = "user-{0}-posts-created";
        private const string UserPostsSavedCacheKeyTemplate = "user-{0}-posts-saved";
        private const string UserPostsVotedCacheKeyTemplate = "user-{0}-posts-voted";

        private ApplicationDbContext _context;
        private ICacheManager<object> _cache;
        private IUserAccessor<Int64> _userAccessor;

        public UserActivitiesService(ApplicationDbContext context, ICacheManager<object> cache, IUserAccessor<Int64> userAccessor)
        {
            _context = context;
            _cache = cache;
            _userAccessor = userAccessor;
        }

        public HashSet<Int64> UserCommentsCreated
        {
            get => (HashSet<Int64>)_cache.GetOrAdd(UserCommentsCreatedCacheKey, valueFactory => UserCommentsCreatedInternal(_userAccessor.UserId));
            set => _cache.Put(UserCommentsCreatedCacheKey, value);
        }

        public HashSet<Int64> UserCommentsSaved
        {
            get => (HashSet<Int64>)_cache.GetOrAdd(String.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId), valueFactory => UserCommentsSavedInternal(_userAccessor.UserId));
            set => _cache.Put(UserCommentsSavedCacheKey, value);
        }

        public Dictionary<Int64, Boolean> UserCommentsVoted
        {
            get => (Dictionary<Int64, Boolean>)_cache.GetOrAdd(UserCommentsVotedCacheKey, valueFactory => UserCommentsVotedInternal(_userAccessor.UserId));
            set => _cache.Put(UserCommentsVotedCacheKey, value);
        }

        public HashSet<Int64> UserPostsCreated
        {
            get => (HashSet<Int64>)_cache.GetOrAdd(UserPostsCreatedCacheKey, valueFactory => UserPostsCreatedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsCreatedCacheKey, value);
        }

        public HashSet<Int64> UserPostsSaved
        {
            get => (HashSet<Int64>)_cache.GetOrAdd(UserPostsSavedCacheKey, valueFactory => UserPostsSavedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsSavedCacheKey, value);
        }

        public Dictionary<Int64, Boolean> UserPostsVoted
        {
            get => (Dictionary<Int64, Boolean>)_cache.GetOrAdd(UserPostsVotedCacheKey, valueFactory => UserPostsVotedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsVotedCacheKey, value);
        }

        private string UserCommentsCreatedCacheKey => String.Format(UserCommentsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsSavedCacheKey => String.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsVotedCacheKey => String.Format(UserCommentsVotedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsCreatedCacheKey => String.Format(UserPostsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsSavedCacheKey => String.Format(UserPostsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsVotedCacheKey => String.Format(UserPostsVotedCacheKeyTemplate, _userAccessor.UserId);

        public void ProcessComments()
        {
            throw new NotImplementedException();
        }

        public void ProcessForums(IEnumerable<ForumDto> forums)
        {

        }

        public void ProcessPosts(IEnumerable<PostDto> posts)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userPostsVoted = UserPostsVoted;
            var userPostsSaved = UserPostsSaved;
            var userPostsCreated = UserPostsCreated;

            foreach (var post in posts)
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
        private HashSet<Int64> UserCommentsCreatedInternal(Int64 userId)
        {
            return _context.Comments
                .Where(x => x.UserId.Equals(userId)).Select(x => x.Id).ToHashSet();
        }

        private HashSet<Int64> UserCommentsSavedInternal(Int64 userId)
        {
            return _context.CommentSaves
                .Where(x => x.UserId.Equals(userId)).Select(x => x.CommentId).ToHashSet();
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

        private HashSet<Int64> UserPostsSavedInternal(Int64 userId)
        {
            return _context.PostSaves
                .Where(x => x.UserId.Equals(userId)).Select(x => x.PostId).ToHashSet();
        }

        private Dictionary<Int64, Boolean> UserPostsVotedInternal(Int64 userId)
        {
            var query = _context.PostVotes
                .Where(x => x.UserId.Equals(userId));

            return query.Select(x => new { x.PostId, x.Direction }).ToDictionary(kvp => kvp.PostId, kvp => kvp.Direction);
        }
    }
}