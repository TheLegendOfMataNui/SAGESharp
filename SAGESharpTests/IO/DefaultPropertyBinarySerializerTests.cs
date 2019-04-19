using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System.Reflection;

namespace SAGESharp.IO
{
    class DefaultPropertyBinarySerializerTests
    {
        private readonly IBinarySerializer<string> serializer = Substitute.For<IBinarySerializer<string>>();

        private readonly IPropertyBinarySerializer<Class> propertySerializer;

        public DefaultPropertyBinarySerializerTests()
        {
            PropertyInfo propertyInfo = typeof(Class)
                .GetProperty(nameof(Class.Property));

            propertySerializer = new DefaultPropertyBinarySerializer<Class, string>(serializer, propertyInfo);
        }

        [SetUp]
        public void Setup()
        {
            serializer.ClearSubstitute();
        }

        [TestCase]
        public void Test_Reading_A_Property_Into_An_Object()
        {
            var obj = new Class();
            var expected = "expected";
            var reader = Substitute.For<IBinaryReader>();

            serializer.Read(reader).Returns(expected);

            propertySerializer.ReadAndSet(reader, obj);

            obj.Property
                .Should()
                .Be(expected);
        }

        class Class
        {
            public string Property { get; set; }
        }
    }
}
