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

        public static Int64? ToInt64OrNull(this string input)
        {
            Int64? returnVal = Int64.TryParse(input, out Int64 parsedValue) ? parsedValue : (Int64?)null;

            return returnVal;
        }

        public static DateTimeOffset? ToDateTimeOffsetOrNull(this string input)
        {
            DateTimeOffset? returnVal = DateTimeOffset.TryParse(input, out DateTimeOffset parsedValue) ? parsedValue : (DateTimeOffset?)null;

            return returnVal;
        }

        public static Decimal? ToDecimalOrNull(this string input)
        {
            Decimal? returnVal = Decimal.TryParse(input, out Decimal parsedValue) ? parsedValue : (Decimal?)null;

            return returnVal;
        }
    }
}
