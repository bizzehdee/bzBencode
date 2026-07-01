using System.IO;
using bzBencode;
using Xunit;

namespace bzEncode.Test
{
    public class BStringTests
    {
        [Fact]
        public void Decode_ReadsValueAndReportsCorrectBytesConsumed()
        {
            var bytes = BencodingUtils.ExtendedASCIIEncoding.GetBytes("4:spam");

            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream, BencodingUtils.ExtendedASCIIEncoding);

            var bytesConsumed = 0;
            var result = BString.Decode(reader, ref bytesConsumed);

            Assert.Equal("spam", result.Value);
            Assert.Equal(bytes.Length, bytesConsumed);
        }
    }
}
