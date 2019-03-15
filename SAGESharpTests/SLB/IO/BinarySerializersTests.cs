using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace SAGESharp.SLB.IO
{
    class BinarySerializersTests
    {
        [TestCase]
        public void Test_LisBinarySerializer_Can_Serialize_All_List_Types()
            => new ListBinarySerializer<int>(Substitute.For<IBinarySerializer<int>>())
                .Should()
                .BeAssignableTo<IBinarySerializer<List<int>>>()
                .And
                .BeAssignableTo<IBinarySerializer<IList<int>>>()
                .And
                .BeAssignableTo<IBinarySerializer<IReadOnlyList<int>>>();
    }
}
