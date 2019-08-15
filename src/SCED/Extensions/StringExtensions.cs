using System;

namespace SCED.Extensions
{
    public static class StringExtensions
    {
        internal static bool Like(this string input, string match)
        {
            input = input ?? "";
            match = match ?? "";

            var searchOpts = new
            {
                Begin = match.StartsWith("%"),
                End = match.EndsWith("%")
            };
            if (searchOpts.Begin && searchOpts.End)
            {
                match = match.Remove(0, 1);
                match = match.Length == 0 ? match : match.Remove(match.Length - 1, 1);
                return input.Contains(match, StringComparison.OrdinalIgnoreCase);
            }
            if (searchOpts.Begin)
            {
                match = match.Remove(0, 1);
                return input.StartsWith(match, StringComparison.OrdinalIgnoreCase);
            }
            if (!searchOpts.End)
            {
                return input.Equals(match, StringComparison.OrdinalIgnoreCase);
            }
            match = match.Remove(match.Length - 1, 1);
            return input.EndsWith(match, StringComparison.OrdinalIgnoreCase);
        }
    }
}