/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.IO.Binary;
using SAGESharp.IO.Yaml;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace SAGESharp.Tests.SLB
{
    static class SerializationTestCase
    {
        public static string PathForTestFile(params string[] path)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "SLB", Path.Combine(path));
    }

    static class SerializationTestCase<T> where T : IEquatable<T>
    {
        #region Binary SLB
        public static void TestReadingBinarySLBFile(SerializationTestCaseData<T> testCaseData)
        {
            var serializer = BinarySerializer.ForType<T>();

            using (var stream = new FileStream(testCaseData.SLBFilePath, FileMode.Open))
            {
                var reader = Reader.ForStream(stream);

                serializer
                    .Read(reader)
                    .Should()
                    .Be(testCaseData.Expected);
            }
        }

        public static void TestWritingBinarySLBFile(SerializationTestCaseData<T> testCaseData)
        {
            var serializer = BinarySerializer.ForType<T>();
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
        public static void TestReadingYamlSLBFile(SerializationTestCaseData<T> testCaseData)
        {
            IDeserializer deserializer = YamlDeserializer.BuildSLBDeserializer();
            string fileContent = File.ReadAllText(testCaseData.YamlFilePath);

            T result = deserializer.Deserialize<T>(fileContent);

            result.Should().Be(testCaseData.Expected);
        }

        public static void TestWritingYamlSLBFile(SerializationTestCaseData<T> testCaseData)
        {
            ISerializer serializer = YamlSerializer.BuildSLBSerializer();

            string result = serializer.Serialize(testCaseData.Expected).Replace("\r", string.Empty);
            string expectedFile = File.ReadAllText(testCaseData.YamlFilePath).Replace("\r", string.Empty);

            result.Should().Be(expectedFile);
        }
        #endregion
    }
}
