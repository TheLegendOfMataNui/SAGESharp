/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB.Character.AnimationEvents;

namespace SAGESharp.Tests.SLB.Character.AnimationEvents
{
    class SerializationTests
    {
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Binary_Animation_Event_Table_File_Successfully(SerializationTestCaseData<AnimationEventsTable> testCaseData)
            => SerializationTestCase<AnimationEventsTable>.TestReadingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_An_Animation_Event_Table_To_A_Binary_File_Successfully(SerializationTestCaseData<AnimationEventsTable> testCaseData)
            => SerializationTestCase<AnimationEventsTable>.TestWritingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Yaml_Animation_Event_Table_File_Successfully(SerializationTestCaseData<AnimationEventsTable> testCaseData)
            => SerializationTestCase<AnimationEventsTable>.TestReadingYamlSLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Yaml_Animation_Event_Table_File_Successfully(SerializationTestCaseData<AnimationEventsTable> testCaseData)
            => SerializationTestCase<AnimationEventsTable>.TestWritingBinarySLBFile(testCaseData);

        static SerializationTestCaseData<AnimationEventsTable>[] TEST_CASES() => new SerializationTestCaseData<AnimationEventsTable>[]
        {
            new SerializationTestCaseData<AnimationEventsTable>(
                description: "Test serializing with an empty file",
                testFilePath: PathForTestFile("EmptyAnimationEventsTable"),
                expectedProvider: TestData.EmptyAnimationEventsTable
            ),
            new SerializationTestCaseData<AnimationEventsTable>(
                description: "Test serializing a file with a simple animation events table",
                testFilePath: PathForTestFile("SimpleAnimationEventsTable"),
                expectedProvider: TestData.SimpleAnimationEventsTable
            ),
            new SerializationTestCaseData<AnimationEventsTable>(
                description: "Test serializing a file with a complex animation events table",
                testFilePath: PathForTestFile("ComplexAnimationEventsTable"),
                expectedProvider: TestData.ComplexAnimationEventsTable
            )
        };

        private static string PathForTestFile(string fileName)
            => SerializationTestCase.PathForTestFile("Character", "AnimationEvents", fileName);
    }
}
