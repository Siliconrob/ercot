using System;
using System.Linq;
using Flurl;

namespace SCED.Extensions
{
    public static class QueryStringExtractor
    {
        public static string ExtractKey(string downloadUrl, QueryStringKeyPattern pattern = null)
        {
            pattern ??= new QueryStringKeyPattern();
            var url = new Url(downloadUrl);
            var queryString = url.QueryParams;
            var reportId = queryString.FirstOrDefault(a => a.Name.Equals(pattern.ExtractionKey ?? "", StringComparison.OrdinalIgnoreCase));
            return reportId.Name == null ? "" : $"{pattern.Prefix}{reportId.Value}{pattern.Suffix}";
        }
    }

    public class QueryStringKeyPattern
    {
        public string ExtractionKey { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }
}