using ForumSiteCore.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ForumSiteCore.DAL.Models;
using Serilog;
using ForumSiteCore.Business.Interfaces;

namespace ForumSiteCore.Business.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserActivitiesService _userActivitiesService;
        public PostService(ApplicationDbContext context, UserActivitiesService userActivitiesService)
        {
            _context = context;
            _userActivitiesService = userActivitiesService;
        }

        public Post Add(Post post)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    post.Created = post.Updated = DateTimeOffset.Now;
                    _context.Posts.Add(post);

                    // user automatically upvotes their own post.
                    Upvote(post.Id, post.UserId);
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while adding post.");
                    transaction.Rollback();
                }
            }
            
            return post;
        }

        public Boolean Downvote(Int64 postId, Int64 userId)
        {
            return Vote(postId, userId, false);
        }

        public Boolean Save(Int64 postId, Int64 userId, Boolean saving)
        {            
            var result = true;
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postSave = _context.PostSaves.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(userId));
                    if (postSave == null)
                    {
                        postSave = new PostSave();
                        postSave.Created = postSave.Updated = DateTimeOffset.Now;

                        postSave.PostId = postId;
                        postSave.UserId = userId;
                        _context.PostSaves.Add(postSave);
                    }
                    else
                    {
                        if (saving == postSave.Inactive)
                        {
                            postSave.Inactive = !saving;
                            postSave.Updated = DateTimeOffset.Now;

                        }
                    }
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                    if (!saving)
                    {
                        _userActivitiesService.UserPostsSaved.Remove(postId);
                    }
                    else
                    {
                        _userActivitiesService.UserPostsSaved.Add(postId);
                    }
                }
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while saving post");
                    transaction.Rollback();
                }

                return result;
            }
        }
        public Boolean Upvote(Int64 postId, Int64 userId)
        {
            return Vote(postId, userId, true);
        }

        internal Boolean Vote(Int64 postId, Int64 userId, Boolean direction)
        {
            var result = true;
                      
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {                
                try
                {
                    var postVote = _context.PostVotes.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(userId));
                    if (postVote == null)
                    {
                        postVote = new PostVote();
                        postVote.Created = postVote.Updated = DateTimeOffset.Now;
                        postVote.UserId = userId;
                        postVote.PostId = postId;
                        postVote.Direction = direction;

                        _context.PostVotes.Add(postVote);
                    }
                    else
                    {
                        if (postVote.Direction != direction)
                        {
                            postVote.Direction = direction;
                            postVote.Updated = DateTimeOffset.Now;
                        }
                    }
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                    if (_userActivitiesService.UserPostsVoted.ContainsKey(postId))
                    {
                        _userActivitiesService.UserPostsVoted[postId] = direction;
                    }
                    else
                    {
                        _userActivitiesService.UserPostsVoted.Add(postId, direction);
                    }
                        
                }
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while voting post");
                    transaction.Rollback();                    
                }

                return result;
            }

        }
    }
}
