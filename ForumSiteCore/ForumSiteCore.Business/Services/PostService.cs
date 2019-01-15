using ForumSiteCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using ForumSiteCore.DAL.Models;
using Serilog;
using AutoMapper;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.Utility;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Diagnostics;

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
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while adding post.");
                    transaction.Rollback();
                }
            }

            return post;
        }

        public PostCommentListingVM Best(Int64 id)
        {
            var comments = _context.CommentsTree
                .FromSql("select * from public.comment_tree({0})", id)
                .Include(x => x.User)
                .Include(x => x.Post)                
                .OrderBy(x => x.Path).ThenBy(x => x.BestScore)
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, Consts.COMMENT_LISTING_TYPE_BEST);
        }

        public PostCommentListingVM Controversial(Int64 id)
        {
            var comments = _context.Comments
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, Consts.COMMENT_LISTING_TYPE_CONTROVERSIAL);
        }

        public Boolean Downvote(Int64 postId, Int64 userId)
        {
            return Vote(postId, userId, false);
        }

        public PostCommentListingVM New(Int64 id)
        {
            var comments = _context.Comments
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, Consts.COMMENT_LISTING_TYPE_NEW);
        }

        public PostSaveVM Save(Int64 postId, Int64 userId)
        {
            // see if the user already saved this at one point
            if (_userActivitiesService.UserPostsSaved.ContainsKey(postId))
            {
                // they did save it. Is the save "inactive"?
                if (_userActivitiesService.UserPostsSaved[postId] == true)
                {
                    // set it to inactive
                    if (UpdatePostSaveInactive(postId, userId, false))
                    {
                        // update our cache item
                        _userActivitiesService.UserPostsSaved[postId] = false;
                        return new PostSaveVM { Status = "success", Saved = true, Message = "PostSave existed and was set from inactive to active" };
                    }
                }
                else // looks like they want to activate this postsave
                {
                    // take care of it in db
                    if (UpdatePostSaveInactive(postId, userId, true))
                    {
                        // update our cache item
                        _userActivitiesService.UserPostsSaved[postId] = true;
                        return new PostSaveVM { Status = "success", Saved = false, Message = "PostSave existed and was set from active to inactive" };
                    }
                }
            }
            else
            {
                if (AddPostSave(postId, userId))
                {
                    _userActivitiesService.UserPostsSaved.Add(postId, false);
                    return new PostSaveVM { Status = "success", Saved = true, Message = "PostSave was created and set to active" };
                }
            }

            return new PostSaveVM { Status = "failure", Saved = false, Message = "PostSave creation failed" };
        }

        public PostCommentListingVM Top(Int64 id)
        {
            var comments = _context.Comments
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, Consts.COMMENT_LISTING_TYPE_TOP);
        }

        public Boolean Upvote(Int64 postId, Int64 userId)
        {
            return Vote(postId, userId, true);
        }

        internal Boolean Vote(Int64 postId, Int64 userId, Boolean direction)
        {
            if (_userActivitiesService.UserPostsVoted.ContainsKey(postId))
            {
                // if the direction being voted is the same as what's stored, then they are trying to delete their vote -- set to inactive
                if (_userActivitiesService.UserPostsVoted[postId].Direction == direction)
                {
                    Boolean curInactiveState = _userActivitiesService.UserPostsVoted[postId].Inactive;
                    if (UpdatePostVoteInactive(postId, userId, !curInactiveState))
                    {
                        _userActivitiesService.UserPostsVoted[postId].Inactive = !curInactiveState;
                        return true;
                    }
                }
                else // change the direction -- if we're changing direction we're resetting active status.
                {
                    Boolean curDirection = _userActivitiesService.UserPostsVoted[postId].Direction;
                    if (UpdatePostVoteDirection(postId, userId, !curDirection))
                    {
                        var data = _userActivitiesService.UserPostsVoted[postId];
                        data.Direction = !curDirection;
                        data.Inactive = false;

                        return true;
                    }
                }
            }
            else // no post vote exists -- time to add -- defaulting to inactive being false.
            {
                if (AddPostVote(postId, userId, direction))
                {
                    _userActivitiesService.UserPostsVoted.Add(postId, new UserActivitiesVoteItem(direction, false));
                    return false;
                }
            }

            return false;
        }

        private Boolean AddPostSave(Int64 postId, Int64 userId)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postSave = new PostSave();
                    postSave.Created = postSave.Updated = DateTimeOffset.Now;

                    postSave.PostId = postId;
                    postSave.UserId = userId;

                    _context.PostSaves.Add(postSave);

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to add PostSave");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private Boolean AddPostVote(Int64 postId, Int64 userId, Boolean direction)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postVote = new PostVote();
                    postVote.Created = postVote.Updated = DateTimeOffset.Now;
                    postVote.UserId = userId;
                    postVote.PostId = postId;
                    postVote.Direction = direction;
                    postVote.Inactive = false;

                    _context.PostVotes.Add(postVote);

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to add PostVote");
                    transaction.Rollback();
                }
            }

            return false;
        }


        private void MapDtos(IList<CommentTree> comments, out PostDto postDto, out IList<CommentDto> commentDtos)
        {
            Post post = comments.FirstOrDefault().Post;
            postDto = Mapper.Map<PostDto>(post);
            commentDtos = Mapper.Map<List<CommentDto>>(comments);
        }

        private void MapDtos(IList<Comment> comments, out PostDto postDto, out IList<CommentDto> commentDtos)
        {
            Post post = comments.FirstOrDefault().Post;
            postDto = Mapper.Map<PostDto>(post);
            commentDtos = Mapper.Map<List<CommentDto>>(comments);
        }

        private Boolean UpdatePostSaveInactive(Int64 postId, Int64 userId, Boolean inactive)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postSave = _context.PostSaves.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(userId));
                    postSave.Inactive = inactive;
                    postSave.Updated = DateTimeOffset.Now;

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to toggle PostSave inactive state");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private Boolean UpdatePostVoteDirection(Int64 postId, Int64 userId, Boolean direction)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postVote = _context.PostVotes.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(userId));
                    postVote.Direction = direction;
                    postVote.Updated = DateTimeOffset.Now;

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to toggle PostVote direction");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private Boolean UpdatePostVoteInactive(Int64 postId, Int64 userId, Boolean inactive)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postVote = _context.PostVotes.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(userId));
                    postVote.Inactive = inactive;
                    postVote.Updated = DateTimeOffset.Now;

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to toggle PostVote inactive state");
                    transaction.Rollback();                    
                }
            }

            return false;
        }
    }
}
