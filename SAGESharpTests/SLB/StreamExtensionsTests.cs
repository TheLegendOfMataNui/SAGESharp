/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
using System.IO;

namespace SAGESharp.SLB
{
    class StreamExtensionsTessts
    {
        private readonly Stream stream = Substitute.For<Stream>();

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_WriteInt()
        {
            stream.WriteInt(0x44332211);

            stream.Received().Write(Matcher.ForEquivalentArray(new byte[] { 0x11, 0x22, 0x33, 0x44 }), 0, 4);
        }

        [Test]
        public void Test_WriteUInt()
        {
            stream.WriteUInt(0xDDCCBBAA);

            stream.Received().Write(Matcher.ForEquivalentArray(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }), 0, 4);
        }
    }
}
