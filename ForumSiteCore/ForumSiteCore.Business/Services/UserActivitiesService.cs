using ForumSiteCore.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Services
{
    public class UserActivitiesService
    {
        private readonly ApplicationDbContext _context;
        private Dictionary<Int64, Boolean> _userPostsVoted;
        private Dictionary<Int64, Boolean> _userCommentsVoted;
        private HashSet<Int64> _userPostsCreated;
        private HashSet<Int64> _userCommentsCreated;
        private HashSet<Int64> _userPostsSaved;
        private HashSet<Int64> _userCommentSaved;

        public UserActivitiesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dictionary<Int64, Boolean> UserPostsVoted { get; set; }
        public Dictionary<Int64, Boolean> UserCommentsVoted { get; set; }
        public HashSet<Int64> UserPostsCreated { get; set; }
        public HashSet<Int64> UserCommentsCreated { get; set; }
        public HashSet<Int64> UserPostsSaved { get; set; }
        public HashSet<Int64> UserCommentSaved { get; set; }

        

    }
}
