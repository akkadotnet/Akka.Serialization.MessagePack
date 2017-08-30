//-----------------------------------------------------------------------
// <copyright file="MsgPackVersionToleranceTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Text;
using Akka.Serialization.Testkit.Util;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using Xunit;

namespace Akka.Serialization.MessagePack.Tests
{
    public class MsgPackVersionToleranceTests : TestKit.Xunit2.TestKit
    {
        public MsgPackVersionToleranceTests() : base(ConfigFactory.GetConfig(typeof(MsgPackSerializer)))
        {
        }

        [Fact]
        public void Can_deserialize_Uri_message_which_was_serialized_on_full_NET()
        {
            // TypelessClass with System.URI serialized on .NET
            var serializedString = "gaZOZXN0ZWTJAAAAbWTZVVN5c3RlbS5VcmksIFN5c3RlbSwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODm1aHR0cDovL21pY3Jvc29mdC5jb20v";

            var deserialized = MessagePackSerializer.Deserialize<TypelessClass>(
                Convert.FromBase64String(serializedString),
                TypelessContractlessStandardResolver.Instance);
            Assert.Equal(new Uri("http://microsoft.com"), deserialized.Nested);
        }

        [Fact]
        public void Can_deserialize_TimeSpan_message_which_was_serialized_on_full_NET()
        {
            // TypelessClass with System.TimeSpan serialized on .NET
            var serializedString = "gaZOZXN0ZWTJAAAAH2S5U3lzdGVtLlRpbWVTcGFuLCBtc2NvcmxpYs6y0F4A";

            var deserialized = MessagePackSerializer.Deserialize<TypelessClass>(
                Convert.FromBase64String(serializedString),
                TypelessContractlessStandardResolver.Instance);
            Assert.Equal(TimeSpan.FromMinutes(5), deserialized.Nested);
        }

        [Fact]
        public void Can_ignore_unexpected_data()
        {
            var address2 = new AddressV2 { City = "New York", Country = "USA", Street = "Jr. Someone" };

            byte[] serialized = MessagePackSerializer.Serialize(address2, StandardResolver.Instance);
            var deserialized = MessagePackSerializer.Deserialize<AddressV1>(serialized, StandardResolver.Instance);

            deserialized.Street.Should().Be(address2.Street);
            deserialized.City.Should().Be(address2.City);
        }

        [Fact]
        public void Can_tolerate_with_missing_data()
        {
            var address2 = new AddressV1 { City = "New York", Street = "Jr. Someone" };

            byte[] serialized = MessagePackSerializer.Serialize(address2, StandardResolver.Instance);
            var deserialized = MessagePackSerializer.Deserialize<AddressV2>(serialized, StandardResolver.Instance);

            deserialized.Street.Should().Be(address2.Street);
            deserialized.City.Should().Be(address2.City);
            deserialized.Country.Should().BeNull();
        }

        [Fact]
        public void Can_tolerate_with_missing_data_with_defaults()
        {
            var address2 = new AddressV1 { City = "New York", Street = "Jr. Someone" };

            byte[] serialized = MessagePackSerializer.Serialize(address2, StandardResolver.Instance);
            var deserialized = MessagePackSerializer.Deserialize<AddressV3>(serialized, StandardResolver.Instance);

            deserialized.Street.Should().Be(address2.Street);
            deserialized.City.Should().Be(address2.City);
            deserialized.Country.Should().Be("Japan");
        }

        protected T AssertAndReturn<T>(T message)
        {
            var serializer = Sys.Serialization.FindSerializerFor(message);
            var serialized = serializer.ToBinary(message);
            var result = serializer.FromBinary(serialized, typeof(T));
            return (T)result;
        }

        protected void AssertEqual<T>(T message)
        {
            var deserialized = AssertAndReturn(message);
            Assert.Equal(message, deserialized);
        }

        public class TypelessClass
        {
            public object Nested { get; set; }
        }

        [MessagePackObject]
        public class AddressV1
        {
            [Key(0)]
            public string City { get; set;  }

            [Key(1)]
            public string Street { get; set; }
        }

        [MessagePackObject]
        public class AddressV2
        {
            [Key(0)]
            public string City { get; set; }

            [Key(1)]
            public string Street { get; set; }

            [Key(2)]
            public string Country { get; set; }
        }

        [MessagePackObject]
        public class AddressV3 : IMessagePackSerializationCallbackReceiver
        {
            [Key(0)]
            public string City { get; set; }

            [Key(1)]
            public string Street { get; set; }

            [Key(2)]
            public string Country { get; set; }

            public void OnBeforeSerialize()
            {
            }

            public void OnAfterDeserialize()
            {
                Country = "Japan";
            }
        }
    }
}
