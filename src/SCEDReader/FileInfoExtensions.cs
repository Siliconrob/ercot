using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SCEDReader
{
    public static class FileInfoExtensions
    {
        public static IEnumerable<FileInfo> UnZip(this FileInfo zipFile)
        {
            var unzippedFiles = new List<FileInfo>();
            if (zipFile == null || !zipFile.Exists)
            {
                return unzippedFiles;
            }
            using (var archive = ZipFile.OpenRead(zipFile.FullName))
            {
                foreach (var entry in archive.Entries)
                {
                    var unzipFileLocation = Path.Combine(zipFile.Directory?.FullName, entry.Name);
                    entry.ExtractToFile(unzipFileLocation, true);
                    unzippedFiles.Add(new FileInfo(unzipFileLocation));
                }
            }
            return unzippedFiles.ToList();
        }
    }
}