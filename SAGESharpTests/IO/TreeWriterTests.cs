/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;

namespace SAGESharp.IO
{
    class TreeWriterTests
    {
        private readonly IBinaryWriter binaryWriter;

        private readonly ITreeWriter treeWriter;

        public TreeWriterTests()
        {
            binaryWriter = Substitute.For<IBinaryWriter>();
            treeWriter = new TreeWriter();
        }

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }
    }
}
