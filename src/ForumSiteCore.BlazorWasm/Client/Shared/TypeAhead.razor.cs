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

                if (value.Length < MinimumLength)
                {
                    _debounceTimer.Stop();
                    SelectedIndex = -1;
                    IsShowingSuggestions = false;
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
            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;
        }

        private void Initialize()
        {
            SearchText = string.Empty;
            Suggestions = new ForumSearchVM();            
            IsShowingSuggestions = false;
            SelectedIndex = -1;
        }

        private async Task HandleKeyUpOnShowDropDown(KeyboardEventArgs args)
        {
            if (args.Key == "ArrowDown")
            {
                MoveSelectedItem(1);
            }
            else if (args.Key == "ArrowUp")
            {
                MoveSelectedItem(-1);
            }    
            else if (args.Key == "Escape")
            {
                Initialize();
            }                
            else if (args.Key == "Enter")
            {
                NavManager.NavigateTo($"/f/{Suggestions.Results[SelectedIndex]}/hot");
                Initialize();
            }
        }

        private void MoveSelectedItem(int num)
        {
            var index = SelectedIndex + num;

            if (index >= Suggestions.Results.Count)
                index = 0;

            if (index < 0)
                index = Suggestions.Results.Count - 1;

            SelectedIndex = index;
        }

        private string GetSelectedClassIfItemSelected(int index)
        {
            var cssClass = "dropdown-item";
            if (index == SelectedIndex)
            {
                cssClass += " active";
            }
               
            return cssClass;
        }

        private async void Search(object sender, ElapsedEventArgs e)
        {
            IsSearching = true;            
            await InvokeAsync(StateHasChanged);
            Suggestions = (await Http.GetJsonAsync<ForumSearchVM>($"api/forums/search/{_searchText}"));
                        
            IsSearching = false;
            IsShowingSuggestions = true;
            await InvokeAsync(StateHasChanged);

        }
    }
}
