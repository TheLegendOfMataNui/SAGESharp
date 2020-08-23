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
                                    X = 0.20935058508f, // 0x1ACC in binary
                                    Y = 0.209869383906f, // 0x1ADD in binary
                                    Z = 0.210388182732f, // 0x1AEE in binary
                                    W = 0.210906981558f // 0x1AFF in binary
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
                                    X = 0.013519287054f, // 0x01BB in binary
                                    Y = 0.01403808588f, // 0x01CC in binary
                                    Z = 0.014556884706f, // 0x01DD in binary
                                    W = 0.015075683532f, // 0x01EE in binary
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x02AA,
                                    X = 0.021331787022f, // 0x02BB in binary
                                    Y = 0.021850585848f, // 0x02CC in binary
                                    Z = 0.022369384674f, // 0x02DD in binary
                                    W = 0.0228881835f, // 0x02EE in binary
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
                                    X = 0.138519286542f, // 0x11BB in binary
                                    Y = 0.139038085368f, // 0x11CC in binary
                                    Z = 0.139556884194f, // 0x11DD in binary
                                    W = 0.14007568302f, // 0x11EE in binary
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
                                    X = 0.26351928603f, // 0x21BB in binary
                                    Y = 0.264038084856f, // 0x21CC in binary
                                    Z = 0.264556883682f, // 0x21DD in binary
                                    W = 0.265075682508f, // 0x21EE in binary
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x22AA,
                                    X = 0.271331785998f, // 0x22BB in binary
                                    Y = 0.271850584824f, // 0x22CC in binary
                                    Z = 0.27236938365f, // 0x22DD in binary
                                    W = 0.272888182476f, // 0x22EE in binary
                                },
                                new TCBQuaternionData()
                                {
                                    Keyframe = 0x23AA,
                                    X = 0.279144285966f, // 0x23BB in binary
                                    Y = 0.279663084792f, // 0x23CC in binary
                                    Z = 0.280181883618f, // 0x23DD in binary
                                    W = 0.280700682444f // 0x23EE in binary
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
