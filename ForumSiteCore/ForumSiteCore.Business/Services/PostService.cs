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
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Enums;
using ForumSiteCore.Business.Interfaces;

namespace ForumSiteCore.Business.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserActivitiesService _userActivitiesService;
        private readonly IUserAccessor<Int64> _userAccessor;
        public PostService(ApplicationDbContext context, UserActivitiesService userActivitiesService, IUserAccessor<Int64> userAccessor)
        {
            _context = context;
            _userActivitiesService = userActivitiesService;
            _userAccessor = userAccessor;
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
                    Vote(post.Id, post.UserId, true);
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
            _userActivitiesService.ProcessComments(postDto, commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupBest);
        }

        public PostCommentListingVM Controversial(Int64 id)
        {
            var comments = _context.CommentsTree
                .FromSql("select * from public.comment_tree({0})", id)
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .OrderBy(x => x.Path).ThenBy(x => x.ControversyScore)
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(postDto, commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupControversial);
        }


        public PostCommentListingVM New(Int64 id)
        {
            var comments = _context.CommentsTree
                .FromSql("select * from public.comment_tree({0})", id)
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .OrderBy(x => x.Path).ThenBy(x => x.Created)
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(postDto, commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupNew);
        }

        public PostSaveVM Save(Int64 postId, Int64 userId)
        {
            // see if the user already saved this at one point
            if (_userActivitiesService.GetUserPostsSaved().ContainsKey(postId))
            {
                // they did save it. Is the save "inactive"?
                if (_userActivitiesService.GetUserPostsSaved()[postId] == true)
                {
                    // set it to inactive
                    if (UpdatePostSaveInactive(postId, userId, false))
                    {
                        // update our cache item
                        _userActivitiesService.GetUserPostsSaved()[postId] = false;
                        return new PostSaveVM { Status = "success", Saved = true, Message = "PostSave existed and was set from inactive to active" };
                    }
                }
                else // looks like they want to activate this postsave
                {
                    // take care of it in db
                    if (UpdatePostSaveInactive(postId, userId, true))
                    {
                        // update our cache item
                        _userActivitiesService.GetUserPostsSaved()[postId] = true;
                        return new PostSaveVM { Status = "success", Saved = false, Message = "PostSave existed and was set from active to inactive" };
                    }
                }
            }
            else
            {
                if (AddPostSave(postId, userId))
                {
                    _userActivitiesService.GetUserPostsSaved().Add(postId, false);
                    return new PostSaveVM { Status = "success", Saved = true, Message = "PostSave was created and set to active" };
                }
            }

            return new PostSaveVM { Status = "failure", Saved = false, Message = "PostSave creation failed" };
        }

        public PostCommentListingVM Top(Int64 id)
        {
            var comments = _context.CommentsTree
                .FromSql("select * from public.comment_tree({0})", id)
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.PostId.Equals(id))
                .OrderBy(x => x.Path).ThenBy(x => x.Upvotes - x.Downvotes)
                .ToList();

            PostDto postDto;
            IList<CommentDto> commentDtos;

            MapDtos(comments, out postDto, out commentDtos);
            _userActivitiesService.ProcessComments(postDto, commentDtos);

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupTop);
        }

        public PostVoteVM Vote(Int64 postId, Int64 userId, Boolean? direction)
        {
            if (_userActivitiesService.GetUserPostsVoted().ContainsKey(postId)) // post vote already exists
            {                 
                UpdatePostVoteDirection(postId, _userAccessor.UserId, direction);
                // get cache values one more time
                // set the direction on the specific item
                // update the cache
                var postsVoted = _userActivitiesService.GetUserPostsVoted();                
                postsVoted[postId] = direction;                
                _userActivitiesService.SetUserPostsVoted(postsVoted);

                return new PostVoteVM { Message = "success on vote", PostId = postId, Status = "success" };
            }
            else // no post vote exists -- time to add
            {
                if (AddPostVote(postId, userId, direction))
                {
                    var postsVoted = _userActivitiesService.GetUserPostsVoted();
                    postsVoted.Add(postId, direction);
                    _userActivitiesService.SetUserPostsVoted(postsVoted);

                    return new PostVoteVM { Message = "Success on vote", PostId = postId, Status = "success" };
                }
            }

            return new PostVoteVM { Message = "Error voting post", PostId = postId, Status = "error" };
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

        private Boolean AddPostVote(Int64 postId, Int64 userId, Boolean? direction)
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

        private Boolean UpdatePostVoteDirection(Int64 postId, Int64 userId, Boolean? direction)
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
    }
}
