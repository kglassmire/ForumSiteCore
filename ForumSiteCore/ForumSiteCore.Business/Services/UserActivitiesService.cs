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
        private const string UserForumsSavedCacheKeyTemplate = "user-{0}-forums-saved";
        private const string UserPostsCreatedCacheKeyTemplate = "user-{0}-posts-created";
        private const string UserPostsSavedCacheKeyTemplate = "user-{0}-posts-saved";
        private const string UserPostsVotedCacheKeyTemplate = "user-{0}-posts-voted";
        private ICacheManager<object> _cache;
        private ApplicationDbContext _context;
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

        public Dictionary<Int64, Boolean> UserCommentsSaved
        {
            get => (Dictionary<Int64, Boolean>)_cache.GetOrAdd(String.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId), valueFactory => UserCommentsSavedInternal(_userAccessor.UserId));
            set => _cache.Put(UserCommentsSavedCacheKey, value);
        }

        public Dictionary<Int64, UserActivitiesVoteItem> UserCommentsVoted
        {
            get => (Dictionary<Int64, UserActivitiesVoteItem>)_cache.GetOrAdd(UserCommentsVotedCacheKey, valueFactory => UserCommentsVotedInternal(_userAccessor.UserId));
            set => _cache.Put(UserCommentsVotedCacheKey, value);
        }

        public Dictionary<Int64, Boolean> UserForumsSaved
        {
            get => (Dictionary<Int64, Boolean>)_cache.GetOrAdd(UserForumsSavedCacheKey, valueFactory => UserForumsSavedInternal(_userAccessor.UserId));
            set => _cache.Put(UserForumsSavedCacheKey, value);
        }

        public HashSet<Int64> UserPostsCreated
        {
            get => (HashSet<Int64>)_cache.GetOrAdd(UserPostsCreatedCacheKey, valueFactory => UserPostsCreatedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsCreatedCacheKey, value);
        }

        public Dictionary<Int64, Boolean> UserPostsSaved
        {
            get => (Dictionary<Int64, Boolean>)_cache.GetOrAdd(UserPostsSavedCacheKey, valueFactory => UserPostsSavedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsSavedCacheKey, value);
        }

        public Dictionary<Int64, UserActivitiesVoteItem> UserPostsVoted
        {
            get => (Dictionary<Int64, UserActivitiesVoteItem>)_cache.GetOrAdd(UserPostsVotedCacheKey, valueFactory => UserPostsVotedInternal(_userAccessor.UserId));
            set => _cache.Put(UserPostsVotedCacheKey, value);
        }

        private string UserCommentsCreatedCacheKey => String.Format(UserCommentsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsSavedCacheKey => String.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsVotedCacheKey => String.Format(UserCommentsVotedCacheKeyTemplate, _userAccessor.UserId);
        private string UserForumsSavedCacheKey => String.Format(UserForumsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsCreatedCacheKey => String.Format(UserPostsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsSavedCacheKey => String.Format(UserPostsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsVotedCacheKey => String.Format(UserPostsVotedCacheKeyTemplate, _userAccessor.UserId);

        public void ProcessComments(IList<CommentDto> comments)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userCommentsVoted = UserCommentsVoted;
            var userCommentsSaved = UserCommentsSaved;
            var userCommentsCreated = UserCommentsCreated;            

            foreach (var comment in comments)
            {
                // is comment in comments voted and is it active?                               
                if (userCommentsVoted.ContainsKey(comment.Id))
                {
                    // if it is active then check the direction
                    if (userCommentsVoted[comment.Id].Inactive == false)
                    {
                        if (userCommentsVoted[comment.Id].Direction == true)
                        {
                            comment.UserVote = Enums.VotedType.Up;
                        }
                        else
                        {
                            comment.UserVote = Enums.VotedType.Down;
                        }
                    }
                    else // otherwise it "has no vote"
                    {
                        comment.UserVote = Enums.VotedType.None;
                    }

                   
                    // is comment in comments created?
                    if (userCommentsCreated.Contains(comment.Id))
                    {
                        comment.UserCreated = true;
                    }

                    // is comment in comments saved
                    if (userCommentsSaved.ContainsKey(comment.Id))
                    {
                        // is it active ?
                        if (userCommentsSaved[comment.Id] == false)
                        {
                            comment.UserSaved = true;
                        }                        
                    }
                }
            }
        }

        public void ProcessForums(IList<ForumDto> forums)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userForumsSaved = UserForumsSaved;

            foreach(var forum in forums)
            {
                // is forum in forums saved
                if (userForumsSaved.ContainsKey(forum.Id))
                {
                    // is it active ?
                    if (userForumsSaved[forum.Id] == false)
                    {
                        forum.UserSaved = true;
                    }
                }
            }
        }

        public void ProcessPosts(IList<PostDto> posts)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userPostsVoted = UserPostsVoted;
            var userPostsSaved = UserPostsSaved;
            var userPostsCreated = UserPostsCreated;

            foreach (var post in posts)
            {
                // is post in posts voted and is it active? 
                if (userPostsVoted.ContainsKey(post.Id))
                {
                    // if it is active then check the direction
                    if (userPostsVoted[post.Id].Inactive == false)
                    {
                        if (userPostsVoted[post.Id].Direction == true)
                        {
                            post.UserVote = Enums.VotedType.Up;
                        }
                        else
                        {
                            post.UserVote = Enums.VotedType.Down;
                        }
                    }
                    else // otherwise it "has no vote"
                    {
                        post.UserVote = Enums.VotedType.None;
                    }
                }

                // is comment in comments created?
                if (userPostsCreated.Contains(post.Id))
                {
                    post.UserCreated = true;
                }

                // is comment in comments saved
                if (userPostsSaved.ContainsKey(post.Id))
                {
                    // is it active ?
                    if (userPostsSaved[post.Id] == false)
                    {
                        post.UserSaved = true;
                    }
                }
            }
        }

        private HashSet<Int64> UserCommentsCreatedInternal(Int64 userId)
        {
            return _context.Comments
                .Where(x => x.UserId.Equals(userId))
                .Select(x => x.Id)
                .ToHashSet();
        }

        private Dictionary<Int64, Boolean> UserCommentsSavedInternal(Int64 userId)
        {
            return _context.CommentSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.CommentId, x.Inactive })
                .ToDictionary(kvp => kvp.CommentId, kvp => kvp.Inactive);
        }

        private Dictionary<Int64, UserActivitiesVoteItem> UserCommentsVotedInternal(Int64 userId)
        {
            return _context.CommentVotes
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new KeyValuePair<Int64, UserActivitiesVoteItem>(x.CommentId, new UserActivitiesVoteItem(x.Direction, x.Inactive)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private Dictionary<Int64, Boolean> UserForumsSavedInternal(Int64 userId)
        {
            return _context.ForumSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.ForumId, x.Inactive })
                .ToDictionary(kvp => kvp.ForumId, kvp => kvp.Inactive);
        }

        private HashSet<Int64> UserPostsCreatedInternal(Int64 userId)
        {
            return _context.Posts
                .Where(x => x.UserId.Equals(userId))
                .Select(x => x.Id)
                .ToHashSet();
        }

        private Dictionary<Int64, Boolean> UserPostsSavedInternal(Int64 userId)
        {
            return _context.PostSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.PostId, x.Inactive })
                .ToDictionary(kvp => kvp.PostId, kvp => kvp.Inactive);
        }

        private Dictionary<Int64, UserActivitiesVoteItem> UserPostsVotedInternal(Int64 userId)
        {
            return _context.PostVotes
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new KeyValuePair<Int64, UserActivitiesVoteItem>(x.PostId, new UserActivitiesVoteItem(x.Direction, x.Inactive)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }


    public class UserActivitiesVoteItem
    {
        public UserActivitiesVoteItem(Boolean direction, Boolean inactive)
        {
            Direction = direction;
            Inactive = inactive;
        }

        public Boolean Direction { get; set; }
        public Boolean Inactive { get; set; }
    }  
}