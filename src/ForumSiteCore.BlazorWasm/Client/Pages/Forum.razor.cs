using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Pages
{
    public partial class Forum : ComponentBase
    {
        private ForumPostListingVM forumPostListing;

        [Parameter] public string Name { get; set; }
        [Parameter] public string Type { get; set; }

        public string ForumUrlName => $"f/{forumPostListing.Forum.Name}";
        
        private bool ForumCanBeSubscribed => forumPostListing.Forum.Id != 0 && forumPostListing.Forum.Id != -1;
        private string ForumSubscribedButtonClass => forumPostListing.Forum.UserSaved ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
        private string ForumSubscribedButtonText => forumPostListing.Forum.UserSaved ? "subscribed" : "subscribe";
        private bool NoMorePosts => false;

        protected override async Task OnParametersSetAsync()
        {
            if (String.IsNullOrWhiteSpace(Name) || String.IsNullOrWhiteSpace(Type))
            {
                Name = "all";
                Type = "hot";
            }

            await LoadForumPostListing();
        }

        protected override async Task OnInitializedAsync()
        {

        }

        private async Task LoadForumPostListing()
        {
            forumPostListing = await Http.GetJsonAsync<ForumPostListingVM>($"api/forums/{Name}/{Type}");
        }

        public void GoToSort(string sort)
        {
            NavManager.NavigateTo($"/f/{forumPostListing.Forum.Name}/{sort}");
        }

        private void Save()
        {

        }
    }
}
