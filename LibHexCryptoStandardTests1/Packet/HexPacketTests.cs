using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibHexCryptoStandard.Packet;
using System;
using System.Collections.Generic;
using System.Text;
using LibHexCryptoStandard.Packet.AES;
using LibNet.Meeting.Packets;

namespace LibHexCryptoStandard.Packet.Tests
{
    [TestClass()]
    public class HexPacketTests
    {
        [TestMethod()]
        public void GetNullTest()
        {
            var bytes = HexPacketConstants.nullBytes;
            Assert.IsNotNull(bytes);
        }

        [TestMethod()]
        public void GetPacketFromArrTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSizeFromTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HexPacketTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DecryptTest()
        {
            Assert.Fail();
        }
    }
}