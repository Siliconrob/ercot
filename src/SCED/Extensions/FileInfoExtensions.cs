﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SCED.Extensions
{
    public static class FileInfoExtensions
    {
        public static IEnumerable<FileInfo> UnZip(this FileInfo zipFile, IEnumerable<string> inputMatchSet = null)
        {
            var matchSet = (inputMatchSet ?? new List<string>()).ToList();
            if (!matchSet.Any())
            {
                matchSet.AddRange(ZipExtensions.SCED60SetFileNames);
            }
            matchSet = matchSet.Distinct().ToList();

            var unzippedFiles = new List<FileInfo>();
            if (zipFile == null || !zipFile.Exists)
            {
                return unzippedFiles;
            }
            using (var archive = ZipFile.OpenRead(zipFile.FullName))
            {
                unzippedFiles = archive.Entries.Select(z => z.ExtractMatch(zipFile.Directory?.FullName, matchSet))
                    .Where(x => x != null).ToList();
            }
            return unzippedFiles.ToList();
        }

        private static FileInfo ExtractMatch(this ZipArchiveEntry entry, string extractDir, IEnumerable<string> matchSet)
        {
            if (!matchSet.Any(z => z.Like($"%{entry.Name}%")))
            {
                return null;
            }
            var unzipFileLocation = Path.Combine(extractDir, entry.Name);
            entry.ExtractToFile(unzipFileLocation, true);
            return new FileInfo(unzipFileLocation);
        }
    }
}