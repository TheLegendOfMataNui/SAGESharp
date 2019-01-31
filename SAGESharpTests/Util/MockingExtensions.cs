using Moq.Language;
using SAGESharp.SLB;
using System;

namespace SAGESharpTests.Util
{
    internal static class MockingExtensions
    {
        // To simplify setting up the results of a stream
        public static ISetupSequentialResult<int> ReturnsIntBytes(
            this ISetupSequentialResult<int> setupSequentialResult,
            int value
        ) {
            var bytes = BitConverter.GetBytes(value);

            return setupSequentialResult
                .Returns(bytes[0])
                .Returns(bytes[1])
                .Returns(bytes[2])
                .Returns(bytes[3]);
        }

        // To simplify setting up the results of a stream
        public static ISetupSequentialResult<int> ReturnsASCIIBytes(
            this ISetupSequentialResult<int> setupSequentialResult,
            string values
        ) {
            foreach (var value in values)
            {
                setupSequentialResult.Returns(value.ToASCIIByte());
            }

            return setupSequentialResult;
        }
    }
}
