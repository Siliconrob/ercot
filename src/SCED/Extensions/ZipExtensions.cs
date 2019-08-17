using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using JM.LinqFaster.Parallel;
using SCED.Models;

namespace SCED.Extensions
{
    public static class ZipExtensions
    {
        public static readonly List<string> SCED60SetFileNames = new List<string> { SettlementResource, GeneralResourceFileName };

        private const string GeneralResourceFileName = "60d_SCED_Gen_Resource_Data";
        private const string SettlementResource = "60d_SCED_SMNE_GEN_RES";

        private static async Task<byte[]> ReadFileDataAsync(this byte[] zipFileData, string fileName)
        {
            zipFileData = zipFileData ?? new byte[] { };
            using (var memStream = new MemoryStream(zipFileData))
            using (var file = new ZipArchive(memStream))
            {
                var matchedEntry = file.Entries.FirstOrDefault(z => z.Name.Like($"%{fileName}%"));
                return matchedEntry != null ? await matchedEntry.ReadContentsAsync() : new byte[] {};
            }
        }

        private static async Task<byte[]> ReadContentsAsync(this ZipArchiveEntry archive)
        {
            using (var destMem = new MemoryStream())
            using (var reader = archive.Open())
            {
                await reader.CopyToAsync(destMem);
                return destMem.ToArray();
            }
        }

        public static async Task<List<SettlementRecord>> ReadSettlements(this byte[] zipFileData)
        {
            zipFileData = zipFileData ?? new byte[] { };
            var SCED60Day = new
            {
                General = new MemoryStream(await zipFileData.ReadFileDataAsync(GeneralResourceFileName))
                    .Records<GeneralRaw>(new GeneralRawMap()),
                Settlement =
                    new MemoryStream(await zipFileData.ReadFileDataAsync(SettlementResource)).Records<SettlementRaw>(
                        new SettlementRawMap())
            };

            var records = SCED60Day.Settlement.SelectP(z => new SettlementRecord
            {
                UtilityName = z.ResourceCode,
                PowerMW = z.IntervalValue,
                PeriodEnd = z.IntervalTime
            }, 1000);

            records.SelectInPlaceP(z =>
            {
                var energySource = SCED60Day.General.WhereSelectP(
                        a => a.ResourceName == z.UtilityName && a.TimeStamp == z.PeriodEnd, raw => raw?.ResourceType, 1000)
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(energySource))
                {
                    z.EnergySource = energySource;
                }
                return z;
            }, 1000);
            var filtered = records.WhereP(z => !string.IsNullOrWhiteSpace(z.EnergySource));
            return filtered;
        }

        private static List<T> Records<T>(this Stream input, ClassMap mapping)
        {
            using (var csv = new CsvReader(new StreamReader(input)))
            {
                csv.Configuration.RegisterClassMap(mapping);
                return csv.GetRecords<T>().ToList();
            }
        }

        private class GeneralRaw
        {
            public string ResourceName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string ResourceType { get; set; }
        }
        private sealed class GeneralRawMap : ClassMap<GeneralRaw>
        {
            public GeneralRawMap()
            {
                Map(m => m.TimeStamp).ConvertUsing(row => DateTime.Parse(row.GetField("SCED Time Stamp")).Round(TimeSpan.FromMinutes(1)).ToUniversalTime());
                Map(m => m.ResourceName).Name("Resource Name");
                Map(m => m.ResourceType).Name("Resource Type");
            }
        }
        
        private class SettlementRaw
        {
            public DateTime IntervalTime { get; set; }
            public int IntervalNumber { get; set; }
            public string ResourceCode { get; set; }
            public double IntervalValue { get; set; }
        }
        
        private sealed class SettlementRawMap : ClassMap<SettlementRaw>
        {
            public SettlementRawMap()
            {
                Map(m => m.IntervalTime).ConvertUsing(row => DateTime.Parse(row.GetField("Interval Time")).Round(TimeSpan.FromMinutes(1)).ToUniversalTime());
                Map(m => m.IntervalNumber).Name("Interval Number");
                Map(m => m.ResourceCode).Name("Resource Code");
                Map(m => m.IntervalValue).Name("Interval Value");
            }
        }
    }
}