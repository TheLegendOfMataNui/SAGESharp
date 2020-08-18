/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Animations;
using SAGESharp.IO.Binary;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.Tests.Animations
{
    class BKDSerializationTests
    {
        private readonly IBinarySerializer<BKD> serializer = BinarySerializer.ForBKDFiles;

        [TestCaseSource(nameof(TEST_CASES))]
        public void Test_Reading_BKD_File_Successfully(SerializationTestCaseData<BKD> testCaseData)
        {
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
        public void Test_Writing_BKD_File_Successfully(SerializationTestCaseData<BKD> testCaseData)
        {
            byte[] expected = File.ReadAllBytes(testCaseData.TestFilePath);

            using (var stream = new MemoryStream())
            {
                var writer = Writer.ForStream(stream);

                serializer.Write(writer, testCaseData.Expected);

                stream.ToArray()
                    .Should()
                    .Equal(expected);
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
                                    Keyframe = 0x1ABB,
                                    X = 0x1ACC,
                                    Y = 0x1ADD,
                                    Z = 0x1AEE,
                                    W = 0x1AFF
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
                                    Keyframe = 0x01AA,
                                    X = 0x01BB,
                                    Y = 0x01CC,
                                    Z = 0x01DD,
                                    W = 0x01EE
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x02AA,
                                    X = 0x02BB,
                                    Y = 0x02CC,
                                    Z = 0x02DD,
                                    W = 0x02EE
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
                                    Keyframe = 0x11AA,
                                    X = 0x11BB,
                                    Y = 0x11CC,
                                    Z = 0x11DD,
                                    W = 0x11EE
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
                                    Keyframe = 0x21AA,
                                    X = 0x21BB,
                                    Y = 0x21CC,
                                    Z = 0x21DD,
                                    W = 0x21EE
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x22AA,
                                    X = 0x22BB,
                                    Y = 0x22CC,
                                    Z = 0x22DD,
                                    W = 0x22EE
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x23AA,
                                    X = 0x23BB,
                                    Y = 0x23CC,
                                    Z = 0x23DD,
                                    W = 0x23EE
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
