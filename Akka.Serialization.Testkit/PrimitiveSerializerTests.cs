//-----------------------------------------------------------------------
// <copyright file="PrimitiveSerializerTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Numerics;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class PrimitiveSerializerTests : TestKit.Xunit2.TestKit
    {
        protected PrimitiveSerializerTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public void Can_Serialize_string()
        {
            string actual = "example-string";
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_bool()
        {
            var actual = false;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_byte()
        {
            byte actual = 12;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_int()
        {
            int actual = 435;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_long()
        {
            long actual = 435L;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_double()
        {
            double actual = 56.56d;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_float()
        {
            float actual = 56.56f;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_decimal()
        {
            decimal actual = 56.56M;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_BigInteger()
        {
            BigInteger actual = 333333333333333;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_Nullable()
        {
            int? actual = 5;
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_NullableWithNull()
        {
            int? actual = null;
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_DateTime()
        {
            DateTime date = new DateTime(2016, 6, 7, 15, 6, 45);
            AssertEqual(date);
        }

        [Fact]
        public void Can_Serialize_DateTimeUtc()
        {
            DateTime date = new DateTime(2016, 6, 7, 15, 6, 45, DateTimeKind.Utc);
            AssertEqual(date);
        }

        [Fact]
        public void Can_Serialize_DateTimeOffset()
        {
            DateTimeOffset date = new DateTimeOffset(2016, 6, 7, 15, 6, 45, TimeSpan.FromSeconds(3600));
            AssertEqual(date);
        }

        [Fact]
        public void Can_Serialize_TimeSpan()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(3658);
            AssertEqual(timeSpan);
        }

        [Fact]
        public void Can_Serialize_Tuple()
        {
            var tuple = Tuple.Create(25, "tuple");
            AssertEqual(tuple);
        }

        [Fact]
        public virtual void Can_Serialize_ValueTuple()
        {
            var valueTuple = new ValueTuple<int, string>(4, "name");
            AssertEqual(valueTuple);
        }

        [Fact]
        public virtual void Can_Serialize_NamedValueTuple()
        {
            var valueTuple = (age: 4, name: "25");
            AssertEqual(valueTuple);
        }

        [Fact]
        public void Can_Serialize_Guid()
        {
            var guid = Guid.NewGuid();
            AssertEqual(guid);
        }

        [Fact]
        public virtual void Can_Serialize_AbsoluteUri()
        {
            var actual = new Uri("http://getakka.net/articles/index.html");
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_RelativeUri()
        {
            var actual = new Uri("/articles/index.html", UriKind.Relative);
            AssertEqual(actual);
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
    }
}
