//-----------------------------------------------------------------------
// <copyright file="HyperionTests.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Serialization.MsgPack;
using Akka.Tests.Serialization;
using MessagePack;
using MessagePack.Resolvers;
using Xunit;
using Xunit.Sdk;

namespace Akka.Serialization.MessagePack.Tests
{
    public class MessagePackTests : AkkaSerializationSpec
    {
        public MessagePackTests() : base(typeof(MsgPackSerializer))
        {
        }

        [Fact]
        public void MessageTest_NotWorks()
        {
            var original = new TestMessage { Age = 15, Name = "John" };
            var serializer = Sys.Serialization.FindSerializerFor(original);
            var serialized = serializer.ToBinary(original);
            var deserialized = serializer.FromBinary(serialized, typeof(TestMessage));

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void MessageTest_Works()
        {
            var original = new TestMessage { Age = 15, Name = "John" };
            var serializer = Sys.Serialization.FindSerializerFor(original);
            var serialized = MessagePackSerializer.Serialize(original, DynamicContractlessObjectResolver.Instance);
            var deserialized = serializer.FromBinary(serialized, typeof(TestMessage));

            Assert.Equal(original, deserialized);
        }
    }

    public class TestMessage
    {
        public int Age { get; set; }

        public string Name { get; set; }

        protected bool Equals(TestMessage other)
        {
            return Age == other.Age && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestMessage)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Age * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}