using ForumSiteCore.Business.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Shared
{
    public partial class CommentCard : ComponentBase
    {
        [Parameter] public CommentDto Comment { get; set; }
        [Parameter] public bool IsRecursive { get; set; }

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
            var outputText = voteDirection ? $"Upvoted comment {Comment.Id}!!!" : $"Downvoted comment {Comment.Id}!!!";
            Console.WriteLine(outputText);
        }

        private string VoteArrowColorCssClass
        {
            get
            {
                if (Comment.UserVote == Business.Enums.VoteType.Down)
                {
                    return "downvoted";
                }
                else if (Comment.UserVote == Business.Enums.VoteType.Up)
                {
                    return "upvoted";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
