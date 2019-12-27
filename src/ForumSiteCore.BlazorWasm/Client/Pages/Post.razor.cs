using ForumSiteCore.Business.Responses;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Pages
{
    public partial class Post : ComponentBase
    {
        private PostCommentListingResponse _postCommentListing;
        
        [Parameter] public long PostId { get; set; }

        [Parameter] public string ForumName { get; set; }
        
        [Parameter] public string Sort { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine("OnParametersSetAsync was called...");

            if (string.IsNullOrWhiteSpace(ForumName))
            {
                throw new InvalidOperationException($"{nameof(ForumName)} was not provided");
            }

            if (PostId == 0)
            {
                throw new InvalidOperationException($"{nameof(PostId)} was not provided");
            }

            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "best";
            }

            await LoadPostCommentListing();

        }

        private async Task LoadPostCommentListing()
        {
            var apiUrl = $"/api/posts/";
            apiUrl += $"{PostId}/comments/{Sort}";
            _postCommentListing = await Http.GetJsonAsync<PostCommentListingResponse>(apiUrl);
        }

        private void GoToSort(string sort)
        {
            var navUrl = $"/f/";
            navUrl += $"{_postCommentListing.Post.ForumName}/{_postCommentListing.Post.Id}/comments/{sort}";
            Console.WriteLine(navUrl);
            NavManager.NavigateTo(navUrl);
        }
    }
}
