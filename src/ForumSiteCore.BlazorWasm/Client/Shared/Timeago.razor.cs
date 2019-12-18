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
        private Timer _timer = new Timer();

        public string RelativeTime { get; set; }
       
        [Parameter]
        public DateTimeOffset Date
        {
            get
            {
                return _date;
            }
            set
            {                
                _timer.Stop();
                Console.WriteLine(value);
                _date = value;
                RelativeTime = CalculateTimeago(_date);
                _timer.Start();
            }
        }

        protected override void OnInitialized()
        {
            _timer = new Timer();
            var nextInterval = CalculateTimeToNextInterval();
            Console.WriteLine(TimeSpan.FromMilliseconds(nextInterval));
            _timer.Interval = nextInterval;
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            Console.WriteLine("timeago initialized");
            _timer.Start();
        }
        private const int TenSeconds = 10;
        private const int OneMinute = 60;
        private const int OneHour = 60 * 60;
        private const int OneDay = 60 * 60 * 24;
        private const int OneMonth = 60 * 60 * 24 * 30;
        private const int OneYear = 60 * 60 * 24 * 365;

        private double CalculateTimeToNextInterval()
        {
            Console.WriteLine(DateTimeOffset.Now);
            var dtNow = DateTimeOffset.Now;

            // timespan between right now and when the item was created
            var ts = dtNow.Subtract(Date);
            Console.WriteLine("rightnow vs when created:" + ts);
            // seconds between when right now and when item was created
            var delta = ts.TotalSeconds;
            if (delta < OneMinute)
            {
                var nextInterval = TimeSpan.FromSeconds(TenSeconds).TotalMilliseconds;
                return nextInterval;
            }
            if (delta < OneHour) 
            {
                var nextInterval = TimeSpan.FromSeconds(OneMinute).TotalMilliseconds;
                return nextInterval;
            }
            if (delta < OneDay) 
            {
                var nextInterval = TimeSpan.FromSeconds(OneDay).TotalMilliseconds;
                return nextInterval;
            }
            if (delta < OneMonth) // 30 * 24 * 60 * 60
            {
                var nextInterval = Date.AddSeconds(OneDay).Subtract(dtNow).TotalMilliseconds;
                return (nextInterval > int.MaxValue) ? int.MaxValue : nextInterval;
            }
            if (delta < OneYear) // 12 * 30 * 24 * 60 * 60
            {                
                //var nextInterval = Date.AddSeconds(OneYear).Subtract(dtNow).TotalMilliseconds;
                //return (nextInterval > int.MaxValue) ? int.MaxValue : nextInterval;
                return int.MaxValue;
            }

            return int.MaxValue;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer elapsed!!");
            RelativeTime = CalculateTimeago(Date);
            var nextInterval = CalculateTimeToNextInterval();
            Console.WriteLine(nextInterval);
            _timer.Interval = nextInterval;
            await InvokeAsync(StateHasChanged);
        }

        private string CalculateTimeago(DateTimeOffset dt)
        {
            var ts = new TimeSpan(DateTimeOffset.Now.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}

