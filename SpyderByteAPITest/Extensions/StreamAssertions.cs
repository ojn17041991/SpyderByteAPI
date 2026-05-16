namespace SpyderByteTest.Extensions
{
    public static class StreamAssertions
    {
        public static byte[] GetBytes(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
