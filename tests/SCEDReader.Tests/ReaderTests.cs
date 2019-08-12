using System.Threading.Tasks;
using Xunit;

namespace SCEDReader.Tests
{
    public class ReaderTests
    {
        [Fact]
        public async Task CurrentListing()
        {
            var listings = await ReportsPage.Available();
            Assert.NotNull(listings);
            foreach (var listing in listings)
            {
                Assert.NotNull(listing);
            }
        }
    }
}