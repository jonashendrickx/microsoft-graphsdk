using System.IO;
using System.IO.Compression;
using GraphSdkDemo.Contracts.Utility.Archiver;
using GraphSdkDemo.Tests.Common.Constants;
using GraphSdkDemo.Utility;
using NUnit.Framework;

namespace GraphSdkDemo.Tests.Utility
{
    public class ArchiverTests
    {
        [Test]
        [Category(Categories.UnitTesting)]
        public void Archive_Returns_ArchiveContent_When_Successful()
        {
            // Arrange
            var input = new FileInputModel[]
            {
                new FileInputModel
                {
                    Content = new byte[] { 0x01, 0x01, 0x01 },
                    Name = "File1.txt"
                },
                new FileInputModel
                {
                    Content = new byte[] { 0x02, 0x02 },
                    Name = "File2.txt"
                }
            };

            // Act
            var result = Archiver.Archive(input);

            // Assert
            Assert.IsNotNull(result);
            using var zippedStream = new MemoryStream(result.Content);
            using ZipArchive zipArchive = new ZipArchive(zippedStream);
            
            var zipEntry1 = zipArchive.GetEntry("File1.txt");
            Assert.IsNotNull(zipEntry1);
            using var unzippedEntryStream1 = zipEntry1.Open();
            using var mse1 = new MemoryStream();
            unzippedEntryStream1.CopyTo(mse1);
            var unzippedArray1 = mse1.ToArray();
            Assert.AreEqual(new byte[] { 0x01, 0x01, 0x01 }, unzippedArray1);

            var zipEntry2 = zipArchive.GetEntry("File2.txt");
            Assert.IsNotNull(zipEntry2);
            using var unzippedEntryStream2 = zipEntry2.Open();
            using var mse2 = new MemoryStream();
            unzippedEntryStream2.CopyTo(mse2);
            var unzippedArray2 = mse2.ToArray();
            Assert.AreEqual(new byte[] { 0x02, 0x02 }, unzippedArray2);
        }
    }
}
