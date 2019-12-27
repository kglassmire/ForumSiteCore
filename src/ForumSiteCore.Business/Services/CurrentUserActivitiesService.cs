﻿using CacheManager.Core;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForumSiteCore.Business.Services
{
    /// <summary>
    /// This service is specific to ensure that page to page operations for the current user are cached.
    /// </summary>
    public class CurrentUserActivitiesService
    {
        private readonly ILogger<CurrentUserActivitiesService> _logger;
        private const string UserCommentsCreatedCacheKeyTemplate = "user-{0}-comments-created";
        private const string UserCommentsSavedCacheKeyTemplate = "user-{0}-comments-saved";
        private const string UserCommentsVotedCacheKeyTemplate = "user-{0}-comments-voted";
        private const string UserForumsSavedCacheKeyTemplate = "user-{0}-forums-saved";
        private const string UserPostsCreatedCacheKeyTemplate = "user-{0}-posts-created";
        private const string UserPostsSavedCacheKeyTemplate = "user-{0}-posts-saved";
        private const string UserPostsVotedCacheKeyTemplate = "user-{0}-posts-voted";
        private readonly ICacheManager<object> _cache;
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor<long> _userAccessor;

        public CurrentUserActivitiesService(ApplicationDbContext context, ICacheManager<object> cache, IUserAccessor<long> userAccessor, ILogger<CurrentUserActivitiesService> logger)
        {
            _context = context;
            _cache = cache;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public HashSet<long> GetUserCommentsCreated()
        {
            return (HashSet<long>)_cache.GetOrAdd(UserCommentsCreatedCacheKey, valueFactory => UserCommentsCreatedInternal(_userAccessor.UserId));
        }

        public void SetUserCommentsCreated(HashSet<long> value)
        {
            _cache.AddOrUpdate(UserCommentsCreatedCacheKey, value, v => value);
        }

        public Dictionary<long, bool> GetUserCommentsSaved()
        {
            return (Dictionary<long, bool>)_cache.GetOrAdd(string.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId), valueFactory => UserCommentsSavedInternal(_userAccessor.UserId));
        }

        public void SetUserCommentsSaved(Dictionary<long, bool> value)
        {
            _cache.AddOrUpdate(UserCommentsSavedCacheKey, value, v => value);
        }

        public Dictionary<long, bool?> GetUserCommentsVoted(long? postId)
        {
            Dictionary<long, bool?> castedCacheItem = null;
            var cacheItems = _cache.Get(UserCommentsVotedCacheKey);
            if (cacheItems == null)
            {
                castedCacheItem = new Dictionary<long, bool?>();
            }
            else
            {
                castedCacheItem = (Dictionary<long, bool?>)cacheItems;
            }
                

            var newItems = UserCommentsVotedInternal(_userAccessor.UserId, postId);
            var latestCacheItems = Utility.CollectionExtensions.MergeDictionaries<Int64, Boolean?>(new List<Dictionary<long, bool?>>{ castedCacheItem, newItems});
            _cache.AddOrUpdate(UserCommentsVotedCacheKey, latestCacheItems, x => latestCacheItems);

            return latestCacheItems;
            //return (Dictionary<Int64, UserActivitiesVoteItem>)_cache.GetOrAdd(UserCommentsVotedCacheKey, valueFactory => UserCommentsVotedInternal(_userAccessor.UserId));
        }

        public void SetUserCommentsVoted(Dictionary<long, bool?> value)
        {
            _cache.AddOrUpdate(UserCommentsVotedCacheKey, value, v => value);
        }

        public Dictionary<long, bool> GetUserForumsSaved()
        {
            return (Dictionary<long, bool>)_cache.GetOrAdd(UserForumsSavedCacheKey, valueFactory => UserForumsSavedInternal(_userAccessor.UserId));
        }

        public void SetUserForumsSaved(Dictionary<long, bool> value)
        {
            _cache.AddOrUpdate(UserForumsSavedCacheKey, value, v => value);
        }

        public HashSet<long> GetUserPostsCreated()
        {
            return (HashSet<long>)_cache.GetOrAdd(UserPostsCreatedCacheKey, valueFactory => UserPostsCreatedInternal(_userAccessor.UserId));
        }

        public void SetUserPostsCreated(HashSet<long> value)
        {
            _cache.AddOrUpdate(UserPostsCreatedCacheKey, value, v => value);
        }

        public Dictionary<long, bool> GetUserPostsSaved()
        {
            return (Dictionary<long, bool>)_cache.GetOrAdd(UserPostsSavedCacheKey, valueFactory => UserPostsSavedInternal(_userAccessor.UserId));
        }

        public void SetUserPostsSaved(Dictionary<long, bool> value)
        {
            _cache.AddOrUpdate(UserPostsSavedCacheKey, value, v => value);
        }

        public Dictionary<long, bool?> GetUserPostsVoted()
        {
            return (Dictionary<long, bool?>)_cache.GetOrAdd(UserPostsVotedCacheKey, valueFactory => UserPostsVotedInternal(_userAccessor.UserId));
        }

        public void SetUserPostsVoted(Dictionary<long, bool?> value)
        {
            _cache.AddOrUpdate(UserPostsVotedCacheKey, value, v => value);
        }

        private string UserCommentsCreatedCacheKey => string.Format(UserCommentsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsSavedCacheKey => string.Format(UserCommentsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserCommentsVotedCacheKey => string.Format(UserCommentsVotedCacheKeyTemplate, _userAccessor.UserId);
        private string UserForumsSavedCacheKey => string.Format(UserForumsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsCreatedCacheKey => string.Format(UserPostsCreatedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsSavedCacheKey => string.Format(UserPostsSavedCacheKeyTemplate, _userAccessor.UserId);
        private string UserPostsVotedCacheKey => string.Format(UserPostsVotedCacheKeyTemplate, _userAccessor.UserId);

        public void ProcessComments(PostDto post, List<CommentDto> comments)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;
            if (!comments.Any())
                return;

            var userCommentsVoted = GetUserCommentsVoted(post.Id);
            var userCommentsSaved = GetUserCommentsSaved();
            var userCommentsCreated = GetUserCommentsCreated();            

            foreach (var comment in comments)
            {
                // is comment in comments voted and is it active?                               
                if (userCommentsVoted.ContainsKey(comment.Id))
                {
                    // if it is active then check the direction
                    if (userCommentsVoted[comment.Id].HasValue)
                    {
                        if (userCommentsVoted[comment.Id] == true)
                        {
                            comment.UserVote = Enums.VoteType.Up;
                        }
                        else
                        {
                            comment.UserVote = Enums.VoteType.Down;
                        }
                    }
                    else // otherwise it "has no vote"
                    {
                        comment.UserVote = Enums.VoteType.None;
                    }

                   
                    // is comment in comments created?
                    if (userCommentsCreated.Contains(comment.Id))
                    {
                        comment.UserCreated = true;
                    }

                    // is comment in comments saved
                    if (userCommentsSaved.ContainsKey(comment.Id))
                    {
                        comment.UserSaved = userCommentsSaved[comment.Id];
                    }
                }
            }
        }

        public void ProcessForums(List<ForumDto> forums)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userForumsSaved = GetUserForumsSaved();

            foreach(var forum in forums)
            {
                // is forum in forums saved
                if (userForumsSaved.ContainsKey(forum.Id))
                {                
                    forum.UserSaved = userForumsSaved[forum.Id];                    
                }
            }
        }

        public void ProcessPosts(List<PostDto> posts)
        {
            if (!_userAccessor.User.Identity.IsAuthenticated)
                return;

            var userPostsVoted = GetUserPostsVoted();
            var userPostsSaved = GetUserPostsSaved();
            var userPostsCreated = GetUserPostsCreated();

            foreach (var post in posts)
            {
                // is post in posts voted and is it active? 
                if (userPostsVoted.ContainsKey(post.Id))
                {
                    // if it is active then check the direction
                    if (userPostsVoted[post.Id].HasValue)
                    {
                        if (userPostsVoted[post.Id] == true)
                        {
                            post.UserVote = Enums.VoteType.Up;
                        }
                        else
                        {
                            post.UserVote = Enums.VoteType.Down;
                        }
                    }
                    else // otherwise it "has no vote"
                    {
                        post.UserVote = Enums.VoteType.None;
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
                    post.UserSaved = userPostsSaved[post.Id];                    
                }
            }
        }

        private HashSet<long> UserCommentsCreatedInternal(long userId)
        {
            return _context.Comments
                .Where(x => x.UserId.Equals(userId))
                .Select(x => x.Id)
                .ToHashSet();
        }

        private Dictionary<long, bool> UserCommentsSavedInternal(long userId)
        {
            return _context.CommentSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.CommentId, x.Saved })
                .ToDictionary(kvp => kvp.CommentId, kvp => kvp.Saved);
        }

        private Dictionary<long, bool?> UserCommentsVotedInternal(long userId, long? postId = null)
        {
            if (postId.HasValue)
                return _context.CommentVotes
                    .Where(x => x.UserId.Equals(userId) && x.PostId.Equals(postId))
                    .Select(x => new KeyValuePair<long, bool?>(x.CommentId, x.Direction))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return _context.CommentVotes
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new KeyValuePair<long, bool?>(x.CommentId, x.Direction))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private Dictionary<long, bool> UserForumsSavedInternal(long userId)
        {
            return _context.ForumSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.ForumId, x.Saved })
                .ToDictionary(kvp => kvp.ForumId, kvp => kvp.Saved);
        }

        private HashSet<long> UserPostsCreatedInternal(long userId)
        {
            return _context.Posts
                .Where(x => x.UserId.Equals(userId))
                .Select(x => x.Id)
                .ToHashSet();
        }

        private Dictionary<long, bool> UserPostsSavedInternal(long userId)
        {
            return _context.PostSaves
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new { x.PostId, x.Saved })
                .ToDictionary(kvp => kvp.PostId, kvp => kvp.Saved);
        }

        private Dictionary<long, bool?> UserPostsVotedInternal(long userId)
        {
            return _context.PostVotes
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new KeyValuePair<long, bool?>(x.PostId, x.Direction))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}