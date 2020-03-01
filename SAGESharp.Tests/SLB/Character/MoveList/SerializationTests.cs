/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB.Character.MoveList;

namespace SAGESharp.Tests.SLB.Character.MoveList
{
    class SerializationTests
    {
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Binary_Move_List_Table_File_Successfully(SerializationTestCaseData<MoveListTable> testCaseData)
            => SerializationTestCase<MoveListTable>.TestReadingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_An_Move_List_Table_To_A_Binary_File_Successfully(SerializationTestCaseData<MoveListTable> testCaseData)
            => SerializationTestCase<MoveListTable>.TestWritingBinarySLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Yaml_Move_List_Table_File_Successfully(SerializationTestCaseData<MoveListTable> testCaseData)
            => SerializationTestCase<MoveListTable>.TestReadingYamlSLBFile(testCaseData);

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Writing_A_Yaml_Move_List_Table_File_Successfully(SerializationTestCaseData<MoveListTable> testCaseData)
            => SerializationTestCase<MoveListTable>.TestWritingYamlSLBFile(testCaseData);

        static SerializationTestCaseData<MoveListTable>[] TEST_CASES() => new SerializationTestCaseData<MoveListTable>[]
        {
            new SerializationTestCaseData<MoveListTable>(
                description: "Test serializing with an empty file",
                testFilePath: PathForTestFile(nameof(TestData.EmptyMoveListTable)),
                expectedProvider: TestData.EmptyMoveListTable
            ),
            new SerializationTestCaseData<MoveListTable>(
                description: "Test serializing a file with a simple move list table",
                testFilePath: PathForTestFile(nameof(TestData.SimpleMoveListTable)),
                expectedProvider: TestData.SimpleMoveListTable
            ),
            new SerializationTestCaseData<MoveListTable>(
                description: "Test serializing a file with a complex move list table",
                testFilePath: PathForTestFile(nameof(TestData.ComplexMoveListTable)),
                expectedProvider: TestData.ComplexMoveListTable
            )
        };

        private static string PathForTestFile(string fileName)
            => SerializationTestCase.PathForTestFile("Character", "MoveList", fileName);
    }
}
