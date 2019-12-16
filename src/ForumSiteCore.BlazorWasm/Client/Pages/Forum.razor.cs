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
        private ForumPostListingVM _forumPostListing;

        [Parameter] public string Name { get; set; }
        [Parameter] public string Sort { get; set; }
        [Parameter] public string Lookback { get; set; }

        public string ForumUrlName => $"f/{_forumPostListing.Forum.Name}";

        private bool ForumCanBeSubscribed => _forumPostListing.Forum.Id != 0 && _forumPostListing.Forum.Id != -1;
        private string ForumSubscribedButtonClass => _forumPostListing.Forum.UserSaved ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
        private string ForumSubscribedButtonText => _forumPostListing.Forum.UserSaved ? "subscribed" : "subscribe";
        private bool NoMorePosts => false;

        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine("OnParametersSetAsync was called...");
            if (String.IsNullOrWhiteSpace(Name))
            {
                Name = "all";                
            }
            if (String.IsNullOrWhiteSpace(Sort))
            {
                Sort = "hot";
            }
            await LoadForumPostListing();
        }

        protected override async Task OnInitializedAsync()
        {

        }

        private async Task LoadForumPostListing()
        {
            var apiUrl = $"/api/forums/";
            apiUrl += $"{Name}/{Sort}";
            if (!string.IsNullOrWhiteSpace(Lookback))
            {
                var parsedLookback = GetLookbackDatetimeOffset(Lookback);
                apiUrl += $"?dtstart={parsedLookback}";
            }
            _forumPostListing = await Http.GetJsonAsync<ForumPostListingVM>(apiUrl);
        }

        private void GoToSort(string sort, string lookback = null)
        {
            var navUrl = $"/f/";
            navUrl += $"{_forumPostListing.Forum.Name}/{sort}";
            if (!string.IsNullOrWhiteSpace(lookback))
            {
                navUrl += $"/{lookback}";
            }
            Console.WriteLine(navUrl);
            NavManager.NavigateTo(navUrl);            
        }

        private string GetLookbackDatetimeOffset(string lookback)
        {
            var arrLookback = lookback.Split('-', 3);
            var amountParsed = Int32.Parse(arrLookback[1]);
            var unit = arrLookback[2];
            DateTime lookbackDateTime = DateTime.Now;

            if (unit.Equals("years", StringComparison.OrdinalIgnoreCase))
            {
                lookbackDateTime = DateTime.Now.AddYears(amountParsed * -1);
            }
            if (unit.Equals("days", StringComparison.OrdinalIgnoreCase))
            {
                lookbackDateTime = DateTime.Now.AddDays(amountParsed * -1);
            }
            if (unit.Equals("hours", StringComparison.OrdinalIgnoreCase))
            {
                lookbackDateTime = DateTime.Now.AddHours(amountParsed * -1);
            }

            return lookbackDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        private void Save()
        {
            
        }
    }
}
