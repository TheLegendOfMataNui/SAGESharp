/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB.Level.Conversation;

namespace SAGESharp.Tests.SLB.Level.Conversation
{
    class SerializationTests
    {
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Binary_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
            => SerializationTestCase<CharacterTable>.TestReadingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Conversation_To_A_Binary_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
            => SerializationTestCase<CharacterTable>.TestWritingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Yaml_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
            => SerializationTestCase<CharacterTable>.TestReadingYamlSLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Yaml_Conversation_File_Successfully(SerializationTestCaseData<CharacterTable> testCaseData)
            => SerializationTestCase<CharacterTable>.TestWritingYamlSLBFile(testCaseData);

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
            => SerializationTestCase.PathForTestFile("Level", "Conversation", fileName);
    }
}
