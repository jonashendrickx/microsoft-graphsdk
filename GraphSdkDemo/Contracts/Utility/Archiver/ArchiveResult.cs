using System;

namespace GraphSdkDemo.Contracts.Utility.Archiver
{
    public class ArchiveResult
    {
        public ArchiveResult(byte[] content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public byte[] Content { get; }
    }
}
