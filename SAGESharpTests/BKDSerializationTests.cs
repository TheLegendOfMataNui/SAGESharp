/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.IO;
using SAGESharp.Testing;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp
{
    class BKDSerializationTests
    {
        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_A_Conversation_File_Successfully(SerializationTestCaseData<BKD> testCaseData)
        {
            // Shouldn't use the serializer directly here
            var serializer = new BinarySerializableSerializer<BKD>();

            using (var stream = new FileStream(testCaseData.TestFilePath, FileMode.Open))
            {
                var reader = Reader.ForStream(stream);

                serializer
                    .Read(reader)
                    .Should()
                    .Be(testCaseData.Expected);
            }
        }

        static object[] TEST_CASES() => new object[]
        {
            new SerializationTestCaseData<BKD>(
                description: "Test serializing with an empty BKD",
                testFilePath: PathForTestFile("EmptyBKD.bkd"),
                expectedProvider: () => new BKD()
                {
                    Length = 1
                }
            ),
            new SerializationTestCaseData<BKD>(
                description: "Test serializing a file with a simple BKD (1 entry of each)",
                testFilePath: PathForTestFile("SimpleBKD.bkd"),
                expectedProvider: () => new BKD()
                {
                    Length = 522,
                    Entries = new List<BKDEntry>()
                    {
                        new BKDEntry()
                        {
                            Id = 1,
                            TCBQuaternionData = new List<TCBQuaternionData>()
                            {
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x1ABB,
                                    Short2 = 0x1ACC,
                                    Short3 = 0x1ADD,
                                    Short4 = 0x1AEE,
                                    Short5 = 0x1AFF
                                }
                            },
                            TCBInterpolatorData1 = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x11111111,
                                    Float1 = 2.5f,
                                    Float2 = 3.5f,
                                    Float3 = -0.875f
                                }
                            },
                            TCBInterpolatorData2 = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x22222222,
                                    Float1 = -49.5f,
                                    Float2 = 4.6875f,
                                    Float3 = -75.25f
                                }
                            }
                        }
                    }
                }
            ),
            new SerializationTestCaseData<BKD>(
                description: "Test serializing a file with a complex BKD (different entry sizes)",
                testFilePath: PathForTestFile("ComplexBKD.bkd"),
                expectedProvider: () => new BKD()
                {
                    Length = 312,
                    Entries = new List<BKDEntry>()
                    {
                        new BKDEntry()
                        {
                            Id = 0x0A
                        },
                        new BKDEntry()
                        {
                            Id = 0x0B,
                            TCBQuaternionData = new List<TCBQuaternionData>()
                            {
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x01AA,
                                    Short2 = 0x01BB,
                                    Short3 = 0x01CC,
                                    Short4 = 0x01DD,
                                    Short5 = 0x01EE
                                },
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x02AA,
                                    Short2 = 0x02BB,
                                    Short3 = 0x02CC,
                                    Short4 = 0x02DD,
                                    Short5 = 0x02EE
                                }
                            }
                        },
                        new BKDEntry()
                        {
                            Id = 0x0C,
                            TCBQuaternionData = new List<TCBQuaternionData>()
                            {
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x11AA,
                                    Short2 = 0x11BB,
                                    Short3 = 0x11CC,
                                    Short4 = 0x11DD,
                                    Short5 = 0x11EE
                                }
                            },
                            TCBInterpolatorData1 = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x11121314,
                                    Float1 = 11f,
                                    Float2 = 12f,
                                    Float3 = 13f
                                },
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x21222324,
                                    Float1 = 21f,
                                    Float2 = 22f,
                                    Float3 = 23f,
                                },
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x31323334,
                                    Float1 = 31f,
                                    Float2 = 32f,
                                    Float3 = 33f,
                                }
                            }
                        },
                        new BKDEntry()
                        {
                            Id = 0x0D,
                            TCBQuaternionData = new List<TCBQuaternionData>()
                            {
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x21AA,
                                    Short2 = 0x21BB,
                                    Short3 = 0x21CC,
                                    Short4 = 0x21DD,
                                    Short5 = 0x21EE
                                },
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x22AA,
                                    Short2 = 0x22BB,
                                    Short3 = 0x22CC,
                                    Short4 = 0x22DD,
                                    Short5 = 0x22EE
                                },
                                new TCBQuaternionData()
                                {
                                    Short1 = 0x23AA,
                                    Short2 = 0x23BB,
                                    Short3 = 0x23CC,
                                    Short4 = 0x23DD,
                                    Short5 = 0x23EE
                                }
                            },
                            TCBInterpolatorData1 = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x41424344,
                                    Float1 = 41f,
                                    Float2 = 42f,
                                    Float3 = 43f
                                }
                            },
                            TCBInterpolatorData2 = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x51525354,
                                    Float1 = 51f,
                                    Float2 = 52f,
                                    Float3 = 53f
                                },
                                new TCBInterpolationData()
                                {
                                    Long1 = 0x61626364,
                                    Float1 = 61f,
                                    Float2 = 62f,
                                    Float3 = 63f
                                }
                            }
                        }
                    }
                }
            )
        };

        private static string PathForTestFile(string fileName)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", fileName);
    }
}
