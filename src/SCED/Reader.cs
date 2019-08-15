using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using HtmlAgilityPack;
using SCED.Extensions;

namespace SCED
{
    public static class Reader
    {
        public static string ExtractId(string downloadUrl)
        {
            var url = new Url(downloadUrl);
            var queryString = url.QueryParams;
            var reportId = queryString.FirstOrDefault(a => a.Name.Equals("doclookupId", StringComparison.OrdinalIgnoreCase));
            return reportId == null ? "" : $"SCED60Day_{reportId.Value}.zip";
        }

        public static string HistoricDisclosureLink { get; set; } =
            "http://mis.ercot.com/misapp/GetReports.do?reportTypeId=13052";

        public static Func<HtmlDocument, List<string>> ExtractFn { get; set; } = document =>
        {
            var uri = new Uri(HistoricDisclosureLink);
            var baseUrl = uri.AbsoluteUri.Replace(uri.PathAndQuery, "");
            return document.DocumentNode.SelectNodes("//tr").Select(tableRow => tableRow.ExtractListing(baseUrl))
                .Where(listing => listing != null).ToList();
        };

        public static async Task<IEnumerable<FileInfo>> DownloadAllAsync(Func<string, string> targetFileNameFn = null)
        {
            var saveFileNameFn = targetFileNameFn ?? (s => null);
            var savedFiles = new List<FileInfo>();
            foreach (var reportLink in await CurrentAvailableAsync())
            {
                savedFiles.Add(await reportLink.SaveReportAsync(saveFileNameFn(reportLink)));
            }
            return savedFiles.ToArray();
        }
        public static Func<Task<HtmlDocument>> ReadDocumentAsync { get; set; }

        public static async Task<List<string>> CurrentAvailableAsync()
        {
            var getHtmlFn = ReadDocumentAsync ?? (async () =>
            {
                var web = new HtmlWeb();
                return await web.LoadFromWebAsync(HistoricDisclosureLink);
            });
            return ExtractFn(await getHtmlFn());
        }

        private static string ExtractListing(this HtmlNode tableRow, string baseUrl)
        {
            string listing = null;
            foreach (var cell in tableRow.ChildNodes)
            {
                if (!cell.HasChildNodes)
                {
                    continue;
                }
                var links = cell.SelectNodes(".//a");
                if (links == null)
                {
                    continue;
                }
                var relativePath = links.SelectMany(a => a.Attributes.Select(z => z.DeEntitizeValue)).FirstOrDefault();
                listing = $"{baseUrl}{relativePath}";
            }
            return listing;
        }
    }
}