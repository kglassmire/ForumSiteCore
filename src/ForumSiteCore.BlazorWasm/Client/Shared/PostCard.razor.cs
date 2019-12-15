using ForumSiteCore.Business.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Shared
{
    public partial class PostCard : ComponentBase
    {
        [Parameter] public PostDto Post { get; set; }

        private bool _showPostDescription = false;
        public bool ShowPostDescription => _showPostDescription;

        public string CreatedText => $"Created {Post.Created.ToString("O")} by {Post.UserName}";

        public string CommentsUrl => $"/f/{Post.ForumName}/{Post.Id}/comments";

        private string PostSavedText
        {
            get
            {
                return Post.UserSaved ? "saved" : "save";
            }
        }

        private string VoteCountColorCssClass
        {
            get
            {
                if (Post.TotalScore < 0)
                {
                    return string.Empty;
                }
                else if (Post.TotalScore > 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private string VoteArrowColorCssClass
        {
            get
            {
                if (Post.UserVote == Business.Enums.VoteType.Down)
                {
                    return "downvoted";
                }
                else if (Post.UserVote == Business.Enums.VoteType.Up)
                {
                    return "upvoted";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void Up()
        {
            Vote(true);
        }

        private void Down()
        {
            Vote(false);
        }

        private void Vote(bool voteDirection)
        {

        }

        private void Save()
        {

        }

        private void TogglePostDescription()
        {
            _showPostDescription = !_showPostDescription;
        }
    }
}
