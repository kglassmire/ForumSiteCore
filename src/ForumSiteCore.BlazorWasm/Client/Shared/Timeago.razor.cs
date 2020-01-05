using ForumSiteCore.BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ForumSiteCore.BlazorWasm.Client.Shared
{
    public partial class Timeago : ComponentBase
    {
        private DateTimeOffset _date;
        [Inject] private TimeagoService _timeagoService { get; set; }
        public string RelativeTime { get; set; }
       
        [Parameter] public string CssClass { get; set; }

        [Parameter]
        public DateTimeOffset Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                Console.WriteLine(Date.ToString("o"));
                RelativeTime = TimeagoService.CalculateTimeago(_date);
            }
        }

        protected override void OnInitialized()
        {
            _timeagoService.Broadcast += ReceivedBroadcast;
        }

        private async void ReceivedBroadcast(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Received broadcast");             
            await InvokeChangeIfNecessary();
        }

        private async Task InvokeChangeIfNecessary()
        {
            var currentRelativeTime = RelativeTime;
            var newRelativeTime = TimeagoService.CalculateTimeago(Date);
            if (!currentRelativeTime.Equals(newRelativeTime))
            {
                Console.WriteLine("Change detected, invoking change...");
                CssClass = "";
                Console.WriteLine("reset css");
                await InvokeAsync(StateHasChanged);
                await Task.Delay(500);
                CssClass = "timeago";
                RelativeTime = newRelativeTime;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}

