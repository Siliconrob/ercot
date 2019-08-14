using System.Threading.Tasks;
using Xunit;

namespace SCEDReader.Tests
{
    public class ReaderTests
    {
        [Theory]
        [InlineData("http://mis.ercot.com/misdownload/servlets/mirDownload?mimic_duns=&doclookupId=673890346")]
        public async Task DownloadFile(string url)
        {
            var savedFile = await url.SaveReportAsync("abc");
            Assert.NotNull(savedFile);
            Assert.True(savedFile.Exists);
        }

        [Theory]
        [InlineData("http://mis.ercot.com/misdownload/servlets/mirDownload?mimic_duns=&doclookupId=673890346")]
        public async Task Unzip(string url)
        {
            var savedFile = await url.SaveReportAsync(ReportsPage.ExtractId(url));
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
            var listings = await ReportsPage.CurrentAvailableAsync();
            Assert.NotNull(listings);
            foreach (var listing in listings)
            {
                Assert.NotNull(listing);
            }
        }

        [Fact]
        public async Task DownloadAllCurrent()
        {
            var reportFiles = await ReportsPage.DownloadAllAsync();
            Assert.NotNull(reportFiles);
            foreach (var reportFile in reportFiles)
            {
                Assert.NotNull(reportFile);
                Assert.True(reportFile.Exists);
            }
        }

    }
}