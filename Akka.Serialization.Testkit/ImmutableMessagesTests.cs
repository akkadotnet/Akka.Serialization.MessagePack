//-----------------------------------------------------------------------
// <copyright file="ImmutableMessagesTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using FluentAssertions;
using System;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class ImmutableMessagesTests : TestKit.Xunit2.TestKit
    {
        protected ImmutableMessagesTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public virtual void Can_serialize_EmptySingleton()
        {
            var message = ImmutableMessages.EmptySingleton.Instance;
            AssertAndReturn(message).Should().BeOfType<ImmutableMessages.EmptySingleton>();
        }

        [Fact]
        public virtual void Can_Serialize_ImmutableMessage()
        {
            var actual = new ImmutableMessages.ImmutableMessage("John", 15);
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_ImmutableMessageWithDefaultParameters()
        {
            var actual = new ImmutableMessages.ImmutableMessageWithDefaultParameters("John");
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_ImmutableMessageWithTwoConstructors()
        {
            var actual = new ImmutableMessages.ImmutableMessageWithDefaultParameters("John");
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

    public static class ImmutableMessages
    {
        public class EmptyMessage { }

        public class EmptySingleton
        {
            public static EmptySingleton Instance { get; } = new EmptySingleton();
        }

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

        public class ImmutableMessageWithPublicFields
        {
            public int Age;

            public string Name;

            private bool Equals(ImmutableMessageWithPublicFields other)
            {
                return string.Equals(Name, other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithPublicFields && Equals((ImmutableMessageWithPublicFields)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public class ImmutableMessage
        {
            public ImmutableMessage(string name, int age)
            {
                Age = age;
                Name = name;
            }

            public int Age { get; }

            public string Name { get; }

            private bool Equals(ImmutableMessage other)
            {
                return String.Equals(Name, (string)other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessage && Equals((ImmutableMessage)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public class ImmutableMessageWithDefaultParameters
        {
            public ImmutableMessageWithDefaultParameters(string name, int age = 10)
            {
                Name = name;
                Age = age;
            }

            public int Age { get; }

            public string Name { get; }

            private bool Equals(ImmutableMessageWithDefaultParameters other)
            {
                return String.Equals(Name, (string)other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithDefaultParameters && Equals((ImmutableMessageWithDefaultParameters)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public class ImmutableMessageWithTwoConstructors
        {
            public ImmutableMessageWithTwoConstructors(int age, string name)
            {
                Age = age;
                Name = name;
            }

            public ImmutableMessageWithTwoConstructors(string name)
            {
                Name = name;
                Age = 4;
            }

            public int Age { get; }

            public string Name { get; }

            private bool Equals(ImmutableMessageWithTwoConstructors other)
            {
                return String.Equals(Name, (string)other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithTwoConstructors && Equals((ImmutableMessageWithTwoConstructors)obj);
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
