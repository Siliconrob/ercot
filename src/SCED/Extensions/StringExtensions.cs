using System;

namespace SCED.Extensions
{
    public static class StringExtensions
    {
        internal static bool Like(this string input, string match)
        {
            input ??= "";
            match ??= "";

            var searchOpts = new
            {
                Begin = match.StartsWith("%"),
                End = match.EndsWith("%")
            };
            switch (searchOpts.Begin)
            {
                case true when searchOpts.End:
                    match = match.Remove(0, 1);
                    match = match.Length == 0 ? match : match.Remove(match.Length - 1, 1);
                    return input.Contains(match, StringComparison.OrdinalIgnoreCase);
                case true:
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