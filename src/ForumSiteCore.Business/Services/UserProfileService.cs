using AutoMapper;
using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForumSiteCore.Business.Services
{
    public class UserProfileService
    {
        private readonly ILogger<UserProfileService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserProfileService(ILogger<UserProfileService> logger,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public List<PostDto> UserCreatedPosts(string username)
        {
            var posts = _context.Posts
                .Include(x => x.User)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();

            return _mapper.Map<List<PostDto>>(posts);
        }

        public List<PostDto> UserSavedPosts(string username)
        {
            var posts = _context.PostSaves
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.User.UserName.Equals(username, StringComparison.Ordinal));

            return _mapper.Map<List<PostDto>>(posts);
        }

        public List<PostDto> UserVotedPosts(string username)
        {
            var posts = _context.PostVotes
                .Include(x => x.User)
                .Include(x => x.Post)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<PostDto>>(posts);
        }

        public List<PostDto> UserCommentedPosts(string username)
        {
            var posts = _context.Comments
                .Include(x => x.Post)
                .Include(x => x.User)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<PostDto>>(posts);
        }

        public List<CommentDto> UserCreatedComments(string username)
        {
            var comments = _context.Comments
                .Include(x => x.User)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public List<CommentDto> UserVotedComments(string username)
        {
            var comments = _context.CommentVotes
                .Include(x => x.User)
                .Include(x => x.Comment)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<CommentDto>>(comments);

        }

        public List<CommentDto> UserSavedComments(string username)
        {
            var comments = _context.CommentSaves
                .Include(x => x.User)
                .Include(x => x.Comment)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public List<CommentDto> UserCommentedComments(string username)
        {
            var comments = _context.Comments
                .Include(x => x.User)
                .Include(x => x.Parent)
                .Where(x => x.User.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<CommentDto>>(comments);
        }
    }
}
