using System;
using bzBencode;
using Xunit;

namespace bzEncode.Test
{
    public class BDictTests
    {
        [Fact]
        public void Encode_WritesKeysInSortedOrder_RegardlessOfInsertionOrder()
        {
            var dict = new BDict
            {
                ["zebra"] = new BInt(1),
                ["apple"] = new BInt(2),
                ["mango"] = new BInt(3)
            };

            var encoded = BencodingUtils.EncodeString(dict);

            Assert.Equal("d5:applei2e5:mangoi3e5:zebrai1ee", encoded);
        }

        [Fact]
        public void CalculateTorrentInfoHash_IsIndependentOfKeyInsertionOrder()
        {
            var inOrder = new BDict
            {
                ["length"] = new BInt(1024),
                ["name"] = new BString("file.bin"),
                ["piece length"] = new BInt(16384)
            };

            var outOfOrder = new BDict
            {
                ["piece length"] = new BInt(16384),
                ["name"] = new BString("file.bin"),
                ["length"] = new BInt(1024)
            };

            var hash1 = BencodingUtils.CalculateTorrentInfoHash(inOrder);
            var hash2 = BencodingUtils.CalculateTorrentInfoHash(outOfOrder);

            Assert.Equal(Convert.ToHexString(hash1), Convert.ToHexString(hash2));
        }
    }
}
