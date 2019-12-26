using ForumSiteCore.Business.Responses;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Pages
{
    public partial class User
    {

        [Parameter] public string UserName { get; set; }
        [Parameter] public string CurrentView { get; set; }

        private UserProfileResponse _userProfileResponse;
        private const string PostsCreated = "posts-created";
        private const string PostsSaved = "posts-saved";
        private const string PostsCommented = "posts-commented";
        private const string PostsVoted = "posts-voted";
        private const string CommentsCreated = "comments-created";
        private const string CommentsSaved = "comments-saved";
        private const string CommentsCommented = "comments-commented";
        private const string CommentsVoted = "commented-voted";

        protected override async Task OnInitializedAsync()
        {

        }

        protected override async Task OnParametersSetAsync()
        {
            if (String.IsNullOrEmpty(CurrentView))
            {
                NavManager.NavigateTo($"/u/{UserName}/posts-created");
                return;
            }

            await LoadUserContent();
        }

        private async Task LoadUserContent()
        {
            var apiUrl = $"/api/userprofile/";
            apiUrl += $"{UserName}/{CurrentView}";

            Console.WriteLine(apiUrl);
            switch (CurrentView.ToLower())
            {
                case PostsCreated:
                    _userProfileResponse = await Http.GetJsonAsync<UserProfileResponse>(apiUrl);
                    break;
                case PostsSaved:
                    return;
                case PostsCommented:
                    return;
                case PostsVoted:
                    return;
                case CommentsCreated:
                    return;
                case CommentsSaved:
                    return;
                case CommentsCommented:
                    return;
                case CommentsVoted:
                    return;
            }


            //_forumPostListing = await Http.GetJsonAsync<ForumPostListingVM>(apiUrl);
        }

    }
}
