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
                    // IntegerLength = 1
                    Length = 0.01666667f
                }
            ),
            new SerializationTestCaseData<BKD>(
                description: "Test serializing a file with a simple BKD (1 entry of each)",
                testFilePath: PathForTestFile("SimpleBKD.bkd"),
                expectedProvider: () => new BKD()
                {
                    // IntegerLength = 522
                    Length = 8.700001f,
                    Entries = new List<BKDEntry>()
                    {
                        new BKDEntry()
                        {
                            Id = 1,
                            RotationData = new List<TCBQuaternionData>()
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
                            TranslationData = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x11111111,
                                    X = 2.5f,
                                    Y = 3.5f,
                                    Z = -0.875f
                                }
                            },
                            ScalingData = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x22222222,
                                    X = -49.5f,
                                    Y = 4.6875f,
                                    Z = -75.25f
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
                    // IntegerLength = 312,
                    Length = 5.20000029f,
                    Entries = new List<BKDEntry>()
                    {
                        new BKDEntry()
                        {
                            Id = 0x0A
                        },
                        new BKDEntry()
                        {
                            Id = 0x0B,
                            RotationData = new List<TCBQuaternionData>()
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
                            RotationData = new List<TCBQuaternionData>()
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
                            TranslationData = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x11121314,
                                    X = 11f,
                                    Y = 12f,
                                    Z = 13f
                                },
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x21222324,
                                    X = 21f,
                                    Y = 22f,
                                    Z = 23f,
                                },
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x31323334,
                                    X = 31f,
                                    Y = 32f,
                                    Z = 33f,
                                }
                            }
                        },
                        new BKDEntry()
                        {
                            Id = 0x0D,
                            RotationData = new List<TCBQuaternionData>()
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
                            TranslationData = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x41424344,
                                    X = 41f,
                                    Y = 42f,
                                    Z = 43f
                                }
                            },
                            ScalingData = new List<TCBInterpolationData>()
                            {
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x51525354,
                                    X = 51f,
                                    Y = 52f,
                                    Z = 53f
                                },
                                new TCBInterpolationData()
                                {
                                    Keyframe = 0x61626364,
                                    X = 61f,
                                    Y = 62f,
                                    Z = 63f
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
