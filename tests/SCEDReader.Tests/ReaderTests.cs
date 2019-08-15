using System;
using System.IO;
using System.Threading.Tasks;
using SCED;
using SCED.Extensions;
using Xunit;

namespace SCEDReader.Tests
{
    public class ReaderTests
    {
        public const string ValidReportUrl = "http://mis.ercot.com/misdownload/servlets/mirDownload?mimic_duns=&doclookupId=673890346";

        [Theory]
        [InlineData(ValidReportUrl)]
        public async Task ReadSettlementData(string url)
        {
            var data = await url.GetDataAsync();
            Assert.NotEmpty(data);
            var settlements = await data.ReadSettlements();
            Assert.All(settlements, Assert.NotNull);
        }


        [Theory]
        [InlineData(ValidReportUrl)]
        public async Task GetFileData(string url)
        {
            var data = await url.GetDataAsync();
            Assert.NotEmpty(data);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.zip");
            File.WriteAllBytes(tempPath, data);
            var info = new FileInfo(tempPath);
            Assert.True(info.Exists);
        }

        [Theory]
        [InlineData(ValidReportUrl)]
        public async Task DownloadFile(string url)
        {
            var savedFile = await url.SaveReportAsync("abc");
            Assert.NotNull(savedFile);
            Assert.True(savedFile.Exists);
        }

        [Theory]
        [InlineData(ValidReportUrl)]
        public async Task Unzip(string url)
        {
            var savedFile = await url.SaveReportAsync(Reader.ExtractId(url));
            Assert.NotNull(savedFile);
            Assert.True(savedFile.Exists);
            var files = savedFile.UnZip();
            foreach (var extracted in files)
            {
                Assert.NotNull(extracted);
                Assert.True(extracted.Exists);
            }
        }

        [Fact]
        public async Task CurrentListing()
        {
            var listings = await Reader.CurrentAvailableAsync();
            Assert.NotNull(listings);
            foreach (var listing in listings)
            {
                Assert.NotNull(listing);
            }
        }

        [Fact]
        public async Task DownloadAllCurrent()
        {
            var reportFiles = await Reader.DownloadAllAsync();
            Assert.NotNull(reportFiles);
            foreach (var reportFile in reportFiles)
            {
                Assert.NotNull(reportFile);
                Assert.True(reportFile.Exists);
            }
        }
    }
}