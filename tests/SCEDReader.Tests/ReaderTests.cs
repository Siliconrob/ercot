using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JM.LinqFaster.Parallel;
using SCED;
using SCED.Extensions;
using Xunit;

namespace SCEDReader.Tests
{
    public class ReaderTests
    {
        [Fact]
        public async Task ReadSettlementData()
        {
            var latestListing = (await SettlementReader.CurrentAvailableAsync()).First();
            Assert.NotNull(latestListing);
            var data = await latestListing.GetDataAsync();
            Assert.NotEmpty(data);
            var settlements = await data.ReadSettlements();
            Assert.All(settlements, Assert.NotNull);
            var nuclearPowerRecords = settlements.WhereP(z => z.EnergySource.Equals("NUC", StringComparison.OrdinalIgnoreCase));
            Assert.NotEmpty(nuclearPowerRecords);
        }


        [Fact]
        public async Task GetFileData()
        {
            var latestListing = (await SettlementReader.CurrentAvailableAsync()).First();
            Assert.NotNull(latestListing);
            var data = await latestListing.GetDataAsync();
            Assert.NotEmpty(data);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.zip");
            await File.WriteAllBytesAsync(tempPath, data);
            var info = new FileInfo(tempPath);
            Assert.True(info.Exists);
        }

        [Fact]
        public async Task DownloadFile()
        {
            var latestListing = (await SettlementReader.CurrentAvailableAsync()).First();
            Assert.NotNull(latestListing);
            var savedFile = await latestListing.SaveReportAsync("abc");
            Assert.NotNull(savedFile);
            Assert.True(savedFile.Exists);
        }

        [Fact]
        public async Task Unzip()
        {
            var latestListing = (await SettlementReader.CurrentAvailableAsync()).First();
            Assert.NotNull(latestListing);
            var savedFile = await latestListing.SaveReportAsync(SettlementReader.ExtractIdFn(latestListing));
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
            var listings = await SettlementReader.CurrentAvailableAsync();
            Assert.NotNull(listings);
            foreach (var listing in listings)
            {
                Assert.NotNull(listing);
            }
        }

        [Fact]
        public async Task DownloadAllCurrent()
        {
            var reportFiles = await SettlementReader.DownloadAllAsync();
            Assert.NotNull(reportFiles);
            foreach (var reportFile in reportFiles)
            {
                Assert.NotNull(reportFile);
                Assert.True(reportFile.Exists);
            }
        }
    }
}