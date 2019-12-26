using ForumSiteCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using ForumSiteCore.DAL.Models;
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
using Microsoft.Extensions.Logging;

namespace ForumSiteCore.Business.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserActivitiesService _userActivitiesService;
        private readonly IUserAccessor<Int64> _userAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(ApplicationDbContext context, 
            CurrentUserActivitiesService userActivitiesService, 
            IUserAccessor<Int64> userAccessor,
            IMapper mapper,
            ILogger<PostService> logger)
        {
            _context = context;
            _userActivitiesService = userActivitiesService;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _logger = logger;
        }        

        public Post Add(Post post)
        {
            _logger.LogDebug("Adding post {Post} for user {User}", post, _userAccessor.UserId);
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    post.Created = post.Updated = DateTimeOffset.Now;
                    _context.Posts.Add(post);

                    // user automatically upvotes their own post.
                    Vote(post.Id, true);
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result = false;
                    _logger.LogError(e, "Error while adding post.");
                    transaction.Rollback();
                }
            }

            _logger.LogDebug("Created post {Post} for {User}", post, _userAccessor.UserId);
            return post;
        }

        public PostCommentListingVM Best(Int64 id)
        {
            _logger.LogDebug("Retrieving best post comment listing for {Post}", id);

            var comments = _context.Comments                
                .Include(x => x.Post).ThenInclude(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.PostId.Equals(id))
                .OrderByDescending(x => x.BestScore)
                .ToList();

            //https://stackoverflow.com/a/24273804
            //var comments2 = _context.CommentsTree.FromSql("select * from public.comment_tree({0})", id)
            //   .Include(x => x.User)
            //   .Include(x => x.Post)
            //   .Where(x => x.PostId.Equals(id))
            //   .OrderBy(x => x.Path).ThenBy(x => x.BestScore)
            //   .ToList();

            PostDto postDto;
            List<CommentDto> commentDtos = null;
            if (comments.Any())
            {
                MapDtos(comments, out postDto, out commentDtos);
                _userActivitiesService.ProcessPosts(new List<PostDto> { postDto });
                _userActivitiesService.ProcessComments(postDto, commentDtos);
            }
            else
            {
                postDto = Get(id);
            }

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupBest);
        }

        public PostCommentListingVM Controversial(Int64 id)
        {
            _logger.LogDebug("Retrieving controversial post comment listing for {Post}", id);


            var comments = _context.Comments
                .Include(x => x.Post).ThenInclude(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.PostId.Equals(id))
                .OrderByDescending(x => x.ControversyScore)
                .ToList();

            PostDto postDto;
            List<CommentDto> commentDtos = null;

            if (comments.Any())
            {
                MapDtos(comments, out postDto, out commentDtos);
                _userActivitiesService.ProcessPosts(new List<PostDto> { postDto });
                _userActivitiesService.ProcessComments(postDto, commentDtos);
            }
            else
            {
                postDto = Get(id);
            }

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupControversial);
        }

        public PostDto Get(Int64 postId)
        {
            var post = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .SingleOrDefault(x => x.Id.Equals(postId));

            return _mapper.Map<PostDto>(post);
        }

        public PostCommentListingVM New(Int64 id)
        {
            _logger.LogDebug("Retrieving new post comment listing for {Post}", id);

            var comments = _context.Comments
                .Include(x => x.Post).ThenInclude(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.PostId.Equals(id))
                .OrderByDescending(x => x.Created)
                .ToList();

            PostDto postDto;
            List<CommentDto> commentDtos = null;

            if (comments.Any())
            {
                MapDtos(comments, out postDto, out commentDtos);
                _userActivitiesService.ProcessPosts(new List<PostDto> { postDto });
                _userActivitiesService.ProcessComments(postDto, commentDtos);
            }
            else
            {
                postDto = Get(id);
            }

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupNew);
        }

        public PostSaveVM Save(Int64 postId, bool saved)
        {
            _logger.LogDebug("Saving post {Post} with value {Saved}", postId, saved);
            // see if the user already saved this at one point
            if (_userActivitiesService.GetUserPostsSaved().ContainsKey(postId))
            {
                _logger.LogDebug("Post already exists, updating value...");
                if (UpdatePostSave(postId, saved))
                {
                    var postSaveCache = _userActivitiesService.GetUserPostsSaved();
                    postSaveCache[postId] = saved;
                    _userActivitiesService.SetUserPostsSaved(postSaveCache);

                    PostSaveVM savedPost = new PostSaveVM { Status = "success", Saved = saved, Message = String.Format("PostSave was updated and set to {0}", saved) };
                    _logger.LogDebug("Post {@Post} saved", savedPost);
                    return savedPost;
                }
            }
            else
            {
                if (AddPostSave(postId))
                {
                    var postSaveCache = _userActivitiesService.GetUserPostsSaved();
                    postSaveCache.Add(postId, false);
                    _userActivitiesService.SetUserPostsSaved(postSaveCache);

                    PostSaveVM savedPost = new PostSaveVM { Status = "success", Saved = saved, Message = "PostSave was created and set to active" };
                    _logger.LogDebug("Post {@Post} saved", savedPost);
                    return savedPost;
                }
            }

            PostSaveVM failedPost = new PostSaveVM { Status = "failure", Saved = false, Message = "PostSave creation failed" };
            _logger.LogError("Post {@Post} failed to save", failedPost);
            return failedPost;
        }

        public PostCommentListingVM Top(Int64 id)
        {
            _logger.LogDebug("Retrieving top post comment listing for {Post}", id);

            var comments = _context.Comments
                .Include(x => x.Post).ThenInclude(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.PostId.Equals(id))
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .ToList();

            PostDto postDto;
            List<CommentDto> commentDtos = null;

            if (comments.Any())
            {
                MapDtos(comments, out postDto, out commentDtos);
                _userActivitiesService.ProcessPosts(new List<PostDto> { postDto });
                _userActivitiesService.ProcessComments(postDto, commentDtos);
            }
            else
            {
                postDto = Get(id);
            }

            return new PostCommentListingVM(postDto, commentDtos, LookupConsts.LookupTop);
        }

        public PostVoteVM Vote(Int64 postId, Boolean? direction)
        {
            if (_userActivitiesService.GetUserPostsVoted().ContainsKey(postId)) // post vote already exists
            {                 
                UpdatePostVoteDirection(postId, direction);
                // get cache values one more time
                // set the direction on the specific item
                // update the cache
                var postsVoted = _userActivitiesService.GetUserPostsVoted();                
                postsVoted[postId] = direction;                
                _userActivitiesService.SetUserPostsVoted(postsVoted);

                return new PostVoteVM { Message = "success on vote", PostId = postId, Status = "success", VoteType = EnumTranslator.DirectionToVoteType(direction) };
            }
            else // no post vote exists -- time to add
            {
                if (AddPostVote(postId, direction))
                {
                    var postsVoted = _userActivitiesService.GetUserPostsVoted();
                    postsVoted.Add(postId, direction);
                    _userActivitiesService.SetUserPostsVoted(postsVoted);

                    return new PostVoteVM { Message = "Success on vote", PostId = postId, Status = "success", VoteType = EnumTranslator.DirectionToVoteType(direction) };
                }
            }

            throw new Exception("error voting post");
        }

        private Boolean AddPostSave(Int64 postId)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postSave = new PostSave();
                    postSave.Created = DateTimeOffset.Now;
                    postSave.Updated = DateTimeOffset.Now;
                    postSave.PostId = postId;
                    postSave.UserId = _userAccessor.UserId;
                    postSave.Saved = true;
                    _context.PostSaves.Add(postSave);

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to add PostSave");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private Boolean AddPostVote(Int64 postId, Boolean? direction)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postVote = new PostVote();
                    postVote.Created = postVote.Updated = DateTimeOffset.Now;
                    postVote.UserId = _userAccessor.UserId;
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
                    _logger.LogError(e, "Failed to add PostVote");
                    transaction.Rollback();
                }
            }

            return false;
        }


        private void MapDtos(List<CommentTree> comments, out PostDto postDto, out List<CommentDto> commentDtos)
        {
            Post post = comments.FirstOrDefault().Post;
            postDto = _mapper.Map<PostDto>(post);
            commentDtos = _mapper.Map<List<CommentDto>>(comments);
        }

        private void MapDtos(List<Comment> comments, out PostDto postDto, out List<CommentDto> commentDtos)
        {
            Post post = comments.FirstOrDefault().Post;
            postDto = _mapper.Map<PostDto>(post);
            commentDtos = _mapper.Map<List<CommentDto>>(comments);
        }

        private Boolean UpdatePostSave(Int64 postId, Boolean save)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postSave = _context.PostSaves.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(_userAccessor.UserId));
                    postSave.Saved = save;
                    postSave.Updated = DateTimeOffset.Now;

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to toggle PostSave inactive state");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private Boolean UpdatePostVoteDirection(Int64 postId, Boolean? direction)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var postVote = _context.PostVotes.SingleOrDefault(x => x.PostId.Equals(postId) && x.UserId.Equals(_userAccessor.UserId));
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
                    _logger.LogError(e, "Failed to toggle PostVote direction");
                    transaction.Rollback();
                }
            }

            return false;
        }
    }
}

//var commentgj = from p in _context.Posts
//                join ct in _context.Comments on p.Id equals ct.PostId into gj
//                where p.Id == id
//                select new
//                {
//                    Post = p,
//                    CommentTree = gj
//                };