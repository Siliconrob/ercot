using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SCEDReader
{
    public static class ReportsPage
    {
        public static string HistoricDisclosureLink { get; set; } =
            "http://mis.ercot.com/misapp/GetReports.do?reportTypeId=13052";

        public static Func<HtmlDocument, List<SCEDDisclosureLink>> ExtractFn { get; set; } = document =>
        {
            var uri = new Uri(HistoricDisclosureLink);
            var baseUrl = uri.AbsoluteUri.Replace(uri.PathAndQuery, "");
            return document.DocumentNode.SelectNodes("//tr").Select(tableRow => tableRow.ExtractListing(baseUrl))
                .Where(listing => listing != null).ToList();
        };

        public static async Task<List<SCEDDisclosureLink>> Available()
        {
            var web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(HistoricDisclosureLink);
            return ExtractFn(htmlDoc);
        }

        private static SCEDDisclosureLink ExtractListing(this HtmlNode tableRow, string baseUrl)
        {
            SCEDDisclosureLink listing = null;
            foreach (var cell in tableRow.ChildNodes)
            {
                if (cell.HasClass("labelOptional_ind"))
                {
                    listing = new SCEDDisclosureLink
                    {
                        Name = cell.InnerText
                    };
                    continue;
                }
                if (!cell.HasChildNodes || listing == null)
                {
                    continue;
                }
                var links = cell.SelectNodes(".//a");
                if (links == null)
                {
                    continue;
                }
                var relativePath = links.SelectMany(a => a.Attributes.Select(z => z.DeEntitizeValue)).FirstOrDefault();
                listing.Link = $"{baseUrl}{relativePath}";
            }
            return listing;
        }
    }

    public class SCEDDisclosureLink
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
}