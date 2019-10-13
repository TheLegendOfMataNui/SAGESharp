/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.IO;
using SAGESharp.Testing;
using System.IO;

namespace SAGESharp.SLB.Level
{
    class SerializationTests
    {
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Conversation_File_Successfully(SerializationTestCaseData<Conversation> testCaseData)
        {
            var serializer = BinarySerializer.ForType<Conversation>();

            using (var stream = new FileStream(testCaseData.TestFilePath, FileMode.Open))
            {
                var reader = Reader.ForStream(stream);

                serializer
                    .Read(reader)
                    .Should()
                    .Be(testCaseData.Expected);
            }
        }

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Conversation_To_A_File_Successfully(SerializationTestCaseData<Conversation> testCaseData)
        {
            var serializer = BinarySerializer.ForType<Conversation>();
            var outputFilePath = $"{testCaseData.TestFilePath}.tst";

            using (var stream = new FileStream(outputFilePath, FileMode.Create))
            {
                var writer = Writer.ForStream(stream);

                serializer.Write(writer, testCaseData.Expected);
            }

            var actual = File.ReadAllBytes(outputFilePath);
            var expected = File.ReadAllBytes(testCaseData.TestFilePath);
            
            actual.Should().Equal(expected);
        }

        static object[] TEST_CASES() => new object[]
        {
            new SerializationTestCaseData<Conversation>(
                description: "Test serializing with an empty file",
                testFilePath: PathForTestFile("EmptyConversation.slb"),
                expectedProvider: TestData.EmptyConversation
            ),
            new SerializationTestCaseData<Conversation>(
                description: "Test serializing a file with a simple conversation",
                testFilePath: PathForTestFile("SimpleConversation.slb"),
                expectedProvider: TestData.SimpleConversation
            ),
            new SerializationTestCaseData<Conversation>(
                description: "Test serializing a file with a complex conversation",
                testFilePath: PathForTestFile("ComplexConversation.slb"),
                expectedProvider: TestData.ComplexConversation
            )
        };

        private static string PathForTestFile(string fileName)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "SLB", "Level", "Conversation", fileName);
    }
}
