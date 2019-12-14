using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.JSInterop;
using ForumSiteCore.Business.ViewModels;

namespace ForumSiteCore.BlazorWasm.Client.Shared
{
    public partial class TypeAhead : ComponentBase
    {
        private string _searchText = string.Empty;
        private Timer _debounceTimer;
        private ForumSearchVM Suggestions { get; set; } = new ForumSearchVM();
        private bool IsSearching { get; set; } = false;
        private bool IsShowingSuggestions { get; set; } = false;
        private ElementReference _searchInput;
        
        [Parameter]
        public int MinimumLength { get; set; } = 1;
        [Parameter]
        public int Debounce { get; set; } = 300;
        [Parameter]
        public int MaximumSuggestions { get; set; } = 10;
        private int SelectedIndex { get; set; }

        private string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                    SelectedIndex = -1;
                }
                else if (value.Length >= MinimumLength)
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
        }

        protected override void OnInitialized()
        {
            Console.WriteLine("Called OnInitialized");
            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;
        }

        private async void Search(object sender, ElapsedEventArgs e)
        {
            IsSearching = true;
            Console.WriteLine("invoking search...");
            await InvokeAsync(StateHasChanged);
            Suggestions = (await Http.GetJsonAsync<ForumSearchVM>($"api/forums/search/{_searchText}"));
            Console.WriteLine($"there are {Suggestions.Results.Count} suggestions..");
            IsSearching = false;
            IsShowingSuggestions = true;
            await InvokeAsync(StateHasChanged);

        }
    }
}
