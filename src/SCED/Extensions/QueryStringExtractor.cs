using System;
using System.Linq;
using Flurl;

namespace SCED
{
    public static class QueryStringExtractor
    {
        public static string ExtractKey(string downloadUrl, string id = "doclookupId")
        {
            var url = new Url(downloadUrl);
            var queryString = url.QueryParams;
            var reportId = queryString.FirstOrDefault(a => a.Name.Equals(id, StringComparison.OrdinalIgnoreCase));
            return reportId == null ? "" : $"SCED60Day_{reportId.Value}.zip";
        }
    }
}