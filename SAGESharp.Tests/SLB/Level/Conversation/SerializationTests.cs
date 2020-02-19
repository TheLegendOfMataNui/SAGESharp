/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.IO.Binary;
using SAGESharp.IO.Yaml;
using SAGESharp.SLB.Level.Conversation;
using System.IO;
using YamlDotNet.Serialization;

namespace SAGESharp.Tests.SLB.Level.Conversation
{
    class SerializationTests
    {
        #region Binary SLB
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Binary_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
        {
            var serializer = BinarySerializer.ForType<CharacterTable>();

            using (var stream = new FileStream(testCaseData.SLBFilePath, FileMode.Open))
            {
                var reader = Reader.ForStream(stream);

                serializer
                    .Read(reader)
                    .Should()
                    .Be(testCaseData.Expected);
            }
        }

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Conversation_To_A_Binary_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
        {
            var serializer = BinarySerializer.ForType<CharacterTable>();
            var outputFilePath = $"{testCaseData.SLBFilePath}.tst";

            using (var stream = new FileStream(outputFilePath, FileMode.Create))
            {
                var writer = Writer.ForStream(stream);

                serializer.Write(writer, testCaseData.Expected);
            }

            var actual = File.ReadAllBytes(outputFilePath);
            var expected = File.ReadAllBytes(testCaseData.SLBFilePath);
            
            actual.Should().Equal(expected);
        }
        #endregion

        #region Yaml
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Yaml_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
        {
            IDeserializer deserializer = YamlDeserializer.BuildSLBDeserializer();
            string fileContent = File.ReadAllText(testCaseData.YamlFilePath);

            CharacterTable result = deserializer.Deserialize<CharacterTable>(fileContent);

            result.Should().Be(testCaseData.Expected);
        }

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Yaml_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
        {
            ISerializer serializer = YamlSerializer.BuildSLBSerializer();

            string result = serializer.Serialize(testCaseData.Expected);
            string expectedFile = File.ReadAllText(testCaseData.YamlFilePath);

            result.Should().Be(expectedFile);
        }
        #endregion

        static SerializationTestCaseData<CharacterTable>[] TEST_CASES() => new SerializationTestCaseData<CharacterTable>[]
        {
            new SerializationTestCaseData<CharacterTable>(
                description: "Test serializing with an empty file",
                testFilePath: PathForTestFile("EmptyConversation"),
                expectedProvider: TestData.EmptyCharacterTable
            ),
            new SerializationTestCaseData<CharacterTable>(
                description: "Test serializing a file with a simple conversation",
                testFilePath: PathForTestFile("SimpleConversation"),
                expectedProvider: TestData.SimpleCharacterTable
            ),
            new SerializationTestCaseData<CharacterTable>(
                description: "Test serializing a file with a complex conversation",
                testFilePath: PathForTestFile("ComplexConversation"),
                expectedProvider: TestData.ComplexCharacterTable
            )
        };

        private static string PathForTestFile(string fileName)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "SLB", "Level", "Conversation", fileName);
    }
}
