using System;
using System.Text.RegularExpressions;

namespace RDumont.Frankie.Core.Liquid
{
    public static class Filters
    {
        public static string DateNet(DateTime date, string format)
        {
            return date.ToString(format);
        }

        public static string IsoDate(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string Summary(string body, int limit)
        {
            var matches = Regex.Matches(body, @"<p.*?>(.+?)</p>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (matches.Count == 0) return "";

            // get text and remove line breaks
            var firstParagraph = matches[0].Groups[1].Value
                .Trim('\r', '\n', ' ')
                .Replace("\r", " ")
                .Replace("\n", " ");

            // remove html
            firstParagraph = Regex.Replace(firstParagraph, @"<.+?>", "");

            // remove space excess
            firstParagraph = Regex.Replace(firstParagraph, @"\s+", " ");

            // only break on the next space, so no words get cut
            var safeLimit = firstParagraph.IndexOfAny(new[] {' ', '\r', '\n'},
                Math.Min(limit, firstParagraph.Length));

            return (safeLimit < 0
                ? firstParagraph
                : firstParagraph.Substring(0, safeLimit))
                + " (...)";
        }
    }
}
