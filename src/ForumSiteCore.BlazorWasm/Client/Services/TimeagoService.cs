using ForumSiteCore.BlazorWasm.Client.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ForumSiteCore.BlazorWasm.Client.Services
{
    public class TimeagoService
    {
        private readonly Timer _timer;
        public EventHandler Broadcast;
        public TimeagoService()
        {
            _timer = new Timer();
            Initialize();
        }

        private void Initialize()
        {
            _timer.Interval = 5 * 1000; // 60 seconds
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            Console.WriteLine("timeago service initialized");
            _timer.Start();
        }

        private void OnBroadcast()
        {
            Console.WriteLine("Broadcasting timeago updates");
            Broadcast?.Invoke(this, EventArgs.Empty);
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnBroadcast();
        }

        public static string CalculateTimeago(DateTimeOffset dt)
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
            if (delta < 3600) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 7200) // 90 * 60
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
