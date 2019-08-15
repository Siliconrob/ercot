using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using SCED.Models;

namespace SCED.Extensions
{
    public static class ZipExtensions
    {
        public static List<string> SCED60DaySet = new List<string> { SettlementResource, GeneralResourceFileName };

        private const string GeneralResourceFileName = "60d_SCED_Gen_Resource_Data";
        private const string SettlementResource = "60d_SCED_SMNE_GEN_RES";

        private static async Task<string> ReadFileDataAsync(this byte[] zipFileData, string fileName)
        {
            zipFileData = zipFileData ?? new byte[] { };
            using (var memStream = new MemoryStream(zipFileData))
            using (var file = new ZipArchive(memStream))
            {
                foreach (var entry in file.Entries)
                {
                    if (!entry.Name.Like($"%{fileName}%"))
                    {
                        continue;
                    }
                    using (var destMem = new MemoryStream())
                    using (var reader = entry.Open())
                    {
                        await reader.CopyToAsync(destMem);
                        return Encoding.UTF8.GetString(destMem.ToArray());
                    }
                }
            }
            return "";
        }

        public static async Task<List<SettlementRecord>> ReadSettlements(this byte[] zipFileData)
        {
            zipFileData = zipFileData ?? new byte[] { };
            var SCED60Day = new
            {
                General = await zipFileData.ReadFileDataAsync(GeneralResourceFileName),
                Settlement = await zipFileData.ReadFileDataAsync(SettlementResource)
            };
            return new List<SettlementRecord>();
        }
    }
}