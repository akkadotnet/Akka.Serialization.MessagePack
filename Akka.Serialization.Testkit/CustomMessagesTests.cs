//-----------------------------------------------------------------------
// <copyright file="CustomMessagesTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using FluentAssertions;
using System;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class CustomMessagesTests : TestKit.Xunit2.TestKit
    {
        protected CustomMessagesTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public virtual void Can_Serialize_EmptyMessage()
        {
            var message = new CustomMessage.EmptyMessage();
            AssertAndReturn(message).Should().BeOfType<CustomMessage.EmptyMessage>();
        }

        [Fact]
        public void Can_Serialize_MessageWithPublicSetters()
        {
            var actual = new CustomMessage.MessageWithPublicSetters()
            {
                Name = "John",
                Age = 15
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithReadonlyFields()
        {
            var actual = new CustomMessage.MessageWithPublicFields();
            actual.Name = "John";
            actual.Age = 15;
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

    public static class CustomMessage
    {
        public class EmptyMessage { }

        public class MessageWithPublicSetters
        {
            public int Age { get; set; }

            public string Name { get; set; }

            private bool Equals(MessageWithPublicSetters other)
            {
                return String.Equals(Name, (string)other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is MessageWithPublicSetters && Equals((MessageWithPublicSetters)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public class MessageWithPublicFields
        {
            public int Age;

            public string Name;

            private bool Equals(MessageWithPublicFields other)
            {
                return string.Equals(Name, other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is MessageWithPublicFields && Equals((MessageWithPublicFields)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }
    }
}
