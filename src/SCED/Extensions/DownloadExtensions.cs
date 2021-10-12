using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;

namespace SCED.Extensions
{
    public static class DownloadExtensions
    {
        public static string SaveDirectory { get; set; } = Path.GetTempPath();

        private static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName ?? "", (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static async Task<byte[]> GetDataAsync(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new byte[] { };
            }
            var request = new FlurlRequest(url);
            using var response = await request.SendAsync(HttpMethod.Get);
            await using var httpStream = await response.GetStreamAsync();
            await using var memStream = new MemoryStream();
            await httpStream.CopyToAsync(memStream);
            return memStream.ToArray();
        }

        public static async Task<FileInfo> SaveReportAsync(this string url, string fileNameToSave = null)
        {
            var cleanedFileName = fileNameToSave.CleanFileName();
            if (string.IsNullOrWhiteSpace(cleanedFileName))
            {
                return await DownloadReportAsync(url);
            }
            var targetFileName = Path.Combine(SaveDirectory, cleanedFileName);
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }
            return await DownloadReportAsync(url, cleanedFileName);
        }

        private static async Task<FileInfo> DownloadReportAsync(string url, string targetFileName = null)
        {
            return string.IsNullOrWhiteSpace(url)
                ? DefaultFileInfo.Value
                : new FileInfo(await url.DownloadFileAsync(SaveDirectory, targetFileName));
        }

        public static class DefaultFileInfo
        {
            public static readonly FileInfo Value = new("null");
        }
    }
}