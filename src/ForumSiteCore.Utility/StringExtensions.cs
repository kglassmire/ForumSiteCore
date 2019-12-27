using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForumSiteCore.Utility
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static long? ToInt64OrNull(this string input)
        {
            long? returnVal = long.TryParse(input, out long parsedValue) ? parsedValue : (long?)null;

            return returnVal;
        }

        public static DateTimeOffset? ToDateTimeOffsetOrNull(this string input)
        {
            DateTimeOffset? returnVal = DateTimeOffset.TryParse(input, out DateTimeOffset parsedValue) ? parsedValue : (DateTimeOffset?)null;

            return returnVal;
        }

        public static decimal? ToDecimalOrNull(this string input)
        {
            decimal? returnVal = decimal.TryParse(input, out decimal parsedValue) ? parsedValue : (decimal?)null;

            return returnVal;
        }
    }
}
