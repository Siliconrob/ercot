using System.Collections.Generic;
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
                foreach (var entry in archive.Entries)
                {
                    if (!matchSet.Any(z => z.Like($"%{entry.Name}%")))
                    {
                        continue;
                    }
                    var unzipFileLocation = Path.Combine(zipFile.Directory?.FullName, entry.Name);
                    entry.ExtractToFile(unzipFileLocation, true);
                    unzippedFiles.Add(new FileInfo(unzipFileLocation));
                }
            }
            return unzippedFiles.ToList();
        }
    }
}