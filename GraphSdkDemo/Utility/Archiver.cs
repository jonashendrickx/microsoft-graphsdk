using System.IO;
using System.IO.Compression;
using GraphSdkDemo.Contracts.Utility.Archiver;

namespace GraphSdkDemo.Utility
{
    public static class Archiver
    {
        public static ArchiveResult Archive(FileInputModel[] files)
        {
            byte[] zipBytes = null;

            using (var compressedFileStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var file in files)
                    {
                        var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Fastest);
                        using var entryStream = new MemoryStream(file.Content);
                        using var zipEntryStream = zipEntry.Open();
                        entryStream.CopyTo(zipEntryStream);
                    }
                }
                zipBytes = compressedFileStream.ToArray();
            }
            return new ArchiveResult(zipBytes);
        }
    }
}
